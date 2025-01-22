using System.Collections;
using Opss.PrimaryAuthorityRegister.Api.Application.Extensions;

namespace Opss.PrimaryAuthorityRegister.Api.Application.UnitTests.Extensions;

public class ReflectionExtensionsTests
{
    [Fact]
    public void CanBeNull_ShouldReturnTrue_ForNonPrimitiveTypes()
    {
        Assert.True(typeof(string).CanBeNull());
        Assert.True(typeof(object).CanBeNull());
        Assert.True(typeof(List<int>).CanBeNull());
    }

    [Fact]
    public void CanBeNull_ShouldReturnFalse_ForPrimitiveTypes()
    {
        Assert.False(typeof(int).CanBeNull());
        Assert.False(typeof(double).CanBeNull());
        Assert.False(typeof(bool).CanBeNull());
    }

    [Fact]
    public void GetAttributes_ShouldReturnAttributes_WhenPresent()
    {
        var memberInfo = typeof(TestClass).GetMethod(nameof(TestClass.TestMethod));
        var attributes = memberInfo.GetAttributes<ObsoleteAttribute>(false);

        Assert.Single(attributes);
        Assert.Equal("This method is obsolete.", attributes.First().Message);
    }

    [Fact]
    public void GetAttributes_ShouldReturnEmpty_WhenNoAttributesPresent()
    {
        var memberInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutAttributes));
        var attributes = memberInfo.GetAttributes<ObsoleteAttribute>(false);

        Assert.Empty(attributes);
    }

    [Fact]
    public void GetNonNullableType_ShouldReturnUnderlyingType_ForNullableTypes()
    {
        Assert.Equal(typeof(int), typeof(int?).GetNonNullableType());
        Assert.Equal(typeof(double), typeof(double?).GetNonNullableType());
    }

    [Fact]
    public void GetNonNullableType_ShouldReturnSameType_ForNonNullableTypes()
    {
        Assert.Equal(typeof(string), typeof(string).GetNonNullableType());
        Assert.Equal(typeof(int), typeof(int).GetNonNullableType());
    }

    [Fact]
    public void HasAttribute_ShouldReturnTrue_WhenAttributeIsPresent()
    {
        var memberInfo = typeof(TestClass).GetMethod(nameof(TestClass.TestMethod));
        Assert.True(memberInfo.HasAttribute<ObsoleteAttribute>());
    }

    [Fact]
    public void HasAttribute_ShouldReturnFalse_WhenAttributeIsNotPresent()
    {
        var memberInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutAttributes));
        Assert.False(memberInfo.HasAttribute<ObsoleteAttribute>());
    }

    [Fact]
    public void NonGenericHasAttribute_ShouldReturnTrue_WhenAttributeIsPresent()
    {
        var memberInfo = typeof(TestClass).GetMethod(nameof(TestClass.TestMethod));
        Assert.True(memberInfo.HasAttribute(typeof(ObsoleteAttribute)));
    }

    [Fact]
    public void NonGenericHasAttribute_ShouldReturnFalse_WhenAttributeIsNotPresent()
    {
        var memberInfo = typeof(TestClass).GetMethod(nameof(TestClass.MethodWithoutAttributes));
        Assert.False(memberInfo.HasAttribute(typeof(ObsoleteAttribute)));
    }

    [Fact]
    public void IsEnumerable_ShouldReturnTrue_ForEnumerableTypes()
    {
        Assert.True(typeof(List<int>).IsEnumerable());
        Assert.True(typeof(int[]).IsEnumerable());
    }

    [Fact]
    public void IsEnumerable_ShouldReturnFalse_ForNonEnumerableTypes()
    {
        Assert.False(typeof(int).IsEnumerable());
        Assert.False(typeof(string).IsEnumerable()); // Explicitly excluded in the implementation
    }

    private class TestClass
    {
        [Obsolete("This method is obsolete.")]
        public void TestMethod() { }

        public void MethodWithoutAttributes() { }
    }
}
