#nullable enable
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Maui.Hosting.Internal
{
	class MauiFactory : IMauiFactory
	{
		readonly ServiceProvider _serviceProvider;

		public MauiFactory(IMauiServiceCollection collection)
		{
			if (this is IImageSourceServiceProvider imgProvider)
			{
				collection.AddSingleton<IImageSourceServiceProvider>(imgProvider);
			}

			_serviceProvider = collection.BuildServiceProvider(new ServiceProviderOptions());
		}

		public object? GetService(Type serviceType) => _serviceProvider.GetService(serviceType);
	}
}