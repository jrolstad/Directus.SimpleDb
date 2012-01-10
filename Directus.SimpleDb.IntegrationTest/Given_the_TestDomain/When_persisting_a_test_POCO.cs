using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Directus.SimpleDb.IntegrationTest.Entities;
using Directus.SimpleDb.Providers;
using FizzWare.NBuilder;
using NUnit.Framework;

namespace Directus.SimpleDb.IntegrationTest.Given_the_TestDomain
{
    [TestFixture]
    public class When_persisting_a_test_POCO
    {
        private TestPOCO _originalEntity;
        private TestPOCO _retrievedEntity;

        [TestFixtureSetUp]
        public void BeforeAll()
        {
            // Arrange
            var provider = new SimpleDBProvider<TestPOCO, string>("a",
                                                                  "b");

            _originalEntity = Builder<TestPOCO>.CreateNew().Build();
            _originalEntity.Identifier = Guid.NewGuid().ToString();

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
    }
}
