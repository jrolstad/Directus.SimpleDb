using System.Linq;
using System.Reflection;
using Amazon.SimpleDB.Model;
using Directus.SimpleDb.Mappers;

namespace Directus.SimpleDb.Adapters
{
    /// <summary>
    /// Converts between POCOs and Amazon SimpleDB ReplaceableItem instances
    /// </summary>
    public class ReplaceableItemAdapter
    {
        private readonly EntityMapper _entityMapper;

        /// <summary>
        /// Constructor with dependencies
        /// </summary>
        /// <param name="entityMapper">Mapper for creating the AWS to POCO map</param>
        public ReplaceableItemAdapter(EntityMapper entityMapper)
        {
            _entityMapper = entityMapper;
        }

        /// <summary>
        /// Given a POCO, converts it to an Amazon ReplaceableItem
        /// </summary>
        /// <typeparam name="T">Type converting from</typeparam>
        /// <param name="item">Item to convert</param>
        /// <returns></returns>
        public virtual ReplaceableItem Convert<T>(T item)
        {
            // Obtain the entity map to work with
            var map = _entityMapper.CreateMap<T>();

            // Convert the properties
            var itemName = GetPropertyValue(item, map.KeyProperty);
            var attributes = GetReplaceableAttributes(item, map);

            // Create the item
            return new ReplaceableItem()
                .WithItemName(itemName)
                .WithAttribute(attributes);
        }

        /// <summary>
        /// Converts persistable properties on the instance to a set of Replaceable items
        /// </summary>
        /// <typeparam name="T">Type to read from</typeparam>
        /// <param name="item">Instance to convert</param>
        /// <param name="map">Entity map</param>
        /// <returns></returns>
        internal ReplaceableAttribute[] GetReplaceableAttributes<T>(T item, EntityMap map)
        {
            return map.PersistableProperties
                .Select(p=>new ReplaceableAttribute()
                    .WithName(p.Name)
                    .WithValue(GetPropertyValue(item, p))
                    .WithReplace(true))
                .ToArray();
        }

        /// <summary>
        /// Gets a string representation of the value for the given property
        /// </summary>
        /// <typeparam name="T">Type to get the value from</typeparam>
        /// <param name="item">Item to get the value from</param>
        /// <param name="property">Property to get the value from</param>
        /// <returns></returns>
        internal string GetPropertyValue<T>(T item, PropertyInfo property)
        {
            var value = property.GetValue(item,null);

            return value == null ? null : value.ToString();
        }
 
    }
}