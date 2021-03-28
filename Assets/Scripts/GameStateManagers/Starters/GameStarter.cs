using UnityEngine;

public class GameStarter : MonoBehaviour
{
    private void Start()
    {
        new ExtendedCoroutine(this, RegionSceneLoader.Instance.LoadScene(Region.Lobby), DestroySelf, true);
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
