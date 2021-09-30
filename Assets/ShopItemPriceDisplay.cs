using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopItemPriceDisplay : MonoBehaviour
{
    [SerializeField]
    private TextMeshPro tmp;

    /// <summary>
    /// Sets the price on the sign.
    /// </summary>
    /// <param name="price">The price to set.</param>
    public void SetPrice(uint price)
    {
        tmp.text = price.ToString();
    }
}
