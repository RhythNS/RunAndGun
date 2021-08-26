using Mirror;
using UnityEngine;

/// <summary>
/// Class for placing a Pickable into the world.
/// </summary>
public class PickableInWorld : NetworkBehaviour
{
    public Pickable Pickable => pickable;
    [SyncVar] [SerializeField] private Pickable pickable;

    public bool IsBuyable => isBuyable;
    [SyncVar] private bool isBuyable;

    /// <summary>
    /// Places a pickable into the world.
    /// </summary>
    /// <param name="pickable">The pickable to be placed.</param>
    /// <param name="position">The position to where it should be placed.</param>
    /// <param name="isBuyable">Whether it is buyable.</param>
    [Server]
    public static void Place(Pickable pickable, Vector3 position, bool isBuyable = false)
    {
        GameObject gObject = Instantiate(PickableDict.Instance.PickableInWorldPrefab);
        gObject.layer = LayerDict.Instance.GetPickableLayer();
        PositionConverter.AdjustZ(ref position);
        gObject.transform.position = position;
        gObject.GetComponent<PickableInWorld>().pickable = pickable;
        gObject.GetComponent<PickableInWorld>().isBuyable = isBuyable;
        NetworkServer.Spawn(gObject);
    }

    public override void OnStartClient()
    {
        GetComponent<SpriteRenderer>().sprite = pickable.Icon ? pickable.Icon : PickableDict.Instance.MissingTexture;
        gameObject.layer = LayerDict.Instance.GetPickableLayer();
    }
}
