using Rhyth.BTree;
using System.Collections.Generic;

/// <summary>
/// Reloads the equipped weapon. Returns failure if something prevented from reloading. Returns
/// success when the weapon reloaded.
/// </summary>
public class ReloadNode : BNodeAdapter
{
    public override string StringToolTip => "Reloads the equipped weapon.\nReturns failure if something prevented from reloading. Returns success when the weapon reloaded.";
    public override int MaxNumberOfChildren => 0;

    private EquippedWeapon weapon;
    private bool hasStartedReload;

    public override void InnerBeginn()
    {
        weapon = tree.AttachedBrain.GetComponent<EquippedWeapon>();
        hasStartedReload = false;
    }

    public override void Update()
    {
        if (hasStartedReload == false)
        {
            if (weapon.Reload() == false)
            {
                CurrentStatus = Status.Failure;
                return;
            }
            hasStartedReload = true;
            return;
        }

        if (weapon.IsReloading == false)
            CurrentStatus = Status.Success;
    }

    protected override BNode InnerClone(Dictionary<Value, Value> originalValueForClonedValue) => CreateInstance<ReloadNode>();

}
