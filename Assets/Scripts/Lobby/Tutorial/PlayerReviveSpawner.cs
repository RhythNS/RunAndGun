using Mirror;
using Rhyth.BTree;
using System.Collections;
using UnityEngine;

public class PlayerReviveSpawner : MonoBehaviour
{
    [SerializeField] private Player playerPrefab;
    [SerializeField] private BTree tree;

    [SerializeField] private WorldEffectInWorld prefab;
    [SerializeField] private WorldEffect[] effects;

    [SerializeField] private float spawnAfterSeconds;
    private ExtendedCoroutine effectSpawn;

    private Player spawnedPlayer;

    [Server]
    public void Spawn()
    {
        if (spawnedPlayer != null)
            return;

        spawnedPlayer = Instantiate(playerPrefab, transform.position, Quaternion.identity);
        spawnedPlayer.PlayerAI.Brain.tree = tree;
        NetworkServer.Spawn(spawnedPlayer.gameObject);

        effectSpawn = new ExtendedCoroutine(this, SpawnWorldEffect(), startNow: true);
    }

    [Server]
    public void DeSpawn()
    {
        NetworkServer.Destroy(spawnedPlayer.gameObject);

        if (effectSpawn != null && effectSpawn.IsFinshed == false)
            effectSpawn.Stop(false);
    }

    private IEnumerator SpawnWorldEffect()
    {
        yield return new WaitForSeconds(spawnAfterSeconds);
        WorldEffectInWorld.Place(prefab, effects, spawnedPlayer.Health, transform.position, Quaternion.identity);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        GizmosUtil.DrawPoint(transform.position);
    }
}
