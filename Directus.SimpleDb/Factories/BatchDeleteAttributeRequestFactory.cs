using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.SimpleDB.Model;
using Directus.SimpleDb.Adapters;

namespace Directus.SimpleDb.Factories
{
    public class BatchDeleteAttributeRequestFactory
    {
        private readonly DeleteableItemAdapter _deleteableItemAdapter;

        public BatchDeleteAttributeRequestFactory(DeleteableItemAdapter deleteableItemAdapter)
        {
            _deleteableItemAdapter = deleteableItemAdapter;
        }

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