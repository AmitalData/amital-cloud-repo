namespace AmitalCloud.Infrastructure.Shared.Reflection
{
    public class EntityTypeResolver
    {
        public (Type entityType, Type listType)? Resolve(string entityName)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var entityType = assemblies
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.Name.Equals(entityName, StringComparison.OrdinalIgnoreCase));

            var listType = assemblies
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(t => t.Name.Equals(entityName + "List", StringComparison.OrdinalIgnoreCase));

            if (entityType != null && listType != null)
                return (entityType, listType);

            return null;
        }
    }
}
