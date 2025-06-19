using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using AmitalCloud.Infrastructure.Model.Interfaces;

namespace AmitalCloud.Infrastructure.Data.Repositories
{
    public partial class FeatureToggleRepository : Repository<FeatureToggle>
    {
        IAmitalCloudContext currentContext;

        public FeatureToggleRepository(int tenant) : this(AmitalCloudContext.GetContext(tenant))
        {
        }
        public FeatureToggleRepository(IAmitalCloudContext context) : base(context)
        {
            currentContext = context;
        }
        public FeatureToggleRepository(IUnitOfWork uow) : base(uow)
        { }

        public IQueryable<FeatureToggle> GetAllByToggleCodeList(List<string> toggleCodes, int tenant)
        {
            return from a in DbSet
                   where a.Tenant == tenant && !a.Inactive && toggleCodes.Contains(a.ToggleCode)
                   select a;
        }



        public bool HasFeatureToggle(string toggleCode, int tenant)
        {
            string entityKeyString = $"HasFeatureToggle({toggleCode}, {tenant})";
            MyDummyClass myDummyClass = CacheManager.GetOrInsertNewObject<MyDummyClass>(entityKeyString, () =>
            {

                MyDummyClass myDummyClass1 = new MyDummyClass();
                myDummyClass1.MyBool = HasFeatureToggle_Slow(toggleCode, tenant);
                return myDummyClass1;
            });
            return myDummyClass?.MyBool ?? false;




        }
        /*public*/
        bool HasFeatureToggle_Slow(string toggleCode, int tenant)
        {
            bool result = false;
            string featureToggleName = "featuretoggle" + toggleCode + tenant;
            if (HttpContextHelper.HttpContext != null)
            {
                if (CacheManager.CacheWrapper.Get(featureToggleName) == null)
                {
                    using (TransactionScope scope = TransactionFactory.GetNewTransaction())
                    {

                        result = (from a in DbSet
                                  where a.ToggleCode == toggleCode
                                  && (a.TenantNumber == tenant || (tenant >= a.FromTenantNumber && tenant <= a.ToTenantNumber))
                                  && !a.Inactive
                                  select a).Any();

                        scope.Complete();
                    }

                    CacheManager.CacheWrapper.Insert(featureToggleName, result, null, System.DateTime.UtcNow.AddHours(12), TimeSpan.Zero);
                }
                else
                {
                    result = (bool)CacheManager.CacheWrapper.Get(featureToggleName);
                }

            }
            else
            {
                using (TransactionScope scope = TransactionFactory.GetNewTransaction())
                {

                    result = (from a in DbSet
                              where a.ToggleCode == toggleCode && (a.TenantNumber == tenant || (tenant >= a.FromTenantNumber && tenant <= a.ToTenantNumber)) && !a.Inactive
                              select a).Any();

                    scope.Complete();
                }

            }
            return result;
        }

    }

}
