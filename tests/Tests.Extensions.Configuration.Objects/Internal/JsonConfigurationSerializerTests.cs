using AutoFixture.Idioms;
using Kralizek.Extensions.Configuration.Internal;
using NUnit.Framework;

namespace Tests.Internal
{
    [TestFixture]
    public class JsonConfigurationSerializerTests
    {
        [Test, CustomAutoData]
        public void Constructor_is_guarded(GuardClauseAssertion assertion)
        {
            assertion.Verify(typeof(JsonConfigurationSerializer).GetConstructors());
        }

        [Test, CustomAutoData] 
        public void Object_is_correctly_serialized(JsonConfigurationSerializer sut, ObjectWithSimpleProperties testSource, string rootSectionName)
        {
            var result = sut.Serialize(testSource, rootSectionName);

            Assert.That(result, Contains.Key($"{rootSectionName}:{nameof(testSource.Text)}"));
            Assert.That(result, Contains.Key($"{rootSectionName}:{nameof(testSource.Value)}"));

            Assert.That(result[$"{rootSectionName}:{nameof(testSource.Text)}"], Is.EqualTo(testSource.Text));
            Assert.That(result[$"{rootSectionName}:{nameof(testSource.Value)}"], Is.EqualTo($"{testSource.Value}"));
        }

        [Test, CustomAutoData]
        public void Object_is_correctly_serialized(JsonConfigurationSerializer sut, ObjectWithInnerObject testSource, string rootSectionName)
        {
            var result = sut.Serialize(testSource, rootSectionName);

            Assert.That(result, Contains.Key($"{rootSectionName}:{nameof(testSource.InnerObject)}:{nameof(testSource.InnerObject.Text)}"));
            Assert.That(result, Contains.Key($"{rootSectionName}:{nameof(testSource.InnerObject)}:{nameof(testSource.InnerObject.Value)}"));

            Assert.That(result[$"{rootSectionName}:{nameof(testSource.InnerObject)}:{nameof(testSource.InnerObject.Text)}"], Is.EqualTo(testSource.InnerObject.Text));
            Assert.That(result[$"{rootSectionName}:{nameof(testSource.InnerObject)}:{nameof(testSource.InnerObject.Value)}"], Is.EqualTo($"{testSource.InnerObject.Value}"));
        }

        [Test, CustomAutoData]
        public void Object_with_array_is_correctly_serialized(JsonConfigurationSerializer sut, ObjectWithSimpleStringArray testSource, string rootSectionName)
        {
            var result = sut.Serialize(testSource, rootSectionName);

            Assert.That(result, Contains.Key($"{rootSectionName}:{nameof(testSource.Texts)}:0"));

            Assert.That(result[$"{rootSectionName}:{nameof(testSource.Texts)}:0"], Is.EqualTo(testSource.Texts[0]));
        }

        [Test, CustomAutoData]
        public void Object_with_array_is_correctly_serialized(JsonConfigurationSerializer sut, ObjectWithSimpleIntArray testSource, string rootSectionName)
        {
            var result = sut.Serialize(testSource, rootSectionName);

            Assert.That(result, Contains.Key($"{rootSectionName}:{nameof(testSource.Values)}:0"));

            Assert.That(result[$"{rootSectionName}:{nameof(testSource.Values)}:0"], Is.EqualTo($"{testSource.Values[0]}"));
        }

        [Test, CustomAutoData]
        public void Object_with_array_is_correctly_serialized(JsonConfigurationSerializer sut, ObjectWithComplexArray testSource, string rootSectionName)
        {
            var result = sut.Serialize(testSource, rootSectionName);

            Assert.That(result, Contains.Key($"{rootSectionName}:{nameof(testSource.Items)}:0:{nameof(ObjectWithSimpleProperties.Text)}"));
            Assert.That(result, Contains.Key($"{rootSectionName}:{nameof(testSource.Items)}:0:{nameof(ObjectWithSimpleProperties.Value)}"));

            Assert.That(result[$"{rootSectionName}:{nameof(testSource.Items)}:0:{nameof(ObjectWithSimpleProperties.Text)}"], Is.EqualTo($"{testSource.Items[0].Text}"));
            Assert.That(result[$"{rootSectionName}:{nameof(testSource.Items)}:0:{nameof(ObjectWithSimpleProperties.Value)}"], Is.EqualTo($"{testSource.Items[0].Value}"));
        }

        [Test, CustomAutoData]
        [Property("Issue", "2")]
        public void Importing_same_object_twice_should_not_throw(JsonConfigurationSerializer sut, ObjectWithComplexArray testSource, string rootSectionName)
        {
            _ = sut.Serialize(testSource, rootSectionName);

            var result = sut.Serialize(testSource, rootSectionName);

            Assert.That(result, Contains.Key($"{rootSectionName}:{nameof(testSource.Items)}:0:{nameof(ObjectWithSimpleProperties.Text)}"));
            Assert.That(result, Contains.Key($"{rootSectionName}:{nameof(testSource.Items)}:0:{nameof(ObjectWithSimpleProperties.Value)}"));

            Assert.That(result[$"{rootSectionName}:{nameof(testSource.Items)}:0:{nameof(ObjectWithSimpleProperties.Text)}"], Is.EqualTo($"{testSource.Items[0].Text}"));
            Assert.That(result[$"{rootSectionName}:{nameof(testSource.Items)}:0:{nameof(ObjectWithSimpleProperties.Value)}"], Is.EqualTo($"{testSource.Items[0].Value}"));
        }
    }
}