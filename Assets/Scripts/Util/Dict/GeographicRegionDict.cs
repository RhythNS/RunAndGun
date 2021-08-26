using UnityEngine;

/// <summary>
/// Dict for what geographic regions a player can connect to.
/// </summary>
public class GeographicRegionDict : MonoBehaviour
{
    public static GeographicRegionDict Instance { get; private set; }

    [System.Serializable]
    public struct RegionName
    {
        public string name;
        public NobleConnect.GeographicRegion region;
    }

    [SerializeField] RegionName[] regionNames;

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("GeographicRegionDict already in scene. Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Get the display name of the region.
    /// </summary>
    /// <param name="region">The region to get the name for.</param>
    /// <returns>The name of the region.</returns>
    public string GetName(NobleConnect.GeographicRegion region)
    {
        for (int i = 0; i < regionNames.Length; i++)
        {
            if (regionNames[i].region == region)
                return regionNames[i].name;
        }

        Debug.LogError("Could not find region in geographicregiondict: " + region);
        return default;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

}
