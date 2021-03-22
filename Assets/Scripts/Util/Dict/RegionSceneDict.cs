using UnityEngine;

public class RegionSceneDict : MonoBehaviour
{
    public static RegionSceneDict Instance { get; private set; }

    [System.Serializable]
    public struct RegionForSceneName
    {
        public Region region;
        public string sceneName;
    }

    [SerializeField] private RegionForSceneName[] regionForSceneNames;

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("RegionSceneDict already in scene. Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public string GetSceneName(Region region)
    {
        for (int i = 0; i < regionForSceneNames.Length; i++)
        {
            if (regionForSceneNames[i].region == region)
            {
                return regionForSceneNames[i].sceneName;
            }
        }
        throw new System.Exception("Region " + region + " not found!");
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

}
