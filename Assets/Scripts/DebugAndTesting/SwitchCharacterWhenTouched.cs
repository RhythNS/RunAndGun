using Mirror;
using UnityEngine;

/// <summary>
/// Switches the CharacterType of a Player when they Overlap with this Object.
/// </summary>
public class SwitchCharacterWhenTouched : MonoBehaviour
{
    [SerializeField] private CharacterType toSwitchTo;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player player) && player.isLocalPlayer)
        {
            if (toSwitchTo == Config.Instance.SelectedPlayerType)
                return;

            JoinMessage joinMessage = JoinMessage.GetDefault();
            joinMessage.characterType = toSwitchTo;
            NetworkClient.Send(joinMessage);
        }
    }
}
