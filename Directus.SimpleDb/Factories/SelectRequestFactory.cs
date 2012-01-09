using System;
using Amazon.SimpleDB.Model;
using Rolstad.Extensions;

namespace Directus.SimpleDb.Factories
{
    public class SelectRequestFactory
    {
        public SelectRequest CreateRequest(string domainName, string nextToken, string identifier=null,bool consistentRead=true)
        {
            return new SelectRequest()
                .WithSelectExpression("Select * from {0}".StringFormat(domainName))
                .WithNextToken(nextToken)
                .WithConsistentRead(true);
        }

    }
}