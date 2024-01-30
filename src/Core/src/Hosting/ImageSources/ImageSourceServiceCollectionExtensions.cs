﻿using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Maui.Hosting
{
	public static class ImageSourceServiceCollectionExtensions
	{
		/// <summary>
		/// Registers an image service with the underlying service container via AddSingleton.
		/// </summary>
		/// <typeparam name="TImageSource">The image type to register for</typeparam>
		/// <typeparam name="TImageSourceService">The service type to register</typeparam>
		/// <param name="services">The service collection</param>
		/// <returns>The service collection</returns>
		public static IImageSourceServiceCollection AddService<TImageSource, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImageSourceService>(this IImageSourceServiceCollection services)
			where TImageSource : IImageSource
			where TImageSourceService : class, IImageSourceService<TImageSource>
		{
#pragma warning disable RS0030 // Do not use banned APIs, the current method is also banned
			services.AddSingleton<IImageSourceService<TImageSource>, TImageSourceService>();
#pragma warning restore RS0030 // Do not use banned APIs

			return services;
		}

		/// <summary>
		/// Registers an image service with the underlying service container via AddSingleton.
		/// </summary>
		/// <typeparam name="TImageSource">The image type to register for</typeparam>
		/// <param name="services">The service collection</param>
		/// <param name="implementationFactory">A factory method to create the service</param>
		/// <returns>The service collection</returns>
		public static IImageSourceServiceCollection AddService<TImageSource>(this IImageSourceServiceCollection services, Func<IServiceProvider, IImageSourceService<TImageSource>> implementationFactory)
			where TImageSource : IImageSource
		{
			services.AddSingleton(provider => implementationFactory(provider.GetRequiredService<IImageSourceServiceProvider>().HostServiceProvider));

			return services;
		}
	}
}