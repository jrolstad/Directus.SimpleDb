using Amazon.SimpleDB.Model;

namespace Directus.SimpleDb.Factories
{
    /// <summary>
    /// Factory for creating a domain request
    /// </summary>
    public class DomainRequestFactory
    {
        /// <summary>
        /// Creates a request to create a domain
        /// </summary>
        /// <param name="domainName"></param>
        /// <returns></returns>
        public CreateDomainRequest Create(string domainName)
        {
            return new CreateDomainRequest().WithDomainName(domainName);
        }
    }
}