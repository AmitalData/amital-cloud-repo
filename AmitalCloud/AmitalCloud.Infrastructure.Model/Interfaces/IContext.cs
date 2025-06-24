using AmitalCloud.Infrastructure.Model.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Data.Common;

namespace AmitalCloud.Infrastructure.Model.Interfaces
{
    public interface IContext : IDisposable
    {
        DbConnection GetConnection();
        DbContext GetActiveDbContext();
        void SetAsModified(object entity);
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        Task<int> SaveChangesAsync();
        int SaveChanges();
        int Tenant { get; }
        DatabaseFacade Database { get; }
        AmitalCloudDBSchema AmitalCloudDBSchema { get; }

    }
}
