using AutoMapper;

namespace AmitalCloud.Infrastructure.Domain.Interfaces
{
    public interface IMapping<TEntityPM, TEntityPOCO, TEntityList>
    {
        IMapper CreateMapper();
    }
    public interface IMappingEncodeBase64NVARCHARFields<TEntityPM>
    {
        void EncodeBase64NVARCHARFields(TEntityPM entityPM);
    }
}
