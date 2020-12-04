using TMPro;
using UnityEngine;

public class HealthUIUpdater : MonoBehaviour
{
    [SerializeField] private Health trackingHealth;
    private TMP_Text text;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }

    private void Update()
    {
        text.text = "Health: " + trackingHealth.CurrentHealth;
    }
}
