using AmitalCloud.Infrastructure.Model.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;
using System.Text;
using System.Transactions;

namespace AmitalCloud.Infrastructure.Data.Repositories
{
    //Generic UnitOfWork Class. 
    //While Creating an Instance of the UnitOfWork object, we need to specify the actual type for the TContext Generic Type
    //In our example, TContext is going to be EmployeeDBContext
    //new() constraint will make sure that this type is going to be a non-abstract type with a parameterless constructor
    public class UnitOfWork<TContext> : IUnitOfWork, IDisposable where TContext : IContext
    {
        private bool _disposed;
        private string _errorMessage = string.Empty;
        private Action _commit;
        private Action _rollback;
        private Action _dispose;
        private TContext _context;
        //The following Object is going to hold the Transaction Object
        //private DbContextTransaction _objTran;
        TransactionScope _tranScope; //= TransactionFactory.GetNewTransaction()
        //Using the Constructor we are initializing the Context Property which is declared in the IUnitOfWork Interface
        //This is nothing but we are storing the DBContext (EmployeeDBContext) object in Context Property
        public UnitOfWork(int tenant)
        {
            _context = (TContext)typeof(TContext).GetMethod("GetContext", BindingFlags.Public | BindingFlags.Static).Invoke(null, new object[] { tenant });

        }
        public UnitOfWork(TContext context)
        {
            _context = context;
        }
        //The Dispose() method is used to free unmanaged resources like files, 
        //database connections etc. at any time.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        //The Context property will return the DBContext object i.e. (EmployeeDBContext) object
        //This Property is declared inside the Parent Interface and Initialized through the Constructor
        public IContext Context { get => _context; }

        //The CreateTransaction() method will create a database Transaction so that we can do database operations
        //by applying do everything and do nothing principle
        public void CreateTransactionScope(TransactionScopeOption option)
        {
            //It will Begin the transaction on the underlying store connection
            _tranScope = new TransactionScope(option);//  TransactionFactory.GetNewTransaction();
            //_objTran = Context.Database.BeginTransaction();
        }

        //If all the Transactions are completed successfully then we need to call this Commit() 
        //method to Save the changes permanently in the database
        public void Commit()
        {
            //Commits the underlying store transaction
            _tranScope.Complete();
            //_objTran.Commit();
        }

        //If at least one of the Transaction is Failed then we need to call this Rollback() 
        //method to Rollback the database changes to its previous state
        public void Rollback()
        {
            //Rolls back the underlying store transaction
            //_objTran.Rollback();
            _tranScope.Dispose();
            //The Dispose Method will clean up this transaction object and ensures Entity Framework
            //is no longer using that transaction.
            //_objTran.Dispose();
        }

        //The Save() Method Implement DbContext Class SaveChanges method 
        //So whenever we do a transaction we need to call this Save() method 
        //so that it will make the changes in the database permanently
        public void Save()
        {
            try
            {
                //Calling DbContext Class SaveChanges method 
                typeof(TContext).GetMethod("SaveChanges", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, Type.EmptyTypes, null).Invoke(Context, null);

                //Context.SaveChanges();
            }
            catch (DbUpdateException dbEx)
            {
                var errorMessage = new StringBuilder();
                if (dbEx.InnerException != null)
                {
                    errorMessage.AppendLine($"Inner Exception: {dbEx.InnerException.Message}");
                }
                foreach (var entry in dbEx.Entries)
                {
                    errorMessage.AppendLine($"Entity of type {entry.Entity.GetType().Name} in state {entry.State} caused the error.");
                }
                throw new Exception(errorMessage.ToString(), dbEx);
            }
        }
        //Disposing of the Context Object
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Context.Dispose();
                    if (_tranScope != null)
                    {
                        _tranScope.Dispose();
                    }
                }
            }
            _disposed = true;
        }

        //
        // Summary:
        //     Occurs when transaction is committed.
        public event Action OnCommit
        {
            add
            {
                _commit = (Action)Delegate.Combine(_commit, value);
            }
            remove
            {
                _commit = (Action)Delegate.Remove(_commit, value);
            }
        }

        //
        // Summary:
        //     Occurs when transaction is rolled back.
        public event Action OnRollback
        {
            add
            {
                _rollback = (Action)Delegate.Combine(_rollback, value);
            }
            remove
            {
                _rollback = (Action)Delegate.Remove(_rollback, value);
            }
        }
    }
}