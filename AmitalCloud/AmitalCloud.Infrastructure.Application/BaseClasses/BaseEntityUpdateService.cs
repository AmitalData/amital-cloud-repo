using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.DBHelpers;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Domain.Enums;
using AmitalCloud.Infrastructure.Domain.Helpers;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using AmitalCloud.Infrastructure.Model;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Model.Enums;
using AmitalCloud.Infrastructure.Model.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Transactions;
using System.Xml;
using System.Xml.Serialization;
using AutoMapper;

namespace AmitalCloud.Infrastructure.Application.BaseClasses
{
    public interface IBaseEntityUpdateService<TEntityPM> where TEntityPM : IEntityPM, new()
    {
        void InitializeEntityPM(TEntityPM entityPM);
        void Update(TEntityPM entityPM, bool commit, TimeSpan? transactionTimeout = null);
    }
    public abstract partial class BaseEntityUpdateService<TEntity, TEntityPM, TEntityParentPM, TEntityList, TkeyType> : IBaseEntityUpdateService<TEntityPM>
        where TEntity : class, new()
        where TEntityPM : IEntityPM, new()
        where TEntityParentPM : IEntityPM
        where TEntityList : class, new()
    {
        protected bool IsNewEntity;
        protected int Tenant;
        protected TEntity EntityPOCO { get; set; }
        protected TEntityPM OldEntityPM { get; set; }
        protected TEntityPM ChangeTrackingEntityPM { get; set; }
        protected string EntityChangeFieldXml { get; set; }
        private Dictionary<string, IContext> additionalContexts;
        protected Dictionary<string, IContext> AdditionalContexts
        {
            get { return additionalContexts; }
            set { additionalContexts = value; }
        }
        protected IMapping<TEntityPM, TEntity, TEntityList> Mapping;
        protected TEntityPM EntityPM;
        protected IRepository<TEntity> Repository;
        protected IContext MainContext;
        protected TEntityParentPM EntityParentPM;
        protected List<string> ErrorsList;
        protected bool ThrowValidationException;
        protected List<FieldChange> FieldChanges;

        public BaseEntityUpdateService() { }

        public BaseEntityUpdateService(IContext mainContext, Dictionary<string, IContext> additionalContexts, int tenant)
        {
            this.Tenant = tenant;
            this.AdditionalContexts = additionalContexts;
            this.MainContext = mainContext;
            this.ErrorsList = new List<string>();
            this.ThrowValidationException = true;

            FieldChanges = new List<FieldChange>();
        }
        public BaseEntityUpdateService(int tenant)
        {
            this.Tenant = tenant;
            this.AdditionalContexts = null;
            this.MainContext = GetContext(tenant);
            this.ErrorsList = new List<string>();
            this.ThrowValidationException = true;

            FieldChanges = new List<FieldChange>();
            //AuditLogRepository = new AuditLogRepository(tenant);
        }

        public void UpdateMulti(List<TEntityPM> entityPMList, List<TEntityPM> deletedEntityPMList, TEntityParentPM entityParentPM, bool commit)
        {
            try
            {
                EntityParentPM = entityParentPM;
                if (EntityParentPM.ChangeSetOp == ChangeSetOperation.Insert)
                {
                }

                foreach (TEntityPM entityPM in entityPMList)
                {
                    Update(entityPM, commit);
                }

                foreach (TEntityPM entityPM in deletedEntityPMList)
                {
                    Update(entityPM, commit);
                }
            }
            finally
            {

            }
        }

        public void Update(TEntityPM entityPM, bool commit, TimeSpan? transactionTimeout = null)
        {
            try
            {
                this.AddContext(entityPM);
                if (Transaction.Current != null && Transaction.Current.IsolationLevel != IsolationLevel.Snapshot)
                {
                    PerformUpdate(entityPM, commit);
                }
                else
                {
                    using (TransactionScope scope = TransactionFactory.GetTransaction(transactionTimeout))
                    {
                        PerformUpdate(entityPM, commit);
                        scope.Complete();
                    }
                }

            }
            finally
            {
                TraceLoadTest(this.GetDebugTrace());
                this.RemoveContext(entityPM);
            }
        }
        protected string GetLoggedUserid(int tenant)
        {
            string email = "system@tenant" + tenant + ".com";
            if (HttpContextHelper.HttpContext != null)
            {
                email = HttpContextHelper.User.Identity.Name;
            }
            Contact loggedContact = new Repository<Contact>(MainContext).GetMulti(a => a.Email == email && a.Tenant == tenant).FirstOrDefault();
            if (loggedContact != null)
            {
                return loggedContact.Id;
            }
            else
            {
                return "";
            }
        }
        private void PerformUpdate(TEntityPM entityPM, bool commit)
        {
            IEntityKeyFields<TEntity, TkeyType> entityKeys = GetKeys(entityPM);
            var iMapConvertFromBase64StringNVARCHARFields = Mapping as IMappingEncodeBase64NVARCHARFields<TEntityPM>;
            if (iMapConvertFromBase64StringNVARCHARFields != null)
            {
                iMapConvertFromBase64StringNVARCHARFields.EncodeBase64NVARCHARFields(entityPM);
            }
            this.EntityPM = entityPM;
            if (entityPM.ChangeSetOp != ChangeSetOperation.None)
            {
                switch (entityPM.ChangeSetOp)
                {
                    case ChangeSetOperation.Insert:
                        {
                            EntityPOCO = new TEntity();
                            FillDefaultValuesOnCreate(entityPM);
                            OnCreating(EntityPM, EntityParentPM);
                            break;
                        }
                    case ChangeSetOperation.Update:
                        {

                            EntityPOCO = Repository.GetSingle(entityKeys);
                            AddStepTrace("GeTEntity");
                            CheckConcurrency(entityPM, EntityPOCO);
                            AddStepTrace("CheckConcurrency");
                            OldEntityPM = new TEntityPM();
                            ChangeTrackingEntityPM = new TEntityPM();
                            var mapper = Mapping.CreateMapper();
                            mapper.Map(EntityPOCO, OldEntityPM);
                            mapper.Map(EntityPOCO, ChangeTrackingEntityPM);
                            mapper.Map(entityPM, ChangeTrackingEntityPM);
                            break;
                        }
                    case ChangeSetOperation.Delete:
                        {
                            EntityPOCO = Repository.GetSingle(entityKeys);

                            break;
                        }
                }
                AddStepTrace("B4OnUpdating");
                EntityChangeFieldXml = GetChangesDetectedXml(ChangeTrackingEntityPM);
                OnUpdating(entityPM);
                AddStepTrace("OnUpdating");
                OnUpdating(entityPM, EntityPOCO);
                AddStepTrace("OnUpdatingEntityPOCO");
                FillDefaultValuesOnUpdate(entityPM);
                AddStepTrace("FillDefaultValuesOnUpdate");
                string changesXml = ""; //GetChangesDetectedXml(ChangeTrackingEntityPM); to be done later on.

                Trace(entityPM, EntityPOCO, changesXml);
                AddStepTrace("Trace");
                Validate(entityPM);
                AddStepTrace("Validate");
                if (ErrorsList != null && ErrorsList.Count > 0)
                {
                    if (ThrowValidationException)
                    {
                        string errors = "";
                        foreach (string error in ErrorsList)
                        {
                            errors = errors + Environment.NewLine + error;
                        }
                        throw new Exception(errors);
                    }
                    else
                    {
                        return;
                    }
                }

                try
                {
                    // detach the tracked entity
                    var newTrackedEntity = MainContext.GetActiveDbContext().ChangeTracker.Entries<TEntity>().FirstOrDefault(e => entityKeys.Predicate.Compile().Invoke(e.Entity));
                    if (newTrackedEntity != null)
                    {
                        MainContext.GetActiveDbContext().Entry(newTrackedEntity.Entity).State = EntityState.Detached;
                    }
                    else
                    {
                        NetCommonHelper.Logger.DevLog.Instance.WriteWarning($"Not found tracked entity");
                    }

                    var mapper = Mapping.CreateMapper();
                    mapper.Map(EntityPM, EntityPOCO);
                }
                catch (AutoMapperMappingException ex)
                {
                    NetCommonHelper.Logger.DevLog.Instance.WriteError($"Failed to map the entity pm to poco: {ex.Message}");
                    throw;
                }

                AddStepTrace("Mapping");
                switch (entityPM.ChangeSetOp)
                {
                    case ChangeSetOperation.Insert:
                        {
                            Repository.Insert(EntityPOCO);
                            AddStepTrace("Insert");
                            break;
                        }
                    case ChangeSetOperation.Update:
                        {
                            Repository.Update(EntityPOCO);
                            AddStepTrace("Update");
                            break;
                        }
                    case ChangeSetOperation.Delete:
                        {
                            Repository.Delete(EntityPOCO);
                            AddStepTrace("Remove");
                            break;
                        }
                }

                UpdateComposition(entityPM);
                AddStepTrace("UpdateComposition");
                if (commit)
                {
                    SubmitChanges();
                }
                AfterUpdating(entityPM, EntityParentPM);
                AddStepTrace("AfterUpdating");
                UpdateCalculatedFields(entityPM, EntityParentPM, EntityPOCO);
                AddStepTrace("UpdateCalculatedFields");
            }
            //scope.Complete();
        }
        protected virtual void SubmitChanges()
        {
            try
            {
                MainContext.SaveChanges();
                //todo implement Unit of Work pattern
            }
            catch (DbUpdateException e)
            {
                throw;
            }
            if (AdditionalContexts != null)
            {
                foreach (IContext context in AdditionalContexts.Values)
                {
                    context.GetType().GetMethod("SaveChanges").Invoke(context, null);
                }
            }
        }
        protected virtual void OnCreating(TEntityPM entityPM, TEntityParentPM entityParentPM)
        {
        }
        protected virtual void FillDefaultValuesOnCreate(TEntityPM entityPM)
        {
        }
        protected virtual void FillDefaultValuesOnUpdate(TEntityPM entityPM)
        {
        }
        protected virtual void UpdateComposition(TEntityPM entityPM)
        {

        }
        protected virtual void OnUpdating(TEntityPM entityPM)
        {

        }
        protected virtual void AfterUpdating(TEntityPM entityPM, TEntityParentPM entityParentPM)
        {

        }
        protected virtual void UpdateCalculatedFields(TEntityPM entityPM, TEntityParentPM entityParentPM, TEntity entityPOCO)
        {

        }
        protected abstract IEntityKeyFields<TEntity, TkeyType> GetKeys(TEntityPM entityPM);
        protected virtual void Trace(TEntityPM entityPM, TEntity entityPOCO, string changesXml)
        {

        }
        protected virtual void TraceLoadTest(string LoadTestLog)
        {

        }
        protected virtual void OnUpdating(TEntityPM entityPM, TEntity entityPOCO)
        {

        }
        private string GetChangesDetectedXml(TEntityPM changesTrackingEntityPM)
        {
            string xml = null;

            if (changesTrackingEntityPM != null)
            {
                Root root = new Root();
                root.Changes = new List<Change>();
                foreach (NotifyPropertyChangeValues value in changesTrackingEntityPM.ChangedProperties)
                {
                    string oldValue = value.OldValue != null ? value.OldValue.ToString() : "";
                    string newValue = value.NewValue != null ? value.NewValue.ToString() : "";

                    if (value.PropertyType == "CustomFieldClass")
                    {
                        if (value.NewValue != null)
                        {
                            object newFieldValue = value.NewValue;
                            if (newFieldValue.GetType() == typeof(CustomFieldClass))
                            {
                                CustomFieldClass classvalue = newFieldValue as CustomFieldClass;
                                newValue = !string.IsNullOrEmpty(classvalue.Value) ? classvalue.Value : "";
                            }
                        }
                        if (value.OldValue != null)
                        {
                            object oldFieldValue = value.OldValue;
                            if (oldFieldValue.GetType() == typeof(CustomFieldClass))
                            {
                                CustomFieldClass classvalue = oldFieldValue as CustomFieldClass;
                                oldValue = !string.IsNullOrEmpty(classvalue.Value) ? classvalue.Value : "";
                            }
                        }
                    }

                    oldValue = this.RemoveInvalidXmlChars(oldValue);
                    newValue = this.RemoveInvalidXmlChars(newValue);

                    Change change = new Change()
                    {
                        fieldName = value.PropertyName,
                        newValue = newValue,
                        oldValue = oldValue,
                    };
                    root.Changes.Add(change);
                }

                xml = SerializeObjectToXml<Root>(root);
            }

            return xml;
        }
        private string RemoveInvalidXmlChars(string text)
        {
            var validXmlChars = text.Where(ch => XmlConvert.IsXmlChar(ch)).ToArray();
            return new string(validXmlChars);
        }


        private string SerializeObjectToXml<T>(T dataObject)
        {
            MemoryStream memstream = new MemoryStream();

            XmlSerializer ser = new XmlSerializer(typeof(T));

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "http://www.champ.aero/GCCS/CargoXML");
            XmlWriterSettings settings = new XmlWriterSettings()
            {
                Indent = true,
                IndentChars = "",
                OmitXmlDeclaration = true,
                NewLineChars = "",
                NewLineHandling = NewLineHandling.Replace,
            };

            XmlWriter writer = XmlTextWriter.Create(memstream, settings);
            writer.WriteRaw("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>\r\n");
            ser.Serialize(writer, dataObject, ns);
            memstream.Seek(0, SeekOrigin.Begin);
            var reader = new StreamReader(memstream);
            string content = reader.ReadToEnd();
            return content;
        }

        protected virtual void CheckConcurrency(TEntityPM entityPM, TEntity entityPOCO)
        {

        }



        public bool DontAddTransaction { get; set; }//if you don't want to add a transaction to the update service. update()

        protected virtual void Validate(TEntityPM entityPM)
        {
        }

        public virtual void InitializeUpdateService()
        {

        }

        public virtual void InitializeEntityPM(TEntityPM entityPM)
        {

        }
        private IContext GetContext(int tenant) => DbContextBaseUtil.GetContext<TEntity>(tenant);
    }




}
