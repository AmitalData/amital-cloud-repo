using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Infrastructure.Domain.EntityPMs
{
    public class UserRolesPM : BaseClasses.BaseEntityPM
    {
        [Key]
        public string Id { get; set; }

        public string Name { get; set; }

        public bool Exists { get; set; }
        public bool Added { get; set; }
        public bool Removed { get; set; }

        public string UserId { get; set; }
    }
}