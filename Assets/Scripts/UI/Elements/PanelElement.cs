using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A UI Element that can be confirmed or canceled.
/// </summary>
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

    /// <summary>
    /// Caled when the confirm button was pressed.
    /// </summary>
    public void OnConfirm()
    {
        if (InnerOnConfirm())
            Hide();
    }

    /// <summary>
    /// Caled when the confirm button was pressed.
    /// </summary>
    /// <returns>Wheter the panel should be closed or not.</returns>
    public virtual bool InnerOnConfirm()
    {
        return true;
    }

    /// <summary>
    /// Called when the cancel button was pressed.
    /// </summary>
    public void OnCancel()
    {
        if (InnerOnCancel())
            Hide();
    }

    /// <summary>
    /// Called when the cancel button was pressed.
    /// </summary>
    /// <returns>Wheter the panel should be closed or not.</returns>
    public virtual bool InnerOnCancel()
    {
        return true;
    }

    /// <summary>
    /// Shows the panel.
    /// </summary>
    public void Show()
    {
        if (Player.LocalPlayer)
            Player.LocalPlayer.Input.enabled = false;

        gameObject.SetActive(true);
        InnerOnShow();
    }

    /// <summary>
    /// Called when the panel should be shown.
    /// </summary>
    public virtual void InnerOnShow() { }

    /// <summary>
    /// Hides the panel.
    /// </summary>
    public void Hide()
    {
        if (Player.LocalPlayer)
            Player.LocalPlayer.Input.enabled = true;

        InnerOnHide();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Called when the panel should be hidden.
    /// </summary>
    public virtual void InnerOnHide() { }

    private void OnDestroy()
    {
        if (confirmButton)
            confirmButton.onClick.RemoveListener(OnConfirm);
        if (cancelButton)
            cancelButton.onClick.RemoveListener(OnCancel);
    }
}
