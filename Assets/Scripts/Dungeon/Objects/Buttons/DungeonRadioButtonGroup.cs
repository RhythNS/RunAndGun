using UnityEngine;

public class DungeonRadioButtonGroup : DungeonButtonGroup
{
    public int CurrentOn => currentOn;
    [SerializeField] private int currentOn = 0;

    public event IntChanged OnSelectedButtonChanged;

    public override void OnStartServer()
    {
        base.OnStartServer();

        if (buttons.Length == 0)
            return;

        if (currentOn < 0 || currentOn >= buttons.Length)
        {
            Debug.LogWarning("Buttongroup " + name + " had an invaild default on (" +
                currentOn + "/" + buttons.Length + ")!");
            currentOn = 0;
        }

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].SetPressed(false, false);
            buttons[i].SetStayPressed(true);
        }

        buttons[currentOn].SetPressed(true);
    }

    public override void OnButtonChanged(DungeonButton dungeonButton, bool newPressed)
    {
        if (newPressed == false)
            return;

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] == dungeonButton)
                currentOn = i;
            else
                buttons[i].SetPressed(false, false);
        }

        OnSelectedButtonChanged?.Invoke(currentOn);
    }
}
