using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Model.Enums;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Text.RegularExpressions;
using System.Transactions;
using IsolationLevel = System.Transactions.IsolationLevel;
using AmitalCloud.Infrastructure.Data.DBHelpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;

namespace AmitalCloud.Infrastructure.Data.BaseClasses
{
    public abstract class DbContextBase : DbContext
    {
        FixedSizedQueue<string> _MyLogQueue = new FixedSizedQueue<string>(30);
        abstract public AmitalCloudDBSchema AmitalCloudDBSchema { get; }
        public DbContextBase() : base()
        {
            Database.GetDbConnection().StateChange += Connection_StateChange;
            InitLog();
        }
        public DbContextBase(DbContextOptions options) : base(options)
        {
            Database.GetDbConnection().StateChange += Connection_StateChange;
            InitLog();
        }

        public override int SaveChanges()
        {
            bool suppressThrow = false;
            var saveChangeLogger = CreateLogger();
            var commandTimeout = this.Database.GetCommandTimeout();
            try
            {
                var to = 2 * 60;
                if (AmitalCloudSettings.DatabaseManagementSystem == "oracle")
                {
                    this.Database.SetCommandTimeout(to);
                }
                else if (ApplicationAppInfo.WorkerRoleCall)
                {
                    this.Database.SetCommandTimeout(10);
                }
                else if (!ApplicationAppInfo.WorkerRoleCall)
                {
                    this.Database.SetCommandTimeout(15);
                }
                var intSave = base.SaveChanges();
                var testEx = false;
                if (testEx)
                {
                    throw new Exception("TTTEST");
                }
                return intSave;
            }
            catch (DbUpdateException dbu)
            {
                FormatDbUpdateException(dbu);
                if (this.AmitalCloudDBSchema == AmitalCloudDBSchema.AMITAL_MAIN)
                {
                    if (AmitalCloudSettings.HandleDbExceptionInject != null)
                    {
                        AmitalCloudSettings.HandleDbExceptionInject(dbu, "SaveChanges:DbUpdateException", GetString4LogGroupBy(saveChangeLogger, Environment.StackTrace.ToString()));
                    }
                }
                if (suppressThrow)
                {
                    return -999;
                }
                throw dbu;
            }
            catch (Exception e)
            {
                if (this.AmitalCloudDBSchema != AmitalCloudDBSchema.AMITAL_LOGS && AmitalCloudSettings.HandleDbExceptionInject != null)
                {
                    AmitalCloudSettings.HandleDbExceptionInject(e, "SaveChanges:Exception", GetString4LogGroupBy(saveChangeLogger, Environment.StackTrace.ToString()));
                }
                else
                {
                    AmitalCloudDebuggerUtil.Break(AmitalDebuggerLevel.Critical);
                }
                if (suppressThrow)
                {
                    return -999;
                }
                throw;
            }
            finally
            {
                saveChangeLogger.Dispose();
                this.Database.SetCommandTimeout(commandTimeout);
            }
        }
        private static string GetString4LogGroupBy(IDbContextLogger saveChangeLogger, string EnvironmentStackTraceToString)
        {
            string mySaveLog = saveChangeLogger.ToString() ?? "";
            string myStack = EnvironmentStackTraceToString ?? "";
            string myHeader = "";
            try
            {
                string[] lines = mySaveLog
                    .Split(Environment.NewLine.ToCharArray())
                    .Skip(1)
                    .ToArray();
                mySaveLog = string.Join(Environment.NewLine, lines);
                mySaveLog = mySaveLog.TrimStart(Environment.NewLine.ToCharArray());
                myHeader = EnvironmentStackTraceToString.Split(Environment.NewLine.ToCharArray()).LastOrDefault(l => l.Contains(".cs:line")) ?? "";
                myHeader += ":";
            }
            catch { }
            return myHeader + "~" + mySaveLog + "~" + myStack; ;
        }
        private void FormatDbUpdateException(DbUpdateException dbu)
        {
            var builder = new StringBuilder("A DbUpdateException was caught while saving changes. ");
            try
            {
                foreach (var result in dbu.Entries)
                {
                    builder.AppendFormat("Type: {0} was part of the problem. ", result.Entity.GetType().Name).AppendLine();
                    if (result.State != EntityState.Deleted && result.CurrentValues != null)
                    {
                        foreach (var propertyName in result.CurrentValues.Properties)
                        {
                            builder.AppendLine().AppendFormat("Property Name: {0}", propertyName);
                            if (result.State != EntityState.Added)
                            {
                                var orgVal = result.OriginalValues[propertyName];
                                builder.AppendFormat("     Original Value: {0}", orgVal);
                            }
                            var curVal = result.CurrentValues[propertyName];
                            builder.AppendFormat("     Current Value: {0}", curVal);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                builder.Append("Error parsing DbUpdateException: " + e.ToString());
            }
            string message = builder.ToString();
            dbu.ChangeExceptionMess(message);
        }
        public IDbContextLogger CreateLogger() => new DbContextLogger(this.Database) as IDbContextLogger;
        public override string ToString()
        {
            var sb = new StringBuilder();
            _MyLogQueue.ToList().ForEach(
                m =>
                {
                    sb.AppendLine(m);
                });
            return base.ToString() + sb.ToString();

        }
        private void InitLog()
        {
            if (AmitalCloudSettings.DatabaseManagementSystem != "oracle")
                return;

            if (!System.Diagnostics.Debugger.IsAttached)
                return;

            if (DbContextBaseUtil.ToLog == null)
            {
                DbContextBaseUtil.ToLog = false;
                NetCommonHelper.Logger.DevLog.Instance.WriteDebug(@"DbContextBase:ToLog:(Default:False due Memory Leak if not Disposed)Any time any place u can set: 
                        DBHelpers.DbContextBaseUtil.ToLog =true;");
                AmitalCloudDebuggerUtil.Break();
            }

            if (DbContextBaseUtil.ToLog.GetValueOrDefault())
            {
                TransactionFactory.RegisterTransactionCompleted();
            }
        }
        private void EnqueueLog(string mess)
        {
            try
            {
                if (mess == Environment.NewLine)
                {
                    return;
                }
                NetCommonHelper.Logger.DevLog.Instance.WriteDebug(mess);
                _MyLogQueue.Enqueue(mess);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public override void Dispose()
        {
            try
            {
                var dbConn = this.Database?.GetDbConnection();
                if (dbConn != null)
                {
                    dbConn.StateChange -= Connection_StateChange;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Dispose error: {ex.Message}");
            }
            base.Dispose();
        }
        private void Connection_StateChange(object sender, StateChangeEventArgs args)
        {
            if (AmitalCloudSettings.System2RedirectFraction == 0)
            {
                return;
            }
            if (ApplySnapshotIsolation())
            {
                SetTransactionIsolationLevel(args);
            }
        }

        private bool ApplySnapshotIsolation()
        {
            Random random = new Random();
            var LuckyNumber = random.Next(1, 101);
            return ((LuckyNumber % AmitalCloudSettings.System2RedirectFraction) == 0);
        }
        private void SetTransactionIsolationLevel(StateChangeEventArgs args)
        {
            if (args.CurrentState == ConnectionState.Open && args.OriginalState != ConnectionState.Open)
            {
                using (var command = Database.GetDbConnection().CreateCommand())
                {
                    if (Transaction.Current == null)
                    {
                        command.CommandText = "SET TRANSACTION ISOLATION LEVEL SNAPSHOT";
                    }
                    else
                    {
                        switch (Transaction.Current.IsolationLevel)
                        {
                            case IsolationLevel.ReadCommitted:
                                command.CommandText = "SET TRANSACTION ISOLATION LEVEL READ COMMITTED";
                                break;
                            case IsolationLevel.ReadUncommitted:
                                command.CommandText = "SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED";
                                break;
                            case IsolationLevel.Snapshot:
                                command.CommandText = "SET TRANSACTION ISOLATION LEVEL SNAPSHOT";
                                break;
                            case IsolationLevel.Serializable:
                                command.CommandText = "SET TRANSACTION ISOLATION LEVEL SERIALIZABLE";
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        static string _UserSlashPass = null;
        public List<string> GetTableNames(string pocoNamespace)
        {
            List<string> tableNameList = new List<string>();
            
            var entityTypes = this.Model.GetEntityTypes();

            foreach (var entityType in entityTypes)
            {
                var clrType = entityType.ClrType;

                if (!clrType.FullName.StartsWith(pocoNamespace))
                    continue;

                var schema = entityType.GetSchema() ?? "dbo";
                var tableName = entityType.GetTableName();

                if (!string.IsNullOrEmpty(tableName))
                {
                    var fullTableName = $"{schema}.{tableName}";
                    if (!tableNameList.Contains(fullTableName))
                    {
                        tableNameList.Add(fullTableName);
                    }
                }
            }

            return tableNameList;
        }
        const bool FeatureRemoveCaseInsensitive = true;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
        public Nullable<returnType> ExecuteReaderSingleResult<returnType>(string sqlReturn1Row, Func<DbDataReader, Nullable<returnType>> GetReturnTypeFromReader)
        where returnType : struct
        {
            NetCommonHelper.Logger.DevLog.Instance.WriteDebug(sqlReturn1Row);
            ////sqlReturn1Row = sqlReturn1Row.TrimEnd(" "[0]).TrimEnd(";"[0]);
            using (var command = this.Database.GetDbConnection().CreateCommand())
            {


                if (this.Database.GetDbConnection().State != System.Data.ConnectionState.Open)
                {
                    this.Database.GetDbConnection().Open();
                }
                command.CommandText = sqlReturn1Row;


                using (var dataReader = command.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SingleResult)
                    )
                {

                    if (dataReader.FieldCount < 1)
                    {
                        return null;
                    }

                    if (!dataReader.Read())
                    {
                        return null;
                    }
                    if (dataReader.IsDBNull(0))
                    {
                        return null;
                    }



                    var ReturnValue = GetReturnTypeFromReader(dataReader);

                    return ReturnValue;
                }
            }

        }
    }
}
