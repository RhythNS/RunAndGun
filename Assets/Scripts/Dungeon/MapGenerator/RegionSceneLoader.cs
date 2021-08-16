using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the un- loading of regions.
/// </summary>
public class RegionSceneLoader : MonoBehaviour
{
    /// <summary>
    /// Singleton.
    /// </summary>
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

    /// <summary>
    /// Loads new regions and unloads the old scenes.
    /// </summary>
    public IEnumerator LoadScene(GenerateLevelMessage generateLevelMessage)
    {
        this.generateLevelMessage = generateLevelMessage;
        yield return LoadScene(generateLevelMessage.region);
    }

    /// <summary>
    /// Loads new regions and unloads the old scenes.
    /// </summary>
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

    /// <summary>
    /// Loads the level that was specified in the GenerateLevelMessage.
    /// </summary>
    public void LoadLevel()
    {
        DungeonCreator.Instance.CreateLevel(generateLevelMessage.levelNumber);
    }

    /// <summary>
    /// Callback for when an async operation finished.
    /// </summary>
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
