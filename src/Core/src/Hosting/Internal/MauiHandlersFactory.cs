#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Maui.Hosting.Internal
{
	sealed class MauiHandlersFactory : IMauiFactory, IMauiHandlersFactory
	{
		public MauiHandlersFactory(IEnumerable<HandlerMauiAppBuilderExtensions.HandlerRegistration> registrationActions)
		{
			_collection = CreateHandlerCollection(registrationActions);
			_serviceProvider = _collection.BuildServiceProvider(new ServiceProviderOptions());
		}

		static MauiHandlersCollection CreateHandlerCollection(IEnumerable<HandlerMauiAppBuilderExtensions.HandlerRegistration> registrationActions)
		{
			var collection = new MauiHandlersCollection();
			if (registrationActions != null)
			{
				foreach (var registrationAction in registrationActions)
				{
					registrationAction.AddRegistration(collection);
				}
			}
			HotReload.MauiHotReloadHelper.RegisterHandlers(collection);
			return collection;
		}

		public object? GetService(Type serviceType)
			=> _serviceProvider.GetService(serviceType);

		public IElementHandler? GetHandler(Type type)
			=> GetService(type) as IElementHandler;

		public IElementHandler? GetHandler<T>() where T : IElement
			=> GetHandler(typeof(T));

		[return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
		public Type? GetHandlerType(Type iview)
			=> _collection.TryGetService(iview, out var descriptor) ? descriptor : null;

		public IMauiHandlersCollection GetCollection() => _collection;
	}
}