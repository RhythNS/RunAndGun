using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manges fmod bus volumes.
/// </summary>
public class VolumeManager : MonoBehaviour
{
    public static VolumeManager Instance { get; private set; }

    [System.Serializable]
    public struct BusAndName
    {
        public string path;
        public string name;
        public float initialVolume;
    }

    [SerializeField] private BusAndName[] busses;

    private Dictionary<string, Bus> nameForBus;

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("VolumeManager already in scene. Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        nameForBus = new Dictionary<string, Bus>();

        for (int i = 0; i < busses.Length; i++)
        {
            Bus bus = RuntimeManager.GetBus(busses[i].path);
            bus.setVolume(busses[i].initialVolume);
            nameForBus.Add(busses[i].name, bus);
        }
    }

    /// <summary>
    /// Set the volume for a specific bus.
    /// </summary>
    /// <param name="volume">The new volume.</param>
    /// <param name="name">The name of the bus.</param>
    public void SetVolume(float volume, string name)
    {
        if (nameForBus.TryGetValue(name, out Bus bus) == false)
        {
            Debug.LogError("Could not find bus with name: " + name);
            return;
        }

        bus.setVolume(volume);
    }

    /// <summary>
    /// Get the volume for a specific bus.
    /// </summary>
    /// <param name="name">The name of the bus.</param>
    /// <returns>The volume.</returns>
    public float GetVolume(string name)
    {
        if (nameForBus.TryGetValue(name, out Bus bus) == false)
        {
            Debug.LogError("Could not find bus with name: " + name);
            return -1;
        }

        bus.getVolume(out float volume);
        return volume;
    }

    /// <summary>
    /// Save the volumes for all busses.
    /// </summary>
    public void Save()
    {
        Tuple<string, float>[] volumes = new Tuple<string, float>[busses.Length];
        for (int i = 0; i < busses.Length; i++)
        {
            nameForBus[busses[i].name].getVolume(out float level);
            volumes[i] = new Tuple<string, float>(busses[i].name, level);
        }
        Config.Instance.volumes = volumes;
        Config.Instance.Save();
    }

    /// <summary>
    /// Load the volume levels of all busses.
    /// </summary>
    /// <param name="volumes"></param>
    public void Load(Tuple<string, float>[] volumes)
    {
        if (volumes == null)
            return;

        for (int i = 0; i < volumes.Length; i++)
        {
            if (nameForBus.TryGetValue(volumes[i].Item1, out Bus bus) == true)
                bus.setVolume(volumes[i].Item2);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

}
