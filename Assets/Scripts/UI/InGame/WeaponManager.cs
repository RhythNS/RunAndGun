using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] IntAs bulletsLeftDisplay;
    [SerializeField] PercentageAs bulletsLeftPercentage;
    [SerializeField] private Image gunIcon;
    [SerializeField] private Sprite emptySprite;

    private ExtendedCoroutine reloadCoroutine;
    private int maxBullets;

    public void RegisterEvents(Player player)
    {
        player.EquippedWeapon.BulletCountChanged += BulletCountChanged;
        player.EquippedWeapon.ReloadingBegins += ReloadingBegins;
        player.EquippedWeapon.WeaponChanged += WeaponChanged;
        SetNullWeapon();
    }

    private void SetNullWeapon()
    {
        gunIcon.sprite = emptySprite;
        bulletsLeftDisplay.UpdateValue(0);
        bulletsLeftDisplay.gameObject.SetActive(false);
        bulletsLeftPercentage.UpdateValue(1.0f);
    }

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

    private void ReloadingBegins(float reloadTime)
    {
        if (reloadCoroutine != null && reloadCoroutine.IsFinshed == false)
            reloadCoroutine.Stop();

        reloadCoroutine = new ExtendedCoroutine(this, ReloadAnimation(reloadTime), startNow: true);
    }

    private void BulletCountChanged(int amount)
    {
        if (maxBullets == 0)
            return;

        if (reloadCoroutine != null && reloadCoroutine.IsFinshed == false)
            reloadCoroutine.Stop();

        bulletsLeftDisplay.UpdateValue(amount);
        bulletsLeftPercentage.UpdateValue((float)amount / (float)maxBullets);
    }

    public void UnRegisterEvents(Player player)
    {
        player.EquippedWeapon.BulletCountChanged -= BulletCountChanged;
        player.EquippedWeapon.ReloadingBegins -= ReloadingBegins;
        player.EquippedWeapon.WeaponChanged -= WeaponChanged;
    }

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
