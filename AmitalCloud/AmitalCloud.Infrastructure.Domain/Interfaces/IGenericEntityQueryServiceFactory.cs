using AmitalCloud.Infrastructure.Domain.Interfaces;
using AmitalCloud.Infrastructure.Model.BaseClasses;

public interface IGenericEntityQueryServiceFactory
{
    IDynamicEntityQueryService Create(string entityName, int tenant);

    IGenericEntityQueryService<TPM> CreateGeneric<TPOCO, TKeys, TPM, TList, TKeyType>(
        IRepository<TPOCO> repo,
        IMapping<TPM, TPOCO, TList> mapping)
        where TPOCO : BaseEntity, new()
        where TPM : IEntityPM, new()
        where TKeys : IEntityKeyFields<TPOCO, TKeyType>, new()
        where TList : class, new();
}