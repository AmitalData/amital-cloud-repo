using AmitalCloud.Infrastructure.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;

namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public static class CacheManager
    {
        private static ICacheWrapper cacheWrapper;
        public static ICacheWrapper CacheWrapper
        {
            get { return cacheWrapper; }
            set
            {
                if (cacheWrapper != null) return; //only set once 
                cacheWrapper = value;
            }
        }

        public static int ClearCacheItems(Func<string, bool> pattrenFunc = null)
        {
            int clearItem = 0;
            var enumerator = CacheManager.CacheWrapper.GetEnumerator();

            while (enumerator.MoveNext())
            {
                try
                {
                    string key = enumerator.Key.ToString();
                    bool toClear = true;
                    if (pattrenFunc != null)
                    {
                        toClear = pattrenFunc(key);
                    }
                    if (toClear)
                    {
                        CacheManager.CacheWrapper.Remove(key);
                        clearItem++;
                    }
                }
                catch (Exception)
                {

                    ///throw;
                }

            }
            return clearItem;
        }

        public static List<string> GetAllCacheKeys()
        {
            var cacheKeys = new List<string>();
            var enumerator = CacheWrapper.GetEnumerator();
            while (enumerator.MoveNext())
            {
                cacheKeys.Add(enumerator.Key.ToString());
            }

            return cacheKeys;
        }
        public static TEntity GetOrInsertNewObject<TEntity>(string entityKeyString, Func<TEntity> GetNewObject, bool fromCache = true, bool donotCacheNull = false, bool supressForceInsert = true, int absoluteExpiration = 30) //Itzik Test
            where TEntity : class
        {
            if (CacheManager.CacheWrapper == null)
            {
                throw new Exception("please int  CacheManager.CacheWrapper if unitest try like  JournalValidator.OverrideITextCodeTranslator ");
            }
            TEntity EntityPM = null;
            object cacheObj = null;
            if (supressForceInsert)
            {
                cacheObj = CacheManager.CacheWrapper.Get(entityKeyString);
            }
            if (cacheObj != null)
            {
                EntityPM = cacheObj as TEntity;
            }
            else
            {
                EntityPM = GetNewObject();
                if (EntityPM != null)
                {
                    CacheManager.CacheWrapper.Insert(entityKeyString, EntityPM,
                        null, System.DateTime.UtcNow.AddMinutes(absoluteExpiration), TimeSpan.Zero);
                }
                else
                {
                    if (!donotCacheNull)
                    {
                        CacheManager.CacheWrapper.Insert(entityKeyString, new NullCache(),
                            null, System.DateTime.UtcNow.AddMinutes(absoluteExpiration), TimeSpan.Zero);
                    }
                }
            }
            return EntityPM;
        }
    }
    /// <summary>
    /// Dummy Entity for Cache Use
    /// </summary>
    public class MyDummyClass
    {
        public int MyInt { get; set; }
        public bool MyBool { get; set; }

    }
}
