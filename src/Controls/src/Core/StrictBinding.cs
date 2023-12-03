using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using Microsoft.Maui.Controls.Xaml.Diagnostics;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Converters;

// TODO this will need to be public and customers will need to use it directly
// BTW original TypedBinding PR: https://github.com/xamarin/Xamarin.Forms/pull/489
// Maybe `StrictBinding` isn't the best name? It doesn't seem very "positive"
// - DirectBinding (would it be confusing with xamrin-macios direct bindings - probably not, too low level, different concepts
// - FastBinding (overpromissing maybe? speed is not the true goal?)
// - x:Bind - https://learn.microsoft.com/en-us/windows/uwp/xaml-platform/x-bind-markup-extension
// - ... TODO

namespace Microsoft.Maui.Controls.Internals
{
	internal static class StrictBinding<TSource>
		where TSource : class
	{
		public static BindingBase Create<TProperty>(
			BindableProperty property,
			Func<TSource, TProperty> getter,
			Action<TSource, TProperty>? setter = default,
			Func<TProperty, TProperty>? convert = default,
			Func<TProperty, TProperty>? convertBack = default,
			BindingMode mode = BindingMode.Default,
			object? source = default,
			TProperty? fallbackValue = default,
			TProperty? targetNullValue = default,
			string? stringFormat = default)
		{
			return Create<TProperty, TProperty>(
				property, getter, setter, convert: convert, convertBack, mode, source, fallbackValue, targetNullValue, stringFormat);
		}

		public static BindingBase Create<TProperty>(
			Func<TSource, TProperty> getter,
			Action<TSource, TProperty>? setter = null,
			Func<TProperty, TProperty>? convert = default,
			Func<TProperty, TProperty>? convertBack = default,
			Tuple<Func<TSource, object>, string>[]? handlers = null,
			BindingMode mode = BindingMode.Default,
			object? source = default,
			TProperty? fallbackValue = default,
			TProperty? targetNullValue = default,
			string? stringFormat = default)
		{
			return Create<TProperty, TProperty>(
				getter, setter, convert, convertBack, handlers, mode, source, fallbackValue, targetNullValue, stringFormat);
		}

		public static BindingBase Create<TProperty>(
			BindableProperty property,
			Func<TSource, TProperty> getter,
			IValueConverter converter,
			Action<TSource, TProperty>? setter = null,
			object? converterParameter = null,
			BindingMode mode = BindingMode.Default,
			object? source = default,
			TProperty? fallbackValue = default,
			TProperty? targetNullValue = default,
			string? stringFormat = default)
		{
			return Create<TProperty, TProperty>(
				property, getter, converter, setter, converterParameter, mode, source, fallbackValue, targetNullValue, stringFormat);
		}

		public static BindingBase Create<TProperty>(
			Func<TSource, TProperty> getter,
			IValueConverter converter,
			Action<TSource, TProperty>? setter = null,
			object? converterParameter = null,
			Tuple<Func<TSource, object>, string>[]? handlers = null,
			BindingMode mode = BindingMode.Default,
			object? source = default,
			TProperty? fallbackValue = default,
			TProperty? targetNullValue = default,
			string? stringFormat = default)
		{
			return Create<TProperty, TProperty>(
				getter, converter, setter, converterParameter, handlers, mode, source, fallbackValue, targetNullValue, stringFormat);
		}

		public static BindingBase Create<TSourceProperty, TTargetProperty>(
			BindableProperty property,
			Func<TSource, TSourceProperty> getter,
			IValueConverter converter,
			Action<TSource, TSourceProperty>? setter = null,
			object? converterParameter = null,
			BindingMode mode = BindingMode.Default,
			object? source = default,
			TTargetProperty? fallbackValue = default,
			TTargetProperty? targetNullValue = default,
			string? stringFormat = default)
		{
			return Create<TSourceProperty, TTargetProperty>(
				getter, converter, setter, converterParameter, PropertyToHandlers(property), mode, source, fallbackValue, targetNullValue, stringFormat);
		}

		public static BindingBase Create<TSourceProperty, TTargetProperty>(
			Func<TSource, TSourceProperty> getter,
			IValueConverter converter,
			Action<TSource, TSourceProperty>? setter = null,
			object? converterParameter = null,
			Tuple<Func<TSource, object>, string>[]? handlers = null,
			BindingMode mode = BindingMode.Default,
			object? source = default,
			TTargetProperty? fallbackValue = default,
			TTargetProperty? targetNullValue = default,
			string? stringFormat = default)
		{
			return Create<TSourceProperty, TTargetProperty>(
				getter, setter, Convert, ConvertBack, handlers, mode, source, fallbackValue, targetNullValue, stringFormat);

			TTargetProperty Convert(TSourceProperty value)
				=> converter.Convert(value, typeof(TTargetProperty), converterParameter, CultureInfo.CurrentUICulture) switch
				{
					TTargetProperty targetValue => targetValue,
					_ => throw new InvalidOperationException("Converter returned invalid value"),
				};

			TSourceProperty ConvertBack(TTargetProperty value)
				=> converter.ConvertBack(value, typeof(TSourceProperty), converterParameter, CultureInfo.CurrentUICulture) switch
				{
					TSourceProperty sourceValue => sourceValue,
					_ => throw new InvalidOperationException("Converter returned invalid value"),
				};
		}

		public static BindingBase Create<TSourceProperty, TTargetProperty>(
			BindableProperty property,
			Func<TSource, TSourceProperty> getter,
			Action<TSource, TSourceProperty>? setter = null,
			Func<TSourceProperty, TTargetProperty>? convert = default,
			Func<TTargetProperty, TSourceProperty>? convertBack = default,
			BindingMode mode = BindingMode.Default,
			object? source = default,
			TTargetProperty? fallbackValue = default,
			TTargetProperty? targetNullValue = default,
			string? stringFormat = default)
		{
			return Create<TSourceProperty, TTargetProperty>(
				getter, setter, convert, convertBack, PropertyToHandlers(property), mode, source, fallbackValue, targetNullValue, stringFormat);
		}

		public static BindingBase Create<TSourceProperty, TTargetProperty>(
			Func<TSource, TSourceProperty> getter,
			Action<TSource, TSourceProperty>? setter = null,
			Func<TSourceProperty, TTargetProperty>? convert = default,
			Func<TTargetProperty, TSourceProperty>? convertBack = default,
			Tuple<Func<TSource, object>, string>[]? handlers = null,
			BindingMode mode = BindingMode.Default,
			object? source = default,
			TTargetProperty? fallbackValue = default,
			TTargetProperty? targetNullValue = default,
			string? stringFormat = default)
		{
			return new StrictBinding<TSource, TSourceProperty, TTargetProperty>(
				SafeGetter,
				setter,
				convert is not null ? WrapConverter(convert) : Convert<TSourceProperty, TTargetProperty>,
				convertBack is not null ? WrapConverter(convertBack) : Convert<TTargetProperty, TSourceProperty>,
				handlers)
			{
				FallbackValue = fallbackValue,
				Mode = mode,
				Source = source,
				StringFormat = stringFormat,
				TargetNullValue = targetNullValue,
			};

			(TSourceProperty?, bool) SafeGetter(TSource source)
			{
				try
				{
					return (getter(source), true);
				}
				catch (Exception ex) when (ex is NullReferenceException || ex is KeyNotFoundException || ex is IndexOutOfRangeException || ex is ArgumentOutOfRangeException)
				{
					return (default, false);
				}
			}

			static (T2?, bool) Convert<T1, T2>(T1 value)
				=> ConversionHelpers.TryConvert(value, out T2? targetValue) ? (targetValue, true) : (default, false);

			static Func<T1, (T2?, bool)> WrapConverter<T1, T2>(Func<T1, T2> converter)
				=> input => (converter(input), true);
		}

		static Tuple<Func<TSource, object>, string>[] PropertyToHandlers(BindableProperty property)
			=> new[] { new Tuple<Func<TSource, object>, string>(source => source, property.PropertyName) };
	}

	internal sealed class StrictBinding<TSource, TSourceProperty, TTargetProperty> : BindingBase
		where TSource : class
	{
		readonly Func<TSource, (TSourceProperty?, bool)> _getter;
		readonly Action<TSource, TSourceProperty>? _setter;
		// TODO should conversion functions also return (TValue, bool) to indicate if it succeeded or not?
		readonly Func<TSourceProperty, (TTargetProperty?, bool)>? _convert;
		readonly Func<TTargetProperty, (TSourceProperty?, bool)>? _convertBack;
		readonly PropertyChangedProxy[]? _handlers;
		readonly WeakReference<object?> _weakSource = new(null);
		readonly WeakReference<BindableObject?> _weakTarget = new(null);

		object? _source;
		BindableProperty? _targetProperty;
		SetterSpecificity _specificity = SetterSpecificity.DefaultValue;

		public StrictBinding(
			Func<TSource, (TSourceProperty?, bool)> getter,
			Action<TSource, TSourceProperty>? setter = null,
			Func<TSourceProperty, (TTargetProperty?, bool)>? convert = default,
			Func<TTargetProperty, (TSourceProperty?, bool)>? convertBack = default,
			Tuple<Func<TSource, object>, string>[]? handlers = null)
		{
#if !NETSTANDARD
			ArgumentNullException.ThrowIfNull(getter);
#else
			if (getter is null) throw new ArgumentNullException(nameof(getter));
#endif

			_getter = getter;
			_setter = setter;
			_convert = convert;
			_convertBack = convertBack;

			if (handlers == null)
			{
				return;
			}

			_handlers = new PropertyChangedProxy[handlers.Length];
			for (var i = 0; i < handlers.Length; i++)
			{
				if (handlers[i] is null)
					continue;
				_handlers[i] = new PropertyChangedProxy(handlers[i].Item1, handlers[i].Item2, this);
			}
		}

		public object? Source
		{
			get { return _source; }
			set
			{
				ThrowIfApplied();
				_source = value;
			}
		}

		// Applies the binding to a previously set source and target.
		internal override void Apply(bool fromTarget = false)
		{
			base.Apply(fromTarget);

			BindableObject? target;
			if (!_weakTarget.TryGetTarget(out target) || target is null)
			{
				Unapply();
				return;
			}

			object? anySource;
			if (_weakSource.TryGetTarget(out anySource) && anySource is TSource source)
			{
				Debug.Assert(_targetProperty is not null);
				ApplyCore(source, target, _targetProperty!, fromTarget, _specificity);
			}
		}

		// Applies the binding to a new source or target.
		internal override void Apply(object context, BindableObject bindObj, BindableProperty property, bool fromBindingContextChanged, SetterSpecificity specificity)
		{
			if (property.ReturnType != typeof(TTargetProperty) && property.ReturnType.IsAssignableFrom(typeof(TTargetProperty)))
			{
				throw new InvalidOperationException($"Strict binding has been applied to a wrong property {property.ReturnType} instead of {typeof(TTargetProperty)}");
			}

			_targetProperty = property;
			_specificity = specificity;

			if (Source is not null && IsApplied && fromBindingContextChanged)
				return;

			if (Source is RelativeBindingSource)
			{
				// TODO
				throw new NotImplementedException($"RelativeBindingSource support is missing");
			}
			else
			{
				// does it make sense to filter out invalid source types?
				object? source = Source ?? Context ?? context;

				base.Apply(source, bindObj, property, fromBindingContextChanged, specificity);

				BindableObject? previousTarget;
				if (_weakTarget.TryGetTarget(out previousTarget) && !ReferenceEquals(previousTarget, bindObj))
					throw new InvalidOperationException("Binding instances cannot be reused");

				_weakTarget.SetTarget(bindObj);

				// If we don't have the source yet, we'll wait for the next Apply call
				if (source is null && !fromBindingContextChanged)
					return;

				object? previousSource;
				if (_weakSource.TryGetTarget(out previousSource) && !ReferenceEquals(previousSource, source))
					throw new InvalidOperationException("Binding instances cannot be reused");

				_weakSource.SetTarget(source);

				if (source is not TSource && source is not null)
				{
					// TODO: What should happen at this point? Tests will be failing if we don't return here... (SetBindingContextAfterContextBindingAndInnerBindings)
					// throw new InvalidOperationException($"Binding source {source.GetType()} is not of expected type {typeof(TSource)}");
					return;
				}

				ApplyCore(source as TSource, bindObj, property, false, specificity);
			}
		}

		internal override BindingBase Clone()
		{
			var handlers = _handlers == null ? null : new Tuple<Func<TSource, object>, string>[_handlers.Length];
			if (_handlers is not null && handlers is not null)
			{
				for (var i = 0; i < _handlers.Length; i++)
				{
					if (_handlers[i] == null)
						continue;
					handlers[i] = new Tuple<Func<TSource, object>, string>(_handlers[i].PartGetter, _handlers[i].PropertyName);
				}
			}

			return new StrictBinding<TSource, TSourceProperty, TTargetProperty>(
				_getter, _setter, _convert, _convertBack, handlers)
			{
				Mode = Mode,
				StringFormat = StringFormat,
				Source = Source,
				FallbackValue = FallbackValue,
				TargetNullValue = TargetNullValue,
			};
		}

		internal override object GetSourceValue(object value, Type targetPropertyType)
		{
			throw new InvalidOperationException($"{nameof(GetSourceValue)} is not supported in {nameof(StrictBinding<TSource, TSourceProperty, TTargetProperty>)}");
		}

		internal override object GetTargetValue(object value, Type sourcePropertyType)
		{
			throw new InvalidOperationException($"{nameof(GetTargetValue)} is not supported in {nameof(StrictBinding<TSource, TSourceProperty, TTargetProperty>)}");
		}

		internal override void Unapply(bool fromBindingContextChanged = false)
		{
			if (Source is not null && fromBindingContextChanged && IsApplied)
				return;

			base.Unapply(fromBindingContextChanged);

			if (_handlers is PropertyChangedProxy[] handlers)
				Unsubscribe(handlers);

			_weakSource.SetTarget(null);
			_weakTarget.SetTarget(null);
		}

		internal void ApplyCore(TSource? source, BindableObject target, BindableProperty property, bool fromTarget, SetterSpecificity specificity)
		{
			var mode = this.GetRealizedMode(property);
			if ((mode == BindingMode.OneWay || mode == BindingMode.OneTime) && fromTarget)
			{
				return;
			}

			if ((mode == BindingMode.OneWay || mode == BindingMode.TwoWay) && source is not null && _handlers is PropertyChangedProxy[] handlers)
			{
				Subscribe(source, handlers);
			}

			var needsGetter = (mode == BindingMode.TwoWay && !fromTarget) || mode == BindingMode.OneWay || mode == BindingMode.OneTime;
			var needsSetter = (mode == BindingMode.TwoWay && fromTarget) || mode == BindingMode.OneWayToSource;

			if (needsGetter)
			{
				if (TryGetValue(source, property, out var targetValue))
				{
					target.SetValueCore<TTargetProperty>(property, targetValue!, SetValueFlags.ClearDynamicResource, BindableObject.SetValuePrivateFlags.Default | BindableObject.SetValuePrivateFlags.Converted, specificity);
				}
				else
				{
					object? value = source is not null ? _getter(source).Item1 : default;
					BindingDiagnostics.SendBindingFailure(this, source, target, property, "Binding", BindingExpression.CannotConvertTypeErrorMessage, value, typeof(TTargetProperty));
				}
			}
			else if (needsSetter)
			{
				// TODO avoid checking nulls over and over again...
				object? value = target.GetValue(property);
				if (!TrySetValue(source, value))
				{
					BindingDiagnostics.SendBindingFailure(this, source, target, property, "Binding", BindingExpression.CannotConvertTypeErrorMessage, value, typeof(TSourceProperty));
				}
			}
		}

		private bool TryGetValue(TSource? source, BindableProperty property, out TTargetProperty? targetValue)
		{
			if (_convert is Func<TSourceProperty, (TTargetProperty?, bool)> convert)
			{
				(TSourceProperty? sourceValue, bool success) = source is not null ? _getter(source) : (default, false);
				if (success)
				{
					if (sourceValue is TSourceProperty value)
					{
						(targetValue, success) = convert(value);
						if (success)
						{
							if (targetValue is string stringValue && StringFormat is not null)
							{
								targetValue = (TTargetProperty)(object)string.Format(StringFormat, stringValue);
							}

							return true;
						}
					}
					else if (TargetNullValue is TTargetProperty targetNullValue)
					{
						targetValue = targetNullValue;
						return true;
					}
					else
					{
						targetValue = default;
						return true;
					}
				}
				else
				{
					if (FallbackValue is TTargetProperty fallbackValue)
					{
						targetValue = fallbackValue;
						return true;
					}
					else if (property.DefaultValue is TTargetProperty defaultValue)
					{
						targetValue = defaultValue;
						return true;
					}
				}
			}
			else
			{
				// TODO: really throw?
				throw new InvalidOperationException("Binding is not properly configured");
			}

			targetValue = default;
			return false;
		}

		private bool TrySetValue(TSource? source, object? value)
		{
			if (source is not null
				&& _setter is Action<TSource, TSourceProperty> setter
				&& _convertBack is Func<TTargetProperty, (TSourceProperty?, bool)> convertBack)
			{
				if (value is TTargetProperty targetValue)
				{
					(TSourceProperty? convertedValue, bool success) = convertBack(targetValue);
					if (success)
					{
						setter(source, convertedValue!); // TODO allow nullabe values??
						return true;
					}
				}
				else if (value is null && TargetNullValue is TTargetProperty targetNullValue)
				{
					(TSourceProperty? convertedValue, bool success)  = convertBack(targetNullValue);
					if (success)
					{
						setter(source, convertedValue!);
						return true;
					}
				}
				else if (value is null)
				{
					// TODO check if null is a valid value for the source property type?
					setter(source, default!);
					return true;
				}
			}
			else
			{
				// TODO: throw?
			}

			return false;
		}

		// TODO share code with TypedBinding
		class PropertyChangedProxy
		{
			public Func<TSource, object> PartGetter { get; }
			public string PropertyName { get; }
			public BindingExpression.WeakPropertyChangedProxy Listener { get; }
			readonly BindingBase _binding;
			PropertyChangedEventHandler handler;

			~PropertyChangedProxy() => Listener?.Unsubscribe();

			public INotifyPropertyChanged? Part
			{
				get
				{
					if (Listener is not null && Listener.TryGetSource(out var target))
						return target;
					return null;
				}
				set
				{
					if (Listener is not null)
					{
						//Already subscribed
						if (Listener.TryGetSource(out var source) && ReferenceEquals(value, source))
							return;

						//clear out previous subscription
						Listener.Unsubscribe();
						Listener.Subscribe(value, handler);
					}
				}
			}

			public PropertyChangedProxy(Func<TSource, object> partGetter, string propertyName, BindingBase binding)
			{
				PartGetter = partGetter;
				PropertyName = propertyName;
				_binding = binding;
				Listener = new BindingExpression.WeakPropertyChangedProxy();
				//avoid GC collection, keep a ref to the OnPropertyChanged handler
				handler = new PropertyChangedEventHandler(OnPropertyChanged);
			}

			void OnPropertyChanged(object? sender, PropertyChangedEventArgs e)
			{
				if (!string.IsNullOrEmpty(e.PropertyName) && string.CompareOrdinal(e.PropertyName, PropertyName) != 0)
					return;

				IDispatcher? dispatcher = (sender as BindableObject)?.Dispatcher;
				dispatcher.DispatchIfRequired(() => _binding.Apply(false));
			}
		}

		// TODO share code with TypedBinding
		static void Subscribe(TSource source, PropertyChangedProxy[] handlers)
		{
			for (var i = 0; i < handlers.Length; i++)
			{
				if (handlers[i] == null)
					continue;
				var part = handlers[i].PartGetter(source);
				if (part == null)
					break;
				var inpc = part as INotifyPropertyChanged;
				if (inpc == null)
					continue;
				handlers[i].Part = (inpc);
			}
		}

		// TODO share code with TypedBinding
		static void Unsubscribe(PropertyChangedProxy[] handlers)
		{
			for (var i = 0; i < handlers.Length; i++)
				handlers[i]?.Listener.Unsubscribe();
		}
	}
}
