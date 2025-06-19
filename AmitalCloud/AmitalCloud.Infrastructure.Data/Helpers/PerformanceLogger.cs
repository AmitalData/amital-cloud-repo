using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using Microsoft.Extensions.Azure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Web;

namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public class PerformanceLogger
    {

        private static ConcurrentDictionary<string, DateTime> logTimes = new ConcurrentDictionary<string, DateTime>();

        public static string LogCurrentTime()
        {
            string key = Guid.NewGuid().ToString();
            logTimes.TryAdd(key, DateTime.Now);

            return key;
        }


        private static DateTime GetLogTime(string key)
        {
            DateTime date;
            if (logTimes.TryGetValue(key, out date))
            {
                return date;
            }
            else
            {
                return DateTime.Now;
            }
            //return logTimes.Where(s => s.Key.Contains(key)).FirstOrDefault().Value;
        }

        private static void RemoveLogTime(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                if (logTimes.Keys.Contains(key))
                {
                    DateTime date;
                    logTimes.TryRemove(key, out date);
                }
            }
        }

        public static void AddServerExecutionTimeHeader(string logKey)
        {
            if (!string.IsNullOrEmpty(logKey))
            {
                DateTime callTime = PerformanceLogger.GetLogTime(logKey);
                DateTime completionTime = DateTime.Now;
                int executionTime = (int)((completionTime.Ticks - callTime.Ticks) / TimeSpan.TicksPerMillisecond);
                HttpContextHelper.Response.Headers.Add("Access-Control-Expose-Headers", "ServerExecutionTime, X-Custom");
                HttpContextHelper.Response.Headers.Add("ServerExecutionTime", executionTime.ToString());

                PerformanceLogger.RemoveLogTime(logKey);
            }

        }

        public static void AddPerformanceLogsList(List<PerformanceLog> logsList)
        {
            Repository<PerformanceLog> repository = new Repository<PerformanceLog>(GlobalContext.GetContext());

            try
            {
                string ip = GetClientIPAddress(); ;

                if (logsList != null && logsList.Count > 0)
                {
                    foreach (var entity in logsList)
                    {
                        entity.Id = Guid.NewGuid().ToString();
                        entity.UserIP = ip;
                        entity.LogDateTimeGMT = DateTime.UtcNow;

                        repository.Insert(entity);
                    }
                }
                repository.SubmitChanges();
            }
            catch (Exception ex)
            {
                ExceptionHandler.HandleException(ex, DateTime.Now, 0, null, "PerformanceLogger.AddPerformanceLogsList", null, null);
            }
        }

        private static string GetClientIPAddress()
        {
            if (HttpContextHelper.Request != null)
            {
                string currentIP = HttpContextHelper.Request.Headers["X-Real-IP"];
                if (string.IsNullOrEmpty(currentIP))
                {
                    currentIP = HttpContextHelper.HttpContext?.Connection?.RemoteIpAddress?.ToString();
                }
                return currentIP;
            }
            return "";
        }
    }

}
