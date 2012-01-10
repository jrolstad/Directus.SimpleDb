using System;
using System.Collections.Generic;
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
    public class When_deleting_an_existing_POCO
    {
        private TestPOCO _originalEntity;
        private IEnumerable<TestPOCO> _allEntitiesAfterDelete;

        [TestFixtureSetUp]
        public void BeforeAll()
        {
            // Arrange
            var provider = new SimpleDBProvider<TestPOCO, string>(Settings.Default.AmazonAccessKey, Settings.Default.AmazonSecretKey);

            this._originalEntity = Builder<TestPOCO>.CreateNew().Build();
            this._originalEntity.Identifier = Guid.NewGuid().ToString();

            var stringBuilder = new StringBuilder();
            Enumerable.Range(0, 100).Each(i=>stringBuilder.Append(Guid.NewGuid().ToString()));
            this._originalEntity.VerLongStringValue = stringBuilder.ToString();

            /* Save it the first time */
            provider.Save(new[] { this._originalEntity });


            // Act
            provider.Delete(new[] { _originalEntity.Identifier});

            _allEntitiesAfterDelete = provider.Get();
        }

        [Test]
        public void Then_the_entity_is_removed()
        {
            var deletedItem = this._allEntitiesAfterDelete.Where(e => e.Identifier == _originalEntity.Identifier);
            Assert.That(deletedItem, Is.Not.Null);
        }

       
    }
}