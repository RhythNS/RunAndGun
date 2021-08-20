using UnityEngine;

public class GameStarter : MonoBehaviour
{
    [SerializeField] private Region regionToLoad;

    private void Start()
    {
        if (regionToLoad != Region.Lobby)
            Debug.Log("Game starting not in lobby! To change this goto the GameStarter Object and change the region to load!");

        new ExtendedCoroutine(this, RegionSceneLoader.Instance.LoadScene(regionToLoad), DestroySelf, true);
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
