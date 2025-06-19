using System.Collections.Generic;

namespace AmitalCloud.Infrastructure.Domain.EntityPMs
{
    public class CustomChildEntity
    {
        public string Name { get; set; }
        public List<CustomChildObjectPM> Values { get; set; }
    }
}
