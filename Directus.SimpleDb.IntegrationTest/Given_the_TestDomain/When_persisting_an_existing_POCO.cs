using System;
using System.Linq;
using System.Text;
using Directus.SimpleDb.IntegrationTest.Entities;
using Directus.SimpleDb.IntegrationTest.Properties;
using Directus.SimpleDb.Providers;
using FizzWare.NBuilder;
using NUnit.Framework;
using Rolstad.Extensions;

namespace Directus.SimpleDb.IntegrationTest.Given_the_TestDomain
{
    [TestFixture]
    public class When_persisting_an_existing_POCO
    {
        private TestPOCO _originalEntity;
        private TestPOCO _retrievedEntity;

        [TestFixtureSetUp]
        public void BeforeAll()
        {
            // Arrange
            var provider = new SimpleDBProvider<TestPOCO, string>(Settings.Default.AmazonAccessKey, Settings.Default.AmazonSecretKey);

            _originalEntity = Builder<TestPOCO>.CreateNew().Build();
            _originalEntity.Identifier = Guid.NewGuid().ToString();

            var stringBuilder = new StringBuilder();
            Enumerable.Range(0, 100).Each(i=>stringBuilder.Append(Guid.NewGuid().ToString()));
            _originalEntity.VerLongStringValue = stringBuilder.ToString();

            /* Save it the first time */
            provider.Save(new[] { _originalEntity });

            /* Now Update */
            _originalEntity.DateTimeValue = DateTime.Today.AddDays(3);
            _originalEntity.VerLongStringValue = "One two";

            // Act
            provider.Save(new[]{_originalEntity});

            _retrievedEntity = provider.Get(_originalEntity.Identifier);
        }

        [Test]
        public void Then_the_entity_is_retrieved()
        {
            Assert.That(_retrievedEntity,Is.Not.Null);
        }

        [Test]
        public void Then_the_identifier_is_populated_correctly()
        {
            Assert.That(_retrievedEntity.Identifier,Is.EqualTo(_originalEntity.Identifier));
        }

        [Test]
        public void Then_the_string_is_populated_correctly()
        {
            Assert.That(_retrievedEntity.StringValue, Is.EqualTo(_originalEntity.StringValue));
        }

        [Test]
        public void Then_the_bool_is_populated_correctly()
        {
            Assert.That(_retrievedEntity.BoolValue, Is.EqualTo(_originalEntity.BoolValue));
        }

        [Test]
        public void Then_the_int_is_populated_correctly()
        {
            Assert.That(_retrievedEntity.IntegerValue, Is.EqualTo(_originalEntity.IntegerValue));
        }

        [Test]
        public void Then_the_datetime_is_populated_correctly()
        {
            Assert.That(_retrievedEntity.DateTimeValue, Is.EqualTo(_originalEntity.DateTimeValue));
        }

        [Test]
        public void Then_the_nullable_datetime_is_populated_correctly()
        {
            Assert.That(_retrievedEntity.NullableDateTimeValue, Is.EqualTo(_originalEntity.NullableDateTimeValue));
        }

        [Test]
        public void Then_the_decimal_is_populated_correctly()
        {
            Assert.That(_retrievedEntity.NullableDecimal, Is.EqualTo(_originalEntity.NullableDecimalValue));
        }

        [Test]
        public void Then_the_nullable_int_is_populated_correctly()
        {
            Assert.That(_retrievedEntity.NullableInteger, Is.EqualTo(_originalEntity.NullableInteger));
        }

        [Test]
        public void Then_the_nullable_decimal_is_populated_correctly()
        {
            Assert.That(_retrievedEntity.NullableDateTimeValue, Is.EqualTo(_originalEntity.NullableDateTimeValue));
        }

        [Test]
        public void Then_the_very_long_string_is_populated_correctly()
        {
            Assert.That(_retrievedEntity.VerLongStringValue, Is.EqualTo(_originalEntity.VerLongStringValue));
        }
    }
}