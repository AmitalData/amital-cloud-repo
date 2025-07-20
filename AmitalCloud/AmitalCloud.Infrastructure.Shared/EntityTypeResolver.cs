using System.Reflection;

namespace AmitalCloud.Infrastructure.Shared.Reflection
{
    public class EntityTypeInfo
    {
        public string EntityName { get; set; } = "";
        public Type PocoType { get; set; }
        public Type ListType { get; set; }
        public Type PmType { get; set; }
        public Type KeysType { get; set; }
        public Type MappingType { get; set; }
    }

    public class EntityTypeResolver
    {
        public EntityTypeInfo? Resolve(string entityName)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var pocoType = FindType(assemblies, entityName);
            var listType = FindType(assemblies, entityName + "List");
            var pmType = FindType(assemblies, entityName + "PM");
            var mappingType = FindType(assemblies, entityName + "DataMapping");
            var keysType = FindGenericType(assemblies, entityName + "Keys", typeof(string));

            if (pocoType == null || listType == null || pmType == null || keysType == null || mappingType == null)
                return null;

            return new EntityTypeInfo
            {
                EntityName = entityName,
                PocoType = pocoType,
                ListType = listType,
                PmType = pmType,
                KeysType = keysType,
                MappingType = mappingType
            };
        }

        private static Type? FindType(Assembly[] assemblies, string typeName)
        {
            return assemblies
                .SelectMany(SafeGetTypes)
                .FirstOrDefault(t => t.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));
        }

        private static Type? FindGenericType(Assembly[] assemblies, string baseTypeName, Type genericArg)
        {
            var openType = assemblies
                .SelectMany(SafeGetTypes)
                .FirstOrDefault(t => t.IsGenericTypeDefinition && t.Name.StartsWith(baseTypeName, StringComparison.OrdinalIgnoreCase));

            return openType?.MakeGenericType(genericArg);
        }

        private static IEnumerable<Type> SafeGetTypes(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch
            {
                return Array.Empty<Type>();
            }
        }
    }

}
