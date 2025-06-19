using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;

namespace AmitalCloud.Infrastructure.Data.Services
{
    public static class QueueThreadStateService
    {

        private static readonly ConcurrentDictionary<string, string> _ConcurrentQueueState;
        static QueueThreadStateService()
        {
            _ConcurrentQueueState = new ConcurrentDictionary<string, string>();
        }

        public static string GetWRKey(string thisGetTypeName)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(Thread.CurrentThread.Name))
                {
                    return Thread.CurrentThread.Name;
                }
                else
                {
                    return $"{thisGetTypeName}:{Thread.CurrentThread?.ManagedThreadId.ToString()}";
                }

            }
            catch (Exception)
            {

                return null;
            }

        }
        public static void Upsert(string queueThreadCode, string queueThreadState)
        {
            try
            {
                var state = $"StartAt-{DateTime.Now.ToString("T")}-S:{queueThreadState}";
                _ConcurrentQueueState.AddOrUpdate(queueThreadCode, state
                     ,
                    (keyToUpdate, existingValue) =>
                    {
                        return state;
                    });

            }
            catch (Exception)
            {

                //throw;
            }

        }
        public static string GetState()
        {
            var sb = new StringBuilder();
            try
            {
                var keys = _ConcurrentQueueState.Keys.ToList().OrderBy(k => k);

                foreach (var key in keys)
                {
                    var value = _ConcurrentQueueState[key];
                    sb
                        //.Append(JsonConvert.SerializeObject(key, Formatting.None, new JsonSerializerSettings { }))
                        .Append(key.ToString())
                        .Append(">")
                        .AppendLine(value);
                }

            }
            catch (Exception e)
            {

                sb.Append("QueueSThreadStateService:ERRRR:").AppendLine(e.Message);
            }
            return sb.ToString();
        }
    }
    public class QueueThreadKey
    {

        public QueueThreadKey(string workerClass, string currentThreadName)
        {

            WorkerClass = workerClass;
            CurrentThreadName = currentThreadName;
        }
        public string WorkerClass { get; private set; }
        public string CurrentThreadName { get; private set; }

        public override string ToString()
        {
            return $"WRClass{WorkerClass},ThreadName:{CurrentThreadName}";
        }
    }
}
