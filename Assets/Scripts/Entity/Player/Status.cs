using FMODUnity;
using Mirror;
using System.Collections;
using UnityEngine;

public delegate void ReviveEvent();

/// <summary>
/// Manages the states that player can be in.
/// </summary>
public class Status : NetworkBehaviour
{
    /// <summary>
    /// Wheter the player can currently interact with anything.
    /// </summary>
    public bool CanInteract => !Dashing && !Reviving && player.Health.Alive;

    /// <summary>
    /// Wheter the player is currently dashing.
    /// </summary>
    public bool Dashing { get; private set; } = false;
    /// <summary>
    /// Wheter the player is currently reviving someone.
    /// </summary>
    public bool Reviving { get; private set; } = false;

    public ReviveEvent OnRevivingOtherPlayerStarted;
    public ReviveEvent OnRevivingOtherPlayerFinished;
    public ReviveEvent OnRevived;

    [SerializeField] [EventRef] public string dashSound;
    [SerializeField] [EventRef] public string reviveSound;

    public Player downedPlayerAbleToRevive;
    private ExtendedCoroutine revivingCoroutine;
    private float timeForPlayerRevive = 3.0f;

    private Player player;
    private Rigidbody2D body;

    private readonly float dashMuliplier = 2.0f;

    private float dashTimer = 0.0f;
    private float dashCooldown = 1.0f;
    private float dashDuration = 0.5f;

    private Vector2 dashVelocity;

    private void Awake()
    {
        player = GetComponent<Player>();
        body = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        player.Stats.OnDodgeChanged += OnDashStatChanged;
        OnDashStatChanged(player.Stats.Dodge);
    }

    private void OnDestroy()
    {
        if (player)
            player.Stats.OnDodgeChanged -= OnDashStatChanged;
    }

    public void OnDashStatChanged(int dashStat)
    {
        SetDashCooldown(PlayerStatsDict.Instance.GetDodgeCooldown(dashStat));
    }

    /// <summary>
    /// Update the time that is needed between the player can dash.
    /// </summary>
    [Server]
    public void SetDashCooldown(float cooldown)
    {
        dashCooldown = cooldown;
    }

    public void OnDownedPlayerInRangeToRevive(Player player)
    {
        downedPlayerAbleToRevive = player;
        // UI.ShowPlayerCanBeRevived(player);
    }

    public void OnDownedPlayerNoLongerInRange()
    {
        downedPlayerAbleToRevive = null;
    }

    /// <summary>
    /// Tries to revive a nearby team mate.
    /// </summary>
    public void TryRevive()
    {
        if (!downedPlayerAbleToRevive || downedPlayerAbleToRevive.Health.Alive)
            return;

        CmdReviveTeammate(downedPlayerAbleToRevive.gameObject);
    }

    /// <summary>
    /// Issues a command to the server that the player wants to revive another player.
    /// </summary>
    /// <param name="other">The player to be revived.</param>
    [Command]
    public void CmdReviveTeammate(GameObject other)
    {
        if (other.TryGetComponent(out Player player) == false)
            return;

        ServerReviving(player);
    }

    /// <summary>
    /// Revives the given player.
    /// </summary>
    [Server]
    public void ServerReviving(Player player)
    {
        // Maybe add a check if the player is in range.

        if (!player || player.Health.Alive)
            return;

        downedPlayerAbleToRevive = player;
        revivingCoroutine = ExtendedCoroutine.ActionAfterSeconds(this, timeForPlayerRevive, OnServerPlayerRevived, true);
        RpcOnRevivingOtherPlayerStarted();
    }

    /// <summary>
    /// Called when the player finished reviving another player.
    /// </summary>
    [Server]
    public void OnServerPlayerRevived()
    {
        if (!downedPlayerAbleToRevive || downedPlayerAbleToRevive.Health.Alive)
            return;

        StatTracker.Instance.GetStat<OtherPlayerRevived>(player).Add(1);
        // TODO: Change the max value to how much the player is revived to.
        downedPlayerAbleToRevive.Health.Revive(downedPlayerAbleToRevive.Health.Max);
        downedPlayerAbleToRevive.Status.RpcOnRevived();
        RpcOnRevivingOtherPlayerFinished();
    }

    /// <summary>
    /// Called on the client if this player was revived.
    /// </summary>
    [ClientRpc]
    public void RpcOnRevived()
    {
        player.Collider2D.isTrigger = false;
        player.gameObject.layer = LayerDict.Instance.GetPlayerLayer();
        FMODUtil.PlayOnTransform(reviveSound, transform);
        OnRevived?.Invoke();
    }

    /// <summary>
    /// Called when the player has started the revive.
    /// </summary>
    [ClientRpc]
    public void RpcOnRevivingOtherPlayerStarted()
    {
        Reviving = true;
        OnRevivingOtherPlayerStarted?.Invoke();
    }

    /// <summary>
    /// Called when the player has finished the revive.
    /// </summary>
    [ClientRpc]
    public void RpcOnRevivingOtherPlayerFinished()
    {
        Reviving = false;
        OnRevivingOtherPlayerFinished?.Invoke();
    }

    private void Update()
    {
        if (Dashing)
        {
            body.velocity = dashVelocity;
            return;
        }

        dashTimer -= Time.deltaTime;
    }

    /// <summary>
    /// Tries to engage dashing.
    /// </summary>
    /// <returns>Wheter it successeded or not.</returns>
    public bool TryDashing()
    {
        if (Dashing == true || dashTimer > 0.0f)
            return false;

        dashVelocity = body.velocity * dashMuliplier;

        StartCoroutine(Dash());

        return true;
    }

    private IEnumerator Dash()
    {
        FMODUtil.PlayOnTransform(dashSound, transform);
        Dashing = true;
        yield return new WaitForSeconds(dashDuration);
        Dashing = false;
        dashTimer = dashCooldown;
    }
}
