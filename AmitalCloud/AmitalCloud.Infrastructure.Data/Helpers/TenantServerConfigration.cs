using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public static class TenantServerConfigration
    {


        public static DateTime GetCurrentDateTime(int tenant)
        {

            if (AmitalCloudSettings.IsCostomsDeploy)
            {
                return DateTime.Now;
            }
            double offsetHours = 0;
            //DateTime dateTime = DateTime.Now;
            //var format = "yyyy-MM-dd HH:mm:ss:fff";
            //var stringDate = DateTime.Now.ToString(format);
            var dateTime = DateTime.UtcNow;//DateTime.ParseExact(stringDate, format, new CultureInfo("en-US"));
            string datetimeoffset = "datetimeoffset" + tenant;
            if (CacheManager.CacheWrapper != null)
            {
                if (CacheManager.CacheWrapper.Get(datetimeoffset) == null)
                {

                    offsetHours = GetCurrentDateWithTimeZoneOffset(tenant);
                    dateTime = dateTime.AddHours(offsetHours);

                    CacheManager.CacheWrapper.Insert(datetimeoffset, offsetHours, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);

                }
                else
                {
                    offsetHours = (double)CacheManager.CacheWrapper.Get(datetimeoffset);
                    dateTime = dateTime.AddHours(offsetHours);
                }
            }




            //double offsetHours = Entity.TimeZoneOffset;

            //if (Entity.DayLightOffset != 0)
            //{

            //    if (Entity.DayLightStartDate.Value.Date <= DateTime.Now.Date && Entity.DayLightEndDate.Value.Date >= DateTime.Now.Date)
            //    {
            //        offsetHours = offsetHours + Entity.DayLightOffset; 
            //    } 
            //}



            //if (dateTime.Date > DateTime.Now.Date)
            //{

            //}

            return dateTime;

        }
        private static double GetCurrentDateWithTimeZoneOffset(int tenant)
        {

            string entityName = "Tenant" + tenant;

            Tenant entity;

            if (CacheManager.CacheWrapper.Get(entityName) == null)
            {
                entity = new Repository<Tenant>(tenant).GetMulti(a => a.Id == tenant).FirstOrDefault();  //  .GetSingleTenant(tenant);
                if (entity != null)
                {
                    CacheManager.CacheWrapper.Insert(entityName, entity, null, System.DateTime.UtcNow.AddMinutes(30), TimeSpan.Zero);
                }
            }
            else
            {
                entity = (Tenant)CacheManager.CacheWrapper.Get(entityName);
            }



            double offsetHours = 0;
            if (entity != null)
            {
                if (entity.TimeZoneOffset != null)
                {
                    offsetHours = entity.TimeZoneOffset.Value;
                }

                if (entity.DayLightOffset != 0)
                {
                    if (entity.DayLightStartDate.Value.Date <= DateTime.Now.Date && entity.DayLightEndDate.Value.Date >= DateTime.Now.Date)
                    {
                        offsetHours = offsetHours + entity.DayLightOffset;
                    }
                }

            }

            return offsetHours;
        }
        public static string GetDbConnection(int tenant)
        {

            return AmitalCloudContext.GetContext(tenant).Database.GetDbConnection().ConnectionString;
        }
        public static DateTime GetEndOfTodayDate(int tenant)
        {
            var todayDate = GetCurrentDateTime(tenant);
            todayDate = new DateTime(todayDate.Year, todayDate.Month, todayDate.Day, 23, 59, 59, 59);
            return todayDate;
        }
        public static DateTime GetStartOfTodayDate(int tenant)
        {
            var todayDate = GetCurrentDateTime(tenant);
            todayDate = new DateTime(todayDate.Year, todayDate.Month, todayDate.Day, 0, 0, 0, 0);
            return todayDate;
        }

        public static DateTime GetLastOfCurrentMonthDate(int tenant)
        {
            var todayDate = GetCurrentDateTime(tenant);
            return new DateTime(todayDate.Year, todayDate.Month, DateTime.DaysInMonth(todayDate.Year, todayDate.Month), 0, 0, 0, 0);
        }
        public static DateTime GetStartOfCurrentMonthDate(int tenant)
        {
            var todayDate = GetCurrentDateTime(tenant);
            return new DateTime(todayDate.Year, todayDate.Month, 1, 0, 0, 0, 0);
        }

        public static DateTime GetLastOfMonthDate(DateTime date)
        {
            return new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month), 0, 0, 0, 0);
        }
        public static DateTime GetStartOfMonthDate(DateTime todayDate)
        {
            return new DateTime(todayDate.Year, todayDate.Month, 1, 0, 0, 0, 0);
        }
    }

    public class TenantServerConfigrationWrapper
    {
        public DateTime GetCurrentDateTime(int tenant)
        {
            return TenantServerConfigration.GetCurrentDateTime(tenant);
        }
    }
}
