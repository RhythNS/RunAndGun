using TMPro;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    public static Respawn Instance { get; private set; }
    [SerializeField] private int sceneToLoad = 0;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        GetComponent<TMP_Text>().text = "You died!\nScore: " + ScoreUpdater.GetScore() + "\nPress R to respawn!";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
    }
}
