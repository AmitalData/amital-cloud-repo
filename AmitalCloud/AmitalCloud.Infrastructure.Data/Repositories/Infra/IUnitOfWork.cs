using AmitalCloud.Infrastructure.Domain.Interfaces;
using AmitalCloud.Infrastructure.Model.Interfaces;
using System;
using System.Transactions;
namespace AmitalCloud.Infrastructure.Data.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        //The following Property is going to hold the context object
        IContext Context { get; }

        //Start the database Transaction
        //       void CreateTransaction();
        void CreateTransactionScope(TransactionScopeOption option);
        //Commit the database Transaction
        void Commit();

        //Rollback the database Transaction
        void Rollback();

        //DbContext Class SaveChanges method
        void Save();
    }
}
