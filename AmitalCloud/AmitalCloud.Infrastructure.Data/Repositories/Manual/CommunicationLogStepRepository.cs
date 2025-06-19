using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Helpers;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System;
using System.Collections.Generic;

using System.Linq;
using AmitalCloud.Infrastructure.Model.Interfaces;

namespace AmitalCloud.Infrastructure.Data.Repositories
{
    public class CommunicationLogStepRepository : Repository<CommunicationLogStep>, IRepository<CommunicationLogStep>
    {
        IAmitalCloudContext currentContext;


        public CommunicationLogStepRepository(IAmitalCloudContext context) : base(context)
        {
            currentContext = context;
        }

        public CommunicationLogStepRepository(int tenant) : this(AmitalCloudContext.GetContext(tenant))
        {
        }
        public CommunicationLogStepRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }

        public IQueryable<CommunicationLogStep> GetCommunicationSteps(int tenant)
        {
            return (from record in context.CommunicationLogSteps where record.Tenant == tenant select record);
        }




        public CommunicationLogStep CommunicationLogStep(string id, int StepNumber, int tenant)
        {
            return (from a in context.CommunicationLogSteps
                        .Include("CommunicationStatusType")
                        .Include("Document")
                    where a.CommunicationLogId == id && a.StepNumber == StepNumber && a.Tenant == tenant
                    select a).FirstOrDefault();

        }


        public IQueryable<CommunicationLogStep> GetCommunicationStepsForCommLog(string comLogId, int tenant)
        {
            return (from a in context.CommunicationLogSteps.Include("CommunicationStatusType").Include("Document")
                    where a.Tenant == tenant && a.CommunicationLogId == comLogId
                    select a);
        }


        new public void Insert(CommunicationLogStep entity)
        {
            CompressUpSert(entity);
            base.Insert(entity);
        }

        private static void CompressUpSert(CommunicationLogStep entity)
        {
            if (!entity.IsLogCompress && !String.IsNullOrWhiteSpace(entity.Log))
            {

                entity.Log = InjectionUtil.Instance.CompressText(entity.Log);
                entity.IsLogCompress = true;
            }
        }



        new public void Update(CommunicationLogStep entity)
        {
            try
            {
                CompressUpSert(entity);

                base.Update(entity);

            }
            catch
            { }

        }

        public List<CommunicationLogStep> All()
        {
            return context.CommunicationLogSteps.ToList();
        }

        public IAmitalCloudContext context
        {
            get { return currentContext; }
        }




        public List<CommunicationLogStep> GetMultiCommunicationLog(string id, int tenant)
        {
            return (from a in context.CommunicationLogSteps
                       .Include("CommunicationStatusType")
                       .Include("Document")
                    where a.CommunicationLogId == id
                    && a.Tenant == tenant
                    orderby a.StepNumber //MUST !!!
                    select a).ToList();

        }

        public IQueryable<CommunicationLogStep> GetQMultiCommunicationLog(string id, int tenant)
        {
            return (from a in context.CommunicationLogSteps
                       .Include("CommunicationStatusType")
                       .Include("Document")
                    where a.CommunicationLogId == id &&
                    a.Tenant == tenant
                    orderby a.StepNumber //MUST !!!
                    select a);

        }
        public IQueryable<string> Get104921()
        {
            //25 - נובמבר - 2019
            //104921 ==select  count(*) from CommunicationLogs where createdate_> sysdate -10
            DateTime sdateTime = new DateTime(2019, 11, 25);
            DateTime edateTime = new DateTime(2019, 12, 05);
            var q =
            context.CommunicationLogs
                .Where(r => r.CreateDate > sdateTime)
                .Where(r => r.CreateDate <= edateTime)
                ///..Take(1000*200)
                //104921 ==select  count(*) from CommunicationLogs where createdate_> sysdate -10
                .Select(r => r.Id);
            return q;
        }
    }

}
