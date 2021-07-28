using UnityEngine;
using UnityEngine.UI;

public abstract class PanelElement : MonoBehaviour
{
    [SerializeField] protected Button confirmButton;
    [SerializeField] protected Button cancelButton;

    private void Awake()
    {
        confirmButton.onClick.AddListener(OnConfirm);
        cancelButton.onClick.AddListener(OnCancel);
    }

    public void OnConfirm()
    {
        if (InnerOnConfirm())
            Hide();
    }

    public virtual bool InnerOnConfirm()
    {
        return true;
    }

    public void OnCancel()
    {
        if (InnerOnCancel())
            Hide();
    }

    public virtual bool InnerOnCancel()
    {
        return true;
    }

    public void Show()
    {
        gameObject.SetActive(true);
        InnerOnShow();
    }

    public virtual void InnerOnShow() { }

    public void Hide()
    {
        InnerOnHide();
        gameObject.SetActive(false);
    }

    public virtual void InnerOnHide() { }

    private void OnDestroy()
    {
        confirmButton.onClick.RemoveListener(OnConfirm);
        cancelButton.onClick.RemoveListener(OnCancel);
    }
}
