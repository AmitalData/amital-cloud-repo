using AmitalCloud.Infrastructure.Application.EntityQueryServices;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using AmitalCloud.Infrastructure.Model.BaseClasses;
using AmitalCloud.Infrastructure.Shared.Reflection;

public class GenericEntityQueryServiceFactory : IGenericEntityQueryServiceFactory
{
    private readonly EntityTypeResolver _resolver = new();

    public IDynamicEntityQueryService Create(string entityName, int tenant)
    {
        var info = _resolver.Resolve(entityName)
                   ?? throw new Exception($"Entity '{entityName}' not found.");
        return new DynamicEntityQueryService(info, tenant);
    }

    public IGenericEntityQueryService<TPM> CreateGeneric<TPOCO, TKeys, TPM, TList, TKeyType>(
        IRepository<TPOCO> repo, IMapping<TPM, TPOCO, TList> mapping)
        where TPOCO : BaseEntity, new()
        where TPM : IEntityPM, new()
        where TKeys : IEntityKeyFields<TPOCO, TKeyType>, new()
        where TList : class, new()
    {
        return new GenericEntityQueryService<TPOCO, TKeys, TPM, TList, TKeyType>(repo, mapping);
    }
}