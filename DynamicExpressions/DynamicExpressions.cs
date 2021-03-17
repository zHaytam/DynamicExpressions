using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DynamicExpressions
{
    public static class DynamicExpressions
    {
        private static readonly Type _stringType = typeof(string);
        private static readonly Type _enumerableType = typeof(IEnumerable<>);

        private static readonly MethodInfo _toStringMethod = typeof(object).GetMethod("ToString");

        private static readonly MethodInfo _stringContainsMethod = typeof(string).GetMethod("Contains"
            , new Type[] { typeof(string) });
        private static readonly MethodInfo _enumerableContainsMethod = typeof(Enumerable).GetMethods().Where(x => string.Equals(x.Name, "Contains", StringComparison.OrdinalIgnoreCase)).Single(x => x.GetParameters().Length == 2).MakeGenericMethod(typeof(string));

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
                FilterOperator.Contains => GetContainsMethodCallExpression(prop, op, constant),
                FilterOperator.NotContains => Expression.Not(GetContainsMethodCallExpression(prop, op, constant)),
                FilterOperator.StartsWith => Expression.Call(prop, _startsWithMethod, PrepareConstant(constant)),
                FilterOperator.EndsWith => Expression.Call(prop, _endsWithMethod, PrepareConstant(constant)),
                FilterOperator.DoesntEqual => Expression.NotEqual(prop, constant),
                FilterOperator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(prop, constant),
                FilterOperator.LessThanOrEqual => Expression.LessThanOrEqual(prop, constant),
                _ => throw new NotImplementedException()
            };
        }

        private static MethodCallExpression GetContainsMethodCallExpression(MemberExpression prop, FilterOperator filterOperator, ConstantExpression constant)
        {
            if (prop.Type == _stringType)
                return Expression.Call(prop, _stringContainsMethod, PrepareConstant(constant));
            else return Expression.Call(_enumerableContainsMethod, prop, PrepareConstant(constant));
        }

        private static Expression PrepareConstant(ConstantExpression constant)
        {
            if (constant.Type == _stringType)
                return constant;
            else if (constant.GetType().GetInterfaces().Any(
            i => i.IsGenericType &&
            i.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                return constant;

            var convertedExpr = Expression.Convert(constant, typeof(object));
            return Expression.Call(convertedExpr, _toStringMethod);
        }
    }
}