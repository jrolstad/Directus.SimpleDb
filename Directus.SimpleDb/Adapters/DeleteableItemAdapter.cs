using System.Collections.Generic;
using System.Linq;
using Amazon.SimpleDB.Model;

namespace Directus.SimpleDb.Adapters
{
    public class DeleteableItemAdapter
    {
        public IEnumerable<DeleteableItem> Convert<I>(IEnumerable<I> identifiers)
        {
            return identifiers
                .Select(i => new DeleteableItem().WithItemName(i.ToString()));
        }
    }
}