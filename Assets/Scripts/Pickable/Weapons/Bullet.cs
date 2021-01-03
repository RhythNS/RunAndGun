using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Set(Health shooter, Sprite sprite, Vector3 position, Vector2 direction, float speed, bool useLocalSpace)
    {
        spriteRenderer.sprite = sprite;
        transform.position = position;
        // TODO: implement
    }
}
