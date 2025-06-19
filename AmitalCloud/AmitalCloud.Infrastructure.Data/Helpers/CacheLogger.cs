using System.Collections.Concurrent;

namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public class CacheLogger
    {
        public static bool IsCacheLoggerEnabled = false;
        public static ConcurrentDictionary<string, int> KeysGetCounter { get; set; } = new ConcurrentDictionary<string, int>();
        public static void LogKey(string key)
        {
            if (IsCacheLoggerEnabled == true)
            {
                KeysGetCounter.AddOrUpdate(key, 1,
                    (keyToUpdate, existingValue) =>
                    {
                        ++existingValue;
                        return existingValue;
                    });
            }

        }


    }

    public class CacheLog
    {
        public string Key { get; set; }
        public int Count { get; set; }
    }


}
