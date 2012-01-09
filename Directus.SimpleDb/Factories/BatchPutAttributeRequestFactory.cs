using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.SimpleDB.Model;
using Directus.SimpleDb.Adapters;

namespace Directus.SimpleDb.Factories
{
    public class BatchPutAttributeRequestFactory<T>
    {
        private readonly ReplaceableItemAdapter<T> _replaceableItemAdapter;

        public BatchPutAttributeRequestFactory(ReplaceableItemAdapter<T> replaceableItemAdapter  )
        {
            _replaceableItemAdapter = replaceableItemAdapter;
        }

        public BatchPutAttributesRequest CreateRequest(IEnumerable<T> itemsToPut, string domainName)
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