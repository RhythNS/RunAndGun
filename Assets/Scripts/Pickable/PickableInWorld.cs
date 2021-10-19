using Mirror;
using System.Collections;
using UnityEngine;

/// <summary>
/// Class for placing a Pickable into the world.
/// </summary>
public class PickableInWorld : NetworkBehaviour
{
    public delegate void InWorldDelegate(PickableInWorld piw);

    public static InWorldDelegate OnSpawned;
    public static InWorldDelegate OnDeSpawned;

    public Pickable Pickable => pickable;
    [SyncVar] [SerializeField] private Pickable pickable;

    public bool IsBuyable => isBuyable;
    [SyncVar] private bool isBuyable;

    [SyncVar] private bool playSpawnAnimation;

    private ExtendedCoroutine spawnRoutine;

    public bool Available { get; private set; } = true;

    /// <summary>
    /// Places a pickable into the world.
    /// </summary>
    /// <param name="pickable">The pickable to be placed.</param>
    /// <param name="position">The position to where it should be placed.</param>
    /// <param name="isBuyable">Whether it is buyable.</param>
    [Server]
    public static PickableInWorld Place(Pickable pickable, Vector3 position, bool isBuyable = false, bool playSpawnAnimation = true)
    {
        GameObject gObject = Instantiate(PickableDict.Instance.PickableInWorldPrefab);
        gObject.layer = LayerDict.Instance.GetPickableLayer();
        PositionConverter.AdjustZ(ref position);
        gObject.transform.position = position;
        PickableInWorld piw = gObject.GetComponent<PickableInWorld>();
        piw.pickable = pickable;
        piw.isBuyable = isBuyable;
        piw.playSpawnAnimation = playSpawnAnimation;
        NetworkServer.Spawn(gObject);
        return piw;
    }

    public override void OnStartClient()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        renderer.sprite = pickable.Icon ? pickable.Icon : PickableDict.Instance.MissingTexture;

        if (playSpawnAnimation == true)
            spawnRoutine = new ExtendedCoroutine(this, SpawnEffect(renderer), startNow: true);

        gameObject.layer = LayerDict.Instance.GetPickableLayer();
    }

    private IEnumerator SpawnEffect(SpriteRenderer renderer)
    {
        Material spawnMat = new Material(MaterialDict.Instance.SpawnMaterial);
        renderer.material = spawnMat;

        yield return EntityMaterialManager.SpawnEffect(spawnMat, 0.75f);
    }

    [Server]
    public void DespawnPickable()
    {
        ClientDespawnPickable();
        StartCoroutine(InnerDespawnPickable());
    }

    [ClientRpc]
    private void ClientDespawnPickable()
    {
        StartCoroutine(InnerDespawnPickable());
    }

    public IEnumerator InnerDespawnPickable()
    {
        if (spawnRoutine != null && spawnRoutine.IsFinshed == false)
            spawnRoutine.Stop(false);

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Material despawnMat = new Material(MaterialDict.Instance.SpawnMaterial);
        spriteRenderer.material = despawnMat;

        yield return EntityMaterialManager.DeSpawnEffect(despawnMat, 0.75f);

        if (gameObject && isServer)
            NetworkServer.Destroy(gameObject);
    }


    public override void OnStartServer()
    {
        OnSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        OnDeSpawned?.Invoke(this);
    }

    [Server]
    public void PickedUp(bool destroy = true)
    {
        Available = false;
        if (destroy == true)
            NetworkServer.Destroy(gameObject);
    }
}
