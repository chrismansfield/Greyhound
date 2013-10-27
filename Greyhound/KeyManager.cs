using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Greyhound
{
    internal class KeyManager
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfo> CachedKeyAccessors;

        static KeyManager()
        {
            CachedKeyAccessors = new ConcurrentDictionary<Type, PropertyInfo>();
        }

        public static object GetKey(object data)
        {
            return CachedKeyAccessors.GetOrAdd(data.GetType(), GetKeyFromType).GetValue(data);
        }

        private static PropertyInfo GetKeyFromType(Type type)
        {
            PropertyInfo propertyInfo;
            IEnumerable<PropertyInfo> propertyInfos = type.GetProperties();
            if (TryGetKeyByAttribute(propertyInfos, out propertyInfo))
                return propertyInfo;
            if (TryGetKeyByConvention(propertyInfos, out propertyInfo))
                return propertyInfo;
            throw new InvalidOperationException(
                string.Format(
                    "Could not find any key in type {0}. Please decorate one of your properties with the Key attribute",
                    type));
        }

        private static bool TryGetKeyByConvention(IEnumerable<PropertyInfo> propertyInfos, out PropertyInfo propertyInfo)
        {
            propertyInfo =
                propertyInfos.FirstOrDefault(
                    pi => pi.Name.Equals(pi.DeclaringType.Name + "Id", StringComparison.OrdinalIgnoreCase)
                          || pi.Name.Equals("id", StringComparison.OrdinalIgnoreCase));
            return propertyInfo != null;
        }

        private static bool TryGetKeyByAttribute(IEnumerable<PropertyInfo> propertyInfos, out PropertyInfo propertyInfo)
        {
            try
            {
                propertyInfo = propertyInfos.SingleOrDefault(pi => Attribute.IsDefined(pi, typeof (KeyAttribute)));
            }
            catch (InvalidOperationException)
            {
                throw new InvalidOperationException(
                    string.Format("Sorry, but at the moment Greyhound only supports entities with one key."));
            }
            return propertyInfo != null;
        }
    }
}