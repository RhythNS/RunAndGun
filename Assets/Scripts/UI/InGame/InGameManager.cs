using UnityEngine;

/// <summary>
/// UI manager for all ui elements that are relevant during gameplay.
/// </summary>
public class InGameManager : MonoBehaviour
{
    [SerializeField] private PercentageAs healthPercentage;
    [SerializeField] private IntAs moneyAmount;
    [SerializeField] private WeaponManager weaponManager;
    [SerializeField] private MiniMapManager miniMapManager;
    [SerializeField] private EmoteManager emoteManager;
    [SerializeField] private EmoteBoard emoteBoard;

    public void RegisterEvents(Player player)
    {
        gameObject.SetActive(true);
        player.Health.CurrentChangedAsPercentage += healthPercentage.UpdateValue;
        healthPercentage.UpdateValue(1.0f);
        player.Inventory.OnMoneyAmountChanged += moneyAmount.UpdateValue;
        moneyAmount.UpdateValue(player.Inventory.money);
        weaponManager.RegisterEvents(player);
        emoteBoard.ClearAll();
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

    public void OnPlayerEmoted(EmoteMessage emoteMessage) => emoteBoard.OnPlayerEmoted(emoteMessage);

    public void ToggleEmotePanel()
    {
        if (emoteManager.Hidden)
            emoteManager.Show();
        else
            emoteManager.Hide();
    }
}
