using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Infrastructure.Domain.EntityPMs
{
    public class CustomerMediatorByProductPM : BaseClasses.BaseEntityPM
    {
        [Key]
        public string ProductTypeCode { get; set; }
        public string MediatorId { get; set; }
        [Key]
        public string CustomerId { get; set; }
        public string MediatorName { get; set; }
    }
}