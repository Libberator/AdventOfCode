using System;
using System.Reflection;

namespace AoC.Solutions;

/// <summary>
///     Apply to a field or property a value that should replace the current value when the Test Cases are running.
/// </summary>
/// <param name="testValue">Value to be used when the test case is being run</param>
/// <remarks>It's up to the developer to ensure the Type matches (this is not enforced). Otherwise, it will throw</remarks>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
internal class TestValueAttribute(object testValue) : Attribute
{
    public object TestValue = testValue;
}

internal static class SolutionModifier
{
    internal static void ApplyTestValues<T>(this T instance) where T : ISolver
    {
        var type = instance.GetType();
        const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic |
                                          BindingFlags.Instance | BindingFlags.Static;

        // overwrite field value
        foreach (var field in type.GetFields(bindingFlags))
        {
            var attribute = field.GetCustomAttribute<TestValueAttribute>();
            if (attribute == null) continue;
            var attributeValue = typeof(TestValueAttribute).GetField("TestValue")!.GetValue(attribute);
            field.SetValue(instance, attributeValue);
        }

        // overwrite property value
        foreach (var property in type.GetProperties(bindingFlags))
        {
            var attribute = property.GetCustomAttribute<TestValueAttribute>();
            if (attribute == null) continue;
            var attributeValue = typeof(TestValueAttribute).GetField("TestValue")!.GetValue(attribute);
            property.SetValue(instance, attributeValue);
        }
    }
}