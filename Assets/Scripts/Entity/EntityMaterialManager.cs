using System.Collections;
using UnityEngine;

public class EntityMaterialManager : MonoBehaviour
{
    private Material originalMaterial, currentEffectMaterial;

    private SpriteRenderer spriteRenderer;
    private ExtendedCoroutine currentTempEffect;

    private readonly float healthEffectDuration = 0.3f;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;
    }

    private void Start()
    {
        Health health = GetComponent<Health>();
        health.CurrentChanged += OnCurrentChanged;
    }

    private void OnCurrentChanged(int prevValue, int newValue)
    {
        if (currentTempEffect != null && !currentTempEffect.IsFinshed)
            return;

        if (prevValue > newValue)
            StartTempEffect(new Material(MaterialDict.Instance.HitMaterial), healthEffectDuration);
        else
            StartTempEffect(new Material(MaterialDict.Instance.HealMaterial), healthEffectDuration);
    }

    private IEnumerator HealthEffect(Material material, float duration)
    {
        float halfDuration = duration * 0.5f;
        float timer = 0;
        bool continueExecuting = true;
        while (continueExecuting)
        {
            timer += Time.deltaTime;
            float perc = halfDuration / timer;
            if (perc > 1.0f)
            {
                perc = 1.0f;
                continueExecuting = false;
            }
            material.SetFloat("Hit Effect Blend", perc);
            yield return null;
        }

        continueExecuting = true;
        while (continueExecuting)
        {
            timer += Time.deltaTime;
            float perc = halfDuration / timer;
            if (perc > 1.0f)
            {
                perc = 1.0f;
                continueExecuting = false;
            }
            material.SetFloat("Hit Effect Blend", 1 - perc);
            yield return null;
        }
    }

    private void StartTempEffect(Material material, float duration)
    {
        if (currentTempEffect != null && currentTempEffect.IsFinshed == false)
            return;

        spriteRenderer.material = currentEffectMaterial = material;
        currentTempEffect = new ExtendedCoroutine(this, HealthEffect(material, duration), ResetMaterial, true);
    }

    private void ResetMaterial()
    {
        spriteRenderer.material = originalMaterial;
        Destroy(currentEffectMaterial);
        currentEffectMaterial = null;
    }

    private void OnDestroy()
    {
        if (TryGetComponent(out Health health))
        {
            health.CurrentChanged -= OnCurrentChanged;
        }
        if (currentEffectMaterial)
            Destroy(currentEffectMaterial);
    }
}
