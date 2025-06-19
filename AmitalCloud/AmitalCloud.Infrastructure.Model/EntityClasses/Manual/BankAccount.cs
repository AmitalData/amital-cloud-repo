using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AmitalCloud.Infrastructure.Model.EntityClasses
{

    public class BankAccount
    {
	 string dbms;

        [Key]
        [Column("Id")]
	    public string Id { get; set; }
        [Column("Tenant")]
	    public int Tenant { get; set; }
        [Column("CreateDate")]
	    public DateTime CreateDate { get; set; }
        [ForeignKey("CreatedByUser")]
        [Column("CreatedByUserId")]
	    public string CreatedByUserId { get; set; }
	      
        public virtual User CreatedByUser { get; set; }
        [Column("UpdateDate")]
	    public DateTime UpdateDate { get; set; }
        [ForeignKey("UpdatedByUser")]
        [Column("UpdatedByUserId")]
	    public string UpdatedByUserId { get; set; }
	      
        public virtual User UpdatedByUser { get; set; }
        [Column("SearchFields")]
	    public string SearchFields { get; set; }
        [Column("LocalName")]
	    public string LocalName { get; set; }
        [Column("EnglishName")]
	    public string EnglishName { get; set; }
        [ForeignKey("BankCode")]
        [Column("BankId")]
	    public string BankId { get; set; }
	      
        public virtual BankCode BankCode { get; set; }
        [Column("BranchNumber")]
	    public string BranchNumber { get; set; }
        [Column("AccountNumber")]
	    public string AccountNumber { get; set; }
        //todo:
        //[ForeignKey("GLAccount")]
        [Column("GLAccountId")]
	    public string GLAccountId { get; set; }
	      
        //public virtual GLAccount GLAccount { get; set; }
        //[ForeignKey("DeferredGLAccount")]
        [Column("DeferredGLAccountId")]
	    public string DeferredGLAccountId { get; set; }
	      
        //public virtual GLAccount DeferredGLAccount { get; set; }
        [Column("IBAN")]
	    public string IBAN { get; set; }
        [Column("SwiftCode")]
	    public string SwiftCode { get; set; }
        [Column("BranchAddress")]
	    public string BranchAddress { get; set; }
        [Column("Inactive")]
	    public bool? Inactive { get; set; }
        [Column("ChequeCounter")]
	    public int? ChequeCounter { get; set; }
        [Column("LastPageNumber")]
	    public string LastPageNumber { get; set; }
        [Column("LastPageEndDate")]
	    public DateTime? LastPageEndDate { get; set; }
        [Column("LastPageCloseBalance")]
	    public decimal? LastPageCloseBalance { get; set; }
        //[ForeignKey("TransferGLAcccount")]
        [Column("TransferGLAcccountId")]
	    public string TransferGLAcccountId { get; set; }
	      
        //public virtual GLAccount TransferGLAcccount { get; set; }
        [ForeignKey("Currency")]
        [Column("CurrencyId")]
	    public string CurrencyId { get; set; }
	      
        public virtual Currency Currency { get; set; }
        [Column("PrintingBranchNumber")]
	    public string PrintingBranchNumber { get; set; }
        [Column("PrintingAccountNumber")]
	    public string PrintingAccountNumber { get; set; }
        [Column("TotalOpenExternalTransactions")]
	    public string TotalOpenExternalTransactions { get; set; }
        [Column("TotalOpenPagesLines")]
	    public string TotalOpenPagesLines { get; set; }
        [Column("ChequeCounterSeriesID")]
	    public int? ChequeCounterSeriesID { get; set; }
    }
}
	 