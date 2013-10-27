using System;

namespace Greyhound
{
    public class NoSubscribersException : Exception
    {
        public NoSubscribersException(string message)
            : base(message)
        {
        }
    }
}