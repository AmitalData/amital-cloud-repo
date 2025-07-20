using AmitalCloud.Infrastructure.Application.BaseClasses;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using AmitalCloud.Infrastructure.Shared.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmitalCloud.Infrastructure.Application.EntityQueryServices
{

    public class DynamicEntityQueryService : IDynamicEntityQueryService
    {
        private readonly object _serviceInstance;
        private readonly Type _serviceType;

        public DynamicEntityQueryService(EntityTypeInfo info, int tenant)
        {
            var keyInterface = info.KeysType.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityKeyFields<,>))
                ?? throw new Exception("Invalid KeysType");

            var tKeyType = keyInterface.GetGenericArguments()[1];

            var repoType = typeof(Repository<>).MakeGenericType(info.PocoType);
            var repo = Activator.CreateInstance(repoType, tenant)
                       ?? throw new Exception("Failed to create repository");

            var mapping = Activator.CreateInstance(info.MappingType)
                          ?? throw new Exception("Failed to create mapping");

            _serviceType = typeof(BaseEntityQueryService<,,,,>).MakeGenericType(
                info.PocoType, info.KeysType, info.PmType, info.ListType, tKeyType);

            _serviceInstance = Activator.CreateInstance(_serviceType, repo, mapping)
                               ?? throw new Exception("Failed to create service");
        }

        private object? Invoke(string methodName, params object[] args)
        {
            var method = _serviceType.GetMethods().FirstOrDefault(m =>
                m.Name == methodName && m.GetParameters().Length == args.Length);

            return method?.Invoke(_serviceInstance, args);
        }

        public object? GetSingle(Dictionary<string, string> keyParams, bool getComposition = true, bool getFromCache = false)
        {
            // יצירת מופע keys
            var keysInstance = Activator.CreateInstance(
                _serviceType.GenericTypeArguments[1]); // index 1 = TEntityKeys

            var initMethod = keysInstance?.GetType().GetMethod("Initialize");
            initMethod?.Invoke(keysInstance, new object[] { keyParams });

            return Invoke("GetSingle", keysInstance!, getComposition, getFromCache);
        }
        public object? GetFirst() => Invoke("GetFirst");

        public List<object> GetMulti(object predicate) => (List<object>)Invoke("GetMulti", predicate);

        public List<object> GetMultiByParent(object parentKeys, bool getFromCache = false, bool getComposition = true)
            => (List<object>)Invoke("GetMultiByParent", parentKeys, getFromCache, getComposition);

        public List<object> GetMultiFromCache(string cacheKey, object predicate, string? include = null)
            => (List<object>)Invoke("GetMultiFromCache", cacheKey, predicate, include);

        public List<TResult> GetMultiSelect<TResult>(object predicate, object selector)
            => (List<TResult>)Invoke("GetMulti", predicate, selector);

        public List<TResult> GetMultiSelect<TResult>(object predicate, object selector, string include)
            => (List<TResult>)Invoke("GetMulti", predicate, selector, include);
    }

}
