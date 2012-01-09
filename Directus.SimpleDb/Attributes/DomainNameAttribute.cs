using System;

namespace Directus.SimpleDb.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DomainNameAttribute : Attribute
    {
        public DomainNameAttribute(string domainName)
        {
            DomainName = domainName;
        }

        public string DomainName { get; private set; }
    }
}