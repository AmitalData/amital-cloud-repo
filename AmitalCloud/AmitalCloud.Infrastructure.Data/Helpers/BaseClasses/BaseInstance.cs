using System;

namespace AmitalCloud.Infrastructure.Data.Helpers.BaseClasses
{
    public abstract class BaseInstance<T> where T : new()
    {
        private static readonly Lazy<T> _instance = new Lazy<T>(() => new T(), System.Threading.LazyThreadSafetyMode.ExecutionAndPublication);
        public static T Instance => _instance.Value;
    }
}
