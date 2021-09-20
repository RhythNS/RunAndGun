using System;
using UnityEngine;

public class KillAllPlayers : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("Kil all players is enabled with F8");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F8))
        {
            Player[] players = FindObjectsOfType<Player>();
            if (players == null || players.Length == 0)
                return;
            Array.ForEach(players, x => x.Health.Damage(int.MaxValue, null));
        }
    }
}
