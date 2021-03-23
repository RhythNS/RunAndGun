using System.Collections;
using System.Diagnostics;

public class DungeonTimer
{
    private readonly Stopwatch stopwatch = new Stopwatch();

    private static readonly int WORKTIME_IN_MS_UNTIL_NEXT_FRAME = 1000 / 30;

    public bool ShouldWait() => stopwatch.ElapsedMilliseconds > WORKTIME_IN_MS_UNTIL_NEXT_FRAME;

    public IEnumerator Wait(float dungeonProgressInPercent)
    {
        DungeonCreator.Instance.SetLoadStatus(dungeonProgressInPercent);
        yield return null;
        stopwatch.Restart();
    }

    public void Start() => stopwatch.Start();

    public void Reset() => stopwatch.Reset();

    public void Restart() => stopwatch.Restart();

    public void Stop() => stopwatch.Stop();
}
