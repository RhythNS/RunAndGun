using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnorePlayerCollisionSetter : MonoBehaviour
{
    [SerializeField] private Collider2D coll;

    private void Awake()
    {
        if (coll == null)
            coll = GetComponent<Collider2D>();
    }

    private void Start()
    {
        PlayersDict.Instance.OnPlayerAdded += OnPlayerAdded;
    }

    private void OnPlayerAdded(Player player)
    {
        Physics2D.IgnoreCollision(coll, player.Collider2D);
    }

    private void OnDestroy()
    {
        if (PlayersDict.Instance)
            PlayersDict.Instance.OnPlayerAdded -= OnPlayerAdded;
    }
}
