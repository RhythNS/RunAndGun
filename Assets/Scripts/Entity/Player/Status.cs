using Mirror;
using System.Collections;
using UnityEngine;

public delegate void ReviveEvent();

public class Status : NetworkBehaviour
{
    public bool CanInteract => !Dashing && !Reviving && player.Health.Alive;

    public bool Dashing { get; private set; } = false;
    public bool Reviving { get; private set; } = false;

    public ReviveEvent OnRevivingOtherPlayerStarted;
    public ReviveEvent OnRevivingOtherPlayerFinished;
    public ReviveEvent OnRevived;

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

    public void TryRevive()
    {
        if (!downedPlayerAbleToRevive || downedPlayerAbleToRevive.Health.Alive)
            return;

        player.CmdReviveTeammate(downedPlayerAbleToRevive.gameObject);
    }

    [Server]
    public void ServerReviving(Player player)
    {
        // if player in range
        if (!player || player.Health.Alive)
            return;

        downedPlayerAbleToRevive = player;
        revivingCoroutine = ExtendedCoroutine.ActionAfterSeconds(this, timeForPlayerRevive, OnServerPlayerRevived, true);
        RpcOnRevivingOtherPlayerStarted();
    }

    [Server]
    public void OnServerPlayerRevived()
    {
        if (!downedPlayerAbleToRevive || downedPlayerAbleToRevive.Health.Alive)
            return;

        // TODO: Change the max value to how much the player is revived to.
        downedPlayerAbleToRevive.Health.Revive(downedPlayerAbleToRevive.Health.Max);
        downedPlayerAbleToRevive.Status.RpcOnRevived();
        RpcOnRevivingOtherPlayerFinished();
    }

    [ClientRpc]
    public void RpcOnRevived()
    {
        player.Collider2D.isTrigger = false;
        player.gameObject.layer = LayerDict.Instance.GetPlayerLayer();
        OnRevived?.Invoke();
    }

    [ClientRpc]
    public void RpcOnRevivingOtherPlayerStarted()
    {
        Reviving = true;
        OnRevivingOtherPlayerStarted?.Invoke();
    }

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
        Dashing = true;
        yield return new WaitForSeconds(dashDuration);
        Dashing = false;
        dashTimer = dashCooldown;
    }
}
