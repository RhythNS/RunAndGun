using UnityEngine;
using UnityEngine.UI;

public class LostConnectionScreen : MonoBehaviour
{
    [SerializeField] private Button button;


    public static LostConnectionScreen Instance { get; private set; }

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogWarning("LostConnectionScreen already in scene. Deleting myself!");
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        button.onClick.AddListener(OnClick);
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    private void OnClick()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;

        button.onClick.RemoveListener(OnClick);
    }
}
