using Amazon.SimpleDB.Model;
using Rolstad.Extensions;

namespace Directus.SimpleDb.Factories
{
    /// <summary>
    /// Factory for creating select requests
    /// </summary>
    public class SelectRequestFactory
    {
        /// <summary>
        /// Creates a Select Request for the given domain
        /// </summary>
        /// <param name="domainName">Domain to query</param>
        /// <param name="nextToken">Next Token to use when querying over large sets</param>
        /// <param name="identifier">Identifier to search for</param>
        /// <param name="consistentRead">If reads should be consistent</param>
        /// <returns></returns>
        public SelectRequest CreateRequest(string domainName, string nextToken, string identifier=null,bool consistentRead=true)
        {
            return new SelectRequest()
                .WithSelectExpression(this.CreateSelectExpression(domainName,identifier))
                .WithNextToken(nextToken)
                .WithConsistentRead(consistentRead);
        }

        /// <summary>
        /// Given a domain name and identifier, creates a select expression
        /// </summary>
        /// <param name="domainName">Domain to create the expression for</param>
        /// <param name="identifier">Identifier to create the expression for</param>
        /// <returns></returns>
        internal string CreateSelectExpression( string domainName, string identifier = null )
        {
            return identifier == null ? 
                "Select * from {0}".StringFormat(domainName) :
                 "Select * from {0} where itemName() = '{1}'".StringFormat(domainName,identifier);
        }
    }
}