 using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Shared.Reflection;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using AutoMapper;
using Microsoft.Extensions.Logging;
using AutoMapper.Internal;
using System.Reflection;


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

             var getSingleGenericMethod = repoType
                .GetMethods()
                .Where(m => m.Name == "GetSingle" && m.IsGenericMethodDefinition)
                .Where(m =>
                {
                    var parameters = m.GetParameters();
                    return parameters.Length == 1 &&
                           parameters[0].ParameterType.IsGenericType &&
                           parameters[0].ParameterType.GetGenericTypeDefinition() == typeof(IEntityKeyFields<,>);
                })
                .SingleOrDefault();

            if (getSingleGenericMethod == null)
                throw new Exception("GetSingle method not found");

             var keyInterface = keysInstance.GetType()
                .GetInterfaces()
                .FirstOrDefault(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(IEntityKeyFields<,>));

            if (keyInterface == null)
                throw new Exception("keysInstance does not implement IEntityKeyFields<,>");

            var keyType = keyInterface.GetGenericArguments()[1];

             var closedMethod = getSingleGenericMethod.MakeGenericMethod(keyType);
             try
            {
                var poco = closedMethod.Invoke(repository, new object[] { keysInstance });
                return poco == null ? null : mapper.Map(poco, pocoType, pmType);

            }
            catch (TargetInvocationException ex)
            {
                Console.WriteLine("Invoke failed!");
                Console.WriteLine(ex.InnerException?.Message);
                Console.WriteLine(ex.InnerException?.StackTrace);
                throw ex;
            }


        }
    }

}
