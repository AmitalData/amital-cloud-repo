using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Transactions;
using AmitalCloud.Infrastructure.Model.Interfaces;

namespace AmitalCloud.Infrastructure.Data.Repositories
{
    public class ObjectFieldRepository : Repository<ObjectField>
    {
        IAmitalCloudContext currentContext;
        public ObjectFieldRepository(IAmitalCloudContext context) : base(context)
        {
            currentContext = context;
        }

        public ObjectFieldRepository(int tenant) : this(AmitalCloudContext.GetContext(tenant))
        {
        }
        public ObjectFieldRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }


        public IQueryable<ObjectField> GetObjectFields()
        {
            return context.ObjectFields;
        }
        public IQueryable<ObjectField> GetObjectFields(int tenant)
        {
            return context.ObjectFields.Where(t => t.Tenant == tenant);
        }
        public static List<ObjectField> GetObjectFieldsByObjectTableName(string objectTableName, int tenant)
        {
            string zerolistName = "tabletenantzeroobjectfields" + objectTableName.ToLower();
            string tenantListName = "tabletenantobjectfields" + objectTableName.ToLower() + tenant;
            List<ObjectField> result = new List<ObjectField>();
            List<ObjectField> currentTenantObjectFields = new List<ObjectField>();
            List<ObjectField> zeroTenantObjectFields = new List<ObjectField>();
            #region Current Tenant Fields

            if (tenant != 0)
            {

                if (Helpers.CacheManager.CacheWrapper.Get(tenantListName) == null)
                {

                    using (TransactionScope scope = Helpers.TransactionFactory.GetNewTransaction())
                    {
                        IAmitalCloudContext context = AmitalCloudContext.GetContext(tenant);
                        currentTenantObjectFields = (from a in context.ObjectFields.Include("ObjectTable_LookUpTable").Include("FullNameTextCode").Include("ShortNameTextCode").Include("ListTextCode").Include("HelpTextCode").Include("ObjectTable")
                                                     where (a.Tenant == tenant) && a.ObjectTable.Name == objectTableName && a.InActive == false
                                                     select a).ToList();

                        scope.Complete();
                    }



                    Helpers.CacheManager.CacheWrapper.Insert(tenantListName, currentTenantObjectFields, null, System.DateTime.UtcNow.AddHours(12), TimeSpan.Zero);
                }
                else
                {
                    currentTenantObjectFields = (List<ObjectField>)Helpers.CacheManager.CacheWrapper.Get(tenantListName);
                }


            }


            #endregion
            #region Tenant Zero Fields


            if (Helpers.CacheManager.CacheWrapper.Get(zerolistName) == null)
            {

                using (TransactionScope scope = Helpers.TransactionFactory.GetNewTransaction())
                {
                    IAmitalCloudContext context = AmitalCloudContext.GetContext(tenant);
                    zeroTenantObjectFields = (from a in context.ObjectFields.Include("ObjectTable_LookUpTable").Include("FullNameTextCode").Include("ShortNameTextCode").Include("ListTextCode").Include("HelpTextCode").Include("ObjectTable")
                                              where (a.Tenant == 0) && a.ObjectTable.Name == objectTableName && a.InActive == false
                                              select a).ToList();

                    scope.Complete();
                }



                Helpers.CacheManager.CacheWrapper.Insert(zerolistName, zeroTenantObjectFields, null, System.DateTime.UtcNow.AddHours(12), TimeSpan.Zero);
            }
            else
            {
                zeroTenantObjectFields = (List<ObjectField>)Helpers.CacheManager.CacheWrapper.Get(zerolistName);
            }


            #endregion
            result = zeroTenantObjectFields.Concat(currentTenantObjectFields).ToList();
            return result;
        }
        public static List<ObjectField> GetObjectFieldsByObjectTableNameWithNoIncludes(string objectTableName, int tenant)
        {
            string zerolistName = "tabletenantzeroobjectfields" + objectTableName.ToLower();
            string tenantListName = "tabletenantobjectfields" + objectTableName.ToLower() + tenant;
            List<ObjectField> result = new List<ObjectField>();
            List<ObjectField> currentTenantObjectFields = new List<ObjectField>();
            List<ObjectField> zeroTenantObjectFields = new List<ObjectField>();
            #region Current Tenant Fields

            if (tenant != 0)
            {

                if (Helpers.CacheManager.CacheWrapper.Get(tenantListName) == null)
                {

                    using (TransactionScope scope = Helpers.TransactionFactory.GetNewTransaction())
                    {
                        IAmitalCloudContext context = AmitalCloudContext.GetContext(tenant);
                        var TableId = (from a in context.ObjectTables
                                       where a.Name == objectTableName
                                       select a.Id).FirstOrDefault();
                        currentTenantObjectFields = (from a in context.ObjectFields//.Include("ObjectTable")//.Include("ObjectTable_LookUpTable").Include("FullNameTextCode").Include("ShortNameTextCode").Include("ListTextCode").Include("HelpTextCode").Include("ObjectTable")
                                                     where (a.Tenant == tenant) && a.ObjectTableId == TableId && a.InActive == false
                                                     select a).ToList();

                        scope.Complete();
                    }



                    Helpers.CacheManager.CacheWrapper.Insert(tenantListName, currentTenantObjectFields, null, System.DateTime.UtcNow.AddHours(12), TimeSpan.Zero);
                }
                else
                {
                    currentTenantObjectFields = (List<ObjectField>)Helpers.CacheManager.CacheWrapper.Get(tenantListName);
                }


            }


            #endregion
            #region Tenant Zero Fields


            if (Helpers.CacheManager.CacheWrapper.Get(zerolistName) == null)
            {

                using (TransactionScope scope = Helpers.TransactionFactory.GetNewTransaction())
                {
                    IAmitalCloudContext context = AmitalCloudContext.GetContext(tenant);
                    var TableId = (from a in context.ObjectTables
                                   where a.Name == objectTableName
                                   select a.Id).FirstOrDefault();
                    zeroTenantObjectFields = (from a in context.ObjectFields//.Include("ObjectTable")//.Include("ObjectTable_LookUpTable").Include("FullNameTextCode").Include("ShortNameTextCode").Include("ListTextCode").Include("HelpTextCode").Include("ObjectTable")
                                              where (a.Tenant == 0) && a.ObjectTableId == TableId && a.InActive == false
                                              select a).ToList();

                    scope.Complete();
                }



                Helpers.CacheManager.CacheWrapper.Insert(zerolistName, zeroTenantObjectFields, null, System.DateTime.UtcNow.AddHours(12), TimeSpan.Zero);
            }
            else
            {
                zeroTenantObjectFields = (List<ObjectField>)Helpers.CacheManager.CacheWrapper.Get(zerolistName);
            }


            #endregion
            result = zeroTenantObjectFields.Concat(currentTenantObjectFields).ToList();
            return result;
        }
        public List<ObjectField> GetCustomObjectFields(string objectTableId, int tenant)
        {
            return context.ObjectFields.Where(t => t.Tenant == tenant && t.ObjectTableId == objectTableId && t.IsCustom == true).ToList();
        }
        public ObjectField GetSingleObjectField(string id, int tenant)
        {
            return (from a in context.ObjectFields
                    where a.Id == id
                    select a).FirstOrDefault();
        }
        public string GetObjectFieldCodeById(string id, int tenant)
        {
            return (from a in context.ObjectFields
                    where a.Id == id && a.Tenant == tenant
                    select a.FieldCode).FirstOrDefault();
        }
        public List<ObjectField> GetAutomationObjectFieldsByObjectTableId(string objectTableId, int tenant)
        {

            string zerolistName = "tabletenantzeroAutomationConditionsObjectFields" + objectTableId.ToLower();
            string tenantListName = "tabletenantAutomationConditionsObjectFields" + objectTableId.ToLower() + tenant;
            List<ObjectField> result = new List<ObjectField>();
            List<ObjectField> currentTenantObjectFields = new List<ObjectField>();
            List<ObjectField> zeroTenantObjectFields = new List<ObjectField>();
            #region Current Tenant Fields

            if (tenant != 0)
            {

                if (Helpers.CacheManager.CacheWrapper.Get(tenantListName) == null)
                {

                    using (TransactionScope scope = Helpers.TransactionFactory.GetNewTransaction())
                    {

                        currentTenantObjectFields = (from a in context.ObjectFields.Include("ObjectTable_LookUpTable").Include("FullNameTextCode").Include("ShortNameTextCode").Include("ListTextCode").Include("HelpTextCode").Include("ObjectTable")
                                                     where (a.Tenant == tenant || (a.DisplayInAutomationAsEnitity == true && a.Tenant == 0)) && a.ObjectTableId == objectTableId && a.InActive == false && (a.AllowedinAutomationConditions == true || a.CanAutomateSetValue == true || a.DisplayInAutomationAsEnitity == true || a.AutomationEmailRecipient == true || a.IsCustom || a.FieldName == "DescriptionOfGoods" || a.FieldName == "MainCarriageFinalDestinationETA" || a.FieldName == "MainCarriageFinalDestinationATA" || a.FieldName == "MainCarriageETD" || a.FieldName == "MainCarriageATD")
                                                     select a).ToList();
                        currentTenantObjectFields = currentTenantObjectFields.Concat(GetEntityAutomationObjectFields(tenant, currentTenantObjectFields)).ToList();
                        scope.Complete();
                    }

                    Helpers.CacheManager.CacheWrapper.Insert(tenantListName, currentTenantObjectFields, null, System.DateTime.UtcNow.AddHours(12), TimeSpan.Zero);
                }
                else
                {
                    currentTenantObjectFields = (List<ObjectField>)Helpers.CacheManager.CacheWrapper.Get(tenantListName);
                }


            }


            #endregion
            #region Tenant Zero Fields


            if (Helpers.CacheManager.CacheWrapper.Get(zerolistName) == null)
            {

                using (TransactionScope scope = Helpers.TransactionFactory.GetNewTransaction())
                {
                    zeroTenantObjectFields = (from a in context.ObjectFields.Include("ObjectTable_LookUpTable").Include("FullNameTextCode").Include("ShortNameTextCode").Include("ListTextCode").Include("HelpTextCode").Include("ObjectTable")
                                              where (a.Tenant == 0) && a.ObjectTableId == objectTableId && a.InActive == false && (a.AllowedinAutomationConditions == true || a.CanAutomateSetValue == true || a.DisplayInAutomationAsEnitity == true || a.AutomationEmailRecipient == true || a.IsCustom || a.FieldName == "DescriptionOfGoods" || a.FieldName == "MainCarriageFinalDestinationETA" || a.FieldName == "MainCarriageFinalDestinationATA" || a.FieldName == "MainCarriageETD" || a.FieldName == "MainCarriageATD")
                                              select a).ToList();
                    zeroTenantObjectFields = zeroTenantObjectFields.Concat(GetEntityAutomationObjectFields(0, zeroTenantObjectFields)).ToList();

                    scope.Complete();
                }



                Helpers.CacheManager.CacheWrapper.Insert(zerolistName, zeroTenantObjectFields, null, System.DateTime.UtcNow.AddHours(12), TimeSpan.Zero);
            }
            else
            {
                zeroTenantObjectFields = (List<ObjectField>)Helpers.CacheManager.CacheWrapper.Get(zerolistName);
            }


            #endregion
            result = zeroTenantObjectFields.Union(currentTenantObjectFields).ToList();
            return result;
        }
        private List<ObjectField> GetEntityAutomationObjectFields(int tenant, List<ObjectField> currentTenantObjectFields)
        {
            List<ObjectField> automationEntityObjectFieldLists = new List<ObjectField>();
            if (currentTenantObjectFields != null && currentTenantObjectFields.Count > 0)
            {
                List<string> entityAutomationObjectTableIds = currentTenantObjectFields.Where(d => d.DisplayInAutomationAsEnitity && !string.IsNullOrEmpty(d.LookUpTableId)).GroupBy(d => d.LookUpTableId).Select(d => d.First().LookUpTableId).ToList();
                if (entityAutomationObjectTableIds.Count > 0)
                {
                    automationEntityObjectFieldLists = (from a in context.ObjectFields.Include("ObjectTable_LookUpTable").Include("FullNameTextCode").Include("ShortNameTextCode").Include("ListTextCode").Include("HelpTextCode").Include("ObjectTable")
                                                        where (a.Tenant == tenant) && entityAutomationObjectTableIds.Contains(a.ObjectTableId) && a.InActive == false && (a.AllowedinAutomationConditions == true || a.CanAutomateSetValue == true || a.DisplayInAutomationAsEnitity == true || a.AutomationEmailRecipient == true || a.IsCustom || a.FieldName == "DescriptionOfGoods" || a.FieldName == "MainCarriageFinalDestinationETA" || a.FieldName == "MainCarriageFinalDestinationATA" || a.FieldName == "MainCarriageETD" || a.FieldName == "MainCarriageATD")
                                                        select a).ToList();
                }
            }
            return automationEntityObjectFieldLists;
        }
        public static List<ObjectField> GetCustomObjectFieldsByObjectTableName(string objectTableName, int tenant)
        {
            string objectFieldsListName = objectTableName.ToLower() + "customobjectfields" + tenant;
            List<ObjectField> objectfields = new List<ObjectField>();
            if (Helpers.CacheManager.CacheWrapper.Get(objectFieldsListName) == null)
            {
                IAmitalCloudContext context = AmitalCloudContext.GetContext(tenant);
                objectfields = (from a in context.ObjectFields.Include("ObjectTable_LookUpTable").Include("FullNameTextCode").Include("ShortNameTextCode").Include("ListTextCode").Include("HelpTextCode").Include("ObjectTable")
                                where a.Tenant == tenant && a.ObjectTable.Name == objectTableName && a.IsCustom == true && a.InActive == false
                                select a).ToList();
                Helpers.CacheManager.CacheWrapper.Insert(objectFieldsListName, objectfields, null, System.DateTime.UtcNow.AddHours(12), TimeSpan.Zero);
            }
            else
            {
                objectfields = (List<ObjectField>)Helpers.CacheManager.CacheWrapper.Get(objectFieldsListName);
            }
            return objectfields;
        }
        public IQueryable<ObjectField> GetPMObjectFieldsByObjectTableName(string objectTableName, int tenant)
        {
            IQueryable<ObjectField> objectfields = from a in context.ObjectFields.Include("ObjectTable_LookUpTable").Include("FullNameTextCode").Include("ShortNameTextCode").Include("ListTextCode").Include("HelpTextCode").Include("ObjectTable")
                                                   where a.Tenant == tenant && a.ObjectTable.Name == objectTableName && a.DisplayInList == false && a.IsCustomFilter == false && a.InActive == false
                                                   select a;
            return objectfields;
        }
        public List<ObjectField> GetPMObjectFieldsByObjectTableId(string objectTableId, int tenant)
        {
            IQueryable<ObjectField> objectfields = from a in context.ObjectFields
                                                   where a.ObjectTableId == objectTableId
                                                   select a;
            return objectfields.ToList();
        }
        public IQueryable<ObjectField> GetObjectFieldsByTenant(int tenant)
        {
            return from a in context.ObjectFields//.Include("ListFieldLableTextCode").Include("FieldLableTextCode").Include("ObjectTable")
                   where a.Tenant == tenant
                   select a;
        }
        public ObjectField GetSingleObjectField(string id)
        {
            return (from a in context.ObjectFields
                    where a.Id == id
                    select a).FirstOrDefault();
        }
        public ObjectField GetSingleObjectFieldByObjectFieldCode(string objectFieldCode)
        {
            string key = $"GetSingleObjectFieldByObjectFieldCode({objectFieldCode})";
            return Helpers.CacheManager.GetOrInsertNewObject<ObjectField>(key, () =>
            {
                return GetSingleObjectFieldByObjectFieldCodeReal(objectFieldCode);
            });
        }
        ObjectField GetSingleObjectFieldByObjectFieldCodeReal(string objectFieldCode)
        {
            return (from a in context.ObjectFields
                    where a.FieldCode == objectFieldCode
                    select a).FirstOrDefault();
        }
        public ObjectFieldModification GetObjectFieldModificationByObjectField(string objectfieldCode, int tenant)
        {
            return (from a in context.ObjectFieldModifications
                    where a.Tenant == tenant && a.ObjectFieldCode == objectfieldCode
                    select a).FirstOrDefault();
        }
        public ObjectFieldModification GetLastObjectFieldModificationByTenant(int tenant)
        {
            Repository<ObjectFieldModification> objectFieldModificationRepo = new Repository<ObjectFieldModification>(context);

            var latestUpdate = objectFieldModificationRepo
                .GetQueryable()
                .AsNoTracking()
                .Where(a => a.Tenant == tenant && a.UpdateDateGMT != null)
                .OrderByDescending(a => a.UpdateDateGMT)
                .FirstOrDefault();

            return latestUpdate;
        }
        public List<ObjectFieldModification> GetAllObjectFieldModificationByTenant(int tenant)
        {
            return (from a in context.ObjectFieldModifications
                    where a.Tenant == tenant
                    select a).ToList();
        }
        public IAmitalCloudContext context
        {
            get { return currentContext; }
        }
        public ObjectField GetSingleObjectFieldById(string id, int tenant, bool includeMetaDataFields = false)
        {
            if (includeMetaDataFields)
            {
                var myField = (from a in context.ObjectFields.Include("FullNameTextCode").Include("ListTextCode")
                               where a.Id == id
                               select a).FirstOrDefault();
                if (myField == null)
                    myField = (from a in context.ObjectFields.Include("FullNameTextCode").Include("ListTextCode")
                               where a.FieldCode == id
                               select a).FirstOrDefault();
                return myField;
            }
            var field = (from a in context.ObjectFields.Include("FullNameTextCode").Include("ListTextCode")
                         where a.Id == id
                         select a).FirstOrDefault();
            if (field == null)
                field = (from a in context.ObjectFields.Include("FullNameTextCode").Include("ListTextCode")
                         where a.FieldCode == id
                         select a).FirstOrDefault();
            return field;
        }
        public ObjectField GetSingleObjectFieldByCode(string code, int tenant)
        {
            return (from a in context.ObjectFields.Include("FullNameTextCode").Include("ListTextCode")
                    where a.Code == code
                    select a).FirstOrDefault();
        }
        public ObjectField GetSingleObjectFieldByFieldCode(string fieldCode, int tenant)
        {
            return (from a in context.ObjectFields.Include("FullNameTextCode")
                    where a.FieldCode == fieldCode
                    select a).FirstOrDefault();
        }
        public ObjectField GetSingleObjectFieldByCode(string code, string objectTableId, int tenant)
        {
            return (from a in context.ObjectFields
                    where a.Code == code && a.ObjectTableId == objectTableId && a.Tenant == tenant
                    select a).FirstOrDefault();
        }
        public ObjectField GetSingleObjectFieldByFieldName(string fieldName, string objectTableId, int tenant)
        {
            return (from a in context.ObjectFields
                    where a.FieldName == fieldName && a.ObjectTableId == objectTableId
                    select a).FirstOrDefault();
        }
        public IQueryable<ObjectField> GetObjectFieldsFromTenanZeroAndMyTenant(int tenant, bool IncludeMetaDataFields = false)
        {
            if (IncludeMetaDataFields)
            {
                return context.ObjectFields.Include("FullNameTextCode").Include("ListTextCode").Where(t => (t.Tenant == tenant || t.Tenant == 0) && t.FieldName.ToLower() != "tenant");
            }
            return context.ObjectFields.Include("FullNameTextCode").Include("ListTextCode").Where(t => t.Tenant == tenant || t.Tenant == 0);
        }
        public IQueryable<ObjectField> GetDigitalObjectFieldsFromTenanZeroAndMyTenant(int tenant, bool IncludeMetaDataFields = false)
        {
            if (IncludeMetaDataFields)
            {
                return context.ObjectFields.Include("FullNameTextCode").Include("ListTextCode")
                   .Where(t => ((t.Tenant == 0 && !t.FieldName.ToLower().StartsWith("Field")) || (t.Tenant == tenant && t.IsCustom == true))
                                   && t.FieldName.ToLower() != "tenant");
            }
            return context.ObjectFields.Include("FullNameTextCode").Include("ListTextCode")
                .Where(t => (t.Tenant == 0 && !t.FieldName.ToLower().StartsWith("Field")) || (t.Tenant == tenant && t.IsCustom == true));
        }
        public IQueryable<ObjectField> GetObjectFieldsByFieldsNamesAndObjectTable(List<string> fieldsNames, string objectTableId)
        {
            return from a in context.ObjectFields
                   where a.ObjectTableId == objectTableId && fieldsNames.Contains(a.FieldName) && a.Tenant == 0
                   select a;

        }
        public ObjectField GetObjectFieldByName(string name, string objectTableId, int tenant)
        {
            ObjectField field;
            string fieldCacheKey = "ObjectField" + name + tenant;

            if (Helpers.CacheManager.CacheWrapper.Get(fieldCacheKey) == null)
            {
                field = GetObjectFieldFromDatabase(name, objectTableId, tenant);

                if (field != null)
                {
                    InsertObjectFieldIntoCache(field, fieldCacheKey);
                }
            }
            else
            {
                field = GetObjectFieldFromCache(fieldCacheKey);

            }
            return field;
        }
        private ObjectField GetObjectFieldFromDatabase(string name, string objectTableId, int tenant)
        {
            var field = context.ObjectFields.Where(d => d.FieldName == name && d.ObjectTableId == objectTableId
                 && (d.Tenant == tenant || d.Tenant == 0)).FirstOrDefault();
            return field;
        }
        private void InsertObjectFieldIntoCache(ObjectField field, string fieldCacheKey)
        {
            Helpers.CacheManager.CacheWrapper.Insert(fieldCacheKey, field, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
        }
        private ObjectField GetObjectFieldFromCache(string fieldCacheKey)
        {
            return (ObjectField)Helpers.CacheManager.CacheWrapper.Get(fieldCacheKey);
        }
        public bool IsExistsCustomObjectFieldByCode(string code, string objectTableId, int tenant)
        {
            return (from a in context.ObjectFields
                    where a.Code == code && a.ObjectTableId == objectTableId && a.Tenant == tenant && a.IsCustom == true
                    select a).Any();
        }
        public int GetCustomObjectFieldCountByCodeAndCopies(string code, string objectTableId, int tenant)
        {
            return (from a in context.ObjectFields
                    where (a.Code == code || (a.Code.Contains(code) && a.Code.Contains("Copy"))) && a.ObjectTableId == objectTableId && a.Tenant == tenant && a.IsCustom == true
                    select a).Count();
        }
    }
}
