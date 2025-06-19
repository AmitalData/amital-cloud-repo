using AmitalCloud.Infrastructure.Model.EntityClasses;
namespace AmitalCloud.Infrastructure.Domain.Interfaces
{
    public interface IObjectTableRepository
    {
        void Insert(ObjectTable entity);
        System.Collections.Generic.List<ObjectTable> GetAll(int tenant);
        //IAmitalCloudContext context { get; }
        ObjectTable GetFirstObjectTable(int tenant);
        System.Collections.Generic.List<ObjectTable> GetMulti<T>(IEntityKeyFields<ObjectTable, T> entityKeys);
        System.Linq.IQueryable<ObjectTable> GetObjects();
        System.Linq.IQueryable<ObjectTable> GetObjectsByTenant(int tenant);
        System.Linq.IQueryable<ObjectTable> GetObjectsByTenantOrTenantZero(int tenant);
        ObjectTable GetObjectTableById(string id, int tenant);
        ObjectTable GetObjectTableByName(string name, int tenant, bool getFromCache);
        string GetObjectTableIdByName(string tablename);
        ObjectTable GetSingle<T>(IEntityKeyFields<ObjectTable, T> entityKeys);
        ObjectTable GetSingleObjectTable(string id, int tenant, bool getFromCache);
        bool IsObjectTableMaster(string objectTableId);
        bool IsObjectTableShipment(string objectTableId);
        void Delete(ObjectTable entity);
        void Update(ObjectTable entity);
    }
}
