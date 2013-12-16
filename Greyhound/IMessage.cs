using System;

namespace Greyhound
{
    public interface IMessage<out T>
    {
        Guid Id { get; }
        T Data { get; }
    }
}