using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEnemySpawner : NetworkBehaviour, IPathfinder
{
    [SerializeField] private EnemyObject still;
    [SerializeField] private EnemyObject moving;
    [SerializeField] private EnemyObject dodging;
    [SerializeField] private EnemyObject fightingBack;

    [SerializeField] private Vector2Int from;
    [SerializeField] private Vector2Int to;

    [SerializeField] private Vector2 spawnPoint;

    public Rect RoomBorder => roomBorder;
    [SerializeField] private Rect roomBorder;

    private void Awake()
    {
        Vector2Int size = to - from;
        roomBorder = new Rect(from, size);
    }

    public List<Vector2> TryFindPath(Vector2Int start, Vector2Int destination)
    {
        List<Vector2> path = new List<Vector2>();

        if (VectorUtil.InBounds(from, to, destination) == false)
            return path;

        throw new System.NotImplementedException();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Vector3 drawFrom = new Vector3(from.x, from.y);
        Vector3 drawTo = new Vector3(to.x, to.y);

        Vector3 size = drawTo - drawFrom;
        Vector3 mid = drawFrom + size * 0.5f;

        Gizmos.DrawWireCube(mid, size);

        GizmosUtil.DrawPoint(spawnPoint);
    }
}
