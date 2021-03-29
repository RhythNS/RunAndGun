using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public enum State
    {
        None, Lobby, Dungeon, Boss
    }

    public static MusicManager Instance { get; private set; }

    [SerializeField] [EventRef] private string eventName;

    private EventInstance instance;
    PARAMETER_ID healthParameter;
    private State currentState = State.None;

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("MusicManager already in scene. Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        instance = RuntimeManager.CreateInstance(eventName);
        instance.getDescription(out EventDescription description);
        description.getParameterDescriptionByName("Health", out PARAMETER_DESCRIPTION healthDesc);
        healthParameter = healthDesc.id;
    }

    public void RegisterPlayer(Player player)
    {
        player.Health.CurrentChangedAsPercentage += HealthChanged;
    }

    public void DeRegisterPlayer()
    {
        Player player = Player.LocalPlayer;
        if (!player)
            return;

        player.Health.CurrentChangedAsPercentage -= HealthChanged;
    }

    private void HealthChanged(float newValue)
    {
        instance.setParameterByID(healthParameter, newValue);
    }

    public void ChangeState(State state)
    {
        if (currentState == state)
            return;

        currentState = state;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;

        instance.release();
        DeRegisterPlayer();
    }
}
