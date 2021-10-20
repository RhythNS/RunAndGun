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

    [SerializeField] private Pickable rewardAfterRevive;
    [SerializeField] private Vector3 rewardSpawnOffset;

    private Player spawnedPlayer;

    [Server]
    public void Spawn()
    {
        if (spawnedPlayer != null)
            return;

        spawnedPlayer = Instantiate(playerPrefab, transform.position, Quaternion.identity);
        spawnedPlayer.PlayerAI.Brain.tree = tree;
        NetworkServer.Spawn(spawnedPlayer.gameObject);
        spawnedPlayer.Health.CurrentChanged += SpawnedPlayerHealthChanged;

        effectSpawn = new ExtendedCoroutine(this, SpawnWorldEffect(), startNow: true);
    }

    private void SpawnedPlayerHealthChanged(int _, int newValue)
    {
        if (newValue <= 0)
            return;

        PickableInWorld.Place(rewardAfterRevive, transform.position + rewardSpawnOffset);
    }

    [Server]
    public void DeSpawn()
    {
        spawnedPlayer.Health.CurrentChanged -= SpawnedPlayerHealthChanged;
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
        Gizmos.color = Color.green;
        GizmosUtil.DrawPoint(transform.position + rewardSpawnOffset);
    }
}
