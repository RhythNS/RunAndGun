using TMPro;
using UnityEngine;

public class IntAsText : IntAs
{
    [SerializeField] private TMP_Text text;

    public override void UpdateValue(int value)
    {
        text.text = value.ToString();
    }
}
