﻿public class JoinServerZone : EnterZone
{
    public override void OnEnter(Player player)
    {
        UIManager.Instance.OptionsManager.ShowJoinServer();
    }

    public override void OnExit(Player player)
    {
        UIManager.Instance.OptionsManager.HideJoinServer();
    }
}
