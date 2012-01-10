using System;
using System.Collections.Generic;
using System.Linq;
using Directus.SimpleDb.IntegrationTest.Entities;
using Directus.SimpleDb.IntegrationTest.Properties;
using Directus.SimpleDb.Providers;
using FizzWare.NBuilder;
using NUnit.Framework;

namespace Directus.SimpleDb.IntegrationTest.Given_the_TestDomain
{
    [TestFixture]
    public class When_getting_all_existing_POCOs
    {
        private TestPOCO _entityOne;
        private IEnumerable<TestPOCO> _allEntities;
        private TestPOCO _entityTwo;

        [TestFixtureSetUp]
        public void BeforeAll()
        {
            // Arrange
            var provider = new SimpleDBProvider<TestPOCO, string>(Settings.Default.AmazonAccessKey, Settings.Default.AmazonSecretKey);

            this._entityOne = Builder<TestPOCO>.CreateNew().Build();
            this._entityOne.Identifier = Guid.NewGuid().ToString();

            this._entityTwo = Builder<TestPOCO>.CreateNew().Build();
            this._entityTwo.Identifier = Guid.NewGuid().ToString();

            /* Save it the first time */
            provider.Save(new[] { this._entityOne, this._entityTwo });


            // Act
            this._allEntities = provider.Get();
        }

        [Test]
        public void Then_all_entities_are_found()
        {
            Assert.That(_allEntities.Count(),Is.GreaterThanOrEqualTo(2));
        }

        [Test]
        public void Then_entity_one_is_found()
        {
            Assert.That(_allEntities.FirstOrDefault(e=>e.Identifier == _entityOne.Identifier), Is.Not.Null);
        }

        [Test]
        public void Then_entity_two_is_found()
        {
            Assert.That(_allEntities.FirstOrDefault(e => e.Identifier == _entityTwo.Identifier), Is.Not.Null);
        }

       
    }
}