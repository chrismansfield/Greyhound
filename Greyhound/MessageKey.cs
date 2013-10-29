using System;
using System.Collections.Generic;

namespace Greyhound
{
    public struct MessageKey : IEqualityComparer<MessageKey>, IEquatable<MessageKey>
    {
        public const String AnyBusName = "760E2510-061F-4B68-A7B3-514464F0A187";

        public MessageKey(string busName, Guid id)
            : this()
        {
            BusName = busName;
            Id = id;
        }

        public string BusName { get; set; }

        public Guid Id { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is MessageKey))
                return false;
            return Equals((MessageKey)obj);
        }

        public bool Equals(MessageKey x, MessageKey y)
        {
            return (x.BusName == AnyBusName || x.BusName == y.BusName) && x.Id == y.Id;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((BusName != null ? BusName.GetHashCode() : 0) * 397) ^ Id.GetHashCode();
            }
        }

        public int GetHashCode(MessageKey obj)
        {
            return obj.GetHashCode();
        }

        public bool Equals(MessageKey other)
        {
            return Equals(this, other);
        }

        public static bool operator ==(MessageKey x, MessageKey y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(MessageKey x, MessageKey y)
        {
            return !(x == y);
        }
    }
}