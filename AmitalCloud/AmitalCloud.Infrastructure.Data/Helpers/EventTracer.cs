using AmitalCloud.Infrastructure.Data.BlobServiceReference;
using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Transactions;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public class EventTracer
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EventTracer(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetUsername()
        {
            return _httpContextAccessor.HttpContext?.User?.Identity?.Name;
        }

        public static void CreateTraceEvent(EventTracerArgs args)
        {
            if (string.IsNullOrEmpty(args.EventTypeCode))
            {
                return;
            }
            int tenant = args.Tenant;
            using (var uow = new UnitOfWork<AmitalCloudContext>(tenant))
            {
                EventTypeRepository eventTypeRepository = new EventTypeRepository(uow);
                EventType eventType = eventTypeRepository.GetSingleEventTypeByCode(args.EventTypeCode, tenant);
                if (eventType == null)
                {
                    throw new Exception("Event Type is not recognized:" + args.EventTypeCode);
                }
                else
                {
                    ObjectTableRepository objectTabelRepository = new ObjectTableRepository(uow);
                    ObjectTable objectTable = objectTabelRepository.GetObjectTableByName(args.ObjectTableName, 0, true);
                    ObjectTable childObjectTable = objectTabelRepository.GetObjectTableByName(args.ChildObjectTableName, 0, true);
                    #region User
                    string myUserId = null;
                    string myCustomerCareUserEmail = null;
                    if (!string.IsNullOrEmpty(args.UserId))
                    {
                        myUserId = args.UserId;
                        if (tenant != 0)
                        {
                            // todo: not working because static function doesnot work with DI
                            // string email = GetUsername();
                            string email = null;
                            if (!string.IsNullOrEmpty(email))
                            {
                                UserRepository userRepository = new UserRepository(uow);
                                User user = userRepository.GetSingleUserByEmail(email, 0, true);
                                if (user != null)
                                {
                                    User systemUser = userRepository.GetSingleUserByEmail("system@tenant" + tenant + ".com", tenant, true);
                                    if (systemUser != null)
                                    {
                                        myUserId = systemUser.Id;
                                    }
                                    myCustomerCareUserEmail = user.Contact.Email;
                                }
                            }

                        }
                    }
                    #endregion
                    #region Dates
                    if (args.LogDateTime == null)
                    {
                        args.LogDateTime = TenantServerConfigration.GetCurrentDateTime(tenant);
                    }

                    else if (args.LogDateTime.Value.Year == 1)
                    {
                        args.LogDateTime = TenantServerConfigration.GetCurrentDateTime(tenant);
                    }

                    if (args.EventDateTime == null)
                    {
                        args.EventDateTime = TenantServerConfigration.GetCurrentDateTime(tenant);
                    }

                    else if (args.EventDateTime.Value.Year == 1)
                    {
                        args.EventDateTime = TenantServerConfigration.GetCurrentDateTime(tenant);
                    }

                    if (args.EventTypeCode == "CWOP" || args.EventTypeCode == "CLOP" || args.EventTypeCode == "CCOP")
                    {
                        args.LogDateTime = args.EventDateTime;
                    }

                    #endregion

                    #region Status Notes
                    if (args.IsAddedManually)
                    {
                        if (!string.IsNullOrEmpty(args.NewStatusId) && !string.IsNullOrEmpty(args.CurrentStatusId))
                        {
                            EntityStatus newStatus = EntityStatusRepository.GetSingleEntityStatus(args.NewStatusId, tenant, true);
                            EntityStatus currentStatus = EntityStatusRepository.GetSingleEntityStatus(args.CurrentStatusId, tenant, true);

                            Contact user = ContactRepository.GetSingleContact(myUserId, tenant, true);
                            if (newStatus != null)
                            {
                                args.Notes = "Status was changed manually from " + currentStatus.Name + " to " + newStatus.Name + " by " + (user != null ? user.EnglishName : "");
                            }
                        }
                    }
                    #endregion

                    string myNotes = args.Notes;
                    if (myNotes != null)
                    {
                        if (AmitalCloudSettings.DatabaseManagementSystem == "oracle")
                        {
                            if (myNotes.Length > 2000)
                            {
                                myNotes = myNotes.Substring(0, 1999);
                            }
                        }

                        else
                        {
                            if (myNotes.Length > 4000)
                            {
                                myNotes = myNotes.Substring(0, 3999);
                            }
                        }
                    }

                    TraceEvent myTraceEvent = new TraceEvent()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Tenant = tenant,
                        EntityId = args.EntityId,
                        EventTypeId = eventType.Id,
                        ObjectTableId = objectTable.Id,
                        LogDateTime = args.LogDateTime.Value,
                        EventDateTime = args.EventDateTime.Value,
                        ExternalId = args.ExternalId?.Trim(),
                        IsAddedManually = args.IsAddedManually,
                        UserId = myUserId,
                        CustomerCareUserEmail = myCustomerCareUserEmail,
                        Notes = myNotes,
                        Location = null,
                        ChildEntityId = args.ChildEntityId,
                        ChildObjectTableId = childObjectTable?.Id,
                    };
                    TraceEventRepository traceEventRepository = new TraceEventRepository(uow);
                    uow.CreateTransactionScope(TransactionScopeOption.Required);
                    traceEventRepository.Insert(myTraceEvent);
                    if (eventType.IsCustomerView)
                    {
                        ContactsUnseenEntitiesHelper.AddUnseenEntityRecord(myTraceEvent.Id, tenant, uow);
                    }

                    if (!string.IsNullOrEmpty(eventType.CustomField) && objectTable.AllowCustomFields)
                    {
                        UpdateEventCustomFieldValue(new UpdateEventCustomFieldArgs() { CustomField = eventType.CustomField, EventDateTime = myTraceEvent.EventDateTime, Entity = args.Entity, EntityId = args.EntityId, ObjectTableName = args.ObjectTableName, Tenant = args.Tenant }, uow);
                    }
                    uow.Save();
                    uow.Commit();
                    //if ((Transaction.Current != null && Transaction.Current.IsolationLevel == System.Transactions.IsolationLevel.Snapshot)
                    //    || (Transaction.Current == null && dbms != "oracle"))
                    //{
                    //    using (var scope = objectContext.GetSnapshotTransaction())
                    //    {


                    //        TraceEventRepository traceEventRepository = new TraceEventRepository(uow);
                    //        traceEventRepository.Insert(myTraceEvent);
                    //        //traceEventRepository.SubmitChanges();
                    //        //objectContext.SaveChanges();
                    //        //scope.Commit();
                    //    }
                    //}
                    //else
                    //{
                    //    TraceEventRepository traceEventRepository = new TraceEventRepository(objectContext);
                    //    traceEventRepository.Insert(myTraceEvent);
                    //    traceEventRepository.SubmitChanges();
                    //    objectContext.SaveChanges();
                    //}
                }
            }
        }
        public static Response CreateTraceEventsList(List<TraceEventParams> traceEventParamsList, int tenant, string objectTableName, string currentStatusId, string param = null)
        {
            Response response = new Response();
            using (var uow = new UnitOfWork<AmitalCloudContext>(tenant))
            {
                EventTypeRepository eventTypesRepository = new EventTypeRepository(uow);
                TraceEventRepository traceEventsRepository = new TraceEventRepository(uow);
                ObjectTableRepository objectTableRepository = new ObjectTableRepository(uow);
                var entityStatusRepository = new Repository<EntityStatus>(uow);
                ObjectTable objectTable = objectTableRepository.GetObjectTableByName(objectTableName, 0, true);
                List<EventType> eventTypes = eventTypesRepository.GetEventTypesByTenantAndObjectTableId(tenant, objectTable.Id).ToList();
                List<EntityStatus> statusList = entityStatusRepository.GetMulti(a => a.Tenant == tenant).ToList();
                string newStatusId = null;
                string lastStatusId = currentStatusId;
                foreach (TraceEventParams traceEventParams in traceEventParamsList)
                {
                    TraceEvent traceEvent = new TraceEvent();
                    traceEvent.Id = Guid.NewGuid().ToString();
                    EventType eventType = eventTypes.Where(e => e.Code == traceEventParams.EventTypeCode).FirstOrDefault();

                    if (eventType == null)
                    {
                        response.HasError = true;
                        response.ErrorMessage = "EventTypeCode:" + traceEventParams.EventTypeCode + " doesn't exist in the database,Upsert this entity before using it," + Environment.NewLine;
                        return response;
                    }

                    if (traceEventParams.Tenant != 0)
                    {
                        UserRepository userRepository = new UserRepository(tenant);
                        User user = userRepository.GetSingleUser(traceEventParams.UserId, 0, false);
                        if (user != null)
                        {

                            User systemUser = userRepository.GetSingleUserByCodeOrEmail(null, "system@tenant" + traceEventParams.Tenant + ".com", traceEventParams.Tenant, true);
                            if (systemUser != null)
                            {
                                traceEventParams.UserId = systemUser.Id;
                            }
                            traceEvent.CustomerCareUserEmail = user.Contact.Email;
                        }
                    }

                    traceEvent.UserId = traceEventParams.UserId;
                    traceEvent.EntityId = traceEventParams.EntityId;
                    if (traceEventParams.LogDate != null)
                    {
                        traceEvent.LogDateTime = traceEventParams.LogDate.Value;
                    }
                    else
                    {
                        traceEvent.LogDateTime = TenantServerConfigration.GetCurrentDateTime(traceEventParams.Tenant);
                    }

                    if (traceEventParams.EventDate != null)
                    {
                        traceEvent.EventDateTime = traceEventParams.EventDate.Value;
                    }

                    if (traceEvent.EventDateTime.Year == 1)
                    {
                        traceEvent.EventDateTime = TenantServerConfigration.GetCurrentDateTime(traceEventParams.Tenant);
                    }

                    traceEvent.Notes = traceEventParams.Notes;
                    traceEvent.Tenant = traceEventParams.Tenant;
                    traceEvent.ObjectTableId = objectTable.Id;
                    traceEvent.EventTypeId = eventType.Id;
                    traceEvent.ExternalId = traceEventParams.ExternalId?.Trim();

                    if (traceEventParams.Manually)
                    {
                        EntityStatus currentStatus = statusList.Where(s => s.Id == traceEventParams.CurrentStatusId).FirstOrDefault();
                        EntityStatus newStatus = statusList.Where(s => s.Id == traceEventParams.NewStatusId).FirstOrDefault();
                        Contact user = new Repository<Contact>(uow).GetMulti(a => a.Id == traceEventParams.UserId).FirstOrDefault();
                        if (newStatus != null)
                            traceEvent.Notes = "Status were changed manually from " + currentStatus.Name + " to " + newStatus.Name + " by " + (user != null ? user.EnglishName : "");
                        traceEvent.IsAddedManually = true;
                    }
                    traceEventsRepository.Insert(traceEvent);
                    if (!traceEventParams.Manually)
                    {
                        DateTime? statusDate = null;
                        newStatusId = GetNewStatusId(lastStatusId, eventType.EntityStatusId, traceEventParams.Tenant, ref statusDate, statusList);
                    }
                    else
                    {
                        newStatusId = traceEventParams.NewStatusId;
                    }
                    lastStatusId = newStatusId;
                    if (traceEvent != null)
                    {
                        if (eventType != null && eventType.IsCustomerView)
                        {
                            ContactsUnseenEntitiesHelper.AddUnseenEntityRecord(traceEvent.Id, traceEventParams.Tenant, uow);
                        }
                    }
                }
                uow.Save();
                uow.Commit();
                response.Result = newStatusId;
                return response;
            }
        }

        public static string GetNewStatusId(string currentStatusId, string newStatusId, int tenant, ref DateTime? statusDate, List<EntityStatus> statusList)
        {
            EntityStatus currentStatus = statusList.Where(s => s.Id == currentStatusId).FirstOrDefault();
            EntityStatus newStatus = statusList.Where(s => s.Id == newStatusId).FirstOrDefault();

            if (currentStatus == null)
            {
                statusDate = TenantServerConfigration.GetCurrentDateTime(tenant);
                return newStatusId;
            }

            else if (newStatus == null)
            {
                return currentStatusId;
            }

            string id = null;

            if (currentStatus.StatusWeight > newStatus.StatusWeight)
            {
                id = currentStatus.Id;
            }

            else
            {
                statusDate = TenantServerConfigration.GetCurrentDateTime(tenant);
                id = newStatus.Id;
            }

            return id;
        }
        public static void DeleteTraceEvent(string eventTypeCode, int tenant, string objectTableName, string entityId)
        {
            if (string.IsNullOrEmpty(eventTypeCode)) return;
            using (var uow = new UnitOfWork<AmitalCloudContext>(tenant))
            {
                TraceEventRepository traceEventRepository = new TraceEventRepository(uow);
                ObjectTable objectTable = new ObjectTableRepository(uow).GetObjectTableByName(objectTableName, 0, true);
                string objectTableId = objectTable.Id;
                List<EventType> allEventTypes = new EventTypeRepository(uow).GetEventTypesByTenantAndObjectTableId(tenant, objectTableId).ToList();
                EventType eventType = allEventTypes.Where(d => d.Code == eventTypeCode).FirstOrDefault();
                if (eventType != null)
                {
                    List<TraceEvent> AllEventTraces = traceEventRepository.GetAllTraceEventsByEventType(entityId, eventType.Id, tenant).ToList();
                    if (AllEventTraces.Count > 0)
                    {
                        foreach (TraceEvent iTraceEvent in AllEventTraces)
                        {
                            iTraceEvent.Deleted = true;
                            traceEventRepository.Update(iTraceEvent);
                        }

                    }
                }
                uow.Save();
                uow.Commit();
            }
        }
        #region UpdateEventCustomFieldValue
        private static void UpdateEventCustomFieldValue(UpdateEventCustomFieldArgs args, IUnitOfWork uow)
        {
            ObjectField objectField = GetCustomObjectFieldConnectedToEvent(args.CustomField, args.EventTypeId, args.Tenant, uow);
            if (objectField != null)
            {
                object entity = args.Entity;
                CustomFieldClass customFieldValue = new CustomFieldClass(objectField.FieldName, args.ObjectTableName, new CustomFieldClass().SetFieldDataType(objectField.DataTypeCode, args.EventDateTime));
                if (entity == null)
                {
                    entity = InjectionUtil.Instance.GetEntityByObjectTableNameAndEntityId(args.ObjectTableName, args.EntityId, args.Tenant);
                    if (entity != null)
                    {
                        SetPropertyValueToEntity(objectField, entity, customFieldValue);
                        InjectionUtil.Instance.UpdateEntity(entity, args.ObjectTableName, args.Tenant);
                    }
                }
                else SetPropertyValueToEntity(objectField, entity, customFieldValue);
            }
        }
        private static ObjectField GetCustomObjectFieldConnectedToEvent(string customFieldName, string eventTypeId, int tenant, IUnitOfWork uow)
        {
            ObjectField objectField = null;
            string customField = !string.IsNullOrEmpty(customFieldName) ? customFieldName : new EventTypeRepository(uow).GetCustomFieldByEventTypeId(eventTypeId, tenant);
            if (!string.IsNullOrEmpty(customField))
            {
                objectField = new ObjectFieldRepository(uow).GetSingleObjectFieldByFieldCode(customField, tenant);
            }
            return objectField;
        }
        private static void SetPropertyValueToEntity(ObjectField objectField, object entity, object fieldValue)
        {
            PropertyInfo propInfo = entity.GetType().GetProperty(objectField.FieldName);
            if (propInfo != null) propInfo.SetValue(entity, fieldValue, null);
        }
        #endregion
    }

    public class TraceEventParams
    {
        public string EventTypeCode { get; set; }
        public int Tenant { get; set; }
        public string EntityId { get; set; }
        public string UserId { get; set; }
        public string Notes { get; set; }
        public string ObjectTableName { get; set; }
        public string CurrentStatusId { get; set; }
        public string NewStatusId { get; set; }
        public bool Manually { get; set; }
        public DateTime? StatusDate { get; set; }
        public DateTime? LogDate { get; set; }
        public DateTime? EventDate { get; set; }
        public string ExternalId { get; set; }
    }


    public class EventTracerArgs
    {
        public int Tenant { get; set; }
        public string Notes { get; set; }
        public string UserId { get; set; }
        public string EntityId { get; set; }
        public string EventTypeCode { get; set; }
        public string ObjectTableName { get; set; }
        public bool IsAddedManually { get; set; }
        public DateTime? LogDateTime { get; set; }
        public DateTime? EventDateTime { get; set; }
        public string ExternalId { get; set; }
        public string NewStatusId { get; set; }
        public string CurrentStatusId { get; set; }
        public object Entity { get; set; }
        public string ChildEntityId { get; set; }
        public string ChildObjectTableName { get; set; }
    }

    public class UpdateEventCustomFieldArgs
    {
        public int Tenant { get; set; }
        public string EntityId { get; set; }
        public string ObjectTableName { get; set; }
        public object Entity { get; set; }
        public string CustomField { get; set; }
        public DateTime EventDateTime { get; set; }
        public string EventTypeId { get; set; }



    }


}
