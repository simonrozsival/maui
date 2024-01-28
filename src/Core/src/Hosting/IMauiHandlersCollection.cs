using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Maui.Hosting
{
	public interface IMauiHandlersCollection : IMauiServiceCollection
	{
		/// <summary>
		/// Registers a handler with the underlying service container via AddTransient.
		/// </summary>
		/// <param name="viewType">The type of view to register</param>
		/// <param name="handlerType">The handler type that represents the element</param>
		/// <returns>The handler collection</returns>
		IMauiHandlersCollection AddHandler(Type viewType, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type handlerType);

		/// <summary>
		/// Registers a handler with the underlying service container via AddTransient.
		/// </summary>
		/// <typeparam name="TType">The type of element to register</typeparam>
		/// <typeparam name="TTypeRender">The handler type that represents the element</typeparam>
		/// <returns>The handler collection</returns>
		IMauiHandlersCollection AddHandler<TType, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TTypeRender>()
			where TType : IElement
			where TTypeRender : IElementHandler;

		/// <summary>
		/// Registers a handler with the underlying service container via AddTransient.
		/// </summary>
		/// <typeparam name="TType">The type of element to register</typeparam>
		/// <param name="handlerImplementationFactory">A factory method to create the handler</param>
		/// <returns>The handler collection</returns>
		IMauiHandlersCollection AddHandler<TType>(Func<IServiceProvider, IElementHandler> handlerImplementationFactory)
			where TType : IElement;

		/// <summary>
		/// Registers a handler with the underlying service container via AddTransient.
		/// </summary>
		/// <param name="viewType">The type of element to register</param>
		/// <param name="handlerType">The handler type that represents the element</param>
		/// <returns>The handler collection</returns>
		IMauiHandlersCollection TryAddHandler(Type viewType, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type handlerType);

		/// <summary>
		/// Registers a handler with the underlying service container via AddTransient.
		/// </summary>
		/// <typeparam name="TType">The type of element to register</typeparam>
		/// <typeparam name="TTypeRender">The handler type that represents the element</typeparam>
		/// <returns>The handler collection</returns>
		IMauiHandlersCollection TryAddHandler<TType, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TTypeRender>()
			where TType : IView
			where TTypeRender : IViewHandler;

		/// <summary>
		/// Registers a handler with the underlying service container via AddTransient.
		/// </summary>
		/// <typeparam name="TType">The type of element to register</typeparam>
		/// <param name="handlerImplementationFactory">A factory method to create the handler</param>
		/// <returns>The handler collection</returns>
		IMauiHandlersCollection TryAddHandler<TType>(Func<IServiceProvider, IElementHandler> handlerImplementationFactory)
			where TType : IElement;
	}
}