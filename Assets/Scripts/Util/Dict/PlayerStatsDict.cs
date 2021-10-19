using UnityEngine;

public class PlayerStatsDict : MonoBehaviour
{
    public static PlayerStatsDict Instance { get; private set; }

    [SerializeField] private int minValue;
    [SerializeField] private int maxValue;

    [SerializeField] private AnimationCurve healthCurve;
    [SerializeField] private AnimationCurve speedCurve;
    [SerializeField] private AnimationCurve luckCurve;
    [SerializeField] private AnimationCurve dodgeCurve;

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("PlayerStatsDict already in scene. Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Normailzes each stat.
    /// </summary>
    /// <param name="effect">The stats to be normalized</param>
    public void NormalizeStats(ref StatsEffect effect)
    {
        effect.health = Mathf.Clamp(effect.health, minValue, maxValue);
        effect.speed = Mathf.Clamp(effect.speed, minValue, maxValue);
        effect.luck = Mathf.Clamp(effect.luck, minValue, maxValue);
        effect.dodge = Mathf.Clamp(effect.dodge, minValue, maxValue);
    }

    public bool InRange(int stat) => stat >= minValue && stat <= maxValue;

    private float GetPercentage(int stat) => (stat - minValue) / (maxValue - minValue);

    /// <summary>
    /// Gets the current maximum movment force.
    /// </summary>
    public float GetMovementForce(int movementStat)
    {
        if (InRange(movementStat) == false)
            Debug.LogError("Movement out of range: " + movementStat);

        // return speed * 600;
        return speedCurve.Evaluate(GetPercentage(movementStat));
    }

    /// <summary>
    /// Gets the dodge cooldown in seconds.
    /// </summary>
    public float GetDodgeCooldown(int dodgeStat)
    {
        //return 3.0f * (1 - (float)(Mathf.Max(1, 10 - dodge) / 10));
        return dodgeCurve.Evaluate(GetPercentage(dodgeStat));
    }

    public int GetHealth(int healthStat)
    {
        return (int)healthCurve.Evaluate(GetPercentage(healthStat));
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
