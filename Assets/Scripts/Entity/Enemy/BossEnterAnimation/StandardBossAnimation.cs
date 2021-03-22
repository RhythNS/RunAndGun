using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardBossAnimation : BossEnterAnimation
{
    public override AnimationType Type => AnimationType.Standard;

    public override IEnumerator PlayAnimation(GameObject boss, BossRoom room)
    {
        PlayerCamera camera = PlayerCamera.Instance;
        camera.enabled = false;

        DungeonCreator.Instance.AdjustMask(new Vector2(room.Border.xMin, room.Border.yMin), room.Border.size);

        yield return EnumeratorUtil.GoToInSecondsSlerp(camera.transform, room.Middle, 2.0f);

        // UI.Instance.DisplayText(boss.GetName(), 1.0f);
        yield return new WaitForSeconds(1.0f);

        if (Player.LocalPlayer.isServer)
        {
            List<Player> activePlayers = PlayersDict.Instance.Players;
            for (int i = 0; i < activePlayers.Count; i++)
            {
                activePlayers[i].SmoothSync.teleportAnyObjectFromServer(room.PlayersStartPos, Quaternion.identity, new Vector3(1, 1, 1));
            }
        }

        yield return EnumeratorUtil.GoToInSecondsSlerp(camera.transform, Player.LocalPlayer.transform.position, 2.0f);

        camera.enabled = true;
    }
}
