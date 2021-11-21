using UnityEngine;

public delegate void WeaponChanged(Weapon newWeapon);
public delegate void IntChanged(int amount);
public delegate void IntChangedWithPrev(int prevValue, int newValue);
public delegate void HealthPercentageChanged(float newValue);
public delegate void PercentageChanged(float percentage);
public delegate void BoolChanged(bool changed);
public delegate void DirectionChanged(Direction direction);
public delegate void Died(GameObject entity);
