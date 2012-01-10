using System;
using System.ComponentModel.DataAnnotations;
using Directus.SimpleDb.Attributes;

namespace Directus.SimpleDb.IntegrationTest.Entities
{
    /// <summary>
    /// Test item for persistence
    /// </summary>
    [DomainName("TestDomain")]
    public class TestPOCO
    {
        [Key]
        public string Identifier { get; set; }

        public decimal NullableDecimalValue { get; set; }

        public string StringValue { get; set; }

        public string VerLongStringValue { get; set; }

        public bool BoolValue { get; set; }

        public int IntegerValue { get; set; }

        public DateTime DateTimeValue { get; set; }

        public DateTime? NullableDateTimeValue { get; set; }

        public int? NullableInteger { get; set; }

        public decimal? NullableDecimal { get; set; }
    }
}