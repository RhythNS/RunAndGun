using System.Collections;
using System.Diagnostics;

public class DungeonTimer
{
    private readonly Stopwatch stopwatch = new Stopwatch();
    private readonly int WORKTIME_IN_MS_UNTIL_NEXT_FRAME;

    private bool setLoadStatus;

    public DungeonTimer(bool setLoadStatus = true)
    {
        this.setLoadStatus = setLoadStatus;
        WORKTIME_IN_MS_UNTIL_NEXT_FRAME = 1000 / Config.Instance.targetFramesPerSecondLoadingScreen;
    }

    public bool ShouldWait() => stopwatch.ElapsedMilliseconds > WORKTIME_IN_MS_UNTIL_NEXT_FRAME;

    public IEnumerator Wait(float dungeonProgressInPercent)
    {
        if (setLoadStatus)
            DungeonCreator.Instance.SetLoadStatus(dungeonProgressInPercent);
        yield return null;
        stopwatch.Restart();
    }

    public void Start() => stopwatch.Start();

    public void Reset() => stopwatch.Reset();

    public void Restart() => stopwatch.Restart();

    public void Stop() => stopwatch.Stop();
}
