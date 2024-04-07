using Microsoft.CodeAnalysis;

namespace Microsoft.Maui.Controls.BindingSourceGen;

internal static class BindingGenerationUtilities
{
    internal static bool IsTypeNullable(ITypeSymbol typeInfo, bool enabledNullable)
    {
        if (!enabledNullable && typeInfo.IsReferenceType)
        {
            return true;
        }

        if (typeInfo.NullableAnnotation == NullableAnnotation.Annotated)
        {
            return true;
        }

        return typeInfo is INamedTypeSymbol namedTypeSymbol
            && namedTypeSymbol.IsGenericType
            && namedTypeSymbol.ConstructedFrom.SpecialType == SpecialType.System_Nullable_T;
    }

    internal static TypeDescription CreateTypeNameFromITypeSymbol(ITypeSymbol typeSymbol, bool enabledNullable)
    {
        var (isNullable, name) = GetNullabilityAndName(typeSymbol, enabledNullable);
        return new TypeDescription(
            GlobalName: name,
            IsNullable: isNullable,
            IsGenericParameter: typeSymbol.Kind == SymbolKind.TypeParameter,
            IsValueType: typeSymbol.IsValueType);
    }

    internal static TypeDescription CreateTypeDescriptionForCast(ITypeSymbol typeSymbol)
    {
        // We can cast to nullable value type or non-nullable reference type
        var name = typeSymbol.IsValueType ?
            ((INamedTypeSymbol)typeSymbol).TypeArguments[0].ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) :
            typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

        return new TypeDescription(
            GlobalName: name,
            IsNullable: typeSymbol.IsValueType,
            IsGenericParameter: typeSymbol.Kind == SymbolKind.TypeParameter,
            IsValueType: typeSymbol.IsValueType);
    }

    internal static (bool, string) GetNullabilityAndName(ITypeSymbol typeSymbol, bool enabledNullable)
    {
        if (typeSymbol.IsReferenceType && (typeSymbol.NullableAnnotation == NullableAnnotation.Annotated || !enabledNullable))
        {
            return (true, typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
        }

        if (IsTypeNullable(typeSymbol, enabledNullable))
        {
            var type = ((INamedTypeSymbol)typeSymbol).TypeArguments[0];
            return (true, type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
        }

        return (false, typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat));
    }
}