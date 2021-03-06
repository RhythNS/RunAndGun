﻿using UnityEngine;
using UnityEngine.Events;

public class UnityEventZone : EnterZone
{
    [SerializeField] private UnityEvent onEnter;
    [SerializeField] private UnityEvent onExit;

    public override void OnEnter(Player player)
    {
        onEnter.Invoke();
    }

    public override void OnExit(Player player)
    {
        onExit.Invoke();
    }
}
