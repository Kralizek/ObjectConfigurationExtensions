using AutoFixture.Idioms;
using Kralizek.Extensions.Configuration.Internal;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Tests.Internal;

[TestFixture]
public class ObjectConfigurationSourceTests
{
    [Test, CustomAutoData]
    public void Constructor_is_guarded(GuardClauseAssertion assertion)
    {
            assertion.Verify(typeof(ObjectConfigurationSource).GetConstructors());
        }

    [Test, CustomAutoData]
    public void Build_creates_a_provider(ObjectConfigurationSource sut, IConfigurationBuilder configurationBuilder)
    {
            var provider = sut.Build(configurationBuilder);

            Assert.That(provider, Is.InstanceOf<ObjectConfigurationProvider>());
        }
}