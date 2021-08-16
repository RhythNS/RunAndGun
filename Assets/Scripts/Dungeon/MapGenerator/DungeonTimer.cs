using System.Collections;
using System.Diagnostics;

/// <summary>
/// Used when a dungeon is generated to keep track of the elapsed time and wheter the
/// dungeon should stop working for a given frame.
/// </summary>
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

    /// <summary>
    /// Checks if the elapsed time is greater than the maximum worktime that should be done in the frame.
    /// </summary>
    /// <returns>Returns true if the game should wait until next frame to do more work.</returns>
    public bool ShouldWait() => stopwatch.ElapsedMilliseconds > WORKTIME_IN_MS_UNTIL_NEXT_FRAME;

    /// <summary>
    /// Sets the loading progress of the local player, yields for next frame and restarts the stopwatch.
    /// </summary>
    /// <param name="dungeonProgressInPercent">The percentage done of generating the dungeon.</param>
    public IEnumerator Wait(float dungeonProgressInPercent)
    {
        if (setLoadStatus)
            DungeonCreator.Instance.SetLoadStatus(dungeonProgressInPercent);
        yield return null;
        stopwatch.Restart();
    }

    /// <summary>
    /// Starts the stopwatch.
    /// </summary>
    public void Start() => stopwatch.Start();

    /// <summary>
    /// Resets the stopwatch.
    /// </summary>
    public void Reset() => stopwatch.Reset();

    /// <summary>
    /// Restarts the stopwatch.
    /// </summary>
    public void Restart() => stopwatch.Restart();

    /// <summary>
    /// Stops the stopwatch.
    /// </summary>
    public void Stop() => stopwatch.Stop();
}
