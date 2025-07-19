 using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Shared.Reflection;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;
using AutoMapper.Internal;


namespace AmitalCloud.Infrastructure.Application.EntityQueryServices
{
    public class GenericEntityQueryService : IGenericEntityQueryService
    {
        public object? GetSingle(string entityName, Dictionary<string, string> keyParams, int tenant)
        {
            var resolver = new EntityTypeResolver();
            var entityInfo = resolver.Resolve(entityName)
                ?? throw new InvalidOperationException($"Entity '{entityName}' could not be resolved.");

            var pocoType = entityInfo.PocoType;
            var pmType = entityInfo.PmType;
            var keysType = entityInfo.KeysType;

            var repoType = typeof(Repository<>).MakeGenericType(pocoType);
            Console.WriteLine($"Constructors for {repoType}:");
            var x = repoType.GetConstructors();
            var y =repoType.GetDeclaredConstructors();
            foreach (var ctor in repoType.GetConstructors())
            {
                Console.WriteLine(ctor.ToString());
            }
            var repository = Activator.CreateInstance(repoType, tenant)
                ?? throw new InvalidOperationException("Failed to create repository.");

            var mappingInstance = Activator.CreateInstance(entityInfo.MappingType)
                ?? throw new InvalidOperationException("Failed to create mapping profile.");
            var profile = mappingInstance as Profile
                ?? throw new InvalidOperationException("Mapping instance is not a valid AutoMapper profile.");

            var expression = new MapperConfigurationExpression();
            expression.AddProfile(profile);

             var loggerFactory = LoggerFactory.Create(builder => { });

             var config = new MapperConfiguration(expression, loggerFactory);

             var mapper = config.CreateMapper();


            var keysInstance = Activator.CreateInstance(keysType);
            var initMethod = keysType.GetMethod("Initialize");
            initMethod?.Invoke(keysInstance, new object[] { keyParams });

            var getSingleMethod = repoType.GetMethod("GetSingle");
            var poco = getSingleMethod?.Invoke(repository, new object[] { keysInstance });

            return poco == null ? null : mapper.Map(poco, pocoType, pmType);
        }
    }

}
