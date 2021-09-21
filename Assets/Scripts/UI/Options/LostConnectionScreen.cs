using UnityEngine;
using UnityEngine.UI;

public class LostConnectionScreen : MonoBehaviour
{
    [SerializeField] private Button button;

    private void Awake()
    {
        button.onClick.AddListener(OnClick);
    }

    public void Show()
    {
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
