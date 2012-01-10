using System.Collections.Generic;
using System.Linq;
using Amazon.SimpleDB;
using Amazon.SimpleDB.Model;
using Directus.SimpleDb.Adapters;
using Directus.SimpleDb.Attributes;
using Directus.SimpleDb.Factories;
using Directus.SimpleDb.Mappers;
using Rolstad.Extensions;
using log4net;

namespace Directus.SimpleDb.Providers
{
    /// <summary>
    /// Amazon SimpleDB Persistence provider for POCOs 
    /// </summary>
    /// <typeparam name="T">Type to persist</typeparam>
    /// <typeparam name="I">Type of the key identifiers</typeparam>
    public class SimpleDBProvider<T,I> where T : new()
    {
        private readonly ILog Logger = LogManager.GetLogger(typeof(SimpleDBProvider<T, I>));

        private readonly BatchDeleteAttributeRequestFactory _deleteFactory;
        private readonly BatchPutAttributeRequestFactory _putFactory;
        private readonly SelectRequestFactory _selectRequestFactory;
        private readonly ItemAdapter _itemAdapter;

        private readonly AmazonSimpleDB _simpleDB;
        private readonly DomainRequestFactory _domainRequestFactory;
        private readonly string _domainName;

        /// <summary>
        /// Default constructor that determines the domain name
        /// </summary>
        internal SimpleDBProvider()
        {
            this._domainName = GetDomainNameForType();
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
            _domainRequestFactory = new DomainRequestFactory();
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
        /// <param name="domainRequestFactory">Factory for domain requests</param>
        public SimpleDBProvider(BatchDeleteAttributeRequestFactory deleteFactory, 
            BatchPutAttributeRequestFactory putFactory, 
            SelectRequestFactory selectRequestFactory, 
            ItemAdapter itemAdapter, 
            AmazonSimpleDB simpleDb,
            DomainRequestFactory domainRequestFactory):this()
        {
            _deleteFactory = deleteFactory;
            _putFactory = putFactory;
            _selectRequestFactory = selectRequestFactory;
            _itemAdapter = itemAdapter;
            _simpleDB = simpleDb;
            _domainRequestFactory = domainRequestFactory;
        }

        /// <summary>
        /// Name of the domain for this type
        /// </summary>
        public string DomainName
        {
            get { return this._domainName; }
        }

        /// <summary>
        /// Obtains a specific instance of the type
        /// </summary>
        /// <param name="identifier">Unique identifier to search for</param>
        /// <returns></returns>
        public T Get(I identifier)
        {
            if(Logger.IsInfoEnabled) Logger.Info("Querying domain '{0}' for item with name '{1}'".StringFormat(_domainName,identifier));

            // Get the data
            var itemsInDomain = QueryDomain(identifier.ToString());

            // Convert it
            return itemsInDomain
                .AsParallel()
                .Select(i => _itemAdapter.Convert<T>(i)).FirstOrDefault();
        }

        /// <summary>
        /// Obtains all instances of the given type
        /// </summary>
        /// <returns></returns>
        public ICollection<T> Get()
        {
            if (Logger.IsInfoEnabled) Logger.Info("Querying domain '{0}' for all items".StringFormat(_domainName));

            // Get the data
            var itemsInDomain = QueryDomain();

            // Convert it
            return itemsInDomain
                .AsParallel()
                .Select(i => _itemAdapter.Convert<T>(i)).ToArray();
        }

        /// <summary>
        /// Saves all instances in the set
        /// </summary>
        /// <param name="itemsToSave">Instances of the given type to persist</param>
        public void Save(ICollection<T> itemsToSave)
        {
            if (Logger.IsInfoEnabled) Logger.Info("Saving {0} items to domain '{1}'".StringFormat(itemsToSave.Count(), _domainName));

            // Create the put request
            var request = _putFactory.CreateRequest(itemsToSave,this.DomainName);
            
            // Create the domain if it doesn't exist yet
            CreateDomain();

            // Send off to AWS
            _simpleDB.BatchPutAttributes(request);
        }

        /// <summary>
        /// Deletes all instances for the given type
        /// </summary>
        /// <param name="itemsToDelete"></param>
        public void Delete(ICollection<I> itemsToDelete)
        {
            // Create the put request
            var identifiers = itemsToDelete.Select(i => i.ToString()).ToArray();

            if (Logger.IsInfoEnabled) Logger.Info("Deleting items '{0}' from domain '{1}'".StringFormat(string.Join(",", identifiers), _domainName));

            var request = _deleteFactory.CreateRequest(identifiers,this.DomainName);

            // Send off to Amazon
            _simpleDB.BatchDeleteAttributes(request);
        }

        /// <summary>
        /// Creates the domain for the given type
        /// </summary>
        public void CreateDomain()
        {
            if (Logger.IsInfoEnabled) Logger.Info("Sending request to create domain '{0}' to ensure it exists".StringFormat(_domainName));

            var request = _domainRequestFactory.Create(this.DomainName);

            _simpleDB.CreateDomain(request);
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
            var domainNameForType = domainAttribute != null ? domainAttribute.DomainName : domainName;

            if (Logger.IsInfoEnabled) Logger.Info("Domain name for type resolved as '{0}'".StringFormat(domainNameForType));
            return domainNameForType;
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
                var request = _selectRequestFactory.CreateRequest(this.DomainName, nextToken, identifier);

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