using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// UI element for displaying the current gun and magazine size.
/// </summary>
public class WeaponManager : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] IntAs bulletsLeftDisplay;
    [SerializeField] PercentageAs bulletsLeftPercentage;
    [SerializeField] private Image gunIcon;
    [SerializeField] private Sprite emptySprite;

    private ExtendedCoroutine reloadCoroutine;
    private int maxBullets;

    /// <summary>
    /// Registers to all relevant events.
    /// </summary>
    /// <param name="player">The player that should be tracked.</param>
    public void RegisterEvents(Player player)
    {
        player.EquippedWeapon.BulletCountChanged += BulletCountChanged;
        player.EquippedWeapon.ReloadingBegins += ReloadingBegins;
        player.EquippedWeapon.WeaponChanged += WeaponChanged;
        SetNullWeapon();
    }

    /// <summary>
    /// Sets to display to the default state.
    /// </summary>
    private void SetNullWeapon()
    {
        gunIcon.sprite = emptySprite;
        bulletsLeftDisplay.UpdateValue(0);
        bulletsLeftDisplay.gameObject.SetActive(false);
        bulletsLeftPercentage.UpdateValue(1.0f);
    }

    /// <summary>
    /// Called when the weapon changed.
    /// </summary>
    /// <param name="newWeapon">The new weapon.</param>
    private void WeaponChanged(Weapon newWeapon)
    {
        if (reloadCoroutine != null && reloadCoroutine.IsFinshed == false)
            reloadCoroutine.Stop();

        if (!newWeapon)
        {
            SetNullWeapon();
            return;
        }

        bulletsLeftDisplay.gameObject.SetActive(true);
        gunIcon.sprite = newWeapon.Icon;
        bulletsLeftDisplay.UpdateValue(maxBullets = newWeapon.MagazineSize);
        bulletsLeftPercentage.UpdateValue(1.0f);
    }

    /// <summary>
    /// Called when the reloading started.
    /// </summary>
    /// <param name="reloadTime">The time it takes to reload the weapon.</param>
    private void ReloadingBegins(float reloadTime)
    {
        if (reloadCoroutine != null && reloadCoroutine.IsFinshed == false)
            reloadCoroutine.Stop();

        reloadCoroutine = new ExtendedCoroutine(this, ReloadAnimation(reloadTime), startNow: true);
    }

    /// <summary>
    /// Called when the current magazine count changed.
    /// </summary>
    /// <param name="amount">T´he new amount of bullets.</param>
    private void BulletCountChanged(int amount)
    {
        if (maxBullets == 0)
            return;

        if (reloadCoroutine != null && reloadCoroutine.IsFinshed == false)
            reloadCoroutine.Stop();

        bulletsLeftDisplay.UpdateValue(amount);
        bulletsLeftPercentage.UpdateValue((float)amount / (float)maxBullets);
    }

    /// <summary>
    /// Unregisters all events.
    /// </summary>
    /// <param name="player">The player that was tracked.</param>
    public void UnRegisterEvents(Player player)
    {
        player.EquippedWeapon.BulletCountChanged -= BulletCountChanged;
        player.EquippedWeapon.ReloadingBegins -= ReloadingBegins;
        player.EquippedWeapon.WeaponChanged -= WeaponChanged;
    }

    /// <summary>
    /// Does the reload animation.
    /// </summary>
    /// <param name="reloadTime">The duration of the animation.</param>
    private IEnumerator ReloadAnimation(float reloadTime)
    {
        float timer = 0.0f;
        bool shouldContinue = true;
        do
        {
            timer += Time.deltaTime;
            float progress = timer / reloadTime;
            if (progress > 1.0f)
            {
                progress = 1.0f;
                shouldContinue = false;
            }

            bulletsLeftPercentage.UpdateValue(progress);
            yield return null;
        } while (shouldContinue);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Player.LocalPlayer.EquippedWeapon.Reload();
    }
}
