using FMOD.Studio;
using FMODUnity;
using UnityEngine;

/// <summary>
/// Manges the music of the entire game.
/// </summary>
public class MusicManager : MonoBehaviour
{
    /// <summary>
    /// The state the music should be in.
    /// </summary>
    public enum State
    {
        None, Lobby, Dungeon, Boss
    }

    public static MusicManager Instance { get; private set; }

    [SerializeField] [EventRef] private string eventName;

    private EventInstance instance;
    PARAMETER_ID healthParameter;
    PARAMETER_ID stateParameter;
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
        description.getParameterDescriptionByName("State", out PARAMETER_DESCRIPTION stateDesc);
        healthParameter = healthDesc.id;
        stateParameter = stateDesc.id;

        instance.start();
    }

    /// <summary>
    /// Register the local player.
    /// </summary>
    /// <param name="player">The local player.</param>
    public void RegisterPlayer(Player player)
    {
        player.Health.CurrentChangedAsPercentage += HealthChanged;
        instance.setParameterByID(healthParameter, 1.0f);
    }

    /// <summary>
    /// Deregister the local player.
    /// </summary>
    public void DeRegisterPlayer()
    {
        Player player = Player.LocalPlayer;
        if (!player)
            return;

        player.Health.CurrentChangedAsPercentage -= HealthChanged;
    }

    /// <summary>
    /// Callback for when the current health value changed.
    /// </summary>
    /// <param name="newValue">The new current health value.</param>
    private void HealthChanged(float newValue)
    {
        instance.setParameterByID(healthParameter, newValue);
    }

    /// <summary>
    /// Change the state of the music.
    /// </summary>
    /// <param name="state">The new state.</param>
    public void ChangeState(State state)
    {
        if (currentState == state)
            return;

        currentState = state;
        instance.setParameterByID(stateParameter, (int)state);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;

        instance.release();
        DeRegisterPlayer();
    }
}
