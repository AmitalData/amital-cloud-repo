using System.Diagnostics;

namespace AmitalCloud.Infrastructure.Data.Helpers
{

    public enum AmitalDebuggerLevel : int
    {
        None = 0,
        Critical = 1,

        Error = 2,

        Warning = 4,

        Information = 8,

        Verbose = 16
    }
    public static class AmitalCloudDebuggerUtil
    {
        public static AmitalDebuggerLevel? MyDebuggerLevel { get; set; }

        public static void Break(AmitalDebuggerLevel CurrentBreak = AmitalDebuggerLevel.Verbose)
        {
            if (!Debugger.IsAttached) return;
            if (AmitalCloudDebuggerUtil.MyDebuggerLevel == null)
            {
                AmitalCloudDebuggerUtil.MyDebuggerLevel = AmitalDebuggerLevel.Information;
                AmitalCloudDebuggerUtil.MyDebuggerLevel = AmitalDebuggerLevel.Verbose;

                NetCommonHelper.Logger.DevLog.Instance.WriteDebug(@"to filter Debug try this
AmitalCloudDebuggerUtil.MyDebuggerLevel = AmitalDebuggerLevel.Information;");
            }
            if (AmitalCloudDebuggerUtil.MyDebuggerLevel < CurrentBreak)
            {
                return;
            }
            //Debugger.Break();
        }
    }

}
