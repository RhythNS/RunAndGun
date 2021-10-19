using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LostConnectionScreen : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text bottomText;

    [SerializeField] private string lostConnectionText;
    [SerializeField] private string kickedText;


    private void Awake()
    {
        button.onClick.AddListener(OnClick);
    }

    public void Show(bool kicked)
    {
        bottomText.text = kicked ? kickedText : lostConnectionText;
        gameObject.SetActive(true);
    }

    private void OnClick()
    {
        gameObject.SetActive(false);
        GlobalsDict.Instance.StartCoroutine(RegionSceneLoader.Instance.LoadScene(Region.Lobby));
    }

    private void OnDestroy()
    {
        button.onClick.RemoveListener(OnClick);
    }
}
