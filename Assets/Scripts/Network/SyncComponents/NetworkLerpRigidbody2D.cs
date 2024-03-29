﻿using Mirror;
using UnityEngine;

// Copied from NetworkLerpRigidbody and adjusted it to 2D Rigidbody
public class NetworkLerpRigidbody2D : NetworkBehaviour
{
    [Header("Settings")]
    [SerializeField] internal Rigidbody2D target = null;
    [Tooltip("How quickly current velocity approaches target velocity")]
    [SerializeField] float lerpVelocityAmount = 0.5f;
    [Tooltip("How quickly current position approaches target position")]
    [SerializeField] float lerpPositionAmount = 0.5f;

    [Tooltip("Set to true if moves come from owner client, set to false if moves always come from server")]
    [SerializeField] bool clientAuthority = false;

    float nextSyncTime;


    [SyncVar()]
    Vector2 targetVelocity;

    [SyncVar()]
    Vector2 targetPosition;

    /// <summary>
    /// Ignore value if is host or client with Authority
    /// </summary>
    /// <returns></returns>
    bool IgnoreSync => isServer || ClientWithAuthority;

    bool ClientWithAuthority => clientAuthority && hasAuthority;

    void OnValidate()
    {
        if (target == null)
        {
            target = GetComponent<Rigidbody2D>();
        }
    }

    void Update()
    {
        if (isServer)
        {
            SyncToClients();
        }
        else if (ClientWithAuthority)
        {
            SendToServer();
        }
    }

    private void SyncToClients()
    {
        targetVelocity = target.velocity;
        targetPosition = target.position;
    }

    private void SendToServer()
    {
        float now = Time.time;
        if (now > nextSyncTime)
        {
            nextSyncTime = now + syncInterval;
            CmdSendState(target.velocity, target.position);
        }
    }

    [Command]
    private void CmdSendState(Vector3 velocity, Vector3 position)
    {
        target.velocity = velocity;
        target.position = position;
        targetVelocity = velocity;
        targetPosition = position;
    }

    void FixedUpdate()
    {
        if (IgnoreSync) { return; }

        target.velocity = Vector3.Lerp(target.velocity, targetVelocity, lerpVelocityAmount);
        target.position = Vector3.Lerp(target.position, targetPosition, lerpPositionAmount);
        // add velocity to position as position would have moved on server at that velocity
        targetPosition += target.velocity * Time.fixedDeltaTime;

        // TODO does this also need to sync acceleration so and update velocity?
    }
}
