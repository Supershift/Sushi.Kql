using System.Linq.Expressions;
using System.Reflection;

namespace Sushi.Kql;

internal static class ReflectionHelper
{
    public static List<MemberInfo> GetMemberTree<T>(Expression<Func<T, object?>> expression)
    {
        return GetMemberTreeInternal(expression.Body);
    }

    public static List<MemberInfo> GetMemberTree(MemberExpression expression)
    {
        return GetMemberTreeInternal(expression);
    }

    private static List<MemberInfo> GetMemberTreeInternal(Expression expression)
    {
        // Handle boxing conversions like (object)x.Property
        if (expression is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
        {
            expression = unary.Operand;
        }

        var path = new List<MemberInfo>();
        while (expression is MemberExpression member)
        {
            path.Add(member.Member);
            expression = member.Expression!;
        }

        //reverse the result, so it starts with the lowest level property
        if (path.Count > 1)
            path.Reverse();

        return path;
    }

    /// <summary>
    /// Gets the value of the member defined by <paramref name="memberInfo"/> on <paramref name="entity"/>.
    /// </summary>
    /// <param name="memberInfo"></param>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static object? GetMemberValue(MemberInfo memberInfo, object entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        switch (memberInfo.MemberType)
        {
            case MemberTypes.Field:
                return ((FieldInfo)memberInfo).GetValue(entity);
            case MemberTypes.Property:
                return ((PropertyInfo)memberInfo).GetValue(entity);
            default:
                throw new ArgumentException($"Only {MemberTypes.Field} and {MemberTypes.Property} are supported.", nameof(memberInfo));
        }
    }

    /// <summary>
    /// Sets <paramref name="value"/> on the property defined by <paramref name="memberInfoTree"/> on <paramref name="entity"/>.
    /// </summary>
    /// <param name="memberInfoTree"></param>
    /// <param name="value"></param>
    /// <param name="entity"></param>    
    public static void SetMemberValue(List<MemberInfo> memberInfoTree, object? value, object entity)
    {
        if (memberInfoTree == null)
            throw new ArgumentNullException(nameof(memberInfoTree));
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));
        if (memberInfoTree.Count == 0)
            throw new ArgumentException("cannot contain zero items", nameof(memberInfoTree));

        // if the expression points to a nested property (ie. Customer.Address.Street), we need to create the objects along the way
        if (memberInfoTree.Count > 1)
        {
            foreach (var memberInfo in memberInfoTree.GetRange(0, memberInfoTree.Count - 1))
            {
                // get current node value, if null, create a new object
                var instance = GetMemberValue(memberInfo, entity);
                if (instance == null)
                {
                    // if it is a non-nullable member AND the value we try to set is not null, we need to create an instance of the type
                    if (value == null)
                    {
                        NullabilityInfoContext context = new();
                        NullabilityInfo? nullInfo = null;
                        switch (memberInfo.MemberType)
                        {
                            case MemberTypes.Field:
                                nullInfo = context.Create((FieldInfo)memberInfo);
                                break;
                            case MemberTypes.Property:
                                nullInfo = context.Create((PropertyInfo)memberInfo);
                                break;
                        }

                        if (nullInfo?.WriteState == NullabilityState.Nullable)
                        {
                            // it is nullable, we don't need to create an object and we can stop traversing the tree
                            return;
                        }
                    }
                    var type = GetMemberType(memberInfo);
                    instance = Activator.CreateInstance(type, true);
                    if (instance == null)
                    {
                        throw new Exception("Cannot use types without a parameterless constructor as subtypes.");
                    }
                    SetMemberValue(memberInfo, instance, entity);
                }
                entity = instance;
            }
        }
        //now set the db value on the final member
        var lastMemberInfo = memberInfoTree.Last();
        SetMemberValue(lastMemberInfo, value, entity);
    }

    /// <summary>
    /// Sets <paramref name="value"/> on the property defined by <paramref name="memberInfo"/> on <paramref name="entity"/>.
    /// </summary>        
    /// <param name="entity"></param>
    /// <param name="memberInfo"></param>
    /// <param name="value"></param>    
    public static void SetMemberValue(MemberInfo memberInfo, object? value, object entity)
    {
        Type targetType = GetMemberType(memberInfo);

        // if the target type is an enum, we need to convert the value to the enum's type
        value = Utility.ConvertValueToEnum(value, targetType);

        try
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    ((FieldInfo)memberInfo).SetValue(entity, value);
                    break;
                case MemberTypes.Property:
                    ((PropertyInfo)memberInfo).SetValue(entity, value, null);
                    break;
                default:
                    throw new ArgumentException($"Only {MemberTypes.Field} and {MemberTypes.Property} are supported.", nameof(memberInfo));
            }
        }
        catch (Exception innerException)
        {
            string message = $"Error while setting the {memberInfo.Name} member with an object of type {value}";

            throw new Exception(message, innerException);
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

    /// <summary>
    /// Checks if a type is a subclass of a generic type.
    /// </summary>    
    /// <returns></returns>
    public static bool IsSubClassOfGeneric(Type genericType, Type typeToCheck)
    {
        Type? type = typeToCheck;
        while (type != null)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == genericType)
            {
                return true;
            }
            type = type.BaseType;
        }
        return false;
    }
}
