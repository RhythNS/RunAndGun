using Mirror;

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
}
