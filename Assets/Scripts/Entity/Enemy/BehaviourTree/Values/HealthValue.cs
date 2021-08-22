using Rhyth.BTree;
using UnityEngine;

/// <summary>
/// Represents an entity that can be targeted.
/// </summary>
public class HealthValue : Value
{
    [SerializeField] private Health health;

    public override Value Clone()
    {
        HealthValue healthValue = CreateInstance<HealthValue>();
        healthValue.health = health;
        return healthValue;
    }

    public override object GetValue()
    {
        return health;
    }

    public Health Get()
    {
        return health;
    }

    public override void SetValue(object obj)
    {
        health = (Health)obj;
    }

    public void Set(Health health)
    {
        this.health = health;
    }
}
