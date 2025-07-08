using AmitalCloud.Infrastructure.Domain.BaseClasses;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using System;

namespace AmitalCloud.Invoice.Domain.EntityPMs
{
    public class BankAccountPM : BaseEntityPM
    {
        public BankAccountPM() : base() { }
    
        public string Id { get; set; }
        public int Tenant { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreatedByUserId { get; set; }
        public virtual UserPM CreatedByUser { get; set; }
        public DateTime UpdateDate { get; set; }
        public string UpdatedByUserId { get; set; }

        public virtual UserPM UpdatedByUser { get; set; }
        public string SearchFields { get; set; }
        public string LocalName { get; set; }
        public string EnglishName { get; set; }
        public string BankId { get; set; }
        public virtual BankCodePM BankCode { get; set; }
        public string BranchNumber { get; set; }
        public string AccountNumber { get; set; }
        public string GLAccountId { get; set; }
        public string DeferredGLAccountId { get; set; }
        public string IBAN { get; set; }
        public string SwiftCode { get; set; }
        public string BranchAddress { get; set; }
        public bool? Inactive { get; set; }
        public int? ChequeCounter { get; set; }
        public string LastPageNumber { get; set; }
        public DateTime? LastPageEndDate { get; set; }
        public decimal? LastPageCloseBalance { get; set; }
        public string TransferGLAcccountId { get; set; }
        public string CurrencyId { get; set; }
        public virtual CurrencyPM Currency { get; set; }
        public string PrintingBranchNumber { get; set; }
        public string PrintingAccountNumber { get; set; }
        public string TotalOpenExternalTransactions { get; set; }
        public string TotalOpenPagesLines { get; set; }
        public int? ChequeCounterSeriesID { get; set; }
    }
}
