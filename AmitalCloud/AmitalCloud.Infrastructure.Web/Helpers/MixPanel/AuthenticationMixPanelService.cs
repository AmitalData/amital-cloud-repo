using AmitalCloud.Infrastructure.Application.EntityQueryServices;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Web.DataContracts;

namespace AmitalCloud.Infrastructure.Web.Helpers.MixPanel
{
    public class AuthenticationMixPanelService
    {
        private const string cargoTrackingProjectToken = "99de9de5af6505a670b915020e51380e";
        private const string logBoxProjectToken = "6da04c25721c3a0269bab184d35fbc1a";

        public static void CreateLoginEventForMixPanel(LoginParameters parameters, int tenant)
        {
            if (!CanCreateLoginEvent(parameters))
                return;

            MixPanelEvent LoginEvent = BuildMixPanelLoginEvent(parameters, tenant);

            string projectToken = parameters.IsCargoTracking ? cargoTrackingProjectToken : logBoxProjectToken;
            MixPanelEventTracker eventTracker = new MixPanelEventTracker(projectToken, parameters.Email, tenant);
            eventTracker.TrackEvent(LoginEvent);
        }

        private static bool CanCreateLoginEvent(LoginParameters parameters)
        {
            return AmitalCloudSettings.AmitalURL != "http://localhost:9996" && (AmitalCloudSettingConfigration.IsLogBoxEnvironment() || parameters.IsCargoTracking);
        }

        private static MixPanelEvent BuildMixPanelLoginEvent(LoginParameters parameters, int tenant)
        {
            string workEnvironment = GetWorkEnvironment(parameters, tenant);

            MixPanelEvent mixPanelEvent = new MixPanelEvent();
            mixPanelEvent.Name = "login";
            mixPanelEvent.AddProperty("email", parameters.Email);
            mixPanelEvent.AddProperty("is_public", "False");
            mixPanelEvent.AddProperty("environment", workEnvironment);
            return mixPanelEvent;
        }

        private static string GetWorkEnvironment(LoginParameters parameters, int tenant)
        {
            if (parameters.IsCargoTracking)
            {
                return "cargoTracking";
            }

            var workEnvironment = AmitalCloudSettingConfigration.GetWorkEnvironment();
            if (workEnvironment != "privatelabel")
            {
                return workEnvironment;
            }
            GlobalTenantQueryService globalTenantQueryService = new GlobalTenantQueryService(tenant);
            string privateLabelId = globalTenantQueryService.GetSingle(tenant, false, false).PrivateLabelId;
            TenantManagmentPrivateLabelsQueryService tenantManagmentPrivateLabelsQuery = new TenantManagmentPrivateLabelsQueryService(tenant);
            return tenantManagmentPrivateLabelsQuery.GetSingle(privateLabelId, false, false)?.PrivateLabelName ?? "";
        }
    }
}