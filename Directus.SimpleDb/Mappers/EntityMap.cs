using System;
using System.Collections.Generic;
using System.Reflection;

namespace Directus.SimpleDb.Mappers
{
    /// <summary>
    /// Map of a given entity
    /// </summary>
    public class EntityMap
    {
        /// <summary>
        /// Type that is mapped
        /// </summary>
        public Type MappedType { get; set; }

        /// <summary>
        /// Property that is the unique identifier
        /// </summary>
        public PropertyInfo KeyProperty { get; set; }

        /// <summary>
        /// Properties that are meant to be persisted
        /// </summary>
        public ICollection<PropertyInfo> PersistableProperties { get; set; } 
    }
}