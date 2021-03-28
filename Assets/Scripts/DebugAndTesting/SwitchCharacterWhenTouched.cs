using Mirror;
using UnityEngine;

public class SwitchCharacterWhenTouched : MonoBehaviour
{
    [SerializeField] private CharacterType toSwitchTo;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player player) && player.isLocalPlayer)
        {
            if (toSwitchTo == Config.Instance.selectedPlayerType)
                return;

            JoinMessage joinMessage = new JoinMessage()
            {
                characterType = toSwitchTo,
                name = player.userName
            };
            NetworkClient.Send(joinMessage);
        }
    }
}
