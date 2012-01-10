using System.Collections.Generic;
using System.Linq;
using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;
using Directus.SimpleDb.Adapters;
using Directus.SimpleDb.Attributes;
using Directus.SimpleDb.Factories;
using Directus.SimpleDb.Mappers;

namespace Directus.SimpleDb.Providers
{
    /// <summary>
    /// Amazon SimpleDB Persistence provider for POCOs 
    /// </summary>
    /// <typeparam name="T">Type to persist</typeparam>
    /// <typeparam name="I">Type of the key identifiers</typeparam>
    public class SimpleDBProvider<T,I> where T : new()
    {
        private readonly BatchDeleteAttributeRequestFactory _deleteFactory;
        private readonly BatchPutAttributeRequestFactory _putFactory;
        private readonly SelectRequestFactory _selectRequestFactory;
        private readonly ItemAdapter _itemAdapter;

        private readonly AmazonSimpleDB _simpleDB;
        private readonly string _domainName;

        /// <summary>
        /// Default constructor that determines the domain name
        /// </summary>
        internal SimpleDBProvider()
        {
            _domainName = GetDomainNameForType();
        }

        /// <summary>
        /// Constructor with only the Amazon credentials
        /// </summary>
        /// <param name="amazonAccessKey"></param>
        /// <param name="amazonSecretKey"></param>
        public SimpleDBProvider(string amazonAccessKey, string amazonSecretKey):this()
        {
            var entityMapper = new EntityMapper();
            _deleteFactory = new BatchDeleteAttributeRequestFactory(new DeleteableItemAdapter());
            _putFactory = new BatchPutAttributeRequestFactory(new ReplaceableItemAdapter(entityMapper));
            _selectRequestFactory = new SelectRequestFactory();
            _itemAdapter = new ItemAdapter(entityMapper);
            _simpleDB = new AmazonSimpleDBClient(amazonAccessKey,amazonSecretKey);
        }

        /// <summary>
        /// Constructor with all dependencies injected
        /// </summary>
        /// <param name="deleteFactory">Factory for creating delete requests</param>
        /// <param name="putFactory">Factory for creating put requests</param>
        /// <param name="selectRequestFactory">Factory for creating select requests</param>
        /// <param name="itemAdapter">Factory for converting select response items to the given POCO of type T</param>
        /// <param name="simpleDb">Amazon SimpleDB instance</param>
        public SimpleDBProvider(BatchDeleteAttributeRequestFactory deleteFactory, 
            BatchPutAttributeRequestFactory putFactory, 
            SelectRequestFactory selectRequestFactory, 
            ItemAdapter itemAdapter, 
            AmazonSimpleDB simpleDb):this()
        {
            _deleteFactory = deleteFactory;
            _putFactory = putFactory;
            _selectRequestFactory = selectRequestFactory;
            _itemAdapter = itemAdapter;
            _simpleDB = simpleDb;
        }

        /// <summary>
        /// Obtains a specific instance of the type
        /// </summary>
        /// <param name="identifier">Unique identifier to search for</param>
        /// <returns></returns>
        public T Get(I identifier)
        {
            // Get the data
            var itemsInDomain = QueryDomain(identifier.ToString());

            // Convert it
            return itemsInDomain.Select(i => _itemAdapter.Convert<T>(i)).FirstOrDefault();
        }

        /// <summary>
        /// Obtains all instances of the given type
        /// </summary>
        /// <returns></returns>
        public IEnumerable<T> Get()
        {
            // Get the data
            var itemsInDomain = QueryDomain();

            // Convert it
            return itemsInDomain.Select(i => _itemAdapter.Convert<T>(i));
        }

        /// <summary>
        /// Saves all instances in the set
        /// </summary>
        /// <param name="itemsToSave">Instances of the given type to persist</param>
        public void Save(IEnumerable<T> itemsToSave)
        {
            // Create the put request
            var request = _putFactory.CreateRequest(itemsToSave,_domainName);
            
            // Send off to AWS
            _simpleDB.BatchPutAttributes(request);
        }

        /// <summary>
        /// Deletes all instances for the given type
        /// </summary>
        /// <param name="itemsToDelete"></param>
        public void Delete (IEnumerable<I> itemsToDelete)
        {
            // Create the put request
            var identifiers = itemsToDelete.Select(i => i.ToString());
            var request = _deleteFactory.CreateRequest(identifiers,_domainName);

            // Send off to Amazon
            _simpleDB.BatchDeleteAttributes(request);
        }

        /// <summary>
        /// Obtains the Domain name for the given type
        /// </summary>
        /// <returns></returns>
        internal string GetDomainNameForType()
        {
            // See if there is a domain name specified
            var currentType = typeof (T);
            var domainName = currentType.Name;
            var domainAttribute =  currentType.GetCustomAttributes(typeof(DomainNameAttribute), true).FirstOrDefault() as DomainNameAttribute;

            // Return either the defined domain name or the given type name
            return domainAttribute != null ? domainAttribute.DomainName : domainName;
        }

        /// <summary>
        /// Queries the given domain
        /// </summary>
        /// <param name="identifier">Identifier to search for</param>
        /// <returns></returns>
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