using UnityEngine;
using UnityEngine.UI;

public class GameModeConfig : PanelElement
{

    [SerializeField] private GameMode testRoom;
    [SerializeField] private GameMode normal;

    public void SetTestRoom()
    {
        LobbyManager.ChangeGameMode(testRoom);
    }

    public void SetNormal()
    {
        LobbyManager.ChangeGameMode(normal);
    }

    public override void InnerOnShow()
    {
        base.InnerOnShow();
    }

    public override bool InnerOnCancel()
    {
        return base.InnerOnCancel();
    }

    public override bool InnerOnConfirm()
    {
        return base.InnerOnConfirm();
    }
}
