using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Amazon.SimpleDB.Model;
using Directus.SimpleDb.Mappers;
using Rolstad.Extensions;
using Attribute = Amazon.SimpleDB.Model.Attribute;

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

            var attributeDictionary = item.Attribute.ToLookup(a=>a.Name);

            // Set the properties
            SetProperty(item.Name,instance,map.KeyProperty);
            map.PersistableProperties.Each(p => attributeDictionary
                                            .Where(a => a.Key == p.Name)
                                            .Each(a => SetProperty(GetFullValue(a), instance, p))
                                            );


            return instance;
        }

        /// <summary>
        /// For a given attribute name, gets the full value
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public string GetFullValue( IEnumerable<Attribute> attributes)
        {
            var builder = new StringBuilder();

            var regex = new Regex(@"\[Sort\d\]");
            attributes.OrderBy(a => a.Value).Each(a => builder.Append(regex.Replace(a.Value,string.Empty,1)));

            return builder.ToString();
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
                property.SetValue(instance, To(value, property.PropertyType), null);
            }
            catch (Exception exception)
            {
                var message = "Unable to set property '{0}' to '{1}'".StringFormat(property.Name, value);
                throw new ApplicationException(message,exception);
            }
           
        }

        /// <summary>
        /// Converts a string type to the defined type.  If <see langword="null"/> or empty, returns that type's default
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <param name="type">Type to convert the value to</param>
        /// <returns></returns>
        public static object To(string value, Type type)
        {
            var result = new object();

            if (!value.IsEmpty())
            {
                // Get the underlying type for Nullable types
                // Since when trying to run Convert.ChangeType(value,Nullable<>) that will blow...
                // so instead, we get the underlying type
                if (type.IsGenericType
                    && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    var t = type.GetGenericArguments()[0];
                    result = System.Convert.ChangeType(value, t);
                }
                // Not a Nullable, get the type
                else
                {
                    result = System.Convert.ChangeType(value, type);
                }
            }

            return result;
        }
    }
}