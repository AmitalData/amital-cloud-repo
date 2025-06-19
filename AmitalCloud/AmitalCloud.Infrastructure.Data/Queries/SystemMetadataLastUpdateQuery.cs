using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Helpers;
using System.Transactions;
using System;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using AmitalCloud.Infrastructure.Model.Interfaces;

namespace AmitalCloud.Infrastructure.Data.Queries
{
    public class SystemMetadataLastUpdateQuery
    {
        private readonly int tenant;
        private int SystemMetadataLastUpdateDefaultId = 1;
        private readonly Repository<SystemMetadataLastUpdate> repository;

        public SystemMetadataLastUpdateQuery(int tenant)
        {
            this.tenant = tenant;
            repository = new Repository<SystemMetadataLastUpdate>(GlobalContext.GetContext(tenant));
        }

        public MetaDataLastUpdateDates GetSystemMetadataLastUpdatesCacheHandle()
        {
            string entityName = "SystemMetadataLastUpdates_" + tenant;
            MetaDataLastUpdateDates metadatalastUpdates;

            if (CacheManager.CacheWrapper != null && CacheManager.CacheWrapper.Get(entityName) != null)
            {
                metadatalastUpdates = (MetaDataLastUpdateDates)CacheManager.CacheWrapper.Get(entityName);
            }
            else
            {
                metadatalastUpdates = GetSystemMetadataLastUpdateFromDB();

                if (CacheManager.CacheWrapper != null && CacheManager.CacheWrapper.Get(entityName) == null && metadatalastUpdates != null)
                {
                    CacheManager.CacheWrapper.Insert(entityName, metadatalastUpdates, null, DateTime.UtcNow.AddMinutes(1), TimeSpan.Zero);
                }
            }
            return metadatalastUpdates;
        }

        private MetaDataLastUpdateDates GetSystemMetadataLastUpdateFromDB()
        {
            MetaDataLastUpdateDates metadata = new MetaDataLastUpdateDates()
            {
                Id = SystemMetadataLastUpdateDefaultId,
            };

            using (TransactionScope scope = TransactionFactory.GetNewTransaction())
            {
                SystemMetadataLastUpdate update = repository.GetSingle(a => a.Id == SystemMetadataLastUpdateDefaultId.ToString());

                metadata.ObjectFieldsSystemUpdateDateGMT = (update != null ? update.ObjectFieldsUpdateDateGMT : DateTime.UtcNow);
                metadata.TranslationsSystemUpdateDateGMT = (update != null ? update.TranslationsUpdateDateGMT : DateTime.UtcNow);

                scope.Complete();
            }

            IAmitalCloudContext amitalCloudContext = AmitalCloudContext.GetContext(tenant);

            ObjectFieldRepository objectFieldsRepository = new ObjectFieldRepository(amitalCloudContext);
            ObjectFieldModification mod = objectFieldsRepository.GetLastObjectFieldModificationByTenant(tenant);
            metadata.ObjectFieldsTenantUpdateDateGMT = (mod != null ? mod.UpdateDateGMT.Value : DateTime.UtcNow);

            TranslationRepository translationRepository = new TranslationRepository(amitalCloudContext);
            Translation translation = translationRepository.GetLastTranslationsByTenant(tenant);
            metadata.TranslationsTenantUpdateDateGMT = (translation != null ? translation.UpdateDateGMT.Value : DateTime.UtcNow);
            return metadata;
        }
    }
}