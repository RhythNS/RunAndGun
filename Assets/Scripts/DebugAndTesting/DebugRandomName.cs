﻿using UnityEngine;

public class DebugRandomName : MonoBehaviour
{
    [SerializeField] private string[] names;

    private void Start()
    {
        Config.Instance.PlayerName = RandomUtil.Element(names);
        Debug.Log("Set random name! Name is now: " + Config.Instance.PlayerName);
    }
}
