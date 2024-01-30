using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using Kralizek.Extensions.Configuration;
using Kralizek.Extensions.Configuration.Internal;
using Moq;
using NUnit.Framework;

namespace Tests.Internal;

[TestFixture]
public class ObjectConfigurationProviderTests
{
    [Test, CustomAutoData]
    public void Constructor_is_guarded(GuardClauseAssertion assertion)
    {
        assertion.Verify(typeof(ObjectConfigurationProvider).GetConstructors());
    }

    [Test, CustomAutoData]
    public void Load_uses_serializer([Frozen] IConfigurationSerializer serializer, [Frozen] object testSource, [Frozen] string rootSectionName, ObjectConfigurationProvider sut)
    {
        sut.Load();

        Mock.Get(serializer).Verify(p => p.Serialize(testSource, rootSectionName), Times.Once);
    }
}