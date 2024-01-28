using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Maui.Hosting
{
	public interface IImageSourceServiceCollection : IMauiServiceCollection
	{
		/// <summary>
		/// Registers an image service with the underlying service container via AddSingleton.
		/// </summary>
		/// <typeparam name="TImageSource">The image type to register for</typeparam>
		/// <typeparam name="TImageSourceService">The service type to register</typeparam>
		/// <returns>The service collection</returns>
		IImageSourceServiceCollection AddService<TImageSource, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImageSourceService>()
			where TImageSource : IImageSource
			where TImageSourceService : class, IImageSourceService<TImageSource>;

		/// <summary>
		/// Registers an image service with the underlying service container via AddSingleton.
		/// </summary>
		/// <typeparam name="TImageSource">The image type to register for</typeparam>
		/// <param name="implementationFactory">A factory method to create the service</param>
		/// <returns>The service collection</returns>
		IImageSourceServiceCollection AddService<TImageSource>(Func<IServiceProvider, IImageSourceService<TImageSource>> implementationFactory)
			where TImageSource : IImageSource;
	}
}