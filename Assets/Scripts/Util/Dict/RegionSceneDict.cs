using UnityEngine;

/// <summary>
/// Dict for data used to get the name of a region based on the enum value of the region.
/// </summary>
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

    /// <summary>
    /// Gets the name of a scene.
    /// </summary>
    /// <param name="region">The region to get the name to.</param>
    /// <returns>The name of the scene.</returns>
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
