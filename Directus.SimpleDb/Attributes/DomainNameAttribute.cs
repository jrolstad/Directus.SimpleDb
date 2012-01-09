using System;

namespace Directus.SimpleDb.Attributes
{
    /// <summary>
    /// Specifies the name of the underlying domain for a given POCO
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DomainNameAttribute : Attribute
    {
        /// <summary>
        /// Constructor with domain name
        /// </summary>
        /// <param name="domainName">Name of the domain</param>
        public DomainNameAttribute(string domainName)
        {
            DomainName = domainName;
        }

        /// <summary>
        /// Name of the specified domain
        /// </summary>
        public string DomainName { get; private set; }
    }
}