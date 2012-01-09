using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Amazon.SimpleDB.Model;
using Directus.SimpleDb.Attributes;
using Rolstad.Extensions;

namespace Directus.SimpleDb.Adapters
{
   
    public class ItemAdapter<T> where T : new()
    {
        private PropertyInfo[] _propertiesToFill;
        private PropertyInfo _keyProperty;

        public ItemAdapter()
        {
            DeterminePropertiesToFill();
        }

        private void DeterminePropertiesToFill()
        {
            var properties = typeof (T).GetProperties();

            _propertiesToFill = properties
                .Where(p => p.GetCustomAttributes(typeof (DoNotPersistAttribute), true).Length == 0)
                .Where(p=>p.CanWrite)
                .ToArray();

            var keyProperties = properties
                .Where(p => p.GetCustomAttributes(typeof (KeyAttribute), true).Length > 0)
                .ToArray();

            if (keyProperties.Length == 0)
                throw new ApplicationException("Key property not found for type {0}".StringFormat(typeof (T).FullName));
            if (keyProperties.Length > 1)
                throw new ApplicationException("More than 1 Key property found for type {0}".StringFormat(typeof (T).FullName));

            _keyProperty = keyProperties.Single();
        }

        public T Convert(Item item )
        {
            var instance = new T();

            _keyProperty.SetValue(instance, System.Convert.ChangeType(item.Name,_keyProperty.PropertyType), null);

            _propertiesToFill.Each(p => item.Attribute
                                            .Where(a => a.Name == p.Name)
                                            .Each(a => p.SetValue(instance, System.Convert.ChangeType(a.Value, p.PropertyType),null))
                                            );


            return instance;
        }
    }
}