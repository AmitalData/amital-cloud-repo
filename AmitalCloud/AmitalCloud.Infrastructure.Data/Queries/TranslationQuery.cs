using System;
using System.Collections.Generic;
using System.Linq;
using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Data.Security;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using AmitalCloud.Infrastructure.Model.Interfaces;

namespace AmitalCloud.Infrastructure.Data.Queries
{
    public class TranslationQuery
    {
        private readonly int tenant;
        private readonly IAmitalCloudContext context;
        private readonly Repository<Translation> repository;

        public TranslationQuery(int tenant)
        {
            this.tenant = tenant;
            context = AmitalCloudContext.GetContext(tenant);
            repository = new Repository<Translation>(context);
        }

        public List<Translation> GetTenantTranslations(string url)
        {
            List<Translation> AllTranslations = repository.GetMulti(a => a.Tenant == tenant);
            
            Repository<Tenant> tenantRepo = new Repository<Tenant>(context);
            Tenant tenantPoco = tenantRepo.GetSingle(a => a.Id == tenant);

            ModifyTranslationByPrivateLabel(AllTranslations, tenantPoco?.Language, url);
            return AllTranslations;
        }

        public List<Translation> GetTenantLanguageTranslations(string language, string url)
        {
            List<Translation> AllTranslations = repository.GetMulti(a => a.Tenant == 0 && a.TranslationHeaderCode == language);

            ModifyTranslationByPrivateLabel(AllTranslations, language, url);
            return AllTranslations;
        }

        private void ModifyTranslationByPrivateLabel(List<Translation> AllTranslations, string language, string url)
        {
            TenantManagmentPrivateLabelsPM privatelabel = null;
          
            if (!url.Contains("system.logbox.co.il") && !url.Contains("cloud.amital.co.il"))
            {
                TenantManagmentPrivateLabelsQuery query = new TenantManagmentPrivateLabelsQuery(tenant);
                privatelabel = query.GetSingleActivePMByUrl_Cache(url);
            }
            if (privatelabel != null)
            {
                Repository<TextCode> textCodeRepo = new Repository<TextCode>(context);
                TextCode textCode = textCodeRepo.GetSingle(a => a.Code == "General.MH.Importers" && a.Tenant == tenant);
                Translation tra = AllTranslations.FirstOrDefault(t => t.TextCodeCode == textCode.Code);
                if (tra != null)
                {
                    tra.TranslatedText = privatelabel.PrivateLabelName;
                }
                else
                {
                    tra = new Translation()
                    {
                        Id = Guid.NewGuid().ToString(),
                        TextCodeId = textCode.Id,
                        TextCode = textCode,
                        Tenant = tenant,
                        TranslatedText = privatelabel.PrivateLabelName,
                        TranslationHeaderCode = language,
                        TextCodeCode = textCode.Code,
                    };

                    AllTranslations.Add(tra);
                }
                textCode = textCodeRepo.GetSingle(a => a.Code == "General.MH.ActivationWizard" && a.Tenant == tenant);
                tra = AllTranslations.FirstOrDefault(t => t.TextCodeCode == textCode.Code);
                if (tra != null)
                {
                    tra.TranslatedText = privatelabel.PrivateLabelShortName + " Services";
                }
                else
                {
                    tra = new Translation()
                    {
                        Id = Guid.NewGuid().ToString(),
                        TextCodeId = textCode.Id,
                        TextCode = textCode,
                        Tenant = tenant,
                        TranslatedText = privatelabel.PrivateLabelShortName + " Services",
                        TranslationHeaderCode = language,
                        TextCodeCode = textCode.Code,
                    };

                    AllTranslations.Add(tra);
                }
            }
        }
    }
}