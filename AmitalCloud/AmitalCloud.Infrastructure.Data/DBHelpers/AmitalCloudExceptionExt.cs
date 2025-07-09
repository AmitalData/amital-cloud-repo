using System;
using System.Reflection;

namespace AmitalCloud.Infrastructure.Data.DBHelpers
{
    public static class AmitalCloudExceptionExt
    {
        public static void ChangeExceptionMessage(this Exception ex, string messagePrefix)
        {
            var mess = ex.Message;
            ex.GetType().GetField("_message", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(ex, messagePrefix + " " + mess);
        }
        public static void ChangeExceptionMess(this Exception ex, string messagePrefix)
        {
            var mess = ex.Message;
            ex.GetType().GetField("_message", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(ex, messagePrefix + " " + mess);
        }
        //public static ExceptionInErrorLog HandleException(this Exception exception, //DateTime clientDate, int tenant, string userId, string userName, string ExtraMessage, string ip)
        //    string TypeOrUser, string ExtraMessage)
        //{
        //    if (!AmitalCloudSettings.IsCostomsDeploy)
        //    {
        //        AmitalCloudSettings.HandleDbExceptionInject(exception, TypeOrUser, ExtraMessage);
        //        return null;
        //    }

        //    var myErrorlogLDC = new ExceptionInErrorLog();
        //    using (var myScope = new LogtitudeDomainScope(myErrorlogLDC))
        //    {
        //        AmitalCloudSettings.HandleDbExceptionInject(exception, TypeOrUser, ExtraMessage);
        //        if (!String.IsNullOrWhiteSpace(myErrorlogLDC.ErrorlogId))
        //        {
        //            return myErrorlogLDC;
        //        }
        //        return null;
        //    }
        //}

    }

}
