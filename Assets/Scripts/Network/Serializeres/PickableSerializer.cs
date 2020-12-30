﻿using Mirror;

public static class PickableSerializer
{
    public static void WritePickable(this NetworkWriter writer, Pickable pickable)
    {
        // This code will not work, if the pickable can be modified at runtime. Consider
        // adding a bool flag to check if the pickable can be edited at runtime and then
        // transmit all modified values.
        writer.WriteByte((byte)pickable.PickableType);
        writer.WriteUInt16(pickable.Id);
    }

    public static Pickable ReadPickable(this NetworkReader reader)
    {
        PickableType type = (PickableType)reader.ReadByte();
        return PickableDict.Instance.Get(type, reader.ReadUInt16());
    }

    public static void WriteItem(this NetworkWriter writer, Item item)
    {
        writer.WriteUInt16(item.Id);
    }

    public static Item ReadItem(this NetworkReader reader)
    {
        return PickableDict.Instance.GetItem(reader.ReadUInt16());
    }

    public static void WriteConsumable(this NetworkWriter writer, Consumable consumable)
    {
        writer.WriteUInt16(consumable.Id);
    }

    public static Consumable ReadConsumable(this NetworkReader reader)
    {
        return PickableDict.Instance.GetConsumbale(reader.ReadUInt16());
    }

    public static void WriteWeapon(this NetworkWriter writer, Weapon weapon)
    {
        writer.WriteUInt16(weapon.Id);
    }

    public static Weapon ReadWeapon(this NetworkReader reader)
    {
        return PickableDict.Instance.GetWeapon(reader.ReadUInt16());
    }
}