using System.Linq;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Domain.Helpers;
using System;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using AmitalCloud.Infrastructure.Model.Interfaces;

namespace AmitalCloud.Infrastructure.Data.Queries
{
    public class TermsofUseQuery
    {
        private readonly int tenant;
        private readonly IAmitalCloudContext context;
        private readonly Repository<TermsofUse> repository;

        public TermsofUseQuery(int tenant)
        {
            this.tenant = tenant;
            context = AmitalCloudContext.GetContext(tenant);
            repository = new Repository<TermsofUse>(AmitalCloudContext.GetContext(tenant));
        }

        public TermsofUseArgs CheckIfGoToTermUseComponent(string userId, string url)
        {
            TermsofUse termofuse = null;
            TermsofUseArgs result = new TermsofUseArgs();
            IGlobalContext globalContext = GlobalContext.GetContext(tenant);

            string PrivateLabelId = new Repository<GlobalTenant>(globalContext).GetSingle(a => a.Id == tenant, a => a.PrivateLabelId);

            if (!string.IsNullOrEmpty(PrivateLabelId))
            {
                termofuse = repository.GetMulti(a => a.PrivateLabelId == PrivateLabelId, orderBy: d => d.VersionNumber, Domain.Enums.OrderByDirection.Descending).FirstOrDefault();
                if (termofuse == null)
                {
                    if (new Repository<TenantManagmentPrivateLabels>(globalContext).GetSingle(a => a.Id == PrivateLabelId, a => a.PrivateLabelUrl) == url)
                    {
                        throw new Exception("You are unable to login without approving the terms of use, please contact your administrator!");
                    }
                }
            }

            bool isLogboxUrl = url.IndexOf("logbox") > -1;

            if (termofuse == null || isLogboxUrl)
            {
                bool UseNewTermsOfUse = new Repository<Tenant>(context).GetSingle(a => a.Id == tenant, a => a.UseNewTermsOfUse);

                termofuse = repository.GetMulti(a => a.Tenant == 0 && a.PrivateLabelId == null && a.IsNew == UseNewTermsOfUse, orderBy: d => d.VersionNumber, Domain.Enums.OrderByDirection.Descending).FirstOrDefault();
            }

            if (termofuse == null)
            {
                result.IsTermOfUse = false;
            }
            else
            {
                result.VersionNumber = (int)termofuse.VersionNumber;
                result.Id = termofuse.Id;
                result.VersionDocumentId = termofuse.VersionDocumentId;
                result.PrivateLabelId = termofuse.PrivateLabelId;

                TermsofUseSignature termsofUseSignaturePM = new Repository<TermsofUseSignature>(context).GetSingle(a => a.ContactId == userId && a.Tenant == tenant && a.TermsofUseId == termofuse.Id);

                bool isLogbox = SettingUtil.DeploymentStage.IsDBStage(SettingUtil.DeploymentStage.Logbox);

                result.IsTermOfUse = termsofUseSignaturePM == null && (!isLogbox || !string.IsNullOrEmpty(PrivateLabelId));
            }
            return result;
        }
    }
}