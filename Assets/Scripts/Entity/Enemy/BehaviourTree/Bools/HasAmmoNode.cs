using Rhyth.BTree;
using System.Collections.Generic;

/// <summary>
/// Checks if the equipped weapon has bullets left.
/// </summary>
public class HasAmmoNode : BoolNode
{
    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue) => CreateInstance<HasAmmoNode>();

    protected override bool InnerIsFulfilled() => tree.AttachedBrain.GetComponent<EquippedWeapon>().HasBulletsLeft;
}
