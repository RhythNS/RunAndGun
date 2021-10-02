using System;

[Serializable]
public class LastConnectedGame
{
    public string lastConnectedIP = "";
    public int lastConnectedPort = 0;
    public DateTime time;
    public bool timeIsConnected;

    public LastConnectedGame(string lastConnectedIP, int lastConnectedPort, DateTime time, bool timeIsConnected)
    {
        this.lastConnectedIP = lastConnectedIP;
        this.lastConnectedPort = lastConnectedPort;
        this.time = time;
        this.timeIsConnected = timeIsConnected;
    }

    public static LastConnectedGame Connected(string ip, int port) => new LastConnectedGame(ip, port, DateTime.Now, true);

    public static LastConnectedGame Disconnected(LastConnectedGame lcg)
    {
        lcg.time = DateTime.Now;
        lcg.timeIsConnected = false;
        return lcg;
    }

    public float GetSecondsTimeDifferenceSinceNow()
    {
        return (float)(DateTime.Now - time).TotalSeconds;
    }
}
