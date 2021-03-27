using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameManager : MonoBehaviour
{
    [SerializeField] private PercentageAs healthPercentage;

    public void RegisterEvents(Player player)
    {
        gameObject.SetActive(true);
        player.Health.CurrentChangedAsPercentage += healthPercentage.UpdateValue;
    }

    public void UnRegisterEvents()
    {
        gameObject.SetActive(false);
        Player player = Player.LocalPlayer;
        if (!player)
            return;

        player.Health.CurrentChangedAsPercentage -= healthPercentage.UpdateValue;

    }
}
