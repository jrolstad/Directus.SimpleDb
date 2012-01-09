using System.Collections.Generic;
using System.Linq;
using Amazon.SimpleDB.Model;

namespace Directus.SimpleDb.Adapters
{
    /// <summary>
    /// Converts a set of identifiers to DeleteableItems
    /// </summary>
    public class DeleteableItemAdapter
    {
        /// <summary>
        /// Converts the set of identifiers to a set of deleteable items
        /// </summary>
        /// <typeparam name="I">Identifier type</typeparam>
        /// <param name="identifiers">Identifiers to convert</param>
        /// <returns></returns>
        public virtual IEnumerable<DeleteableItem> Convert<I>(IEnumerable<I> identifiers)
        {
            return identifiers
                .Select(i => new DeleteableItem().WithItemName(i.ToString()));
        }
    }
}