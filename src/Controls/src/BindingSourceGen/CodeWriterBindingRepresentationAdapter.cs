namespace Microsoft.Maui.Controls.BindingSourceGen;

public class CodeWriterBindingRepresentationAdapter
{
    public static CodeWriterBinding Transform(CodeWriterBinding source) {
        return TransformExplicitCastsToAsCasts(source);
    }

    private static CodeWriterBinding TransformExplicitCastsToAsCasts(CodeWriterBinding source) {
        
        var path = source.Path;
        var array = new IPathPart[path.Length];
		bool foundCast = false;

		for (int i = 0; i < path.Length; i++)
		{
			var part = path[i];
			if (part is Cast)
			{
				foundCast = true;
			}

			if (foundCast && part is MemberAccess memberAccess)
			{
				array[i] = new ConditionalAccess(memberAccess);
			}
			else
			{
				array[i] = part;
			}
        }

        return source with { Path = new EquatableArray<IPathPart>(array) };
    }
}