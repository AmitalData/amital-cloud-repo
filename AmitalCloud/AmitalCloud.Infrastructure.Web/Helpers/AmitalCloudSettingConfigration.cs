using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Domain.Helpers;
using AmitalCloud.Infrastructure.Data.Helpers;

public static class AmitalCloudSettingConfigration
{
    public static string GetWorkEnvironment()
    {
        if (string.IsNullOrEmpty(AmitalCloudSettings.WorkEnvironment)) return "Amital";
        return IsLogBoxEnvironment() ? GetLogboxWorkEnvironment() : AmitalCloudSettings.WorkEnvironment;
    }

    public static bool IsLogBoxEnvironment()
    {
        return SettingUtil.DeploymentStage.IsDBStage(SettingUtil.DeploymentStage.Logbox);
    }

    private static string GetLogboxWorkEnvironment()
    {
        return HttpContextHelper.Request.Host.Host.ToLower().Contains(".logbox.") ? "logbox" : "privatelabel";
    }
}
