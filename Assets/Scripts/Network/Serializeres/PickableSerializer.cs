using Mirror;

/// <summary>
/// Serializers for Pickables.
/// </summary>
public static class PickableSerializer
{
    public static void WritePickable(this NetworkWriter writer, Pickable pickable)
    {
        // This code will not work, if the pickable can be modified at runtime. Consider
        // adding a bool flag to check if the pickable can be edited at runtime and then
        // transmit all modified values.
        if (pickable == null)
        {
            writer.WriteByte(0);
            writer.WriteUInt16(0);
        }
        else
        {
            writer.WriteByte((byte)pickable.PickableType);
            writer.WriteUInt16(pickable.Id);
        }
    }

    public static Pickable ReadPickable(this NetworkReader reader)
    {
        PickableType type = (PickableType)reader.ReadByte();
        return PickableDict.Instance.Get(type, reader.ReadUInt16());
    }

    public static void WriteItem(this NetworkWriter writer, Item item)
    {
        writer.WriteUInt16(item == null ? (ushort)0 : item.Id);
    }

    public static Item ReadItem(this NetworkReader reader)
    {
        return PickableDict.Instance.GetItem(reader.ReadUInt16());
    }

    public static void WriteConsumable(this NetworkWriter writer, Consumable consumable)
    {
        writer.WriteUInt16(consumable == null ? (ushort)0 : consumable.Id);
    }

    public static Consumable ReadConsumable(this NetworkReader reader)
    {
        return PickableDict.Instance.GetConsumbale(reader.ReadUInt16());
    }

    public static void WriteWeapon(this NetworkWriter writer, Weapon weapon)
    {
        writer.WriteUInt16(weapon == null ? (ushort)0 : weapon.Id);
    }

    public static Weapon ReadWeapon(this NetworkReader reader)
    {
        return PickableDict.Instance.GetWeapon(reader.ReadUInt16());
    }

    public static void WriteStatusEffect(this NetworkWriter writer, StatusEffect statusEffect)
    {
        writer.WriteUInt16(statusEffect == null ? (ushort)0 : statusEffect.Id);
    }

    public static StatusEffect ReadStatusEffect(this NetworkReader reader)
    {
        return PickableDict.Instance.GetStatusEffect(reader.ReadUInt16());
    }

    public static void WriteWorldEffect(this NetworkWriter writer, WorldEffect worldEffect)
    {
        writer.WriteUInt16(worldEffect == null ? (ushort)0 : worldEffect.Id);
    }

    public static WorldEffect ReadWorldEffect(this NetworkReader reader)
    {
        return PickableDict.Instance.GetWorldEffect(reader.ReadUInt16());
    }
}
