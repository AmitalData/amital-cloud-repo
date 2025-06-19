using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Infrastructure.Domain.EntityPMs
{
    public class CustomerSalesmanByProductPM : BaseClasses.BaseEntityPM
    {
        [Key]
        public string ProductTypeCode { get; set; }
        public string SalesmanUserId { get; set; }
        [Key]
        public string CustomerId { get; set; }
        public string SalesmanUserName { get; set; }
    }
}