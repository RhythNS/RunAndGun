using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmoteMessageDisplay : MonoBehaviour
{
    [SerializeField] private Image frameImage;
    [SerializeField] private Image emoteImage;

    public void Set(EmoteMessage emoteMessage, EmoteBoard emoteBoard)
    {
        List<Player> players = PlayersDict.Instance.Players;
        Player player = players.Find(x => x.playerId == emoteMessage.playerID);
        EmoteDict.Emote? emote = EmoteDict.Instance.GetEmote(emoteMessage.emoteID);

        if (player == null || emote == null || player.playerIndex > 3)
        {
            Destroy(gameObject);
            return;
        }

        Color playerColor = CharacterDict.Instance.PlayerColors[player.playerIndex];
        emoteImage.sprite = emote.Value.sprite;
        frameImage.color = playerColor;

        StartCoroutine(OnShow(emoteBoard.MessageAliveTime, emoteBoard.ScaleCurve));
    }

    private IEnumerator OnShow(float aliveTime, AnimationCurve scaleCurve)
    {
        float timer = 0.0f;

        while (timer < aliveTime)
        {
            float eval = scaleCurve.Evaluate(timer / aliveTime);
            transform.localScale = Vector3.one * eval;

            yield return null;
            timer += Time.deltaTime;
        }

        Destroy(gameObject);
    }
}
