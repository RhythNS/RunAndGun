using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSprite : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer r;

    [SerializeField]
    private Sprite[] sprites;

    void Start()
    {
        r.sprite = sprites[Random.Range(0, sprites.Length)];
    }
}
