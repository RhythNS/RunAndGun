using Mirror;
using UnityEngine;

/// <summary>
/// This class manages the local Weapon Animator. It looks horrible, but I do not know of another way to do this.
/// For any animation event it first triggers the local WeaponAnimator for the event. It then checks if it is
/// run on the server or the client. If it is on the server then it sends the event as an rpc to everyone. If it
/// is on the client then the client sends a cmd to the server which then sends an rpc to all clients.
/// </summary>
public class NetworkWeaponAnimator : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnDirectionChanged))] private Vector2 serverDirection;

    public WeaponAnimator WeaponAnimator { get; private set; }

    private bool serverAuthority = false;

    private void Awake()
    {
        WeaponAnimator = GetComponentInChildren<WeaponAnimator>(); // TODO: <-- maybe change this
    }

    public override void OnStartServer()
    {
        serverAuthority = true;
    }
        //serverAuthority = isServer || GetComponent<Entity>().EntityType == EntityType.Enemy;
        //Debug.Log(gameObject.name + " " + serverAuthority);

    public void OnStartedFire()
    {
        WeaponAnimator.OnStartedFire();

        if (serverAuthority)
            RpcOnStartedFire();
        else
            CmdOnStartedFire();
    }

    public void OnStoppedFire()
    {
        WeaponAnimator.OnStoppedFire();

        if (serverAuthority)
            RpcOnStoppedFire();
        else
            CmdOnStoppedFire();
    }

    public void OnSingleShotFired()
    {
        WeaponAnimator.OnSingleShotFired();
        if (serverAuthority)
            RpcOnSingleShotFired();
        else
            CmdOnSingleShotFired();
    }

    public void OnStartedReload()
    {
        WeaponAnimator.OnStartedReload();

        if (serverAuthority)
            RpcOnStartedReload();
        else
            CmdOnStartedReload();
    }

    public void OnStoppedReload()
    {
        WeaponAnimator.OnStoppedReload();

        if (serverAuthority)
            RpcOnStoppedReload();
        else
            CmdOnStoppedReload();
    }

    public void SetDirection(Vector2 direction)
    {
        WeaponAnimator.SetDirection(direction);

        if (serverAuthority)
            serverDirection = direction;
        else
            CmdSetDirection(direction);
    }

    public void OnWeaponChanged(Weapon newWeapon)
    {
        WeaponAnimator = WeaponAnimator.Replace(WeaponAnimator, newWeapon);
    }

    private void OnDirectionChanged(Vector2 _, Vector2 newDir)
    {
        if (!isLocalPlayer)
            WeaponAnimator.SetDirection(newDir);
    }

    [Command]
    private void CmdOnStartedFire()
    {
        RpcOnStartedFire();
    }

    [Command]
    private void CmdOnStoppedFire()
    {
        RpcOnStoppedFire();
    }

    [Command]
    private void CmdOnSingleShotFired()
    {
        RpcOnSingleShotFired();
    }

    [Command]
    private void CmdOnStartedReload()
    {
        RpcOnStartedReload();
    }

    [Command]
    private void CmdOnStoppedReload()
    {
        RpcOnStoppedReload();
    }

    [Command]
    private void CmdSetDirection(Vector2 direction)
    {
        serverDirection = direction;
    }

    [ClientRpc(includeOwner = false)]
    private void RpcOnStartedFire()
    {
        WeaponAnimator.OnStartedFire();
    }

    [ClientRpc(includeOwner = false)]
    private void RpcOnStoppedFire()
    {
        WeaponAnimator.OnStoppedFire();
    }

    [ClientRpc(includeOwner = false)]
    private void RpcOnSingleShotFired()
    {
        WeaponAnimator.OnSingleShotFired();
    }

    [ClientRpc(includeOwner = false)]
    private void RpcOnStartedReload()
    {
        WeaponAnimator.OnStartedReload();
    }

    [ClientRpc(includeOwner = false)]
    private void RpcOnStoppedReload()
    {
        WeaponAnimator.OnStoppedReload();
    }
}
