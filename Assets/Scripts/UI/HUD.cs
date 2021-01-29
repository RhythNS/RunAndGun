using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [Header("LocalPlayer")]
    [SerializeField]
    private Text textHpNum;
    [SerializeField]
    private Image imgHpBar;
    [SerializeField]
    private Image imgPlayerIcon;

    [Header("Other Players")]
    [SerializeField]
    private Text[] textPlayersHpNum;
    [SerializeField]
    private Image[] imgPlayersHpBar;
    [SerializeField]
    private Image[] imgPlayersPlayerIcon;

    [Header("Boss")]
    [SerializeField]
    private Text textBossHpNum;
    [SerializeField]
    private Image imgBossHpBar;
    [SerializeField]
    private Image imgBossIcon;

    [Header("Weapons")]
    [SerializeField]
    private Text textWeaponName;
    [SerializeField]
    private Text textAmmoCount;
    [SerializeField]
    private Image imgWeaponIcon;
}
