using UnityEngine;

public abstract class Input : MonoBehaviour
{
    public abstract InputType InputType { get; }

    public static void AttachInput(GameObject gameObject)
    {
        if (gameObject.TryGetComponent(out Input input))
            input.Remove();

        switch (Config.Instance.selectedInput)
        {
            case InputType.KeyMouse:
                gameObject.AddComponent<KeyMouseInput>();
                break;
            /*
        case InputType.Keyboard:
            break;
        case InputType.Controller:
            break;
        case InputType.Mobile:
            break;
            */
            default:
                Debug.LogError("InputType " + Config.Instance.selectedInput + " not found!");
                break;
        }
    }

    public abstract void Remove();
}
