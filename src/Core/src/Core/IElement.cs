namespace Microsoft.Maui
{
	public interface IElement
	{
		/// <summary>
		/// Gets or sets the View Handler of the Element.
		/// </summary>
		IElementHandler? Handler { get; set; }

		/// <summary>
		/// Gets the Parent of the Element.
		/// </summary>
		IElement? Parent { get; }

#pragma warning disable RS0016 // Symbol 'CreateElementHandler' is not part of the declared public API
		/// <summary>
		/// Creates the View Handler of the Element.
		/// </summary>
		IElementHandler? CreateElementHandler(IMauiContext context);
#pragma warning restore RS0016

#pragma warning disable RS0016 // Symbol 'GetElementHandlerType' is not part of the declared public API
		/// <summary>
		/// Gets the View Handler type of the Element.
		/// </summary>
		System.Type? GetElementHandlerType(IMauiContext context);
#pragma warning restore RS0016
	}
}