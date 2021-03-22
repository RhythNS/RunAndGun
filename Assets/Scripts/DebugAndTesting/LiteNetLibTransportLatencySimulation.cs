using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
struct QueuedMessage
{
    public int connectionId;
    public byte[] bytes;
    public float time;
}

[HelpURL("https://mirror-networking.gitbook.io/docs/transports/latency-simulaton-transport")]
[DisallowMultipleComponent]
public class LiteNetLibTransportLatencySimulation : LiteNetLibTransport
{
    [Header("Common")]
    [Tooltip("Spike latency via perlin(Time * speedMultiplier) * spikeMultiplier")]
    [Range(0, 1)] public float latencySpikeMultiplier;
    [Tooltip("Spike latency via perlin(Time * speedMultiplier) * spikeMultiplier")]
    public float latencySpikeSpeedMultiplier = 1;

    [Header("Reliable Messages")]
    [Tooltip("Reliable latency in seconds")]
    public float reliableLatency;
    // note: packet loss over reliable manifests itself in latency.
    //       don't need (and can't add) a loss option here.
    // note: reliable is ordered by definition. no need to scramble.

    [Header("Unreliable Messages")]
    [Tooltip("Packet loss in %")]
    [Range(0, 1)] public float unreliableLoss;
    [Tooltip("Unreliable latency in seconds")]
    public float unreliableLatency;
    [Tooltip("Scramble % of unreliable messages, just like over the real network. Mirror unreliable is unordered.")]
    [Range(0, 1)] public float unreliableScramble;

    // message queues
    // list so we can insert randomly (scramble)
    List<QueuedMessage> reliableClientToServer = new List<QueuedMessage>();
    List<QueuedMessage> reliableServerToClient = new List<QueuedMessage>();
    List<QueuedMessage> unreliableClientToServer = new List<QueuedMessage>();
    List<QueuedMessage> unreliableServerToClient = new List<QueuedMessage>();

    // random
    // UnityEngine.Random.value is [0, 1] with both upper and lower bounds inclusive
    // but we need the upper bound to be exclusive, so using System.Random instead.
    // => NextDouble() is NEVER < 0 so loss=0 never drops!
    // => NextDouble() is ALWAYS < 1 so loss=1 always drops!
    System.Random random = new System.Random();

    // noise function can be replaced if needed
    protected virtual float Noise(float time) => Mathf.PerlinNoise(time, time);

    // helper function to simulate latency
    float SimulateLatency(int channeldId)
    {
        // spike over perlin noise.
        // no spikes isn't realistic.
        // sin is too predictable / no realistic.
        // perlin is still deterministic and random enough.
        float spike = Noise(Time.time * latencySpikeSpeedMultiplier) * latencySpikeMultiplier;

        // base latency
        switch (channeldId)
        {
            case Channels.Reliable:
                return reliableLatency + spike;
            case Channels.Unreliable:
                return unreliableLatency + spike;
            default:
                return 0;
        }
    }

    // helper function to simulate a send with latency/loss/scramble
    void SimulateSend(int connectionId, int channelId, ArraySegment<byte> segment, float latency, List<QueuedMessage> reliableQueue, List<QueuedMessage> unreliableQueue)
    {
        // segment is only valid after returning. copy it.
        // (allocates for now. it's only for testing anyway.)
        byte[] bytes = new byte[segment.Count];
        Buffer.BlockCopy(segment.Array, segment.Offset, bytes, 0, segment.Count);

        // enqueue message. send after latency interval.
        QueuedMessage message = new QueuedMessage
        {
            connectionId = connectionId,
            bytes = bytes,
            time = Time.time + latency
        };

        switch (channelId)
        {
            case Channels.Reliable:
                // simulate latency
                reliableQueue.Add(message);
                break;
            case Channels.Unreliable:
                // simulate packet loss
                bool drop = random.NextDouble() < unreliableLoss;
                if (!drop)
                {
                    // simulate scramble (Random.Next is < max, so +1)
                    bool scramble = random.NextDouble() < unreliableScramble;
                    int last = unreliableQueue.Count;
                    int index = scramble ? random.Next(0, last + 1) : last;

                    // simulate latency
                    unreliableQueue.Insert(index, message);
                }
                break;
            default:
                Debug.LogError($"{nameof(LatencySimulation)} unexpected channelId: {channelId}");
                break;
        }
    }

    public override bool Available() => base.Available();

    public override void ClientConnect(string address)
    {
        base.OnClientConnected = OnClientConnected;
        base.OnClientDataReceived = OnClientDataReceived;
        base.OnClientError = OnClientError;
        base.OnClientDisconnected = OnClientDisconnected;
        base.ClientConnect(address);
    }

    public override void ClientConnect(Uri uri)
    {
        base.OnClientConnected = OnClientConnected;
        base.OnClientDataReceived = OnClientDataReceived;
        base.OnClientError = OnClientError;
        base.OnClientDisconnected = OnClientDisconnected;
        base.ClientConnect(uri);
    }

    public override bool ClientConnected() => base.ClientConnected();

    public override void ClientDisconnect()
    {
        base.ClientDisconnect();
        reliableClientToServer.Clear();
        unreliableClientToServer.Clear();
    }

    public override void ClientSend(int channelId, ArraySegment<byte> segment)
    {
        float latency = SimulateLatency(channelId);
        SimulateSend(0, channelId, segment, latency, reliableClientToServer, unreliableClientToServer);
    }

    public override Uri ServerUri() => base.ServerUri();

    public override bool ServerActive() => base.ServerActive();

    public override string ServerGetClientAddress(int connectionId) => base.ServerGetClientAddress(connectionId);

    public override bool ServerDisconnect(int connectionId) => base.ServerDisconnect(connectionId);

    public override void ServerSend(int connectionId, int channelId, ArraySegment<byte> segment)
    {
        float latency = SimulateLatency(channelId);
        SimulateSend(connectionId, channelId, segment, latency, reliableServerToClient, unreliableServerToClient);
    }

    public override void ServerStart()
    {
        base.OnServerConnected = OnServerConnected;
        base.OnServerDataReceived = OnServerDataReceived;
        base.OnServerError = OnServerError;
        base.OnServerDisconnected = OnServerDisconnected;
        base.ServerStart();
    }

    public override void ServerStop()
    {
        base.ServerStop();
        reliableServerToClient.Clear();
        unreliableServerToClient.Clear();
    }

    public override void ClientEarlyUpdate() => base.ClientEarlyUpdate();
    public override void ServerEarlyUpdate() => base.ServerEarlyUpdate();
    public override void ClientLateUpdate()
    {
        // flush reliable messages after latency
        while (reliableClientToServer.Count > 0)
        {
            // check the first message time
            QueuedMessage message = reliableClientToServer[0];
            if (message.time <= Time.time)
            {
                // send and eat
                base.ClientSend(Channels.Reliable, new ArraySegment<byte>(message.bytes));
                reliableClientToServer.RemoveAt(0);
            }
            // not enough time elapsed yet
            break;
        }

        // flush unreliable messages after latency
        while (unreliableClientToServer.Count > 0)
        {
            // check the first message time
            QueuedMessage message = unreliableClientToServer[0];
            if (message.time <= Time.time)
            {
                // send and eat
                base.ClientSend(Channels.Unreliable, new ArraySegment<byte>(message.bytes));
                unreliableClientToServer.RemoveAt(0);
            }
            // not enough time elapsed yet
            break;
        }

        // update wrapped transport too
        base.ClientLateUpdate();
    }
    public override void ServerLateUpdate()
    {
        // flush reliable messages after latency
        while (reliableServerToClient.Count > 0)
        {
            // check the first message time
            QueuedMessage message = reliableServerToClient[0];
            if (message.time <= Time.time)
            {
                // send and eat
                base.ServerSend(message.connectionId, Channels.Reliable, new ArraySegment<byte>(message.bytes));
                reliableServerToClient.RemoveAt(0);
            }
            // not enough time elapsed yet
            break;
        }

        // flush unreliable messages after latency
        while (unreliableServerToClient.Count > 0)
        {
            // check the first message time
            QueuedMessage message = unreliableServerToClient[0];
            if (message.time <= Time.time)
            {
                // send and eat
                base.ServerSend(message.connectionId, Channels.Unreliable, new ArraySegment<byte>(message.bytes));
                unreliableServerToClient.RemoveAt(0);
            }
            // not enough time elapsed yet
            break;
        }

        // update wrapped transport too
        base.ServerLateUpdate();
    }

    public override int GetMaxBatchSize(int channelId) => base.GetMaxBatchSize(channelId);
    public override int GetMaxPacketSize(int channelId = 0) => base.GetMaxPacketSize(channelId);

    public override void Shutdown() => base.Shutdown();

 //   public override string ToString() => $"{nameof(LatencySimulation)} {wrap}";
}
