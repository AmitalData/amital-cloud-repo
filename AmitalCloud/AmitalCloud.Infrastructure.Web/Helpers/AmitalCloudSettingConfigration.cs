using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Domain.Helpers;
using AmitalCloud.Infrastructure.Data.Helpers;

public static class AmitalCloudSettingConfigration
{
    public static string GetWorkEnvironment()
    {
        if (string.IsNullOrEmpty(AmitalCloudSettings.WorkEnvironment)) return "Amital";
        return  AmitalCloudSettings.WorkEnvironment;
    }

 
}
