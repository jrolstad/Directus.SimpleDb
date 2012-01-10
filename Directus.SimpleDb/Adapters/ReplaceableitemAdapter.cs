using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Amazon.SimpleDB.Model;
using Directus.SimpleDb.Mappers;
using Rolstad.Extensions;

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
                .AsParallel()
                .SelectMany(p=>Convert(GetPropertyValue(item,p),p.Name))
                .ToArray();
        }

        /// <summary>
        /// Converts a string into an enumeration of replaceable attributes
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="attributeName">name of the attribute</param>
        /// <returns></returns>
        private IEnumerable<ReplaceableAttribute> Convert(string value, string attributeName)
        {
            // Ensure nulls are converted to empty
            var nullSafeValue = value ?? string.Empty;

            // Split up the string and convert into attributse
            var chunkedValues = Chunk(nullSafeValue,500).ToArray();

            // If everything falls into 1 attribute we're good; lets get out of here
            if(chunkedValues.Length <= 1)
            {
                return new []
                           {
                               new ReplaceableAttribute()
                                   .WithName(attributeName)
                                   .WithValue(nullSafeValue)
                                   .WithReplace(true)
                           };
            }

            var attributes = new List<ReplaceableAttribute>();
            var attributeCount = 0;
            foreach (var item in chunkedValues)
            {
                var attributeValue = "[Sort{0}]{1}".StringFormat(attributeCount,item);

                attributes.Add(new ReplaceableAttribute
                                   {
                                       Name = attributeName, 
                                       Replace = false, 
                                       Value = attributeValue
                                   });
                attributeCount++;
            }

            attributes.First().WithReplace(true);

            return attributes;
        }

        /// <summary>
        /// Chunks a string up into an enumeration of strings
        /// </summary>
        /// <param name="stringToChunk">String being chunked</param>
        /// <param name="chunkSize">Size of the chunks</param>
        /// <returns></returns>
        private IEnumerable<string> Chunk (string stringToChunk, int chunkSize)
        {
            if (stringToChunk.IsEmpty())
            {
                yield return string.Empty;
            }
            else
            {
                for (int offset = 0; offset < stringToChunk.Length; offset += chunkSize)
                {
                    int size = Math.Min(chunkSize, stringToChunk.Length - offset);
                    yield return stringToChunk.Substring(offset, size);
                }
            }
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