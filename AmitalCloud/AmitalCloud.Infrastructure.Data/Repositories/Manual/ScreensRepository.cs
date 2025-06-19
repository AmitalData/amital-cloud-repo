using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System.Linq;
using AmitalCloud.Infrastructure.Model.Interfaces;
namespace AmitalCloud.Infrastructure.Data.Repositories
{
    public class ScreensRepository : Repository<Screen>
    {
        IAmitalCloudContext amitalCloudContext;

        public ScreensRepository(IAmitalCloudContext context) : base(context)
        {
            amitalCloudContext = context;
        }
        public ScreensRepository(int tenant) : this(AmitalCloudContext.GetContext(tenant))
        {
        }
        public IQueryable<Screen> GetScreens() => this.context.Screens;
        public IQueryable<Screen> GetScreensByTenant(int tenant)
        {
            return from a in context.Screens
                   where a.Tenant == tenant
                   select a;
        }
        public Screen GetSingleScreen(string id)
        {
            return (from a in context.Screens.Include("ObjectTable")
                    where a.Id == id
                    select a).FirstOrDefault();
        }
        public ScreenModification GetScreenModificationByScreen(string screenCode, int tenant)
        {
            return (from a in context.ScreenModifications
                    where a.ScreenCode == screenCode && a.Tenant == tenant
                    select a).FirstOrDefault();
        }
        public Screen GetByCode(string code, int tenant) => GetMulti(a => a.Code == code && a.Tenant == tenant).FirstOrDefault();
        public IAmitalCloudContext context
        {
            get { return amitalCloudContext; }
        }
    }
}
