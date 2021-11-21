using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// UI Element for showing the bosses name and health.
/// </summary>
public class BossUIManager : MonoBehaviour
{
    public static BossUIManager Instance { get; private set; }

    private Health currentTrackedHealth;

    [SerializeField] private GameObject nameObject;
    [SerializeField] private GameObject healthObject;

    [SerializeField] private TMP_Text nameDisplay;
    [SerializeField] private PercentageAs healthPercentage;
    [SerializeField] private CanvasGroup group;

    [SerializeField] private AnimationCurve nameCurve;

    [SerializeField] private float healthFadeInTime;
    [SerializeField] private AnimationCurve healthCurve;

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("BossUIManager already in scene. Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;
        nameObject.SetActive(false);
        healthObject.SetActive(false);
    }

    /// <summary>
    /// Shows the name first and then the health of an entity.
    /// </summary>
    /// <param name="entity">The entity to be tracked.</param>
    /// <param name="nameDisplayLength">The duration of how long the name should be displayed for.</param>
    public void Show(Entity entity, float nameDisplayLength)
    {
        if (!entity || entity.TryGetComponent(out currentTrackedHealth) == false)
        {
            Debug.LogError("BossUI tried to display an entity (" + (entity ? entity.name : "null") + ") without health!");
            return;
        }

        gameObject.SetActive(true);
        nameObject.SetActive(false);
        healthObject.SetActive(false);

        healthPercentage.UpdateValue((float)(currentTrackedHealth.Current) / (float)(currentTrackedHealth.Max));
        currentTrackedHealth.CurrentChangedAsPercentage += healthPercentage.UpdateValue;
        nameDisplay.text = entity.name;
        currentTrackedHealth.OnDied += OnBossDied;
        StartCoroutine(DoHealthAnimation(nameDisplayLength));
    }

    /// <summary>
    /// Unsubscribe from all subscribed to events.
    /// </summary>
    private void UnsubscribeFromEvents()
    {
        if (currentTrackedHealth)
        {
            currentTrackedHealth.CurrentChangedAsPercentage -= healthPercentage.UpdateValue;
            currentTrackedHealth.OnDied -= OnBossDied;
        }
        currentTrackedHealth = null;
    }

    /// <summary>
    /// Callback for when the boss died.
    /// </summary>
    private void OnBossDied(GameObject _)
    {
        UnsubscribeFromEvents();

        new ExtendedCoroutine
            (this,
            EnumeratorUtil.FadeGroupCurve(group, healthCurve, healthFadeInTime, true),
            OnHealthHidden,
            true
            );
    }

    /// <summary>
    /// Called when the health is fully hidden.
    /// </summary>
    private void OnHealthHidden()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Displays the name first and then displays the health.
    /// </summary>
    /// <param name="nameDisplayLength">How long the name should display for.</param>
    /// <returns></returns>
    private IEnumerator DoHealthAnimation(float nameDisplayLength)
    {
        nameObject.SetActive(true);

        RectTransform rectTransform = nameObject.transform as RectTransform;
        rectTransform.localPosition = new Vector2(0.0f, 0.0f);
        Vector3 pos = new Vector3(0.0f, -rectTransform.rect.height * 1.5f);

        yield return EnumeratorUtil.GoToInSecondsLocalCurve(rectTransform, pos, nameCurve, nameDisplayLength);
        nameObject.SetActive(false);

        group.alpha = 0.0f;
        healthObject.SetActive(true);
        yield return EnumeratorUtil.FadeGroupCurve(group, healthCurve, healthFadeInTime);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;

        UnsubscribeFromEvents();
    }
}
