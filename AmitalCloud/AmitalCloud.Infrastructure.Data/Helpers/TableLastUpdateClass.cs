using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Counters;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System;
using System.Linq;
using System.Web;
using AmitalCloud.Infrastructure.Model.Interfaces;

namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public class TableLastUpdateM
    {
        public string ObjectTableId { get; set; }
        public int AlternativeUserTenant { get; set; }
        public string AlternativeUserId { get; set; }

    }
    public static class TableLastUpdateClass
    {

        public static void UpdateTableHistory(int tenant, string tableName
            , TableLastUpdateM tableLastUpdateM = null)
        {
            using (var uow = new UnitOfWork<AmitalCloudContext>(tenant))
            {
                User loggedUser = null;
                ObjectTable entityObjectTable = null;
                IRepository<ObjectTableLastUpdate> tableLastUpdateRepository = new Repository<ObjectTableLastUpdate>(uow);
                ObjectTableRepository objectTabelRepository = new ObjectTableRepository(uow);
                IRepository<User> userRepository = new Repository<User>(uow);
                entityObjectTable = objectTabelRepository.GetObjectTableByName(tableName, tenant, true);
                tableName = tableName ?? "";
                if (entityObjectTable == null && tableName.EndsWith("s", StringComparison.OrdinalIgnoreCase))
                {
                    var tableWithoutS = tableName.Substring(0, Math.Max(0, tableName.Length - 1));
                    entityObjectTable = objectTabelRepository.GetObjectTableByName(tableName, tenant, true);
                }
                loggedUser = loggedUser ?? GetUser(tenant, tableLastUpdateM);
                entityObjectTable = entityObjectTable ?? GetObjectTable(tenant, tableName, tableLastUpdateM, (IAmitalCloudContext)uow.Context);
                if (loggedUser == null)
                {
                    if (HttpContextHelper.User != null)
                    {
                        if (!string.IsNullOrEmpty(HttpContextHelper.User.Identity.Name))
                        {
                            loggedUser = userRepository.GetMulti(d => d.Tenant == tenant && d.Contact.Email == HttpContextHelper.User.Identity.Name).FirstOrDefault();                //GetSingleUserByEmail(HttpContextHelper.User.Identity.Name, tenant, true);
                        }
                    }
                }
                if (entityObjectTable != null && !entityObjectTable.CacheOnClient)
                {
                    return;
                }
                if (entityObjectTable != null && loggedUser != null)
                {
                    ObjectTableLastUpdateUpsert(tenant, loggedUser, entityObjectTable, tableLastUpdateRepository);
                    uow.Save();
                }
            }
        }
        private static void ObjectTableLastUpdateUpsert(int tenant, User loggedContact, ObjectTable entityObjectTable, IRepository<ObjectTableLastUpdate> tableLastUpdateRepository)
        {
            ObjectTableLastUpdate tableLastUpdate = tableLastUpdateRepository.GetMulti(d => d.Id == entityObjectTable.Id && d.Tenant == tenant).FirstOrDefault();
            if (tableLastUpdate != null)
            {
                tableLastUpdate.LastUpdateDate = DateTime.UtcNow;
                tableLastUpdate.UpdatedByUserId = loggedContact.Id;
                tableLastUpdateRepository.Update(tableLastUpdate);
            }
            else
            {
                tableLastUpdate = new ObjectTableLastUpdate()
                {
                    Id = IdCounter.GetNumber("ObjectTableLastUpdate", tenant),
                    Tenant = tenant,
                    LastUpdateDate = DateTime.UtcNow,
                    ObjectTableId = entityObjectTable.Id,
                    UpdatedByUserId = loggedContact.Id,
                };
                tableLastUpdateRepository.Insert(tableLastUpdate);
            }
        }
        private static ObjectTable GetObjectTable(int tenant, string tableName, TableLastUpdateM tableLastUpdateM, IAmitalCloudContext context)
        {
            ObjectTableRepository objectTabelRepository = new ObjectTableRepository(context);
            ObjectTable entityObjectTable = null;
            if (tableLastUpdateM != null && !string.IsNullOrWhiteSpace(tableLastUpdateM.ObjectTableId))
            {
                entityObjectTable = objectTabelRepository.GetObjectTableById(tableLastUpdateM.ObjectTableId, tenant);
            }
            if (entityObjectTable == null)
            {
                entityObjectTable = objectTabelRepository.GetObjectTableByName(tableName, tenant, true);
            }
            return entityObjectTable;
        }
        private static User GetUser(int tenant, TableLastUpdateM tableLastUpdateM)
        {
            IAmitalCloudContext context = AmitalCloudContext.GetContext(tenant);
            IRepository<Contact> contactRepository = new Repository<Contact>(context);
            IRepository<User> userRepository = new Repository<User>(context);
            User loggedUser = null;

            if (tableLastUpdateM != null && !String.IsNullOrWhiteSpace(tableLastUpdateM.AlternativeUserId))
            {
                var email = contactRepository.GetMulti(a => a.Tenant == tableLastUpdateM.AlternativeUserTenant && a.Id == tableLastUpdateM.AlternativeUserId).FirstOrDefault();    //GetEmailContactByIdAndTenant(tableLastUpdateM.AlternativeUserTenant, tableLastUpdateM.AlternativeUserId);
                loggedUser = GetSingleUserByEmail(tableLastUpdateM.AlternativeUserTenant, userRepository, email.Email);
            }
            if (loggedUser == null)
            {
                if (HttpContextHelper.User != null &&
                    !string.IsNullOrEmpty(HttpContextHelper.User.Identity.Name))
                {
                    loggedUser = GetSingleUserByEmail(tenant, userRepository, HttpContextHelper.User.Identity.Name);// userRepository.GetSingleUserByEmail(HttpContextHelper.User.Identity.Name, tenant);
                }
                else
                {
                    loggedUser = GetSingleUserByEmail(tenant, userRepository, "system@tenant" + tenant + ".com"); //userRepository.GetSingleUserByEmail("system@tenant" + tenant + ".com", tenant);
                }
            }
            return loggedUser;
        }

        private static User GetSingleUserByEmail(int tenant, IRepository<User> userRepository, string email)
        {
            return userRepository.GetMulti(a => a.Contact.Email == email && a.Tenant == tenant).FirstOrDefault();
            //GetSingleUserByEmail(email, tableLastUpdateM.AlternativeUserTenant);
        }
        //private static Contact GetContact(int tenant, TableLastUpdateM tableLastUpdateM)
        //{
        //    ContactRepository contactRepository = new ContactRepository(tenant);
        //    Contact loggedContact = null;
        //    if (tableLastUpdateM != null && !String.IsNullOrWhiteSpace(tableLastUpdateM.AlternativeUserId))
        //    {
        //        var email = contactRepository.GetEmailContactByIdAndTenant(tableLastUpdateM.AlternativeUserTenant, tableLastUpdateM.AlternativeUserId);
        //        loggedContact = contactRepository.GetSingleContactByEmail(email, tableLastUpdateM.AlternativeUserTenant);
        //        var maybeCostomerCare = true;
        //        if (maybeCostomerCare && loggedContact == null)// mayby customer care 
        //        {
        //            email = contactRepository.GetEmailContactByIdAndTenant(0, tableLastUpdateM.AlternativeUserId);
        //            loggedContact = contactRepository.GetSingleContactByEmail(email, 0);
        //        }
        //    }
        //    if (loggedContact == null)
        //    {
        //        loggedContact = contactRepository.GetSingleContactByEmail(AuthenticationUtil.GetAuthenticatedUser(), tenant);//In Worker Role Can Crash !!!
        //    }
        //    return loggedContact;
        //}
        //public static void UpdateTableHistory(int tenant, string tableName, IAmitalCloudContext context)
        //{
        //    ObjectTableLastUpdateRepository tableLastUpdateRepository = new ObjectTableLastUpdateRepository(context);
        //    ObjectTableRepository objectTabelRepository = new ObjectTableRepository(context);
        //    ContactRepository contactRepository = new ContactRepository(tenant);
        //    if (HttpContextHelper != null && HttpContextHelper.User != null)
        //    {
        //        if (!string.IsNullOrEmpty(HttpContextHelper.User.Identity.Name))
        //        {
        //            User loggedContact = GetUser(tenant, null);//contactRepository.GetSingleContactByEmail(HttpContextHelper.User.Identity.Name, tenant);
        //            ObjectTable entityObjectTable = objectTabelRepository.GetObjectTableByName(tableName, tenant, true);
        //            if (entityObjectTable != null && loggedContact != null && entityObjectTable.CacheOnClient)
        //            {
        //                ObjectTableLastUpdate tableLastUpdate = tableLastUpdateRepository.GetSingleObjectTableLastUpdate(entityObjectTable.Id, tenant);
        //                if (tableLastUpdate != null)
        //                {
        //                    tableLastUpdate.LastUpdateDate = DateTime.UtcNow;
        //                    tableLastUpdate.UpdatedByUserId = loggedContact.Id;
        //                    tableLastUpdateRepository.Update(tableLastUpdate);
        //                }
        //                else
        //                {
        //                    tableLastUpdate = new ObjectTableLastUpdate()
        //                    {
        //                        Id = IdCounter.GetNumber("ObjectTableLastUpdate", tenant),
        //                        Tenant = tenant,
        //                        LastUpdateDate = DateTime.UtcNow,
        //                        ObjectTableId = entityObjectTable.Id,
        //                        UpdatedByUserId = loggedContact.Id,
        //                    };
        //                    tableLastUpdateRepository.Add(tableLastUpdate);
        //                }
        //                tableLastUpdateRepository.SubmitChanges();
        //            }
        //        }
        //    }
        //}
        //public static void UpdateSystemMetaDataHistory(bool updateFields = true, bool updateTranslations = true)
        //{
        //    using (TransactionScope scope = TransactionFactory.GetNewTransaction())
        //    {
        //        SystemMetadataLastUpdateRepository systemMetdaDataRep = new SystemMetadataLastUpdateRepository();
        //        SystemMetadataLastUpdate lastUpdates = systemMetdaDataRep.GetSingleSystemMetadataLastUpdate("1");
        //        if (lastUpdates != null)
        //        {
        //            if (updateFields)
        //                lastUpdates.ObjectFieldsUpdateDateGMT = DateTime.UtcNow;
        //            if (updateTranslations)
        //                lastUpdates.TranslationsUpdateDateGMT = DateTime.UtcNow;

        //            systemMetdaDataRep.Update(lastUpdates);
        //        }
        //        else
        //        {
        //            lastUpdates = new SystemMetadataLastUpdate()
        //            {
        //                Id = "1",
        //                ObjectFieldsUpdateDateGMT = DateTime.UtcNow,
        //                TranslationsUpdateDateGMT = DateTime.UtcNow,
        //            };

        //            systemMetdaDataRep.Add(lastUpdates);
        //        }

        //        systemMetdaDataRep.SubmitChanges();

        //        scope.Complete();
        //    }
        //}


        //public static void UpdateAllClosedTablesHistory()
        //{
        //    IAmitalCloudContext context = AmitalCloudContext.GetContext(0);
        //    ObjectTableLastUpdateRepository tableLastUpdateRepository = new ObjectTableLastUpdateRepository(context);
        //    ObjectTableRepository objectTabelRepository = new ObjectTableRepository(context);
        //    ContactRepository contactRepository = new ContactRepository(0);

        //    List<ObjectTable> objectTablesList = objectTabelRepository.GetObjectsByTenant(0).Where(t=>t.IsClosed).ToList();
        //    Contact loggedContact = contactRepository.GetSingleContactByEmail("system@tenant0.com", 0);

        //    foreach (ObjectTable table in objectTablesList)
        //    {

        //        ObjectTableLastUpdate tableLastUpdate = tableLastUpdateRepository.GetSingleObjectTableLastUpdate(table.Id, 0);
        //        if (tableLastUpdate != null)
        //        {
        //            tableLastUpdate.LastUpdateDate = DateTime.UtcNow;
        //            tableLastUpdate.UpdatedByUserId = loggedContact.Id;

        //            tableLastUpdateRepository.Update(tableLastUpdate);
        //        }
        //        else
        //        {

        //            tableLastUpdate = new ObjectTableLastUpdate()
        //            {
        //                Id = IdCounter.GetNumber("ObjectTableLastUpdate", 0),
        //                Tenant = 0,
        //                LastUpdateDate = DateTime.UtcNow,
        //                ObjectTableId = table.Id,
        //                UpdatedByUserId = loggedContact.Id,

        //            };

        //            tableLastUpdateRepository.Add(tableLastUpdate);
        //        }



        //    }

        //    tableLastUpdateRepository.SubmitChanges();


        //}
    }
}
