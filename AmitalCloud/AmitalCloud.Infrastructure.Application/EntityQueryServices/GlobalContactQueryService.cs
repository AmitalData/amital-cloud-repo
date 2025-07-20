


using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using System.Linq.Expressions;

namespace AmitalCloud.Infrastructure.Application.EntityQueryServices
{ 
   public partial class GlobalContactQueryService
   {
        public List<GlobalContactPM> GetContactsByEmail(string email, int tenant)
        {
            var repository = new Repository<GlobalContact>(tenant);

            Expression<Func<GlobalContact, bool>> predicate = c =>
                c.Email.ToLower() == email.ToLower() &&
                !c.InActive &&
                c.GlobalTenant.IsActive ;

            return repository.GetMulti<GlobalContactPM>(predicate, "GlobalTenant");
        }


    }
}
