namespace Greyhound
{
    public interface IFilter<T>
    {
        bool Match(IMessage<T> message);
    }
}