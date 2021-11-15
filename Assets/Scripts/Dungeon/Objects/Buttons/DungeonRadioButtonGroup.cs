using UnityEngine;

public class DungeonRadioButtonGroup : DungeonButtonGroup
{
    [SerializeField] private int defaultOn = 0;

    public override void OnStartServer()
    {
        base.OnStartServer();

        if (buttons.Length == 0)
            return;

        if (defaultOn < 0 || defaultOn >= buttons.Length)
        {
            Debug.LogWarning("Buttongroup " + name + " had an invaild default on (" +
                defaultOn + "/" + buttons.Length + ")!");
            defaultOn = 0;
        }

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].SetPressed(false, false);
            buttons[i].SetStayPressed(true);
        }

        buttons[defaultOn].SetPressed(true);
    }

    public override void OnButtonChanged(DungeonButton dungeonButton, bool newPressed)
    {
        if (newPressed == false)
            return;

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] != dungeonButton)
                buttons[i].SetPressed(false, false);
        }
    }
}
