namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public static class ApplicationAppInfo
    {

        public static bool WorkerRoleCall { get; set; }

        public static int GetDataBaseTimeOut()
        {
            int timeout = 120;
            if (WorkerRoleCall)
            {
                timeout = 1200;
            }
            return timeout;


        }

    }

}
