using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Maui.Hosting;

namespace Microsoft.Maui.Controls
{
    internal static class ConversionMauiAppBuilderExtensions
    {
        internal static MauiAppBuilder ConfigureConversions(this MauiAppBuilder builder, Action<TypeConversionService>? configureDelegate)
        {
            ConfigureConversions(builder.Services, configureDelegate);
            return builder;
        }

        internal static IServiceCollection ConfigureConversions(this IServiceCollection services, Action<TypeConversionService>? configureDelegate)
        {
            services.TryAddSingleton<TypeConversionService>(sp => new TypeConversionService(sp.GetServices<ConversionRegistration>()));
            if (configureDelegate != null)
            {
                services.AddSingleton<ConversionRegistration>(new ConversionRegistration(configureDelegate));
            }

            return services;
        }

        internal class ConversionRegistration
        {
            private readonly Action<TypeConversionService> _registerAction;

            public ConversionRegistration(Action<TypeConversionService> registerAction)
            {
                _registerAction = registerAction;
            }

            internal void Register(TypeConversionService service)
            {
                _registerAction(service);
            }
        }
    }
}
