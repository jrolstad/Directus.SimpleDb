using System;
using System.Collections.Generic;
using Amazon.SimpleDB.Model;

namespace Directus.SimpleDb.Adapters
{
    public class ReplaceableItemAdapter<T>
    {
        public ReplaceableItem Convert(T item)
        {
            throw new NotImplementedException();
        }
    }
}