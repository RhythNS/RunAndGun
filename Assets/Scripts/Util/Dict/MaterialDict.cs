using UnityEngine;

/// <summary>
/// Dict for kinds of materials.
/// </summary>
public class MaterialDict : MonoBehaviour
{
    public static MaterialDict Instance { get; private set; }

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("MaterialDict already in scene! Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Material of an entity that just got hit.
    /// </summary>
    public Material HitMaterial => hitMaterial;
    [SerializeField] private Material hitMaterial;

    /// <summary>
    /// Material of an entity that just got healed.
    /// </summary>
    public Material HealMaterial => healMaterial;
    [SerializeField] private Material healMaterial;

    /// <summary>
    /// Material of an entity that just spawned.
    /// </summary>
    public Material SpawnMaterial => spawnMaterial;
    [SerializeField] private Material spawnMaterial;

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
