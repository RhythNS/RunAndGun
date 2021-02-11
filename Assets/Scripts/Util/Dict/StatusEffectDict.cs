using UnityEngine;

public class StatusEffectDict : MonoBehaviour
{
    public static StatusEffectDict Instance { get; private set; }

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("StatusEffectDict already in scene! Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public ParticleSystem PosionParticleSystem => posionParticleSystem;
    [SerializeField] private ParticleSystem posionParticleSystem;

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
