namespace AmitalCloud.Infrastructure.Domain.DataContracts
{
    public class FilterCriteria
    {
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
        public string FieldValue2 { get; set; }
        public string Operator { get; set; }
        public bool IsCustom { get; set; }
        public bool DisplayInList { get; set; }
        public bool IsCustomField { get; set; }
        public string FieldDataType { get; set; }
        public bool IgnoreFilter { get; set; }
        public bool IsCacheOnClient { get; set; }
        public bool IsLookUpfilter { get; set; }
        public object QueryFilterItems { get; set; }
        public string FilterType { get; set; }
    }
}
