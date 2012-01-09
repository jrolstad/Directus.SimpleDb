using System;

namespace Directus.SimpleDb.Attributes
{
    /// <summary>
    /// The DoNotPersist attribute blocks blocks this property from being saved (persisted) to SimpleDB
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class DoNotPersistAttribute : Attribute
    {
    }
}