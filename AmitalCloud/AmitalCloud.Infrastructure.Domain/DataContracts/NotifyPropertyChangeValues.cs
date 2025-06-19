using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Infrastructure.Domain.DataContracts
{
    public class NotifyPropertyChangeValues
    {
        [Key]
        public string Id { get; set; }
        public string PropertyName { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }
        public string PropertyType { get; set; }
    }
}
