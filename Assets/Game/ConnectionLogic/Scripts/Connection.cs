using System;

[Serializable]
public struct Connection
{
    public ulong Connector1Id;
    public ulong Connector2Id;

    public Connection(ulong connector1, ulong connector2)
    {
        Connector1Id = connector1;
        Connector2Id = connector2;
    }

    public override bool Equals(object obj)
    {
        return (obj is Connection other) &&
               ((other.Connector1Id == Connector1Id && other.Connector2Id == Connector2Id) ||
                (other.Connector1Id == Connector2Id && other.Connector2Id == Connector1Id));
    }

    public static bool operator ==(Connection connection1, Connection connection2)
    {
        return (connection1.Connector1Id == connection2.Connector1Id && connection1.Connector2Id == connection2.Connector2Id) ||
               (connection1.Connector1Id == connection2.Connector2Id && connection1.Connector2Id == connection2.Connector1Id);
    }

    public static bool operator !=(Connection connection1, Connection connection2)
    {
        return !(connection1.Connector1Id == connection2.Connector1Id && connection1.Connector2Id == connection2.Connector2Id) &&
               !(connection1.Connector1Id == connection2.Connector2Id && connection1.Connector2Id == connection2.Connector1Id);
    }

    public override int GetHashCode() => Connector1Id.GetHashCode() ^ Connector2Id.GetHashCode();

    public override string ToString() => $"Connector1 ID: {Connector1Id}, Connector2 ID {Connector2Id}";
}