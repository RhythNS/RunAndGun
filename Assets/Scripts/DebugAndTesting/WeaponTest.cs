using Mirror;
using UnityEngine;

/// <summary>
/// Spawns a specified weapon on a specified position.
/// </summary>
public class WeaponTest : NetworkBehaviour
{
    [SerializeField] private Weapon weaponToDrop;
    [SerializeField] private Vector2 spawnAt;

    public override void OnStartServer()
    {
        PickableInWorld.Place(weaponToDrop, spawnAt);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 ur = spawnAt - new Vector2(0.5f, -0.5f);
        Vector3 ul = spawnAt - new Vector2(-0.5f, -0.5f);
        Vector3 or = spawnAt - new Vector2(0.5f, 0.5f);
        Vector3 ol = spawnAt - new Vector2(-0.5f, 0.5f);

        Gizmos.DrawLine(ur, ol);
        Gizmos.DrawLine(ul, or);
    }
}
