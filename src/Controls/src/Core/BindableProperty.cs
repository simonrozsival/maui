#nullable disable
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Maui.Controls.Xaml;

namespace Microsoft.Maui.Controls
{
	/// <include file="../../docs/Microsoft.Maui.Controls/BindableProperty.xml" path="Type[@FullName='Microsoft.Maui.Controls.BindableProperty']/Docs/*" />
	[DebuggerDisplay("{PropertyName}")]
	[System.ComponentModel.TypeConverter(typeof(BindablePropertyConverter))]
	public sealed class BindableProperty
	{
		internal const DynamicallyAccessedMemberTypes DeclaringTypeMembers = DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicMethods;
		internal const DynamicallyAccessedMemberTypes ReturnTypeMembers = DynamicallyAccessedMemberTypes.PublicParameterlessConstructor;

		public delegate void BindingPropertyChangedDelegate(BindableObject bindable, object oldValue, object newValue);

		public delegate void BindingPropertyChangedDelegate<in TPropertyType>(BindableObject bindable, TPropertyType oldValue, TPropertyType newValue);

		public delegate void BindingPropertyChangingDelegate(BindableObject bindable, object oldValue, object newValue);

		public delegate void BindingPropertyChangingDelegate<in TPropertyType>(BindableObject bindable, TPropertyType oldValue, TPropertyType newValue);

		public delegate object CoerceValueDelegate(BindableObject bindable, object value);

		public delegate TPropertyType CoerceValueDelegate<TPropertyType>(BindableObject bindable, TPropertyType value);

		public delegate object CreateDefaultValueDelegate(BindableObject bindable);

		public delegate TPropertyType CreateDefaultValueDelegate<in TDeclarer, out TPropertyType>(TDeclarer bindable);

		public delegate bool ValidateValueDelegate(BindableObject bindable, object value);

		public delegate bool ValidateValueDelegate<in TPropertyType>(BindableObject bindable, TPropertyType value);

		/// <include file="../../docs/Microsoft.Maui.Controls/BindableProperty.xml" path="//Member[@MemberName='UnsetValue']/Docs/*" />
		public static readonly object UnsetValue = new object();

		BindableProperty(string propertyName, [DynamicallyAccessedMembers(ReturnTypeMembers)] Type returnType, [DynamicallyAccessedMembers(DeclaringTypeMembers)] Type declaringType, object defaultValue, BindingMode defaultBindingMode = BindingMode.OneWay,
								 ValidateValueDelegate validateValue = null, BindingPropertyChangedDelegate propertyChanged = null, BindingPropertyChangingDelegate propertyChanging = null,
								 CoerceValueDelegate coerceValue = null, BindablePropertyBindingChanging bindingChanging = null, bool isReadOnly = false, CreateDefaultValueDelegate defaultValueCreator = null)
		{
			if (propertyName == null)
				throw new ArgumentNullException(nameof(propertyName));
			if (returnType is null)
				throw new ArgumentNullException(nameof(returnType));
			if (declaringType is null)
				throw new ArgumentNullException(nameof(declaringType));

			// don't use Enum.IsDefined as its redonkulously expensive for what it does
			if (defaultBindingMode != BindingMode.Default && defaultBindingMode != BindingMode.OneWay && defaultBindingMode != BindingMode.OneWayToSource && defaultBindingMode != BindingMode.TwoWay && defaultBindingMode != BindingMode.OneTime)
				throw new ArgumentException($"Not a valid type of BindingMode. Property: {returnType} {declaringType.Name}.{propertyName}. Default binding mode: {defaultBindingMode}", nameof(defaultBindingMode));

			if (defaultValue == null && Nullable.GetUnderlyingType(returnType) == null && returnType.IsValueType)
				defaultValue = Activator.CreateInstance(returnType);

			if (defaultValue != null && !returnType.IsInstanceOfType(defaultValue))
				throw new ArgumentException($"Default value did not match return type. Property: {returnType} {declaringType.Name}.{propertyName} Default value type: {defaultValue.GetType().Name}, ", nameof(defaultValue));

			if (defaultBindingMode == BindingMode.Default)
				defaultBindingMode = BindingMode.OneWay;

			PropertyName = propertyName;
			ReturnType = returnType;
			DeclaringType = declaringType;
			DefaultValue = defaultValue;
			DefaultBindingMode = defaultBindingMode;
			PropertyChanged = propertyChanged;
			PropertyChanging = propertyChanging;
			ValidateValue = validateValue;
			CoerceValue = coerceValue;
			BindingChanging = bindingChanging;
			IsReadOnly = isReadOnly;
			DefaultValueCreator = defaultValueCreator;
		}

		/// <include file="../../docs/Microsoft.Maui.Controls/BindableProperty.xml" path="//Member[@MemberName='DeclaringType']/Docs/*" />
		[DynamicallyAccessedMembers(DeclaringTypeMembers)]
		public Type DeclaringType { get; private set; }

		/// <include file="../../docs/Microsoft.Maui.Controls/BindableProperty.xml" path="//Member[@MemberName='DefaultBindingMode']/Docs/*" />
		public BindingMode DefaultBindingMode { get; private set; }

		/// <include file="../../docs/Microsoft.Maui.Controls/BindableProperty.xml" path="//Member[@MemberName='DefaultValue']/Docs/*" />
		public object DefaultValue { get; }

		/// <include file="../../docs/Microsoft.Maui.Controls/BindableProperty.xml" path="//Member[@MemberName='IsReadOnly']/Docs/*" />
		public bool IsReadOnly { get; private set; }

		/// <include file="../../docs/Microsoft.Maui.Controls/BindableProperty.xml" path="//Member[@MemberName='PropertyName']/Docs/*" />
		public string PropertyName { get; }

		/// <include file="../../docs/Microsoft.Maui.Controls/BindableProperty.xml" path="//Member[@MemberName='ReturnType']/Docs/*" />
		[DynamicallyAccessedMembers(ReturnTypeMembers)] 
		public Type ReturnType { get; }

		internal BindablePropertyBindingChanging BindingChanging { get; private set; }

		internal CoerceValueDelegate CoerceValue { get; private set; }

		internal CreateDefaultValueDelegate DefaultValueCreator { get; }

		internal BindingPropertyChangedDelegate PropertyChanged { get; private set; }

		internal BindingPropertyChangingDelegate PropertyChanging { get; private set; }

		internal ValidateValueDelegate ValidateValue { get; private set; }

		/// <include file="../../docs/Microsoft.Maui.Controls/BindableProperty.xml" path="//Member[@MemberName='Create']/Docs/*" />
		public static BindableProperty Create(string propertyName, [DynamicallyAccessedMembers(ReturnTypeMembers)] Type returnType, [DynamicallyAccessedMembers(DeclaringTypeMembers)] Type declaringType, object defaultValue = null, BindingMode defaultBindingMode = BindingMode.OneWay,
											  ValidateValueDelegate validateValue = null, BindingPropertyChangedDelegate propertyChanged = null, BindingPropertyChangingDelegate propertyChanging = null,
											  CoerceValueDelegate coerceValue = null, CreateDefaultValueDelegate defaultValueCreator = null)
		{
			return new BindableProperty(propertyName, returnType, declaringType, defaultValue, defaultBindingMode, validateValue, propertyChanged, propertyChanging, coerceValue,
				defaultValueCreator: defaultValueCreator);
		}

		/// <include file="../../docs/Microsoft.Maui.Controls/BindableProperty.xml" path="//Member[@MemberName='CreateAttached']/Docs/*" />
		public static BindableProperty CreateAttached(string propertyName, [DynamicallyAccessedMembers(ReturnTypeMembers)] Type returnType, [DynamicallyAccessedMembers(DeclaringTypeMembers)] Type declaringType, object defaultValue, BindingMode defaultBindingMode = BindingMode.OneWay,
													  ValidateValueDelegate validateValue = null, BindingPropertyChangedDelegate propertyChanged = null, BindingPropertyChangingDelegate propertyChanging = null,
													  CoerceValueDelegate coerceValue = null, CreateDefaultValueDelegate defaultValueCreator = null)
		{
			return CreateAttached(propertyName, returnType, declaringType, defaultValue, defaultBindingMode, validateValue, propertyChanged, propertyChanging, coerceValue, null, false, defaultValueCreator);
		}

		/// <include file="../../docs/Microsoft.Maui.Controls/BindableProperty.xml" path="//Member[@MemberName='CreateAttachedReadOnly']/Docs/*" />
		public static BindablePropertyKey CreateAttachedReadOnly(string propertyName, [DynamicallyAccessedMembers(ReturnTypeMembers)] Type returnType, [DynamicallyAccessedMembers(DeclaringTypeMembers)] Type declaringType, object defaultValue, BindingMode defaultBindingMode = BindingMode.OneWayToSource,
																 ValidateValueDelegate validateValue = null, BindingPropertyChangedDelegate propertyChanged = null, BindingPropertyChangingDelegate propertyChanging = null,
																 CoerceValueDelegate coerceValue = null, CreateDefaultValueDelegate defaultValueCreator = null)
		{
			return
				new BindablePropertyKey(CreateAttached(propertyName, returnType, declaringType, defaultValue, defaultBindingMode, validateValue, propertyChanged, propertyChanging, coerceValue, null, true,
					defaultValueCreator));
		}

		/// <include file="../../docs/Microsoft.Maui.Controls/BindableProperty.xml" path="//Member[@MemberName='CreateReadOnly']/Docs/*" />
		public static BindablePropertyKey CreateReadOnly(string propertyName, [DynamicallyAccessedMembers(ReturnTypeMembers)] Type returnType, [DynamicallyAccessedMembers(DeclaringTypeMembers)] Type declaringType, object defaultValue, BindingMode defaultBindingMode = BindingMode.OneWayToSource,
														 ValidateValueDelegate validateValue = null, BindingPropertyChangedDelegate propertyChanged = null, BindingPropertyChangingDelegate propertyChanging = null,
														 CoerceValueDelegate coerceValue = null, CreateDefaultValueDelegate defaultValueCreator = null)
		{
			return
				new BindablePropertyKey(new BindableProperty(propertyName, returnType, declaringType, defaultValue, defaultBindingMode, validateValue, propertyChanged, propertyChanging, coerceValue,
					isReadOnly: true, defaultValueCreator: defaultValueCreator));
		}

		internal static BindableProperty Create(string propertyName, [DynamicallyAccessedMembers(ReturnTypeMembers)] Type returnType, [DynamicallyAccessedMembers(DeclaringTypeMembers)] Type declaringType, object defaultValue, BindingMode defaultBindingMode, ValidateValueDelegate validateValue,
												BindingPropertyChangedDelegate propertyChanged, BindingPropertyChangingDelegate propertyChanging, CoerceValueDelegate coerceValue, BindablePropertyBindingChanging bindingChanging,
												CreateDefaultValueDelegate defaultValueCreator = null)
		{
			return new BindableProperty(propertyName, returnType, declaringType, defaultValue, defaultBindingMode, validateValue, propertyChanged, propertyChanging, coerceValue, bindingChanging,
				defaultValueCreator: defaultValueCreator);
		}

		internal static BindableProperty CreateAttached(string propertyName, [DynamicallyAccessedMembers(ReturnTypeMembers)] Type returnType, [DynamicallyAccessedMembers(DeclaringTypeMembers)] Type declaringType, object defaultValue, BindingMode defaultBindingMode, ValidateValueDelegate validateValue,
														BindingPropertyChangedDelegate propertyChanged, BindingPropertyChangingDelegate propertyChanging, CoerceValueDelegate coerceValue, BindablePropertyBindingChanging bindingChanging,
														bool isReadOnly, CreateDefaultValueDelegate defaultValueCreator = null)
		{
			return new BindableProperty(propertyName, returnType, declaringType, defaultValue, defaultBindingMode, validateValue, propertyChanged, propertyChanging, coerceValue, bindingChanging, isReadOnly,
				defaultValueCreator);
		}

		internal object GetDefaultValue(BindableObject bindable)
		{
			if (DefaultValueCreator != null)
				return DefaultValueCreator(bindable);

			return DefaultValue;
		}

		internal delegate void BindablePropertyBindingChanging(BindableObject bindable, BindingBase oldValue, BindingBase newValue);
	}
}
