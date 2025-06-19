using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Infrastructure.Web.DataContracts
{
    public class FieldsTranslations
    {
        [Key]
        public string TextCodeId { get; set; }
        public string Code { get; set; }
        public string TranslatedText { get; set; }
        public string TranslatedTextPlural { get; set; }
        public string DefaultText { get; set; }
        public string DefaultTextPlural { get; set; }
        public int Tenant { get; set; }
        public string TypeCode { get; set; }
        public string ObjectTableID { get; set; }
        public int TranslationTenent { get; set; }
        public string ObjectTableName { get; set; }
        public string TranslationLanguageCode { get; set; }
        public DateTime? TranslateDate { get; set; }
        public string TranslatedByUserId { get; set; }
        public bool IsTranslated { get; set; }
        public string TextCodeCode { get; set; }
        public string ObjectTableTypeCode { get; set; }
        public List<FieldsTranslations> DirtyFields { get; set; }
    }
}