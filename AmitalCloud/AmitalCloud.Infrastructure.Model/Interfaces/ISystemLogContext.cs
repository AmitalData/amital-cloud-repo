using AmitalCloud.Infrastructure.Model.EntityClasses;
using Microsoft.EntityFrameworkCore;

namespace AmitalCloud.Infrastructure.Model.Interfaces
{ 
    public interface ISystemLogContext : IContext
    {
        DbSet<ContactActivityLog> ContactActivityLogs { get; }
        DbSet<ErrorLog> ErrorLogs { get; }
        DbSet<BatchServicesLog> BatchServicesLogs { get; }
        DbSet<FailedLoginLog> FailedLoginLogs { get; }

        void DetectChanges();
    }
}
