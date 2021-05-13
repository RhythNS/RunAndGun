using System;
using UnityEngine;

public class InGameManager : MonoBehaviour
{
    [SerializeField] private PercentageAs healthPercentage;
    [SerializeField] private IntAs moneyAmount;
    [SerializeField] private WeaponManager weaponManager;
    [SerializeField] private MiniMapManager miniMapManager;

    public void RegisterEvents(Player player)
    {
        gameObject.SetActive(true);
        player.Health.CurrentChangedAsPercentage += healthPercentage.UpdateValue;
        healthPercentage.UpdateValue(1.0f);
        player.Inventory.OnMoneyAmountChanged += moneyAmount.UpdateValue;
        moneyAmount.UpdateValue(player.Inventory.money);
        weaponManager.RegisterEvents(player);
    }

    public void UnRegisterEvents()
    {
        gameObject.SetActive(false);
        Player player = Player.LocalPlayer;
        if (!player)
            return;

        player.Health.CurrentChangedAsPercentage -= healthPercentage.UpdateValue;
        player.Inventory.OnMoneyAmountChanged -= moneyAmount.UpdateValue;

        if (weaponManager)
            weaponManager.UnRegisterEvents(player);
    }
}
