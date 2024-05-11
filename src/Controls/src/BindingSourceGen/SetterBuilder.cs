namespace Microsoft.Maui.Controls.BindingSourceGen;

public sealed record Setter(string[] PatternMatchingExpressions, string AssignmentStatement)
{
    public static Setter From(
        TypeDescription sourceTypeDescription,
        EquatableArray<IPathPart> path,
        bool considerAllReferenceTypesPotentiallyNullable = false,
        string sourceVariableName = "source",
        string assignedValueExpression = "value")
    {
        var builder = new SetterBuilder(considerAllReferenceTypesPotentiallyNullable, sourceVariableName, sourceTypeDescription, assignedValueExpression);

        if (path.Length > 0)
        {

            foreach (var part in path)
            {
                builder.AddPart(part);
            }
        }

        return builder.Build();
    }

    private sealed class SetterBuilder
    {
        private readonly bool _considerAllReferenceTypesPotentiallyNullable;
        private readonly string _sourceVariableName;
        private readonly string _assignedValueExpression;

        private string _expression;
        private int _variableCounter = 0;
        private List<string>? _patternMatching;
        
        private IPathPart? _currentPart;
        private IPathPart? _previousPart;
        private IPathPart _sourcePart;

        public SetterBuilder(bool considerAllReferenceTypesPotentiallyNullable, string sourceVariableName, TypeDescription sourceTypeDescription, string assignedValueExpression)
        {
            _considerAllReferenceTypesPotentiallyNullable = considerAllReferenceTypesPotentiallyNullable;
            _sourceVariableName = sourceVariableName;
            _assignedValueExpression = assignedValueExpression;

            _sourcePart = new MemberAccess(sourceVariableName, sourceTypeDescription.IsValueType, sourceTypeDescription.IsValueType && sourceTypeDescription.IsNullable);
            _currentPart = _sourcePart;

            _expression = sourceVariableName;
        }

        public void AddPart(IPathPart nextPart)
        {
            var newPart =  HandleCurrentPart(nextPart);
            _previousPart = _currentPart;
            _currentPart = newPart;

        }

        private IPathPart? HandleCurrentPart(IPathPart? nextPart)
        {
            if (_currentPart is { } currentPart && currentPart != _sourcePart)
            {
                if (currentPart is Cast { TargetType: var targetType })
                {
                    AddIsExpression(targetType.GlobalName);

                    if (nextPart is ConditionalAccess { Part: var innerPart })
                    {
                        // skip next conditional access, the current `is` expression handles it
                        return innerPart;
                    }
                }
                else if (currentPart is ConditionalAccess { Part: var innerPart })
                {
                    AddIsExpression("{}");
                    _expression = AccessExpressionBuilder.Build(_expression, innerPart);
                }
                else if ((_previousPart is MemberAccess previousPartAccess && !previousPartAccess.IsValueType || _previousPart is IndexAccess previousPartIndexAccess && !previousPartIndexAccess.IsValueType || _previousPart is ConditionalAccess) && _considerAllReferenceTypesPotentiallyNullable)
                {
                    AddIsExpression("{}");
                    _expression = AccessExpressionBuilder.Build(_expression, currentPart);
                }
                else
                {
                    _expression = AccessExpressionBuilder.Build(_expression, currentPart);
                }
            }

            return nextPart;
        }

        public void AddIsExpression(string target)
        {
            var nextVariableName = CreateNextUniqueVariableName();
            var isExpression = $"{_expression} is {target} {nextVariableName}";

            _patternMatching ??= new();
            _patternMatching.Add(isExpression);
            _expression = nextVariableName;
        }

        private string CreateNextUniqueVariableName()
        {
            return $"p{_variableCounter++}";
        }

        private string CreateAssignmentStatement()
        {
            HandleCurrentPart(nextPart: null);
            return $"{_expression} = {_assignedValueExpression};";
        }

        public Setter Build()
        {
            var assignmentStatement = CreateAssignmentStatement();
            var patterns = _patternMatching?.ToArray() ?? Array.Empty<string>();
            return new Setter(patterns, assignmentStatement);
        }
    }
}
