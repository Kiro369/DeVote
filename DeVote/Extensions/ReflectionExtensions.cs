using System;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace DeVote.Extensions
{
    public static class ReflectionExtensions
    {
        [Pure]
        public static T[] GetCustomAttributes<T>(this ICustomAttributeProvider type)
            where T : Attribute
        {
            Contract.Requires(type != null);
            Contract.Ensures(Contract.Result<T[]>() != null);

            var attribs = type.GetCustomAttributes(typeof(T), false) as T[];
            Contract.Assume(attribs != null);
            return attribs;
        }

        [Pure]
        public static T GetCustomAttribute<T>(this ICustomAttributeProvider type)
            where T : Attribute
        {
            Contract.Requires(type != null);

            return type.GetCustomAttributes<T>().TryGet(0);
        }

        [Pure]
        public static bool IsAssignableTo(this Type type, Type other)
        {
            Contract.Requires(type != null);
            Contract.Requires(other != null);

            return other.IsAssignableFrom(type);
        }

        /// <summary>
        ///     Checks if the type is a simple type.
        ///     Simple types are primitive types and strings.
        /// </summary>
        [Pure]
        public static bool IsSimple(this Type type)
        {
            Contract.Requires(type != null);

            return type.IsEnum || type.IsNumeric() || type == typeof(string) || type == typeof(char) ||
                   type == typeof(bool);
        }

        [Pure]
        public static bool IsNumeric(this Type type)
        {
            Contract.Requires(type != null);

            return type.IsInteger() || type.IsFloatingPoint();
        }

        [Pure]
        public static bool IsFloatingPoint(this Type type)
        {
            Contract.Requires(type != null);

            return type == typeof(float) || type == typeof(double) || type == typeof(decimal);
        }

        [Pure]
        public static bool IsInteger(this Type type)
        {
            Contract.Requires(type != null);

            return type == typeof(int) || type == typeof(uint) || type == typeof(short) || type == typeof(ushort) ||
                   type == typeof(byte) || type == typeof(sbyte) || type == typeof(long) || type == typeof(ulong);
        }

        public static T ForgeDelegate<T>(this MethodInfo method)
            where T : class
        {
            Contract.Requires(method != null);

            Type type = typeof(T);
            if (!type.IsAssignableTo(typeof(Delegate)))
                throw new ArgumentException("Type T is not a delegate type.");

            return Delegate.CreateDelegate(type, method) as T;
        }

        /// <summary>
        /// Extension for 'Object' that copies the properties to a destination object.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        public static void CopyProperties(this object source, object destination)
        {
            // If any this null throw an exception
            if (source == null || destination == null)
                throw new Exception("Source or/and Destination Objects are null");
            // Getting the Types of the objects
            Type typeDest = destination.GetType();
            Type typeSrc = source.GetType();

            // Iterate the Properties of the source instance and  
            // populate them from their desination counterparts  
            PropertyInfo[] srcProps = typeSrc.GetProperties();
            foreach (PropertyInfo srcProp in srcProps)
            {
                if (!srcProp.CanRead)
                {
                    continue;
                }
                PropertyInfo targetProperty = typeDest.GetProperty(srcProp.Name);
                if (targetProperty == null)
                {
                    continue;
                }
                if (!targetProperty.CanWrite)
                {
                    continue;
                }
                if (targetProperty.GetSetMethod(true) != null && targetProperty.GetSetMethod(true).IsPrivate)
                {
                    continue;
                }
                if ((targetProperty.GetSetMethod().Attributes & MethodAttributes.Static) != 0)
                {
                    continue;
                }
                if (!targetProperty.PropertyType.IsAssignableFrom(srcProp.PropertyType))
                {
                    continue;
                }
                // Passed all tests, lets set the value
                targetProperty.SetValue(destination, srcProp.GetValue(source, null), null);
            }
        }
    }

}
