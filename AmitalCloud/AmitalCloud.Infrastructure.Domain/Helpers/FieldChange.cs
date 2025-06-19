using System.Collections.Generic;

namespace AmitalCloud.Infrastructure.Domain.Helpers
{
    public class FieldChange
    {
        public string Field { get; set; }
        public object OldValue { get; set; }
        public object NewValue { get; set; }

        public static void Add(object oldValue, object newValue, string fieldName, List<FieldChange> changedProperties)
        {
            if (oldValue != newValue && (oldValue == null || !oldValue.Equals(newValue)))
            {
                if (changedProperties == null)
                {
                    changedProperties = new List<FieldChange>();
                }

                changedProperties.Add(new FieldChange()
                {
                    Field = fieldName,
                    OldValue = oldValue,
                    NewValue = newValue
                });
            }
        }
    }

}
