using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RegionSceneLoader : MonoBehaviour
{
    public static RegionSceneLoader Instance { get; private set; }

    private GenerateLevelMessage generateLevelMessage;

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("RegionSceneLoader already in scene. Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private int numberOfOperationsNotDone = 0;

    public IEnumerator LoadScene(GenerateLevelMessage generateLevelMessage)
    {
        this.generateLevelMessage = generateLevelMessage;
        yield return LoadScene(generateLevelMessage.region);
    }

    public IEnumerator LoadScene(Region region)
    {
        if (RegionDict.Instance)
        {
            if (RegionDict.Instance.Region == region)
                yield break;

            AsyncOperation unLoad = SceneManager.UnloadSceneAsync(RegionSceneDict.Instance.GetSceneName(RegionDict.Instance.Region));
            unLoad.completed += OperationFinished;
            numberOfOperationsNotDone++;
        }

        AsyncOperation load = SceneManager.LoadSceneAsync(RegionSceneDict.Instance.GetSceneName(region), LoadSceneMode.Additive);
        load.completed += OperationFinished;
        numberOfOperationsNotDone++;

        while (numberOfOperationsNotDone != 0)
            yield return null;
    }

    public void LoadLevel()
    {
        DungeonCreator.Instance.CreateLevel(generateLevelMessage.levelNumber);
    }

    private void OperationFinished(AsyncOperation operation)
    {
        operation.completed -= OperationFinished;
        numberOfOperationsNotDone--;
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
