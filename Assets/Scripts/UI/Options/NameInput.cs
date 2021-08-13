using TMPro;
using UnityEngine;

public class NameInput : PanelElement
{
    [SerializeField] private TMP_InputField nameInput;

    public override bool InnerOnConfirm()
    {
        string name = nameInput.text;
        if (name == null || name.Length < 3 || name.Length > 12)
            return false;

        Player.LocalPlayer.StateCommunicator.CmdChangeName(name);
        Config.Instance.PlayerName = name;
        return true;
    }

    public override void InnerOnShow()
    {
        nameInput.text = Player.LocalPlayer.name;
    }
}
