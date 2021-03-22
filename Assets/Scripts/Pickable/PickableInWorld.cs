using Mirror;
using UnityEngine;

public class PickableInWorld : NetworkBehaviour
{
    public Pickable Pickable => pickable;
    [SyncVar] private Pickable pickable;

    public bool IsBuyable => isBuyable;
    [SyncVar] private bool isBuyable;

    [Server]
    public static void Place(Pickable pickable, Vector3 position, bool isBuyable = false)
    {
        GameObject gObject = Instantiate(PickableDict.Instance.PickableInWorldPrefab);
        gObject.transform.position = position;
        gObject.GetComponent<PickableInWorld>().pickable = pickable;
        gObject.GetComponent<PickableInWorld>().isBuyable = isBuyable;
        NetworkServer.Spawn(gObject);
    }

    public override void OnStartClient()
    {
        GetComponent<SpriteRenderer>().sprite = pickable.Icon ? pickable.Icon : PickableDict.Instance.MissingTexture;
    }
}
