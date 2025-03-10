﻿using System.Reflection;

namespace Opss.PrimaryAuthorityRegister.Api.Application.Extensions;

/// <summary>
/// Provides a number of extension methods to relfection-based classes, consiting of small
/// utility methods that can cut down on a small amount of boilerplate elsewhere in the codebase.
/// </summary>
public static class ReflectionExtensions
{
    /// <summary>
    /// Indicates whether or not the specified type is a nullable type or not.
    /// </summary>
    /// <param name="type">The type which is to be checked.</param>
    /// <returns>Whether or not the specified type is a Nullable type.</returns>
    public static bool CanBeNull(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        
        return !type.IsPrimitive;
    }

    /// <summary>
    /// Gets attributes of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of attribute to look for.</typeparam>
    /// <param name="memberInfo">The member to search for attributes.</param>
    /// <param name="inherit"><c>true</c> to search this member's inheritance chain to find the attributes; otherwise, <c>false</c>. 
    /// This parameter is ignored for properties and events.</param>
    /// <returns>The list of attributes for this member that are of the given type.</returns>
    public static IEnumerable<T> GetAttributes<T>(this MemberInfo memberInfo, bool inherit)
    {
        ArgumentNullException.ThrowIfNull(memberInfo);

        return memberInfo.GetCustomAttributes(typeof(T), inherit).Cast<T>();
    }

    /// <summary>
    /// Given a type will get its non-nullable version of it represents a nullable type (e.g. Nullable&lt;int&gt;),
    /// or the type if it is already a non-nullable type.
    /// </summary>
    /// <param name="type">The type to get a non-nullable version from.</param>
    /// <returns>The non-nullable version of the given type.</returns>
    public static Type? GetNonNullableType(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            return Nullable.GetUnderlyingType(type);
        }

        return type;
    }

    /// <summary>
    /// Indicates whether the given member has any attributes of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of attribute to look for.</typeparam>
    /// <param name="memberInfo">The member to search for attributes.</param>
    /// <param name="inherit"><c>true</c> to search this member's inheritance chain to find the attributes; otherwise, <c>false</c>. 
    /// This parameter is ignored for properties and events.</param>
    /// <returns>Whether this member has any attributes of the specified type.</returns>
    public static bool HasAttribute<T>(this MemberInfo memberInfo, bool inherit = true)
    {
        ArgumentNullException.ThrowIfNull(memberInfo);
        
        return memberInfo.GetCustomAttributes(typeof(T), inherit).Length != 0;
    }

    /// <summary>
    /// Indicates whether the given member has any attributes of the specified type.
    /// </summary>
    /// <param name="memberInfo">The member to search for attributes.</param>
    /// <param name="attributeType">The type of attribute to look for.</param>
    /// <param name="inherit"><c>true</c> to search this member's inheritance chain to find the attributes; otherwise, <c>false</c>. 
    /// This parameter is ignored for properties and events.</param>
    /// <returns>Whether this member has any attributes of the specified type.</returns>
    public static bool HasAttribute(this MemberInfo memberInfo, Type attributeType, bool inherit = true)
    {
        ArgumentNullException.ThrowIfNull(memberInfo);
        
        return memberInfo.GetCustomAttributes(attributeType, inherit).Length != 0;
    }

    /// <summary>
    /// Indicates whether or not the specified type is an IEnumerable type, such as a List
    /// or Set, excluding the string type which would typically not be treated as an actual
    /// enumerable type.
    /// </summary>
    /// <param name="type">The type which is to be checked.</param>
    /// <returns>Whether or not the specified type is an IEnumerable type.</returns>
    public static bool IsEnumerable(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        
        return type != typeof(string) && type.GetInterface("IEnumerable") != null;
    }
}
