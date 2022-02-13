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
    }

}
