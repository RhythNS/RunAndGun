using Mirror;
using UnityEngine;

public class SwitchCharacterWhenTouched : MonoBehaviour
{
    [SerializeField] private CharacterType toSwitchTo;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Player player) && player.TryGetComponent(out RAGInput _))
        {
            if (toSwitchTo == Config.Instance.selectedPlayerType)
                return;

            JoinMessage joinMessage = new JoinMessage()
            {
                characterType = toSwitchTo,
                name = Config.Instance.name
            };
            NetworkClient.Send(joinMessage);
        }
    }
}
