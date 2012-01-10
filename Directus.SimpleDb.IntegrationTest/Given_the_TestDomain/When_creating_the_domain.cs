using Amazon.SimpleDB.Model;
using Directus.SimpleDb.IntegrationTest.Entities;
using Directus.SimpleDb.IntegrationTest.Properties;
using Directus.SimpleDb.Providers;
using NUnit.Framework;

namespace Directus.SimpleDb.IntegrationTest.Given_the_TestDomain
{
    public class When_creating_the_domain
    {
        private SimpleDBProvider<TestPOCO, string> _provider;
        private DomainMetadataResponse _domainInfo;

        [TestFixtureSetUp]
        public void BeforeAll()
        {
            //Arrange
            _provider = new SimpleDBProvider<TestPOCO, string>(Settings.Default.AmazonAccessKey, Settings.Default.AmazonSecretKey);

            // Act
            _provider.CreateDomain();

            var simpleDB = new Amazon.SimpleDB.AmazonSimpleDBClient(Settings.Default.AmazonAccessKey, Settings.Default.AmazonSecretKey);

            _domainInfo = simpleDB.DomainMetadata(new DomainMetadataRequest().WithDomainName(this._provider.DomainName));
        }

        [Test]
        public void Then_the_domain_is_created()
        {
            // Assert
            Assert.That(_domainInfo, Is.Not.Null);
            Assert.That(_domainInfo.DomainMetadataResult, Is.Not.Null);
        }
    }
}