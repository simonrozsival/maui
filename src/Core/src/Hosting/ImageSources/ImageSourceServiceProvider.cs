#nullable enable

using System;
using System.Collections.Concurrent;
using Microsoft.Maui.Hosting.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Maui.Hosting
{
	sealed class ImageSourceServiceProvider : IMauiFactory, IImageSourceServiceProvider
	{
		static readonly string ImageSourceInterface = typeof(IImageSource).FullName!;
		static readonly Type ImageSourceServiceType = typeof(IImageSourceService<>);

		readonly ConcurrentDictionary<Type, Type> _imageSourceCache = new ConcurrentDictionary<Type, Type>();
		readonly ConcurrentDictionary<Type, Type> _serviceCache = new ConcurrentDictionary<Type, Type>();

		private readonly ServiceProvider _serviceProvider;

		public ImageSourceServiceProvider(IImageSourceServiceCollection collection, IServiceProvider hostServiceProvider)
		{
			collection.AddSingleton<IImageSourceServiceProvider>(this);
			_serviceProvider = collection.BuildServiceProvider(new ServiceProviderOptions());

			HostServiceProvider = hostServiceProvider;
		}

		public IServiceProvider HostServiceProvider { get; }

		public object? GetService(Type serviceType) =>
			_serviceProvider.GetService(serviceType);

		public IImageSourceService? GetImageSourceService(Type imageSource) =>
			(IImageSourceService?)GetService(GetImageSourceServiceType(imageSource));

		public Type GetImageSourceServiceType(Type imageSource) =>
			_serviceCache.GetOrAdd(imageSource, type =>
			{
				var genericConcreteType = ImageSourceServiceType.MakeGenericType(type);

				if (genericConcreteType != null && GetService(genericConcreteType) != null)
					return genericConcreteType;

				return ImageSourceServiceType.MakeGenericType(GetImageSourceType(type));
			});

		public Type GetImageSourceType(Type imageSource) =>
			_imageSourceCache.GetOrAdd(imageSource, CreateImageSourceTypeCacheEntry);

		Type CreateImageSourceTypeCacheEntry(Type type)
		{
			if (type.IsInterface)
			{
				if (type.GetInterface(ImageSourceInterface) != null)
					return type;
			}
			else
			{
				foreach (var directInterface in type.GetInterfaces())
				{
					if (directInterface.GetInterface(ImageSourceInterface) != null)
						return directInterface;
				}
			}

			throw new InvalidOperationException($"Unable to find the image source type because none of the interfaces on {type.Name} were derived from {nameof(IImageSource)}.");
		}
	}
}