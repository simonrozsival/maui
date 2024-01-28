#nullable enable
using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Maui.Hosting.Internal
{
	class MauiHandlersCollection : MauiServiceCollection, IMauiHandlersCollection
	{
		public IMauiHandlersCollection AddHandler(
			Type viewType,
			[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type handlerType)
		{
			Add(ServiceDescriptor.Transient(viewType, handlerType));
			return this;
		}

		public IMauiHandlersCollection AddHandler<TType, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TTypeRender>()
			where TType : IElement
			where TTypeRender : IElementHandler
		{
			Add(ServiceDescriptor.Transient(typeof(TType), typeof(TTypeRender)));
			return this;
		}

		public IMauiHandlersCollection AddHandler<TType>(
			Func<IServiceProvider, IElementHandler> handlerImplementationFactory)
			where TType : IElement
		{
			Add(ServiceDescriptor.Transient(typeof(TType), handlerImplementationFactory));
			return this;
		}

		public IMauiHandlersCollection TryAddHandler(
			Type viewType,
			[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type handlerType)
		{
			Add(ServiceDescriptor.Transient(viewType, handlerType));
			return this;
		}

		public IMauiHandlersCollection TryAddHandler<TType, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TTypeRender>()
			where TType : IView
			where TTypeRender : IViewHandler
		{
			Add(ServiceDescriptor.Transient(typeof(TType), typeof(TTypeRender)));
			return this;
		}

		public IMauiHandlersCollection TryAddHandler<TType>(
			Func<IServiceProvider, IElementHandler> handlerImplementationFactory)
			where TType : IElement
		{
			Add(ServiceDescriptor.Transient(typeof(TType), handlerImplementationFactory));
			return this;
		}
	}
}