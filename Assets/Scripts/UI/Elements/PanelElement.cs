using UnityEngine;
using UnityEngine.UI;

public abstract class PanelElement : MonoBehaviour
{
    [SerializeField] protected Button confirmButton;
    [SerializeField] protected Button cancelButton;

    private void Awake()
    {
        if (confirmButton)
            confirmButton.onClick.AddListener(OnConfirm);
        if (cancelButton)
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
        if (Player.LocalPlayer)
            Player.LocalPlayer.Input.enabled = false;

        gameObject.SetActive(true);
        InnerOnShow();
    }

    public virtual void InnerOnShow() { }

    public void Hide()
    {
        if (Player.LocalPlayer)
            Player.LocalPlayer.Input.enabled = true;

        InnerOnHide();
        gameObject.SetActive(false);
    }

    public virtual void InnerOnHide() { }

    private void OnDestroy()
    {
        if (confirmButton)
            confirmButton.onClick.RemoveListener(OnConfirm);
        if (cancelButton)
            cancelButton.onClick.RemoveListener(OnCancel);
    }
}
