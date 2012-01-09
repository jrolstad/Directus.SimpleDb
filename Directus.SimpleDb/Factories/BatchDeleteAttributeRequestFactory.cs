using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.SimpleDB.Model;
using Directus.SimpleDb.Adapters;

namespace Directus.SimpleDb.Factories
{
    /// <summary>
    /// Factory for creating BatchDeleteAttributeRequest
    /// </summary>
    public class BatchDeleteAttributeRequestFactory
    {
        private readonly DeleteableItemAdapter _deleteableItemAdapter;

        /// <summary>
        /// Constructor with dependencies
        /// </summary>
        /// <param name="deleteableItemAdapter">dapter for converting identifiers to DeletableItems</param>
        public BatchDeleteAttributeRequestFactory(DeleteableItemAdapter deleteableItemAdapter)
        {
            _deleteableItemAdapter = deleteableItemAdapter;
        }

        /// <summary>
        /// Given a set of identifiers and a domain name, creates a delete request
        /// </summary>
        /// <param name="identifiers">Identifiers to delete</param>
        /// <param name="domainName">Domain the request is for</param>
        /// <returns></returns>
        public BatchDeleteAttributesRequest CreateRequest (IEnumerable<string> identifiers, string domainName)
        {
            // Convert identifiers to deleable items
            var items = _deleteableItemAdapter
                .Convert(identifiers)
                .ToArray();

            // Create the request
            return new BatchDeleteAttributesRequest()
                .WithDomainName(domainName)
                .WithItem(items);


        }
    }
}