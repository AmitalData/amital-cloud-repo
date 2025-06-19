namespace AmitalCloud.Infrastructure.Domain.DataContracts
{
    public class ApiQueryFilters
    {

        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public string SortBy { get; set; }
        public string SortDirection { get; set; }
        public bool GetCount { get; set; }
        public int? Tenant { get; set; }

        public string Filter1Name { get; set; }
        public string Filter1Value { get; set; }
        public string Filter1Operator { get; set; }

        public string Filter2Name { get; set; }
        public string Filter2Value { get; set; }
        public string Filter2Operator { get; set; }

        public string Filter3Name { get; set; }
        public string Filter3Value { get; set; }
        public string Filter3Operator { get; set; }

        public string Filter4Name { get; set; }
        public string Filter4Value { get; set; }
        public string Filter4Operator { get; set; }

        public string Filter5Name { get; set; }
        public string Filter5Value { get; set; }
        public string Filter5Operator { get; set; }

        public string Filter6Name { get; set; }
        public string Filter6Value { get; set; }
        public string Filter6Operator { get; set; }

        public string Filter7Name { get; set; }
        public string Filter7Value { get; set; }
        public string Filter7Operator { get; set; }

        public string Filter8Name { get; set; }
        public string Filter8Value { get; set; }
        public string Filter8Operator { get; set; }

        public string Filter9Name { get; set; }
        public string Filter9Value { get; set; }
        public string Filter9Operator { get; set; }

        public string Filter10Name { get; set; }
        public string Filter10Value { get; set; }
        public string Filter10Operator { get; set; }

        public string AdditionalFilters { get; set; }

        public string TreeFilters { get; set; }

        public string ParentEntityId { get; set; }

        public string ParentObjectTableName { get; set; }

        public bool DontApplyVirtualization { get; set; }

        public bool GetAll { get; set; }
        public string ParentEntity { get; set; }

        public string ProfileCode { get; set; }
        public string ObjectTableId { get; set; }
    }
}
