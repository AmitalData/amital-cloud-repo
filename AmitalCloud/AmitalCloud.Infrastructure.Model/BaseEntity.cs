using AmitalCloud.Infrastructure.Model.Interfaces;
namespace AmitalCloud.Infrastructure.Model.BaseClasses
{
    public class BaseEntity : IEntity
    {
        protected string dbms;
        protected const bool hasTenant = true;
        public static bool HasTenant => hasTenant;
        //[Column("SearchFields")]
        //public string SearchFields { get; set; }
    }
}