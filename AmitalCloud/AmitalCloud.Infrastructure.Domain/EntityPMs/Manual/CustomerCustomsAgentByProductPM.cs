using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Infrastructure.Domain.EntityPMs
{
    public class CustomerCustomsAgentByProductPM : BaseClasses.BaseEntityPM
    {
        [Key]
        public string ProductTypeCode { get; set; }
        public string CustomsAgentId { get; set; }
        [Key]
        public string CustomerId { get; set; }
        public string CustomsAgentName { get; set; }
    }
}
