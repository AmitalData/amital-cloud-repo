using System.Linq;

namespace AmitalCloud.Infrastructure.Domain.Interfaces
{
    public interface IMapping<TEntityPM, TEntityPOCO, TEntityList>
    {
        void PMToPOCO(TEntityPM entityPM, TEntityPOCO entityPOCO);
        void POCOToPM(TEntityPM entityPM, TEntityPOCO entityPOCO);
        void CustomPMToPOCO(TEntityPM entityPM, TEntityPOCO entityPOCO);
        void CustomPOCOToPM(TEntityPM entityPM, TEntityPOCO entityPOCO);
        void PMToOldPM(TEntityPM entityPM, TEntityPM oldEntityPM);
        void POCOToList(TEntityPOCO entityPOCO, TEntityList entityList);
        IQueryable<TEntityList> GetIqueryableList(IQueryable<TEntityPOCO> iQueryable);

    }
    public interface IMappingEncodeBase64NVARCHARFields<TEntityPM>
    {
        void EncodeBase64NVARCHARFields(TEntityPM entityPM);
    }
}
