using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.EntityLists;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System.Linq;
using AmitalCloud.Infrastructure.Model.Interfaces;

namespace AmitalCloud.Infrastructure.Data.Queries
{
    public class TenantManagmentPrivateLabelsQuery
    {
        private IRepository<TenantManagmentPrivateLabels> repository;
        IGlobalContext context;

        public TenantManagmentPrivateLabelsQuery(int tenant) : this(GlobalContext.GetContext(tenant))
        {
        }
        public TenantManagmentPrivateLabelsQuery(IGlobalContext context) : this(new Repository<TenantManagmentPrivateLabels>(context))
        {
            this.context = context;
        }
        public TenantManagmentPrivateLabelsQuery(IRepository<TenantManagmentPrivateLabels> repository)
        {
            this.repository = repository;
        }

        public TenantManagmentPrivateLabelsPM GetSingleActivePMByUrl_Cache(string url)
        {
            string entityKeyString = $"GetSingleActivePMByUrl_Cache({url})";
            var res = CacheManager.GetOrInsertNewObject<TenantManagmentPrivateLabelsPM>(entityKeyString,
                () => { return this.GetSingleActivePMByUrl(url); });
            return res;
        }
        public TenantManagmentPrivateLabelsPM GetSingleActivePMByUrl(string url)
        {
            TenantManagmentPrivateLabelsPM entity = (from a in context.TenantManagmentPrivateLabels
                                                     where a.PrivateLabelUrl == url && a.InActive == false
                                                     select new TenantManagmentPrivateLabelsPM()
                                                     {
                                                         Id = a.Id,
                                                         PrivateLabelName = a.PrivateLabelName,
                                                         PrivateLabelShortName = a.PrivateLabelShortName,
                                                         PrivateLabelUrl = a.PrivateLabelUrl,
                                                         PrivateLabelDomain = a.PrivateLabelDomain,
                                                         ReceiveAllStatuses = a.ReceiveAllStatuses,
                                                         MainLogo = a.MainLogo,
                                                         InActive = a.InActive,
                                                         HybridPartnerId = a.HybridPartnerId,
                                                         ContactUsEmail = a.ContactUsEmail,
                                                         SearchFields = a.SearchFields,
                                                         SmallLogo = a.SmallLogo,
                                                         BackgroundImageId = a.BackgroundImageId,
                                                         LoginImageId = a.LoginImageId,
                                                         MainColor = a.MainColor,
                                                         LoginProgressImageId = a.LoginProgressImageId,
                                                         ForgetPasswordImageId = a.ForgetPasswordImageId,
                                                         SecondaryColor = a.SecondaryColor,
                                                         HasLogboxAccess = a.HasLogboxAccess,
                                                         MainTabHighlightColor = a.MainTabHighlightColor,
                                                         DocumentTypeHighlightColor = a.DocumentTypeHighlightColor,
                                                         IsCustomsActivated = a.IsCustomsActivated,
                                                         IsExportActivated = a.IsExportActivated,
                                                         QueryFiltersHighlightColor = a.QueryFiltersHighlightColor,
                                                         CreateShipmentsWithoutDocs = a.CreateShipmentsWithoutDocs,
                                                         CreateOShipmentsWithoutDocs = a.CreateOShipmentsWithoutDocs,
                                                         FilingInboxDomain = a.FilingInboxDomain,
                                                         DistributorCode = a.DistributorCode
                                                     }).FirstOrDefault();

            return entity;
        }
    }
}