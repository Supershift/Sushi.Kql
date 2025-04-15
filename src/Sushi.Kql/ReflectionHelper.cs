using System.Linq.Expressions;
using System.Reflection;

namespace Sushi.Kql;

internal static class ReflectionHelper
{
    public static Stack<MemberInfo> GetMemberTree<T>(Expression<Func<T, object?>> expression)
    {
        return GetMemberTreeInternal(expression.Body);
    }

    public static Stack<MemberInfo> GetMemberTree(MemberExpression expression)
    {
        return GetMemberTreeInternal(expression);
    }

    private static Stack<MemberInfo> GetMemberTreeInternal(Expression expression)
    {
        // Handle boxing conversions like (object)x.Property
        if (expression is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
        {
            expression = unary.Operand;
        }

        var path = new Stack<MemberInfo>();
        while (expression is MemberExpression member)
        {
            path.Push(member.Member);
            expression = member.Expression!;
        }

        return path;
    }

    public static void SetNestedProperty(object target, string[] path, object? value)
    {
        var current = target;
        PropertyInfo? property;

        for (int i = 0; i < path.Length; i++)
        {
            var type = current.GetType();
            property = type.GetProperty(path[i]);

            if (property == null)
                throw new InvalidOperationException($"Property '{path[i]}' not found on '{type.Name}'");

            if (i == path.Length - 1)
            {
                // Convert value to property type and set it
                var convertedValue = Convert.ChangeType(
                    value,
                    Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType
                );
                property.SetValue(current, convertedValue);
            }
            else
            {
                // Traverse into or create nested object
                var nested = property.GetValue(current);
                if (nested == null)
                {
                    nested = Activator.CreateInstance(property.PropertyType)!;
                    property.SetValue(current, nested);
                }
                current = nested;
            }
        }
    }

    public static Type GetMemberType(MemberInfo memberInfo)
    {
        Type type = memberInfo.MemberType switch
        {
            MemberTypes.Field => ((FieldInfo)memberInfo).FieldType,
            MemberTypes.Property => ((PropertyInfo)memberInfo).PropertyType,
            _ => throw new ArgumentException($"Only {4} and {16} are supported.", nameof(memberInfo)),
        };
        var underlyingType = Nullable.GetUnderlyingType(type);
        return underlyingType ?? type;
    }
}
