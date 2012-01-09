using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;
using Directus.SimpleDb.Adapters;
using Directus.SimpleDb.Attributes;
using Directus.SimpleDb.Factories;
using Rolstad.Extensions;

namespace Directus.SimpleDb.Providers
{
    public class SimpleDBProvider<T,I>
    {
        private readonly BatchDeleteAttributeRequestFactory _deleteFactory;
        private readonly BatchPutAttributeRequestFactory<T> _putFactory;
        private readonly SelectRequestFactory _selectRequestFactory;
        private readonly ItemAdapter<T> _itemAdapter;

        private AmazonSimpleDB _simpleDB;
        private string _domainName;

        public SimpleDBProvider(BatchDeleteAttributeRequestFactory deleteFactory, BatchPutAttributeRequestFactory<T> putFactory, SelectRequestFactory selectRequestFactory, ItemAdapter<T> itemAdapter, AmazonSimpleDB simpleDb)
        {
            _deleteFactory = deleteFactory;
            _putFactory = putFactory;
            _selectRequestFactory = selectRequestFactory;
            _itemAdapter = itemAdapter;

            _domainName = GetDomainNameForType();
            _simpleDB = simpleDb;
        }

        public T Get(I identifier)
        {
            var itemsInDomain = QueryDomain(identifier.ToString());

            return itemsInDomain.Select(i => _itemAdapter.Convert(i)).FirstOrDefault();
        }

        public IEnumerable<T> Get()
        {
            var itemsInDomain = QueryDomain();

            return itemsInDomain.Select(i => _itemAdapter.Convert(i));
        }

        public void Save(IEnumerable<T> itemsToSave)
        {
            var request = _putFactory.CreateRequest(itemsToSave,_domainName);
            
            var response = _simpleDB.BatchPutAttributes(request);
        }

        public void Delete (IEnumerable<I> itemsToDelete)
        {
            var identifiers = itemsToDelete.Select(i => i.ToString());
            var request = _deleteFactory.CreateRequest(identifiers,_domainName);

            var response = _simpleDB.BatchDeleteAttributes(request);
        }

        internal string GetDomainNameForType()
        {
            var currentType = typeof (T);
            var domainName = currentType.Name;
            var domainAttribute =  currentType.GetCustomAttributes(typeof(DomainNameAttribute), true).FirstOrDefault() as DomainNameAttribute;

            return domainAttribute != null ? domainAttribute.DomainName : domainName;
        }

        internal ICollection<Item> QueryDomain(string identifier = null)
        {
            var isComplete = false;
            string nextToken = null;
            var receivedItems = new List<Item>();

            // Run the select many times until there are no more to get
            while (!isComplete)
            {
                // Setup the select to use the next token if defined
                var request = _selectRequestFactory.CreateRequest(_domainName, nextToken, identifier);

                // Run the select
                var response = _simpleDB.Select(request);

                // Determine if we need to keep on going
                isComplete = !response.SelectResult.IsSetNextToken();
                nextToken = response.SelectResult.NextToken;

                // Add the results
                receivedItems.AddRange(response.SelectResult.Item);
            }

            return receivedItems;
        }
    }
}