using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using UnityEngine;

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

    public void SetVolume(float volume, string name)
    {
        if (nameForBus.TryGetValue(name, out Bus bus) == false)
        {
            Debug.LogError("Could not find bus with name: " + name);
            return;
        }

        bus.setVolume(volume);
    }

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

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

}
