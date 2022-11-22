public class UniquePostProvider
{
    public int mIdCounter = 0;
    
    public int GetId()
    {
        return ++mIdCounter;
    }
}