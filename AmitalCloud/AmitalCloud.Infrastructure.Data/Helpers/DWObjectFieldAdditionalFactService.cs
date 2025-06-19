using AmitalCloud.Infrastructure.Data.Queries;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using System.Collections.Generic;
using System.Linq;

namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public class DWObjectFieldAdditionalFactService
    {
        public List<DWObjectFieldPM> DWObjectFieldPMs { get; set; } = new List<DWObjectFieldPM>();
        private bool IsHaveAddAdditionalFactFields = false;
        public DWObjectTablePM DwObjectTable = null;
        private readonly DWObjectFieldQuery dWObjectFieldQuery = null;
        private readonly string factTableCode = string.Empty;
        private readonly int tenant;
        private readonly bool groupedByCategory = false;

        public DWObjectFieldAdditionalFactService(DWObjectFieldAdditionalFactArgs dWObjectFieldAdditionalFactArgs)
        {

            this.factTableCode = dWObjectFieldAdditionalFactArgs.FactTableCode;
            this.tenant = dWObjectFieldAdditionalFactArgs.Tenant;
            this.groupedByCategory = dWObjectFieldAdditionalFactArgs.GroupedByCategory;
            dWObjectFieldQuery = new DWObjectFieldQuery(0);
            IsHaveAddAdditionalFactFields = DwObjectTable != null && !string.IsNullOrEmpty(DwObjectTable.AdditionalFactCode);
            if (!dWObjectFieldAdditionalFactArgs.DontLoadDwObjectField) LoadDWObjectFieldsWithAdditionalFactFields();
        }

        public void LoadDWObjectFieldsWithAdditionalFactFields()
        {
            string factCode = !string.IsNullOrEmpty(DwObjectTable.ParentFactCode) ? DwObjectTable.ParentFactCode : factTableCode;

            var factDWObjectFieldPMs = groupedByCategory ? dWObjectFieldQuery.GetDWObjectFieldPMsByDWObjectTabelAndTenantGroupedByCategory(factCode, DwObjectTable.RecordType) : dWObjectFieldQuery.GetDWObjectFieldByDWObjectTableCode(factTableCode).Where(d => d.DisplayInQueryBuilder == true && (string.IsNullOrEmpty(d.RecordType) || (!string.IsNullOrEmpty(d.RecordType) && d.RecordType.IndexOf(DwObjectTable.RecordType) > -1))).ToList();
            DWObjectFieldPMs = new List<DWObjectFieldPM>();
            if (IsHaveAddAdditionalFactFields)
            {
                var additionalFactDWObjectFieldPMs = groupedByCategory ? dWObjectFieldQuery.GetDWObjectFieldPMsByDWObjectTabelAndTenantGroupedByCategory(DwObjectTable.AdditionalFactCode, DwObjectTable.RecordType) : dWObjectFieldQuery.GetDWObjectFieldByDWObjectTableCode(DwObjectTable.AdditionalFactCode).Where(d => string.IsNullOrEmpty(d.RecordType) || (!string.IsNullOrEmpty(d.RecordType) && d.RecordType.IndexOf(DwObjectTable.RecordType) > -1)).ToList();
                foreach (DWObjectFieldPM additionalFactField in additionalFactDWObjectFieldPMs.Where(d => (d.IsMeasurement == false || IsShipmentProfitField(d)) && (d.DisplayInQueryBuilder || (d.IsCustom && !DwObjectTable.HasCustomFields))))
                {
                    if (IsShipmentProfitField(additionalFactField))
                        additionalFactField.AggregationTypeCode = "MAX";

                    var dwObjectField = !string.IsNullOrEmpty(additionalFactField.OriginalObjectFieldCode) ? factDWObjectFieldPMs.Where(d => d.OriginalObjectFieldCode == additionalFactField.OriginalObjectFieldCode && d.DisplayInQueryBuilder).FirstOrDefault() : null;
                    if (dwObjectField == null)
                    {
                        dwObjectField = !string.IsNullOrEmpty(additionalFactField.Code) ? factDWObjectFieldPMs.Where(d => d.Code == additionalFactField.Code && d.DisplayInQueryBuilder).FirstOrDefault() : null;
                        if (dwObjectField == null && ContainTableRecordType(additionalFactField))
                        {
                            DWObjectFieldPMs.Add(additionalFactField);
                        }
                    }
                }
            }

            DWObjectFieldPMs = DWObjectFieldPMs.Concat(factDWObjectFieldPMs).ToList();
        }

        private static readonly HashSet<string> ProfitCodes = new HashSet<string> {
            "[Profit]", "[Profit ( Local )]", "[Accounted Profit]", "[Accounted Profit(Local)]"
        };

        private bool IsShipmentProfitField(DWObjectFieldPM Field)
        {
            return ProfitCodes.Contains(Field.Code) && factTableCode == "Fact_ARInvoices" && Field.DWObjectTableCode == "Fact_Shipments";
        }

        private bool ContainTableRecordType(DWObjectFieldPM additionalFactField)
        {
            if (string.IsNullOrEmpty(additionalFactField.RecordType)) return true;
            string[] recordTypes = additionalFactField.RecordType.Split(',');

            foreach (var recordType in recordTypes)
            {
                if (recordType.Trim() == DwObjectTable.RecordType) return true;
            }
            return false;
        }
    }

    public class DWObjectFieldAdditionalFactArgs
    {
        public string FactTableCode { get; set; }
        public int Tenant { get; set; }
        public bool DontLoadDwObjectField { get; set; }
        public bool GroupedByCategory { get; set; }
    }
}
