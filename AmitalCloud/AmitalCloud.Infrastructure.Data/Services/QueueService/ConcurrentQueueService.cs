using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace AmitalCloud.Infrastructure.Data.Services
{
    public class ConcurrentQueueService<T>
    {
        private static readonly Dictionary<string, ConcurrentQueue<T>> QueueDefinitions = new Dictionary<string, ConcurrentQueue<T>>();
        private ConcurrentQueue<T> CurrentQueue;

        public ConcurrentQueueService(string queueCode)
        {
            SetCurrentQueue(queueCode);
        }

        private void SetCurrentQueue(string queueCode)
        {
            if (!QueueDefinitions.Keys.Contains(queueCode))
            {
                QueueDefinitions.Add(queueCode, new ConcurrentQueue<T>());
            }

            this.CurrentQueue = QueueDefinitions[queueCode];
        }

        public T TryDequeue()
        {
            T result;
            this.CurrentQueue.TryDequeue(out result);
            return result;
        }

        public T TryPeek()
        {
            T result;
            this.CurrentQueue.TryPeek(out result);

            //this.CurrentQueue.up
            return result;
        }

        public void Enqueue(T message)
        {
            this.CurrentQueue.Enqueue(message);
        }

        //public static ConcurrentQueue<T> GetQueue(string queueName)
        //{
        //    if (!QueueDefinitions.Keys.Contains(queueName))
        //    {
        //        QueueDefinitions.Add(queueName, new ConcurrentQueue<T>());
        //    }

        //    return QueueDefinitions[queueName];
        //}
    }
}
