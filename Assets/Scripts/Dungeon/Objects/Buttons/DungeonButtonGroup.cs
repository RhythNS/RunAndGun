using Mirror;
using UnityEngine;

public abstract class DungeonButtonGroup : NetworkBehaviour
{
    [SerializeField] protected DungeonButton[] buttons;

    public override void OnStartServer()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].group = this;
        }
    }

    public abstract void OnButtonChanged(DungeonButton dungeonButton, bool newPressed);
}
