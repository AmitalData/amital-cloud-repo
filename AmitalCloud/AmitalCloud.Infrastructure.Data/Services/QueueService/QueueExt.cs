using AmitalCloud.Infrastructure.Data.Services;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System;
using Azure.Messaging.ServiceBus;

namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public static class QueueExt
    {

        public enum QueuePropertyNames : int
        {
            Priority, LastExecAt, Retries, InterfaceTypeCode,
            DebugMode,
            ProcessState,
            Tenant, DcaAnalyzeAggregateKey,
            CorrelationId
        }
        public static TValue GetProperty<TValue>(this QueueResponse msg, QueuePropertyNames key, TValue defualtValue)
            where TValue : IConvertible
        {
            if (!msg.MessageValues.ContainsKey(key.ToString()))
            {
                msg.MessageValues[key.ToString()] = defualtValue.ToString();
            }
            var val = msg.MessageValues[key.ToString()].ChangeValue<TValue>();
            return val;
        }
        public static TValue GetProperty<TValue>(this CustomDBQueueMessage msg, QueuePropertyNames key, TValue defualtValue)
            where TValue : IConvertible
        {
            if (!msg.Properties.ContainsKey(key.ToString()))
            {
                msg.Properties[key.ToString()] = defualtValue.ToString();
            }
            var val = msg.Properties[key.ToString()].ChangeValue<TValue>();
            return val;
        }
        public static TValue GetProperty<TValue>(this ServiceBusMessage msg, QueuePropertyNames key, TValue defualtValue)
            where TValue : IConvertible
        {
            if (!msg.ApplicationProperties.ContainsKey(key.ToString()))
            {
                msg.ApplicationProperties[key.ToString()] = defualtValue;
            }
            var val = (TValue)msg.ApplicationProperties[key.ToString()];
            return val;
        }
        public static TValue SetProperty<TValue>(this ServiceBusMessage msg, QueuePropertyNames key, TValue newValue)
            where TValue : IConvertible
        {
            msg.GetProperty<TValue>(key, newValue);
            msg.ApplicationProperties[key.ToString()] = newValue; ;
            return newValue;
        }
        public static TValue SetProperty<TValue>(this CustomDBQueueMessage msg, QueuePropertyNames key, TValue newValue)
            where TValue : IConvertible
        {
            msg.GetProperty<TValue>(key, newValue);
            msg.Properties[key.ToString()] = newValue.ToString(); ;
            return newValue;
        }
        public static T ChangeValue<T>(this string value)
            where T : IConvertible
        {
            return (T)Convert.ChangeType(value, typeof(T));
        }
        public static bool SafeComplete(this ServiceBusReceiver receiver, ServiceBusReceivedMessage msg)
        {
            try
            {
                receiver.CompleteMessageAsync(msg).Wait();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool SafeAbandon(this ServiceBusReceiver receiver, ServiceBusReceivedMessage msg, IDictionary<string, object> properties = null)
        {
            try
            {
                if (properties != null)
                    receiver.AbandonMessageAsync(msg, properties).Wait();
                else
                    receiver.AbandonMessageAsync(msg).Wait();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
    public enum SBQueueNames
    {
        CustomsMessagingSheetBQ, SendDataToExternalServicesBQ, updateclosedtables, SendWEBAPIMessage2MamanQ
            , AnalyzeQueueMQ
            , CustomsHSMSignWR
    }
}
