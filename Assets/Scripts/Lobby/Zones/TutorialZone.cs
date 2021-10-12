using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialZone : MonoBehaviour
{
    [SerializeField] private Vector2 halfBounds;

    private Vector2 min, max;
    private List<PickableInWorld> activeWeapons = new List<PickableInWorld>();

    private void Start()
    {
        Vector2 pos = transform.position;
        min = pos - halfBounds;
        max = pos + halfBounds;
    }

    public void Register()
    {
        PickableInWorld.OnSpawned += OnPickableSpawned;
        PickableInWorld.OnDeSpawned += OnPickableDeSpawned;
    }

    public void DeRegister()
    {
        PickableInWorld.OnSpawned -= OnPickableSpawned;
        PickableInWorld.OnDeSpawned -= OnPickableDeSpawned;
    }

    private void OnPickableSpawned(PickableInWorld piw)
    {
        Vector2 pos = piw.transform.position;
        if (pos.x < min.x || pos.y < min.y || pos.x > max.x || pos.y > max.y)
        {
            piw.PickedUp(false);
            piw.DespawnPickable();
            return;
        }

        if (piw.Pickable.PickableType == PickableType.Weapon)
        {
            if (activeWeapons.Count > 0)
            {
                for (int i = 0; i < activeWeapons.Count; i++)
                {
                    if (activeWeapons[i])
                        activeWeapons[i].DespawnPickable();
                }
                activeWeapons.Clear();
            }
            activeWeapons.Add(piw);
        }
    }

    private void OnPickableDeSpawned(PickableInWorld piw)
    {
        if (piw.Pickable.PickableType == PickableType.Weapon)
            activeWeapons.Remove(piw);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 size = halfBounds * 2;
        size.z = 1.0f;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, size);
    }
}
