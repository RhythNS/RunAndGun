using TMPro;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    private void OnEnable()
    {
        GetComponent<TMP_Text>().text = "You died!\nScore: " + ScoreUpdater.GetScore() + "\nPress R to respawn!";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
