#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Maui.Hosting
{
	public interface IMauiServiceCollection : IEnumerable, IEnumerable<ServiceDescriptor>
	{
		bool TryGetService(Type serviceType, out ServiceDescriptor? descriptor);
	}
}