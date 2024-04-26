using Microsoft.Maui.Controls.BindingSourceGen;
using Xunit;

namespace BindingSourceGen.UnitTests;

public class CodeWriterBindingRepresentationAdapterTests
{
    [Fact]
    public void CorrectlyTransformsExplicitCastToAsCast()
    {
        var sourceBinding = new CodeWriterBinding(
                new InterceptorLocation(@"Path\To\Program.cs", 3, 7),
                new TypeDescription("global::Foo"),
                new TypeDescription("int", IsValueType: true),
                new EquatableArray<IPathPart>([
                    new MemberAccess("C"),
                    new Cast(new TypeDescription("global::C")),
                    new MemberAccess("X"),
                ]),
                SetterOptions: new(IsWritable: true));

        
        var expectedBinding = new CodeWriterBinding(
                new InterceptorLocation(@"Path\To\Program.cs", 3, 7),
                new TypeDescription("global::Foo"),
                new TypeDescription("int", IsValueType: true),
                new EquatableArray<IPathPart>([
                    new MemberAccess("C"),
                    new Cast(new TypeDescription("global::C")),
                    new ConditionalAccess(new MemberAccess("X")),
                ]),
                SetterOptions: new(IsWritable: true));

        var convertedBinding = CodeWriterBindingRepresentationAdapter.Transform(sourceBinding);
        Assert.Equal(expectedBinding, convertedBinding);
    }

    [Fact]
    public void CorrectlyTransformsSeriesOfMembersAccesesFollowingAnExplicitCast()
    {
        var sourceBinding = new CodeWriterBinding(
                new InterceptorLocation(@"Path\To\Program.cs", 3, 7),
                new TypeDescription("global::Foo"),
                new TypeDescription("int", IsValueType: true),
                new EquatableArray<IPathPart>([
                    new MemberAccess("C"),
                    new Cast(new TypeDescription("global::C")),
                    new MemberAccess("X"),
                    new MemberAccess("Y"),
                    new MemberAccess("Z"),
                ]),
                SetterOptions: new(IsWritable: true));

        
        var expectedBinding = new CodeWriterBinding(
                new InterceptorLocation(@"Path\To\Program.cs", 3, 7),
                new TypeDescription("global::Foo"),
                new TypeDescription("int", IsValueType: true),
                new EquatableArray<IPathPart>([
                    new MemberAccess("C"),
                    new Cast(new TypeDescription("global::C")),
                    new ConditionalAccess(new MemberAccess("X")),
                    new ConditionalAccess(new MemberAccess("Y")),
                    new ConditionalAccess(new MemberAccess("Z")),
                ]),
                SetterOptions: new(IsWritable: true));

        var convertedBinding = CodeWriterBindingRepresentationAdapter.Transform(sourceBinding);
        Assert.Equal(expectedBinding, convertedBinding);
    }
}