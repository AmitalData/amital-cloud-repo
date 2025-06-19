using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using AmitalCloud.Infrastructure.Model.Enums;
using AmitalCloud.Infrastructure.Model.Interfaces;
using AmitalCloud.Infrastructure.Data.BaseClasses;
using AmitalCloud.Infrastructure.Data.Helpers;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using AmitalCloud.Infrastructure.Data.DBHelpers;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;


namespace AmitalCloud.Infrastructure.Data.Context
{
    public class SystemLogContext : DbContextBase, ISystemLogContext
    {
        private SystemLogContext()
        {
        }
        private SystemLogContext(DbContextOptions options) : base(options)
        {
            InitializeContext();
        }
        private void InitializeContext()
        {
            this.ChangeTracker.LazyLoadingEnabled = true;
            this.ChangeTracker.AutoDetectChangesEnabled = false;
        }
        public static ISystemLogContext GetContext(int tenant)
        {
            string dbConnectionInfo = ConfigurationHelper.GetConnectionString(DbContextBaseUtil.SystemLogsConnectionString);
            dbConnectionInfo = DbContextBaseUtil.GetConnectionStringWithAmitalNetRole(dbConnectionInfo);

            DbContextOptionsBuilder<SystemLogContext> optionsBuilder = new DbContextOptionsBuilder<SystemLogContext>();

            if (AmitalCloudSettings.DatabaseManagementSystem == "oracle")
            {
                DbConnection connection = DatabaseInitializer.GetConnection(dbConnectionInfo, 5);
                optionsBuilder.UseOracle(connection);
            }
            else
            {
                dbConnectionInfo = DatabaseInitializer.GetConnectionString(dbConnectionInfo);
                optionsBuilder.UseSqlServer(dbConnectionInfo);
            }

            if (DbContextBaseUtil.ToLog.GetValueOrDefault())
            {
                optionsBuilder
                    .EnableSensitiveDataLogging()
                    .LogTo(message => System.Diagnostics.Debug.WriteLine(message), LogLevel.Debug);
            }

            return new SystemLogContext(optionsBuilder.Options);
        }
        protected override AmitalCloudDBSchema AmitalCloudDBSchema
        {
            get { return AmitalCloudDBSchema.AMITAL_LOGS; }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
        public DbSet<ErrorLog> ErrorLogs { get; set; }
        public DbSet<ContactActivityLog> ContactActivityLogs { get; set; }
        public DbSet<BatchServicesLog> BatchServicesLogs { get; set; }
        public DbSet<FailedLoginLog> FailedLoginLogs { get; set; }
        public DbSet<FailedTokenLog> FailedTokenLogs { get; set; }

        public int Tenant => throw new NotImplementedException();

        public void SetAsModified(object entity)
        {
            this.Entry(entity).State = EntityState.Modified;
        }
        public void DetectChanges()
        {
            this.ChangeTracker.DetectChanges();
        }
        public override int SaveChanges()
        {
            DetectChanges();
            try
            {
                return base.SaveChanges();
            }
            catch
            {
            }
            return 1;
        }
        public DbConnection GetConnection()
        {
            return this.Database.GetDbConnection();
        }

        public DbContext GetActiveDbContext()
        {
            return this;
        }

        public Task<int> SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                base.Dispose();
            }
        }

        public override void Dispose()
        {
            Dispose(true);
        }
    }
}
