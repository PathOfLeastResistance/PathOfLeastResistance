public interface IEventResolver<T>
{
    public void Resolve(T state);
}
