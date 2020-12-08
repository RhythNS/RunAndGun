using TMPro;
using UnityEngine;

public class Stairs : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player _))
        {
            Respawn.Instance.gameObject.SetActive(true);
            Respawn.Instance.GetComponent<TMP_Text>().text = "You won!\nScore: " + ScoreUpdater.GetScore() + "\nPress R to respawn!"; ;
            Destroy(gameObject);
        }
    }
}
