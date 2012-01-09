using System;
using System.Linq;
using System.Reflection;
using Amazon.SimpleDB.Model;
using Directus.SimpleDb.Mappers;
using Rolstad.Extensions;

namespace Directus.SimpleDb.Adapters
{
    /// <summary>
    /// Converts between POCOs and Amazon SimpleDB Item instances
    /// </summary>
    public class ItemAdapter
    {
        private readonly EntityMapper _entityMapper;

        /// <summary>
        /// Constructor with dependencies
        /// </summary>
        /// <param name="entityMapper">Mapper for creating the AWS to POCO map</param>
        public ItemAdapter(EntityMapper entityMapper)
        {
            _entityMapper = entityMapper;
        }

        /// <summary>
        /// Converts an an Amazon SimpleDB Item into an instance to type T
        /// </summary>
        /// <typeparam name="T">Type to convert to</typeparam>
        /// <param name="item">SimpleDB Item to read from</param>
        /// <returns></returns>
        public virtual T Convert<T>(Item item ) where T : new()
        {
            var instance = new T();

            // Obtain the entity map to work with
            var map = _entityMapper.CreateMap<T>();

            // Set the properties
            SetProperty(item.Name,instance,map.KeyProperty);
            map.PersistableProperties.Each(p => item.Attribute
                                            .Where(a => a.Name == p.Name)
                                            .Each(a => SetProperty(a.Value, instance, p))
                                            );


            return instance;
        }

        /// <summary>
        /// Given a property and instance, sets the value on it
        /// </summary>
        /// <typeparam name="T">Type of the instance where the property resides</typeparam>
        /// <param name="value">Value to set on the property</param>
        /// <param name="instance">Instance to set the value on</param>
        /// <param name="property">Property to set</param>
        internal void SetProperty<T>(string value, T instance, PropertyInfo property )
        {
            try
            {
                property.SetValue(instance, System.Convert.ChangeType(value, property.PropertyType), null);
            }
            catch (Exception exception)
            {
                var message = "Unable to set property '{0}' to '{1}'".StringFormat(property.Name, value);
                throw new ApplicationException(message,exception);
            }
           
        }
    }
}