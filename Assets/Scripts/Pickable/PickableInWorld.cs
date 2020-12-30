using Mirror;
using UnityEngine;

public class PickableInWorld : NetworkBehaviour
{
    public Pickable Pickable => pickable;
    [SyncVar] private Pickable pickable;

    [Server]
    public static void Place(Pickable pickable, Vector3 position)
    {
        GameObject gObject = Instantiate(PickableDict.Instance.PickableInWorldPrefab);
        gObject.transform.position = position;
        gObject.GetComponent<PickableInWorld>().pickable = pickable;
        NetworkServer.Spawn(gObject);
    }

    public override void OnStartClient()
    {
        GetComponent<SpriteRenderer>().sprite = pickable.Icon ? pickable.Icon : PickableDict.Instance.MissingTexture;
    }
}
