using FMODUnity;
using Mirror;
using UnityEngine;

public class Health : NetworkBehaviour
{
    /// <summary>
    /// The max amount of hitpoints.
    /// </summary>
    [SyncVar(hook = nameof(OnMaxChanged))] private int max = 200;
    /// <summary>
    /// The current amount of hitpoints.
    /// </summary>
    [SyncVar(hook = nameof(OnCurrentChanged))] private int current = 200;
    /// <summary>
    /// The current amount of defence points.
    /// </summary>
    [SyncVar(hook = nameof(OnDefenceChanged))] private int defence = 10;

    [SerializeField] [EventRef] private string hitSound;
    [SerializeField] [EventRef] private string diedSound;

    public event IntChangedWithPrev MaxChanged;
    public event IntChangedWithPrev CurrentChanged;
    public event IntChangedWithPrev DefenceChanged;
    public event HealthPercentageChanged CurrentChangedAsPercentage;
    public event Died OnDied;

    /// <summary>
    /// The current amount defence points.
    /// </summary>
    public int Defence => defence;
    /// <summary>
    /// The max amount of hitpoints.
    /// </summary>
    public int Max => max;
    /// <summary>
    /// The current amount of hitpoints.
    /// </summary>
    public int Current => current;
    /// <summary>
    /// How much damage in total was taken.
    /// </summary>
    public int DamageTaken => max - current;
    /// <summary>
    /// Checks if entity is still alive.
    /// </summary>
    public bool Alive => current > 0;

    public EntityType EntityType { get; private set; }
    public StatusEffectList StatusEffectList { get; private set; }
    private Player onPlayer = null;

    private void Awake()
    {
        onPlayer = GetComponent<Player>();
        EntityType = GetComponent<Entity>().EntityType;
        StatusEffectList = GetComponent<StatusEffectList>();
    }

    private void OnEnable()
    {
        AliveHealthDict.Instance.Register(this);
    }

    private void OnDisable()
    {
        AliveHealthDict.Instance.DeRegister(this);
    }

    [Server]
    public void Init(int maxHealth)
    {
        max = current = maxHealth;
    }

    [Server]
    public void SetMax(int amount)
    {
        max = amount;
        if (current > max)
            current = max;
        // TODO: If the max goes up, this the current also go up?
    }

    [Server]
    public void SetDefence(int amount)
    {
        defence = amount;
    }

    [Server]
    public void Revive(int amount)
    {
        if (enabled)
        {
            Debug.Log("Cant revive " + gameObject.name + "! He is not dead!");
            return;
        }

        enabled = true;
        current = Mathf.Clamp(amount, 0, max);
    }

    public void Damage(int amount, Health inflicter)
    {
        if (!enabled)
            return;

        bool isPlayer = EntityType == EntityType.Player;
        if (isPlayer && isLocalPlayer)
            CmdDamage(amount, inflicter);
        else if (!isPlayer && isServer)
            ServerDamage(amount, inflicter);
    }

    /// <summary>
    /// Subtracts the specified amount from the current health total. Wenn the total
    /// reaches 0 then OnDied is called.
    /// </summary>
    [Command]
    private void CmdDamage(int amount, Health inflicter)
    {
        ServerDamage(amount, inflicter);
    }

    [Server]
    private void ServerDamage(int amount, Health inflicter)
    {
        if (!this || !enabled)
            return;

        Player source = null;
        bool trackStat = EntityType != EntityType.Player && inflicter != null && amount > 0 && inflicter.TryGetComponent(out source) == true;

        if (trackStat)
            StatTracker.Instance.GetStat<DamageInflicted>(source).Add(amount);

        // TODO: Add defence to this
        current = Mathf.Clamp(current - amount, 0, max);
        if (current == 0)
        {
            Debug.Log(gameObject.name + " has died!");

            if (trackStat)
                StatTracker.Instance.GetStat<KillStat>(source).Add(1);

            if (onPlayer != null)
                StatTracker.Instance.GetStat<TimesDied>(onPlayer).Add(1);

            RpcOnDied();
            GetComponent<IDieable>().Die();
            enabled = false;
        }
    }

    /// <summary>
    /// Called when the health points reached 0.
    /// </summary>
    [ClientRpc]
    private void RpcOnDied()
    {
        OnDied?.Invoke();
        FMODUtil.PlayOnTransform(diedSound, transform);
        if (!isServer)
            GetComponent<IDieable>().Die();
    }

    /// <summary>
    /// Called when the current health amount changed.
    /// </summary>
    /// <param name="prevHealth">The previous health amount.</param>
    /// <param name="currentHealth">The current health amount.</param>
    private void OnCurrentChanged(int prevHealth, int currentHealth)
    {
        CurrentChanged?.Invoke(prevHealth, currentHealth);
        CurrentChangedAsPercentage?.Invoke((float)currentHealth / (float)max);

        if (prevHealth > currentHealth)
            FMODUtil.PlayOnTransform(hitSound, transform);
    }

    private void OnMaxChanged(int prevMax, int currentMax)
    {
        MaxChanged?.Invoke(prevMax, currentMax);
    }

    private void OnDefenceChanged(int prevDefence, int currentDefence)
    {
        DefenceChanged?.Invoke(prevDefence, currentDefence);
    }
}
