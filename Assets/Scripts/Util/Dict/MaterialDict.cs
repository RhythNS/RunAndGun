using UnityEngine;

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

    public Material HitMaterial => hitMaterial;
    [SerializeField] private Material hitMaterial;
    
    public Material HealMaterial => healMaterial;
    [SerializeField] private Material healMaterial;

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
