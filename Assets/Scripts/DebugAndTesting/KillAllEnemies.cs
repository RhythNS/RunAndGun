using System;
using UnityEngine;

/// <summary>
/// Kills all currently spawned enemies.
/// </summary>
public class KillAllEnemies : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("Kil all enemies is enabled with F2");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            Enemy[] enemies = FindObjectsOfType<Enemy>();
            if (enemies == null || enemies.Length == 0)
                return;
            Array.ForEach(enemies, x => x.Health.Damage(int.MaxValue, null));
        }
    }
}
