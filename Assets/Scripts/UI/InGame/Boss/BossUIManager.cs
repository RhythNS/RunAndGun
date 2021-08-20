using System.Collections;
using TMPro;
using UnityEngine;

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

        currentTrackedHealth.CurrentChangedAsPercentage += healthPercentage.UpdateValue;
        nameDisplay.text = entity.name;
        currentTrackedHealth.OnDied += OnBossDied;
        StartCoroutine(DoHealthAnimation(nameDisplayLength));
    }

    private void UnsubscribeFromEvents()
    {
        if (currentTrackedHealth)
        {
            currentTrackedHealth.CurrentChangedAsPercentage -= healthPercentage.UpdateValue;
            currentTrackedHealth.OnDied -= OnBossDied;
        }
        currentTrackedHealth = null;

    }

    private void OnBossDied()
    {
        UnsubscribeFromEvents();

        new ExtendedCoroutine
            (this,
            EnumeratorUtil.FadeGroupCurve(group, healthCurve, healthFadeInTime, true),
            OnHealthHidden,
            true
            );
    }

    private void OnHealthHidden()
    {
        gameObject.SetActive(false);
    }

    private IEnumerator DoHealthAnimation(float nameDisplayLength)
    {
        nameObject.SetActive(true);
        RectTransform rectTransform = nameObject.transform as RectTransform;
        Vector3 pos = rectTransform.localPosition;
        rectTransform.anchoredPosition = new Vector2(0.0f, 0.0f);

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
