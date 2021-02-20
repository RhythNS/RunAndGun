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

        Material material = new Material(prevValue > newValue ? MaterialDict.Instance.HitMaterial : MaterialDict.Instance.HealMaterial);
        StartTempEffect(HealthEffect(material, healthEffectDuration), material);
    }

    public void PlaySpawnEffect(float duration = 0.6f)
    {
        Material spawnMaterial = new Material(MaterialDict.Instance.SpawnMaterial);
        StartTempEffect(SpawnEffect(spawnMaterial, duration), spawnMaterial);
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

    private IEnumerator SpawnEffect(Material material, float duration)
    {
        float timer = duration;
        bool continueExecuting = true;
        while (continueExecuting)
        {
            timer -= Time.deltaTime;
            float perc = timer / duration;
            if (perc < 0.0f)
            {
                continueExecuting = false;
                perc = 0.0f;
            }
            Debug.Log(perc);
            material.SetFloat("_FadeAmount", perc);
            yield return null;
        }
    }

    private void StartTempEffect(IEnumerator enumerator, Material material)
    {
        if (currentTempEffect != null && currentTempEffect.IsFinshed == false)
            return;

        spriteRenderer.material = currentEffectMaterial = material;
        currentTempEffect = new ExtendedCoroutine(this, enumerator, ResetMaterial, true);
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
