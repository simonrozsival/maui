using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Microsoft.Maui
{
    // TODO this attriubte should be public eventually
    internal abstract class ImplicitCastCollectionBuilder
    {
        public abstract void RegisterCast<TFrom, TTo>(Func<TFrom, TTo> cast);
    }

    internal sealed class ImplicitCastsConverter
    {
        internal static ImplicitCastsConverter Instance { get; } = new();

        private readonly Dictionary<Type, Dictionary<Type, Func<object, object>>> _conversions = new();
        private readonly HashSet<Type> _registeredTypes = new();

        internal bool TryCast(ref object value, Type targetType)
        {
            if (TryGetConversion(value.GetType(), targetType, out var conversion))
            {
                value = conversion.Invoke(value);
                return true;
            }

            return false;
        }

        private void EnsureRegistered(Type type)
        {
            lock (_conversions)
            {
                if (!_registeredTypes.Add(type))
                {
                    return;
                }

                if (type.IsPrimitive)
                {
                    RegisterPrimitiveTypeCasts(type);
                    return;
                }

                ImplicitCastCollection? collection = null;
                foreach (var attribute in type.GetCustomAttributes<RegisteredCastsAttribute>())
                {
                    collection ??= new ImplicitCastCollection();
                    attribute.RegisterCasts(collection);
                }

                if (collection?.RegisteredConverters is not null)
                {
                    Accept(collection.RegisteredConverters);
                }
            }
        }

        private void Accept(IEnumerable<Registration> registrations)
        {
            lock (_conversions)
            {
                foreach (var registration in registrations)
                {
                    Add(registration.From, registration.To, registration.Conversion, overrideExisting: true);
                }
            }
        }

        private void RegisterPrimitiveTypeCasts(Type type)
        {
            lock (_conversions)
            {
                if (type == typeof(sbyte))
                {
                    Add<sbyte, short>(static x => x);
                    Add<sbyte, int>(static x => x);
                    Add<sbyte, long>(static x => x);
                    Add<sbyte, float>(static x => x);
                    Add<sbyte, double>(static x => x);
                    Add<sbyte, decimal>(static x => x);
                }
                else if (type == typeof(byte))
                {
                    Add<byte, short>(static x => x);
                    Add<byte, ushort>(static x => x);
                    Add<byte, int>(static x => x);
                    Add<byte, uint>(static x => x);
                    Add<byte, ulong>(static x => x);
                    Add<byte, float>(static x => x);
                    Add<byte, double>(static x => x);
                    Add<byte, decimal>(static x => x);
                }
                else if (type == typeof(char))
                {
                    Add<char, ushort>(static x => x);
                    Add<char, int>(static x => x);
                    Add<char, uint>(static x => x);
                    Add<char, long>(static x => x);
                    Add<char, ulong>(static x => x);
                    Add<char, float>(static x => x);
                    Add<char, double>(static x => x);
                    Add<char, decimal>(static x => x);
                }
                else if (type == typeof(short))
                {
                    Add<short, int>(static x => x);
                    Add<short, long>(static x => x);
                    Add<short, float>(static x => x);
                    Add<short, double>(static x => x);
                    Add<short, decimal>(static x => x);
                }
                else if (type == typeof(ushort))
                {
                    Add<ushort, int>(static x => x);
                    Add<ushort, uint>(static x => x);
                    Add<ushort, long>(static x => x);
                    Add<ushort, ulong>(static x => x);
                    Add<ushort, float>(static x => x);
                    Add<ushort, double>(static x => x);
                    Add<ushort, decimal>(static x => x);
                }
                else if (type == typeof(int))
                {
                    Add<int, long>(static x => x);
                    // Add<int, float>(static x => x);
                    // Add<int, double>(static x => x);
                    Add<int, decimal>(static x => x);
                }
                else if (type == typeof(uint))
                {
                    Add<uint, long>(static x => x);
                    Add<uint, ulong>(static x => x);
                    // Add<uint, float>(static x => x);
                    Add<uint, double>(static x => x);
                    Add<uint, decimal>(static x => x);
                }
                else if (type == typeof(long))
                {
                    // Add<long, float>(static x => x);
                    Add<long, double>(static x => x);
                    Add<long, decimal>(static x => x);
                }
                else if (type == typeof(ulong))
                {
                    // Add<ulong, float>(static x => x);
                    Add<ulong, double>(static x => x);
                    Add<ulong, decimal>(static x => x);
                }
                else if (type == typeof(float))
                {
                    Add<float, double>(static x => x);
                }
                else if (type == typeof(double))
                {
                    // no conversions
                }
                else if (type == typeof(bool))
                {
                    // no conversions
                }
            }
        }

        private void Add<TSource, TTarget>(Func<TSource, TTarget> converter, bool overrideExisting = false)
        {
            Add(typeof(TSource), typeof(TTarget), x => converter((TSource)x)!, overrideExisting);
        }

        private void Add(Type sourceType, Type targetType, Func<object, object> converter, bool overrideExisting = false)
        {
            if (!_conversions.TryGetValue(sourceType, out var converters))
            {
                _conversions[sourceType] = converters = new();
            }

            if (overrideExisting)
            {
                converters[targetType] = converter;
            }
            else
            {
#if !NETSTANDARD2_0
                converters.TryAdd(targetType, converter);
#else
                if (!converters.ContainsKey(targetType))
                {
                    converters[targetType] = converter;
                }
#endif
            }
        }

        private bool TryGetConversion(Type sourceType, Type targetType, [NotNullWhen(true)] out Func<object, object>? conversion)
        {
            // TODO: unwrap byreflike types first?

            // Register converters for these types if we haven't seen them before
            EnsureRegistered(sourceType);
            EnsureRegistered(targetType);

            if (TryGetDirectConversion(sourceType, targetType, out conversion))
            {
                return true;
            }

            if (TryGetIndirectConversion(sourceType, targetType, out conversion))
            {
                // Cache the combined converter for future use.
                // TODO: Should we cache the combined indirect conversions separately from the direct ones?
                // so that we can override it when we get a direct conversion?
                lock (_conversions)
                {
                    Add(sourceType, targetType, conversion);
                }

                return true;
            }

            conversion = null;
            return false;
        }

        private static Func<object, object> CombineConversions(Func<object, object> inner, Func<object, object> outer)
        {
            return x => outer(inner(x));
        }

        private bool TryGetDirectConversion(Type sourceType, Type targetType, [NotNullWhen(true)] out Func<object, object>? conversion)
        {
            if (targetType.IsAssignableFrom(sourceType))
            {
                conversion = Identity;
                return true;
            }

            if (_conversions.TryGetValue(sourceType, out var converters)
                && converters.TryGetValue(targetType, out conversion))
            {
                return true;
            }

            conversion = null;
            return false;
        }

        private bool TryGetIndirectConversion(Type sourceType, Type targetType, [NotNullWhen(true)] out Func<object, object>? conversion)
        {
            Type? closestIntermediateType = null;
            Func<object, object>? sourceToIntermediate = null;
            Func<object, object>? intermediateToTarget = null;

            foreach (var intermediateType in _conversions.Keys)
            {
                if (TryGetDirectConversion(sourceType, intermediateType, out var firstCast)
                    && TryGetDirectConversion(intermediateType, targetType, out var secondCast))
                {
                    // TODO how do we find closest match if we're trying to pick a better of `float` and `int`?
                    // This is a weak point of the current implementation. Replace it with a better algorithm.
                    
                    // On the other hand, implicit casts should never lose information.
                    // - I don't think converting from int -> float is lossles. The implicit cast should not allow that.

                    if (closestIntermediateType is null || closestIntermediateType.IsAssignableFrom(intermediateType))
                    {
                        closestIntermediateType = intermediateType;
                        sourceToIntermediate = firstCast;
                        intermediateToTarget = secondCast;
                    }
                }
            }

            if (closestIntermediateType is not null
                && sourceToIntermediate is not null
                && intermediateToTarget is not null)
            {
                conversion = CombineConversions(sourceToIntermediate, intermediateToTarget);
                return true;
            }

            conversion = null;
            return false;
        }

        private static object Identity(object x) => x;

        private sealed class ImplicitCastCollection : ImplicitCastCollectionBuilder
        {
            internal List<Registration>? RegisteredConverters { get; private set; }

            public override void RegisterCast<TFrom, TTo>(Func<TFrom, TTo> converter)
            {
                RegisteredConverters ??= new();
                RegisteredConverters.Add(new(typeof(TFrom), typeof(TTo), x => converter((TFrom)x)!)); // TODO get rid of "!" somehow
            }
        }

        private record struct Registration(Type From, Type To, Func<object, object> Conversion);
    }
}