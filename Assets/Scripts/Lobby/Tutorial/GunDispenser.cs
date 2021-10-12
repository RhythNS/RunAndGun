using Mirror;
using UnityEngine;

public class GunDispenser : MonoBehaviour
{
    [SerializeField] private Vector3 gunSpawnPoint;
    [SerializeField] private Weapon[] toSpawnWeapons;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (NetworkServer.active == false ||
            collision.gameObject.TryGetComponent(out Player _) == false)
            return;

        PickableInWorld.Place(RandomUtil.Element(toSpawnWeapons), transform.position + gunSpawnPoint);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        GizmosUtil.DrawPoint(transform.position + gunSpawnPoint);
    }
}
