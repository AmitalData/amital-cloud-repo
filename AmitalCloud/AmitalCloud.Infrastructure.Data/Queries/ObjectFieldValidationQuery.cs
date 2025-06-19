using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using AmitalCloud.Infrastructure.Model.Interfaces;

namespace AmitalCloud.Infrastructure.Data.Queries
{
    public class ObjectFieldValidationQuery
    {
        IRepository<ObjectFieldValidation> repository;

        public ObjectFieldValidationQuery(int tenant)
        {
            repository = new Repository<ObjectFieldValidation>(tenant);
        }

        public ObjectFieldValidationQuery(IRepository<ObjectFieldValidation> objectFieldValidationRepository)
        {
            repository = objectFieldValidationRepository;
        }

        public ObjectFieldValidationPM GetSinglePM(string id, int tenant)
        {
            ObjectFieldValidationPM objectFieldValidationPm = (from a in repository.GetMulti(a =>
                                                                a.Tenant == tenant && a.Id == id)
                                                               select new ObjectFieldValidationPM()
                                                               {

                                                                   Id = a.Id,
                                                                   Tenant = a.Tenant,
                                                                   ErrorMessage = a.ErrorMessage,
                                                                   ValidationExpression = a.ValidationExpression,
                                                                   ObjectFieldId = a.ObjectFieldId,
                                                                   ValidationOrder = a.ValidationOrder,
                                                                   Condition = a.Condition,
                                                                   Code = a.Code,
                                                                   ObjectFieldCode = a.ObjectFieldCode,
                                                               }).FirstOrDefault();




            return objectFieldValidationPm;
        }

        public ObjectFieldValidationPM GetSingleObjectFieldValidationPMById(string id, int tenant)
        {
            ObjectFieldValidationPM objectFieldValidationPm = (from a in repository.GetMulti(a =>
                                                                a.Tenant == tenant && a.Id == id)
                                                               select new ObjectFieldValidationPM()
                                                               {

                                                                   Id = a.Id,
                                                                   Tenant = a.Tenant,
                                                                   ErrorMessage = a.ErrorMessage,
                                                                   ValidationExpression = a.ValidationExpression,
                                                                   ObjectFieldId = a.ObjectFieldId,
                                                                   ValidationOrder = a.ValidationOrder,
                                                                   Condition = a.Condition,
                                                                   Code = a.Code,
                                                                   ObjectFieldCode = a.ObjectFieldCode,
                                                               }).FirstOrDefault();
            return objectFieldValidationPm;
        }



        public IQueryable<ObjectFieldValidationPM> GetObjectFieldValidationPMsByTenant(int tenant)
        {
            IQueryable<ObjectFieldValidationPM> objectFieldValidationPMs = (from a in repository.GetMulti(a => a.Tenant == tenant || a.Tenant == 0)
                                                                            select new ObjectFieldValidationPM()
                                                                            {

                                                                                Id = a.Id,
                                                                                Tenant = a.Tenant,
                                                                                ErrorMessage = a.ErrorMessage,
                                                                                ValidationExpression = a.ValidationExpression,
                                                                                ObjectFieldId = a.ObjectFieldId,
                                                                                ValidationOrder = a.ValidationOrder,
                                                                                Condition = a.Condition,
                                                                                Code = a.Code,
                                                                                ObjectFieldCode = a.ObjectFieldCode,
                                                                            }).AsQueryable();
            return objectFieldValidationPMs;
        }

        public List<ObjectFieldValidationPM> GetObjectFieldValidationPMsByObjectFieldCode(string objectFieldCode, int tenant)
        {
            List<ObjectFieldValidationPM> tenantZeroQuery;
            List<ObjectFieldValidationPM> currentTenantQuery;
            List<ObjectFieldValidationPM> query = new List<ObjectFieldValidationPM>();
            string listName = "objectfieldvalidationpms" + objectFieldCode + tenant;
            List<ObjectTableRule> selectedRules = new List<ObjectTableRule>();
            if (CacheManager.CacheWrapper.Get(listName) == null)
            {
                using (TransactionScope scope = TransactionFactory.GetNewTransaction())
                {
                    //    AmitalCloudContext  amitalCloudContext = (AmitalCloudContext)AmitalCloudContext.GetContext(0);
                    tenantZeroQuery = (from a in repository.GetMulti(a => (a.Tenant == 0) && a.ObjectFieldCode == objectFieldCode)
                                       select new ObjectFieldValidationPM()
                                       {

                                           Id = a.Id,
                                           Tenant = a.Tenant,
                                           ErrorMessage = a.ErrorMessage,
                                           ValidationExpression = a.ValidationExpression,
                                           ObjectFieldId = a.ObjectFieldId,
                                           ValidationOrder = a.ValidationOrder,
                                           Condition = a.Condition,
                                           Code = a.Code,
                                           ObjectFieldCode = a.ObjectFieldCode,
                                       }).ToList();


                }

                using (TransactionScope scope = TransactionFactory.GetNewTransaction())
                {
                    //  AmitalCloudContext amitalCloudContext = (AmitalCloudContext)AmitalCloudContext.GetContext(tenant);
                    currentTenantQuery = (from a in repository.GetMulti(a => (a.Tenant == tenant) && a.ObjectFieldCode == objectFieldCode)
                                          select new ObjectFieldValidationPM()
                                          {

                                              Id = a.Id,
                                              Tenant = a.Tenant,
                                              ErrorMessage = a.ErrorMessage,
                                              ValidationExpression = a.ValidationExpression,
                                              ObjectFieldId = a.ObjectFieldId,
                                              ValidationOrder = a.ValidationOrder,
                                              Condition = a.Condition,
                                              Code = a.Code,
                                              ObjectFieldCode = a.ObjectFieldCode,
                                          }).ToList();

                }
                query = tenantZeroQuery.Concat(currentTenantQuery).ToList();
                CacheManager.CacheWrapper.Insert(listName, query, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
            }
            else
            {
                query = (List<ObjectFieldValidationPM>)CacheManager.CacheWrapper.Get(listName);
            }


            return query;
        }


        public static List<ObjectFieldValidationPM> GetObjectFieldValidationPMs(int tenant)
        {
            List<ObjectFieldValidationPM> tenantZeroQuery;
            List<ObjectFieldValidationPM> currentTenantQuery = new List<ObjectFieldValidationPM>();
            List<ObjectFieldValidationPM> query = new List<ObjectFieldValidationPM>();
            string listName = "objectfieldvalidationpms" + tenant;
            List<ObjectTableRule> selectedRules = new List<ObjectTableRule>();
            if (CacheManager.CacheWrapper.Get(listName) == null)
            {
                using (TransactionScope scope = TransactionFactory.GetNewTransaction())
                {
                    IAmitalCloudContext context = AmitalCloudContext.GetContext(tenant);
                    tenantZeroQuery = (from a in context.ObjectFieldValidations
                                       where (a.Tenant == 0)
                                       select new ObjectFieldValidationPM()
                                       {

                                           Id = a.Id,
                                           Tenant = a.Tenant,
                                           ErrorMessage = a.ErrorMessage,
                                           ValidationExpression = a.ValidationExpression,
                                           ObjectFieldId = a.ObjectFieldId,
                                           ValidationOrder = a.ValidationOrder,
                                           Condition = a.Condition,
                                           Code = a.Code,
                                           ObjectFieldCode = a.ObjectFieldCode,
                                       }).ToList();


                }

                if (tenant != 0)
                {
                    using (TransactionScope scope = TransactionFactory.GetNewTransaction())
                    {
                        IAmitalCloudContext context = AmitalCloudContext.GetContext(tenant);
                        currentTenantQuery = (from a in context.ObjectFieldValidations
                                              where (a.Tenant == tenant)
                                              select new ObjectFieldValidationPM()
                                              {

                                                  Id = a.Id,
                                                  Tenant = a.Tenant,
                                                  ErrorMessage = a.ErrorMessage,
                                                  ValidationExpression = a.ValidationExpression,
                                                  ObjectFieldId = a.ObjectFieldId,
                                                  ValidationOrder = a.ValidationOrder,
                                                  Condition = a.Condition,
                                                  Code = a.Code,
                                                  ObjectFieldCode = a.ObjectFieldCode,
                                              }).ToList();

                    }
                }
                query = tenantZeroQuery.Concat(currentTenantQuery).ToList();
                CacheManager.CacheWrapper.Insert(listName, query, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
            }
            else
            {
                query = (List<ObjectFieldValidationPM>)CacheManager.CacheWrapper.Get(listName);
            }


            return query;
        }

    }
}