using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AmitalCloud.Infrastructure.Model.Interfaces;

namespace AmitalCloud.Infrastructure.Data.Repositories
{
    public class TranslationRepository : Repository<Translation>
    {

        IAmitalCloudContext currentContext;
        public TranslationRepository(IAmitalCloudContext context) : base(context)
        {
            currentContext = context;
        }
        public TranslationRepository(int tenant) : this(AmitalCloudContext.GetContext(tenant))
        {
        }

        public IQueryable<Translation> GetTranslations()
        {
            return context.Translations.Include("TextCode");
        }
        public Translation GetSingleTranslation(int tenant, string code, string language)
        {
            return context.Translations.Where(d => d.Tenant == tenant && d.TextCodeCode == code && (d.TranslationHeader.Description == language || d.TranslationHeaderCode == language)).Include("TextCode").FirstOrDefault();
        }
        public List<Translation> GetTranslationsByTenantList(int tenant)
        {
            return (from a in context.Translations.Include("TextCode")
                    where a.Tenant == tenant
                    select a).ToList();
        }
        public IQueryable<Translation> GetTranslationsByTenant(int tenant)
        {
            return (from a in context.Translations.Include("TextCode")
                    where a.Tenant == tenant
                    select a);
        }
        public List<Translation> GetTranslationsWithoutESByTenant(int tenant)
        {
            return (from a in context.Translations.Include("TextCode")
                    where a.Tenant == tenant && a.TranslationHeaderCode != "ES"
                    select a).ToList();
        }
        public Translation GetLastTranslationsByTenant(int tenant)
        {
            string cacheKey = $"LastTranslationsByTenant_{tenant}";
            Translation lastTranslation = (Translation)Helpers.CacheManager.CacheWrapper.Get(cacheKey);
            if (lastTranslation == null)
            {
                lastTranslation = GetQueryable()
                    .AsNoTracking()
                    .Where(a => a.Tenant == tenant && a.UpdateDateGMT != null)
                    .OrderByDescending(a => a.UpdateDateGMT)
                    .FirstOrDefault();

                if (Helpers.CacheManager.CacheWrapper.Get(cacheKey) == null)
                {
                    if (lastTranslation != null)
                    {
                        Helpers.CacheManager.CacheWrapper.Insert(cacheKey, lastTranslation, null, DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
                    }
                    else
                    {
                        Helpers.CacheManager.CacheWrapper.Insert(cacheKey, new Translation() { UpdateDateGMT = new DateTime(2015, 1, 1) }, null, DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
                    }
                }
            }
            return lastTranslation;
        }
        private static void InvalidateLastTranslationCache(Translation entity)
        {
            string entityName = "LastTranslationsByTenant" + entity.Tenant;
            if (Helpers.CacheManager.CacheWrapper.Get(entityName) != null)
            {
                Helpers.CacheManager.CacheWrapper.Invalidate(entityName);
            }
        }
        public IAmitalCloudContext context
        {
            get { return currentContext; }
        }
    }
}