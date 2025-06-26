using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Counters;
using AmitalCloud.Infrastructure.Data.EntityDataMappings;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.EntityKeys;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AmitalCloud.Infrastructure.Model.Interfaces;
namespace AmitalCloud.Infrastructure.Data.Services
{
    public class CustomChildObjectService
    {

        bool isNewEntity;
        private int tenant;
        //public CustomChildObject Poco { get; set; }
        private bool isChange = false;
        public IAmitalCloudContext ObjectContext
        {
            get { return objectContext; }
            set { objectContext = value; }
        }

        private CustomChildObjectPM entityPM;
        private IAmitalCloudContext objectContext;
        //private IRepository<IAmitalCloudContext,CustomChildObject,string> entityRepository;
        private Contact loggedContact;
        public CustomChildObjectService(int tenant)
        {
            this.tenant = tenant;
            this.isChange = false;
            //this.entityRepository = new Repository<CustomChildObject, string>(objectContext);
            this.GetLoggedContact();

        }
        //public CustomChildObjectService(IAmitalCloudContext objectContext, int tenant)
        //{
        //    this.tenant = tenant;
        //    this.isChange = false;
        //    this.ObjectContext = objectContext;
        //    this.entityRepository = new Repository<CustomChildObject, string>(objectContext);
        //    this.GetLoggedContact();

        //}

        public void Create(CustomChildObjectPM theEntityPm, Repository<CustomChildObject> repo)
        {
            this.isNewEntity = true;
            this.entityPM = theEntityPm;
            this.entityPM.Id = IdCounter.GetNumber("CustomChildObject", tenant).ToString();
            this.entityPM.CreateDate = TenantServerConfigration.GetCurrentDateTime(entityPM.Tenant);
            this.entityPM.UpdateDate = TenantServerConfigration.GetCurrentDateTime(entityPM.Tenant);
            this.entityPM.UpdatedBy = this.loggedContact != null ? this.loggedContact.Id : this.entityPM.UpdatedBy;
            this.entityPM.CreatedBy = this.loggedContact != null ? this.loggedContact.Id : this.entityPM.CreatedBy;
            var Poco = new CustomChildObject();
           // CustomChildObjectMapping.MapEntity(theEntityPm, Poco, isNewEntity);
            repo.Insert(Poco);
            this.isChange = true;

        }

        public void Update(CustomChildObjectPM theEntityPm, Repository<CustomChildObject> repo)
        {
            this.isNewEntity = false;
            this.entityPM = theEntityPm;
            this.entityPM.UpdateDate = TenantServerConfigration.GetCurrentDateTime(entityPM.Tenant);
            this.entityPM.UpdatedBy = this.loggedContact != null ? this.loggedContact.Id : this.entityPM.UpdatedBy;

            var Poco = repo.GetSingle(new CustomChildObjectKeys<string>() { Id = theEntityPm.Id });   //, entityPM.Tenant);
            if (Poco == null) return;
            //CustomChildObjectMapping.MapEntity(theEntityPm, Poco, isNewEntity);
            repo.Update(Poco);
            this.isChange = true;
        }


        public void Delete(CustomChildObjectPM theEntityPm, Repository<CustomChildObject> repo)
        {
            var Poco = repo.GetSingle(new CustomChildObjectKeys<string>() { Id = theEntityPm.Id }); //    GetById(, theEntityPm.Tenant);
            if (Poco == null) return;
            repo.Delete(Poco);
            this.isChange = true;
        }

        public void Updates(List<CustomChildObjectPM> customChildObjects)
        {
            using (var uow = new UnitOfWork<AmitalCloudContext>(tenant))
            {
                var repo = new Repository<CustomChildObject>(uow);
                customChildObjects.ForEach((customChildObject) =>
                {
                    switch (customChildObject.ChangeSetOp)
                    {
                        case Domain.Enums.ChangeSetOperation.Insert: { Create(customChildObject, repo); break; }
                        case Domain.Enums.ChangeSetOperation.Update: { Update(customChildObject, repo); break; }
                        case Domain.Enums.ChangeSetOperation.Delete: { Delete(customChildObject, repo); break; }
                        default: { break; }
                    }
                });

                if (!this.isChange) return;
                uow.Save();
            }
        }

        private void GetLoggedContact()
        {
            if (!string.IsNullOrEmpty(HttpContextHelper.User?.Identity?.Name))
            {
                this.loggedContact = new Repository<Contact>(this.ObjectContext).GetMulti(d => d.Tenant == tenant && d.Email == HttpContextHelper.User.Identity.Name).FirstOrDefault();   //GetSingleContactByEmail((HttpContext.Current.User.Identity.Name), tenant, true);
                return;
            }

            this.loggedContact = new Repository<Contact>(this.ObjectContext).GetMulti(d => d.Tenant == tenant && d.Email == "system@tenant" + tenant.ToString() + ".com").FirstOrDefault();//.GetSingleContactByEmail(("system@tenant" + tenant.ToString() + ".com"), tenant, true);
        }


    }
}