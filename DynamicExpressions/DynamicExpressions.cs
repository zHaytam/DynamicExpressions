using System;
using System.Linq.Expressions;
using System.Reflection;

namespace DynamicExpressions
{
    public static class DynamicExpressions
    {
        private static readonly Type _stringType = typeof(string);

        private static readonly MethodInfo _toStringMethod = typeof(object).GetMethod("ToString");

        private static readonly MethodInfo _containsMethod = typeof(string).GetMethod("Contains"
            , new Type[] { typeof(string) });
        private static readonly MethodInfo _containsMethodIgnoreCase = typeof(string).GetMethod("Contains"
            , new Type[] { typeof(string), typeof(StringComparison) });

        private static readonly MethodInfo _endsWithMethod
            = typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) });

        private static readonly MethodInfo _startsWithMethod
                    = typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) });

        public static Expression<Func<TEntity, bool>> GetPredicate<TEntity>(string property, FilterOperator op, object value)
        {
            var param = Expression.Parameter(typeof(TEntity));
            return Expression.Lambda<Func<TEntity, bool>>(GetFilter(param, property, op, value), param);
        }

        public static Expression<Func<TEntity, object>> GetPropertyGetter<TEntity>(string property)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            var param = Expression.Parameter(typeof(TEntity));
            var prop = param.GetNestedProperty(property);
            var convertedProp = Expression.Convert(prop, typeof(object));
            return Expression.Lambda<Func<TEntity, object>>(convertedProp, param);
        }

        internal static Expression GetFilter(ParameterExpression param, string property, FilterOperator op, object value)
        {
            var constant = Expression.Constant(value);
            var prop = param.GetNestedProperty(property);
            return CreateFilter(prop, op, constant);
        }

        private static Expression CreateFilter(MemberExpression prop, FilterOperator op, ConstantExpression constant)
        {
            return op switch
            {
                FilterOperator.Equals => Expression.Equal(prop, constant),
                FilterOperator.GreaterThan => Expression.GreaterThan(prop, constant),
                FilterOperator.LessThan => Expression.LessThan(prop, constant),
                FilterOperator.Contains => Expression.Call(prop, _containsMethod, PrepareConstant(constant)),
                FilterOperator.ContainsIgnoreCase => Expression.Call(prop, _containsMethodIgnoreCase, PrepareConstant(constant), Expression.Constant(StringComparison.OrdinalIgnoreCase)),
                FilterOperator.StartsWith => Expression.Call(prop, _startsWithMethod, PrepareConstant(constant)),
                FilterOperator.EndsWith => Expression.Call(prop, _endsWithMethod, PrepareConstant(constant)),
                FilterOperator.DoesntEqual => Expression.NotEqual(prop, constant),
                FilterOperator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(prop, constant),
                FilterOperator.LessThanOrEqual => Expression.LessThanOrEqual(prop, constant),
                _ => throw new NotImplementedException()
            };
        }

        private static Expression PrepareConstant(ConstantExpression constant)
        {
            if (constant.Type == _stringType)
                return constant;

            var convertedExpr = Expression.Convert(constant, typeof(object));
            return Expression.Call(convertedExpr, _toStringMethod);
        }
    }
}