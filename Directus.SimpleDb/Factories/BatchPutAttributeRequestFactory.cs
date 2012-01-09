using System.Collections.Generic;
using System.Linq;
using Amazon.SimpleDB.Model;
using Directus.SimpleDb.Adapters;

namespace Directus.SimpleDb.Factories
{
    /// <summary>
    /// Factory for creating BatchPutAttributeRequest
    /// </summary>
    public class BatchPutAttributeRequestFactory 
    {
        private readonly ReplaceableItemAdapter _replaceableItemAdapter;

        /// <summary>
        /// Constructor with dependencies
        /// </summary>
        /// <param name="replaceableItemAdapter">Adapter for converting POCOs to ReplaceableItems</param>
        public BatchPutAttributeRequestFactory(ReplaceableItemAdapter replaceableItemAdapter  )
        {
            _replaceableItemAdapter = replaceableItemAdapter;
        }

        /// <summary>
        /// Given a set of POCOs, creates a batch request of ReplaceableItems
        /// </summary>
        /// <typeparam name="T">Type of the POCO</typeparam>
        /// <param name="itemsToPut">POCOs to place in the request</param>
        /// <param name="domainName">Domain the request is for</param>
        /// <returns></returns>
        public BatchPutAttributesRequest CreateRequest<T>(IEnumerable<T> itemsToPut, string domainName)
        {
            // Convert the items
            var items = itemsToPut
                .Select(i => _replaceableItemAdapter.Convert(i))
                .ToArray();

            // Create the request
            return new BatchPutAttributesRequest()
                .WithDomainName(domainName)
                .WithItem(items);

        } 
    }
}