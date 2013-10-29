namespace Greyhound
{
    public interface IFilter<in T>
    {
        bool Match(IMessage<T> message);
    }
}