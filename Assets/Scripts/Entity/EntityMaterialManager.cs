using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Manages material effects for an entity.
/// </summary>
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

    /// <summary>
    /// Callback when the current health amount changed.
    /// </summary>
    /// <param name="prevValue">The previous health amount.</param>
    /// <param name="newValue">The new health amount.</param>
    private void OnCurrentChanged(int prevValue, int newValue)
    {
        if (currentTempEffect != null && !currentTempEffect.IsFinshed)
            return;

        Material material = new Material(prevValue > newValue ? MaterialDict.Instance.HitMaterial : MaterialDict.Instance.HealMaterial);
        StartTempEffect(HealthEffect(material, healthEffectDuration), material);
    }

    /// <summary>
    /// Plays a spawn effect. Should be called when the entity was first created.
    /// </summary>
    /// <param name="duration">The duration of the effect.</param>
    public void PlaySpawnEffect(float duration = 0.6f)
    {
        Material spawnMaterial = new Material(MaterialDict.Instance.SpawnMaterial);
        StartTempEffect(SpawnEffect(spawnMaterial, duration), spawnMaterial);
    }

    /// <summary>
    /// Plays a despawn effect. Should be called before the entity is destroyed.
    /// </summary>
    /// <param name="duration">The duration of the effect.</param>
    /// <param name="onFinished">Callback when the effect has finished playing.</param>
    public void PlayDeSpawnEffect(float duration = 0.6f, Action onFinished = null)
    {
        Material spawnMaterial = new Material(MaterialDict.Instance.SpawnMaterial);

        if (currentTempEffect != null && currentTempEffect.IsFinshed == false)
            currentTempEffect.Stop(true);

        spriteRenderer.material = currentEffectMaterial = spawnMaterial;
        currentTempEffect = new ExtendedCoroutine(this, DeSpawnEffect(spawnMaterial, duration), onFinished, true);
    }

    /// <summary>
    /// Handles the displaying of the health effect.
    /// </summary>
    /// <param name="material">The health material.</param>
    /// <param name="duration">The duration of the effect.</param>
    public static IEnumerator HealthEffect(Material material, float duration)
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

    /// <summary>
    /// Handles the displaying of the spawn effect.
    /// </summary>
    /// <param name="material">The spawn material.</param>
    /// <param name="duration">The duration of the effect.</param>
    public static IEnumerator SpawnEffect(Material material, float duration)
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
            material.SetFloat("_FadeAmount", perc);
            yield return null;
        }
    }

    /// <summary>
    /// Handles the displaying of the despawn effect.
    /// </summary>
    /// <param name="material">The despawn material.</param>
    /// <param name="duration">The duration of the effect.</param>
    public static IEnumerator DeSpawnEffect(Material material, float duration)
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
            material.SetFloat("_FadeAmount", 1 - perc);
            yield return null;
        }
    }

    /// <summary>
    /// Handles the displaying a temp effect.
    /// </summary>
    /// <param name="material">The temp material.</param>
    /// <param name="duration">The duration of the effect.</param>
    private void StartTempEffect(IEnumerator enumerator, Material material)
    {
        if (currentTempEffect != null && currentTempEffect.IsFinshed == false)
            return;

        spriteRenderer.material = currentEffectMaterial = material;
        currentTempEffect = new ExtendedCoroutine(this, enumerator, ResetMaterial, true);
    }

    /// <summary>
    /// Resets the material to the default material.
    /// </summary>
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
