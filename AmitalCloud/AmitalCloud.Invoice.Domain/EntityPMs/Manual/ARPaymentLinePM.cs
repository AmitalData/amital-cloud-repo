using AmitalCloud.Invoice.Domain.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Invoice.Domain.EntityPMs
{
    [CustomValidation(typeof(IInvoiceClassLevelValidator), "ValidateClass")]
    public class ARPaymentLinePM
    {
        [Key]
        public string Id { get; set; }
        public int Tenant { get; set; }
        public string ARPaymentId { get; set; }
        public string ChequeOrPaymentRef { get; set; }

        [CustomValidation(typeof(IInvoiceValidationClass), "ValidateClass")]
        public double? Amount { get; set; }
        public DateTime? ValueDate { get; set; }
        public string Bank { get; set; }
        public string Branch { get; set; }
        public string Account { get; set; }
    }
}