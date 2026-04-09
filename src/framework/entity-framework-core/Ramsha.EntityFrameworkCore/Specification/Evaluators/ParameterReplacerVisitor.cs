namespace Ramsha.EntityFrameworkCore
{
    public sealed class ParameterReplacerVisitor : ExpressionVisitor
    {
        private ParameterExpression _oldParameter;
        private Expression _newExpression;

        public ParameterReplacerVisitor(ParameterExpression oldParameter, Expression newExpression) =>
            (_oldParameter, _newExpression) = (oldParameter, newExpression);

        internal void Update(ParameterExpression oldParameter, Expression newExpression) =>
            (_oldParameter, _newExpression) = (oldParameter, newExpression);

        protected override Expression VisitParameter(ParameterExpression node) =>
            node == _oldParameter ? _newExpression : node;
    }
}