#nullable enable
using System;

namespace Microsoft.Maui
{
    // TODO: make public
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = true)]
	internal abstract class RegisteredCastsAttribute : Attribute
	{
		protected internal abstract void RegisterCasts(ImplicitCastCollectionBuilder builder);
	}
}
