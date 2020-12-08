using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowSprite : MonoBehaviour
{
    [SerializeField] public Transform toFollow;
    [SerializeField] private float minDistance = 50.0f;
    [SerializeField] private float strength = 1.0f;

    private float distanceSqr;
    private Vector3 velocity = new Vector3(0.0f, 0.0f, 0.0f);

    private void Start()
    {
        toFollow = FindObjectOfType<Player>().transform;
        distanceSqr = minDistance * minDistance;
    }

    private void FixedUpdate()
    {
        Vector3 direction = toFollow.position - transform.position;
        direction.z = 0.0f;

        if (direction.sqrMagnitude < distanceSqr)
            velocity *= 0.9f;
        else
            velocity = direction * strength;

        transform.position += velocity * Time.fixedDeltaTime;
    }
}
