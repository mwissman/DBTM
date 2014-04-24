using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace DBTM.Domain
{
    public static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var enumed in enumerable)
            {
                action.Invoke(enumed);
            }
        }

        public static string GetMemberName<T>(this Expression<Func<T, object>> expressionToReflectAgainst)
        {
            MemberExpression memberExpression = null;

            if (expressionToReflectAgainst.Body.NodeType == ExpressionType.Convert)
            {
                memberExpression = (expressionToReflectAgainst.Body as UnaryExpression).Operand as MemberExpression;
            }
            else if (expressionToReflectAgainst.Body.NodeType == ExpressionType.MemberAccess)
            {
                memberExpression = (expressionToReflectAgainst.Body as MemberExpression);
            }

            if (memberExpression == null)
            {
                throw new ArgumentException("Supplied lambda expression does not map to a property");
            }

            PropertyInfo propertyInfo = (memberExpression.Member as PropertyInfo);

            if (propertyInfo != null)
            {
                return propertyInfo.Name;
            }

            return (memberExpression.Member as FieldInfo).Name;
        }
    }
}