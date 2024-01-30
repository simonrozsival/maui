using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Maui.Hosting
{
	public static partial class MauiHandlersCollectionExtensions
	{
		/// <summary>
		/// Registers a handler with the underlying service container via AddTransient.
		/// </summary>
		/// <param name="handlersCollection">The element collection</param>
		/// <param name="viewType">The type of view to register</param>
		/// <param name="handlerType">The handler type that represents the element</param>
		/// <returns>The handler collection</returns>
		public static IMauiHandlersCollection AddHandler(
			this IMauiHandlersCollection handlersCollection,
			Type viewType,
			[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type handlerType)
		{
			handlersCollection.AddTransient(viewType, _ => Activator.CreateInstance(handlerType)!);
			return handlersCollection;
		}

		/// <summary>
		/// Registers a handler with the underlying service container via AddTransient.
		/// </summary>
		/// <typeparam name="TType">The type of element to register</typeparam>
		/// <typeparam name="TTypeRender">The handler type that represents the element</typeparam>
		/// <param name="handlersCollection">The handler collection</param>
		/// <returns>The handler collection</returns>
		public static IMauiHandlersCollection AddHandler<TType, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TTypeRender>(
			this IMauiHandlersCollection handlersCollection)
			where TType : IElement
			where TTypeRender : IElementHandler
		{
			handlersCollection.AddTransient(typeof(TType), _ => Activator.CreateInstance<TTypeRender>());
			return handlersCollection;
		}

		/// <summary>
		/// Registers a handler with the underlying service container via AddTransient.
		/// </summary>
		/// <typeparam name="TType">The type of element to register</typeparam>
		/// <param name="handlersCollection">The handler collection</param>
		/// <param name="handlerImplementationFactory">A factory method to create the handler</param>
		/// <returns>The handler collection</returns>
		public static IMauiHandlersCollection AddHandler<TType>(
			this IMauiHandlersCollection handlersCollection,
			Func<IServiceProvider, IElementHandler> handlerImplementationFactory)
			where TType : IElement
		{
			handlersCollection.AddTransient(typeof(TType), handlerImplementationFactory);
			return handlersCollection;
		}

		/// <summary>
		/// Registers a handler with the underlying service container via AddTransient.
		/// </summary>
		/// <param name="handlersCollection">The handler collection</param>
		/// <param name="viewType">The type of element to register</param>
		/// <param name="handlerType">The handler type that represents the element</param>
		/// <returns>The handler collection</returns>
		public static IMauiHandlersCollection TryAddHandler(
			this IMauiHandlersCollection handlersCollection,
			Type viewType,
			[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type handlerType)
		{
			handlersCollection.TryAddTransient(viewType, _ => Activator.CreateInstance(handlerType)!);
			return handlersCollection;
		}

		/// <summary>
		/// Registers a handler with the underlying service container via AddTransient.
		/// </summary>
		/// <typeparam name="TType">The type of element to register</typeparam>
		/// <typeparam name="TTypeRender">The handler type that represents the element</typeparam>
		/// <param name="handlersCollection">The handler collection</param>
		/// <returns>The handler collection</returns>
		public static IMauiHandlersCollection TryAddHandler<TType, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TTypeRender>(
			this IMauiHandlersCollection handlersCollection)
			where TType : IView
			where TTypeRender : IViewHandler
		{
			handlersCollection.TryAddTransient(typeof(TType), _ => Activator.CreateInstance<TTypeRender>());
			return handlersCollection;
		}

		/// <summary>
		/// Registers a handler with the underlying service container via AddTransient.
		/// </summary>
		/// <typeparam name="TType">The type of element to register</typeparam>
		/// <param name="handlersCollection">The handler collection</param>
		/// <param name="handlerImplementationFactory">A factory method to create the handler</param>
		/// <returns>The handler collection</returns>
		public static IMauiHandlersCollection TryAddHandler<TType>(
			this IMauiHandlersCollection handlersCollection,
			Func<IServiceProvider, IElementHandler> handlerImplementationFactory)
			where TType : IElement
		{
			handlersCollection.TryAddTransient(typeof(TType), handlerImplementationFactory);
			return handlersCollection;
		}
	}
}