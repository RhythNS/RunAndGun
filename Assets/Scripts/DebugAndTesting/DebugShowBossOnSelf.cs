using UnityEngine;

public class DebugShowBossOnSelf : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("Show Boss ui on local player with F7");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F7))
            BossUIManager.Instance.Show(Player.LocalPlayer, 2.0f);
    }
}
