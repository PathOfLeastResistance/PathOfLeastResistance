public class UniqueIdProvider
{
    private uint mIdCounter = 0;

    public uint GetId()
    {
        return ++mIdCounter;
    }
}