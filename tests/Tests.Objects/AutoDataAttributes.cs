using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;

namespace Tests;

[AttributeUsage(AttributeTargets.Method)]
public class CustomAutoDataAttribute : AutoDataAttribute
{
    public CustomAutoDataAttribute() : base(FixtureHelper.CreateFixture)
    {
            
    }
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class CustomInlineAutoDataAttribute : InlineAutoDataAttribute
{
    public CustomInlineAutoDataAttribute(params object[] arguments) : base(FixtureHelper.CreateFixture, arguments)
    {
            
    }
}

public static class FixtureHelper
{
    public static IFixture CreateFixture()
    {
        var fixture = new Fixture();

        fixture.Customize(new AutoMoqCustomization
        {
            ConfigureMembers = true,
            GenerateDelegates = true
        });

        return fixture;
    }
}