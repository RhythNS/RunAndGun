using Mirror;
using Rhyth.BTree;
using UnityEngine;

public class PlayerReviveSpawner : MonoBehaviour
{
    [SerializeField] private Player playerPrefab;
    [SerializeField] private BTree tree;
    private Player spawnedPlayer;

    [Server]
    public void Spawn()
    {
        if (spawnedPlayer != null)
            return;

        spawnedPlayer = Instantiate(playerPrefab, transform.position, Quaternion.identity);
        spawnedPlayer.PlayerAI.Brain.tree = tree;
        NetworkServer.Spawn(spawnedPlayer.gameObject);
    }

    [Server]
    public void DeSpawn()
    {
        NetworkServer.Destroy(spawnedPlayer.gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        GizmosUtil.DrawPoint(transform.position);
    }
}
