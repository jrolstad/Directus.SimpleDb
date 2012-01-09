using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Directus.SimpleDb.Attributes;
using Rolstad.Extensions;

namespace Directus.SimpleDb.Mappers
{
    /// <summary>
    /// Creates a map of the given type
    /// </summary>
    public class EntityMapper
    {
        /// <summary>
        /// Given a type T, creates a map for it
        /// </summary>
        /// <typeparam name="T">Type to create the map for</typeparam>
        /// <returns></returns>
        public EntityMap CreateMap<T>()
        {
            // Get the properties
            var properties = typeof(T).GetProperties();

            // Get the maps
            var keyProperty = GetKeyProperty<T>(properties);
            var propertiesToFill = GetPersistableProperties<T>(properties);

            // Create the map
            return new EntityMap {KeyProperty = keyProperty, PersistableProperties = propertiesToFill};
        }

        /// <summary>
        /// Obtains an array of all persistable properties
        /// </summary>
        /// <typeparam name="T">Type to get properties for</typeparam>
        /// <param name="properties">Properties on the Type</param>
        /// <returns></returns>
        internal PropertyInfo[] GetPersistableProperties<T>( PropertyInfo[] properties )
        {
            var propertiesToFill = properties
                .Where(p => p.GetCustomAttributes(typeof (DoNotPersistAttribute), true).Length == 0)
                .Where(p => p.GetCustomAttributes(typeof (KeyAttribute), true).Length == 0)
                .Where(p => p.CanWrite)
                .ToArray();

            return propertiesToFill;
        }

        /// <summary>
        /// Obtains the Key property for the type
        /// </summary>
        /// <typeparam name="T">Type to get the key for</typeparam>
        /// <param name="properties">Properties on the type</param>
        /// <returns></returns>
        internal PropertyInfo GetKeyProperty<T>(PropertyInfo[] properties)
        {
            // Get all potential matches
            var keyProperties = properties
                .Where(p => p.GetCustomAttributes(typeof (KeyAttribute), true).Length > 0)
                .ToArray();

            // Ensure we have 1 and only 1 key property
            if (keyProperties.Length == 0)
                throw new ApplicationException("Key property not found for type {0}".StringFormat(typeof (T).FullName));

            if (keyProperties.Length > 1)
                throw new ApplicationException("More than 1 Key property found for type {0}".StringFormat(typeof (T).FullName));

            // Get the single property
            var keyProperty = keyProperties.Single();

            return keyProperty;
        }
    }
}