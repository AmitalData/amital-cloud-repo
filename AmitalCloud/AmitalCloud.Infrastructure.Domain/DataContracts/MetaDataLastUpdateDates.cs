using System;
using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Infrastructure.Domain.DataContracts
{
    public class MetaDataLastUpdateDates
    {
        [Key]
        public int Id { get; set; }
        public DateTime TranslationsSystemUpdateDateGMT { get; set; }
        public DateTime ObjectFieldsSystemUpdateDateGMT { get; set; }
        public DateTime TranslationsTenantUpdateDateGMT { get; set; }
        public DateTime ObjectFieldsTenantUpdateDateGMT { get; set; }
    }
}