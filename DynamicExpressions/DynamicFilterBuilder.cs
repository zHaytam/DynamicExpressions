using System;
using System.Linq.Expressions;

namespace DynamicExpressions
{
    public class DynamicFilterBuilder<TEntity>
    {
        private readonly ParameterExpression _param;

        public DynamicFilterBuilder() : this(Expression.Parameter(typeof(TEntity))) { }

        DynamicFilterBuilder(ParameterExpression param)
        {
            _param = param;
        }

        private Expression Expression { get; set; }

        public DynamicFilterBuilder<TEntity> And(string property, FilterOperator op, object value)
        {
            var newExpr = DynamicExpressions.GetFilter(_param, property, op, value);
            Expression = Expression == null ? newExpr : Expression.AndAlso(Expression, newExpr);
            return this;
        }

        public DynamicFilterBuilder<TEntity> And(Action<DynamicFilterBuilder<TEntity>> action)
        {
            var builder = new DynamicFilterBuilder<TEntity>(_param);
            action(builder);

            if (builder.Expression == null)
                throw new Exception("Empty builder");

            Expression = Expression == null ? builder.Expression : Expression.AndAlso(Expression, builder.Expression);
            return this;
        }

        public DynamicFilterBuilder<TEntity> Or(string property, FilterOperator op, object value)
        {
            var newExpr = DynamicExpressions.GetFilter(_param, property, op, value);
            Expression = Expression == null ? newExpr : Expression.OrElse(Expression, newExpr);
            return this;
        }

        public DynamicFilterBuilder<TEntity> Or(Action<DynamicFilterBuilder<TEntity>> action)
        {
            var builder = new DynamicFilterBuilder<TEntity>(_param);
            action(builder);

            if (builder.Expression == null)
                throw new Exception("Empty builder");

            Expression = Expression == null ? builder.Expression : Expression.OrElse(Expression, builder.Expression);
            return this;
        }

        public Expression<Func<TEntity, bool>> Build()
        {
            return Expression.Lambda<Func<TEntity, bool>>(Expression, _param);
        }

        public Func<TEntity, bool> Compile() => Build().Compile();
    }
}
