#nullable enable
using System;
using System.Collections.Generic;

namespace Microsoft.Maui.Controls
{
    internal sealed class TypeConversionService
    {
        private readonly Dictionary<(Type From, Type To), Func<object, object>> _conversions = new();

        internal TypeConversionService(IEnumerable<ConversionMauiAppBuilderExtensions.ConversionRegistration> registrations)
        {
            foreach (var registration in registrations)
            {
                registration.Register(this);
            }
        }

        public void AddConversion<TFrom, TTo>(Func<TFrom, TTo> conversionFunction)
        {
            _conversions.Add((typeof(TFrom), typeof(TTo)), (object input) => (object)conversionFunction((TFrom)input!)!);
        }

        internal bool TryConvert(ref object value, Type targetType)
        {
            if (_conversions.TryGetValue((value.GetType(), targetType), out var conversion)
                && conversion(value) is object convertedValue)
            {
                value = convertedValue;
                return true;
            }

            return false;
        }
    }
}
