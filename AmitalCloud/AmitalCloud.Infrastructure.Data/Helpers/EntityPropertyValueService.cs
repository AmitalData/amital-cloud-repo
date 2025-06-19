using System.Reflection;

namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public class EntityPropertyValueService
    {
        private object entity = null;
        public EntityPropertyValueService(object entity)
        {
            this.entity = entity;
        }

        public object Get(string propertyName)
        {
            PropertyInfo propertyInfo = GetPropertyInfo(propertyName);
            if (propertyInfo == null) return null;
            return propertyInfo.GetValue(entity, null);
        }

        public PropertyInfo GetPropertyInfo(string propertyName)
        {
            Type type = entity.GetType();
            return type.GetProperty(propertyName);
        }
    }
}
