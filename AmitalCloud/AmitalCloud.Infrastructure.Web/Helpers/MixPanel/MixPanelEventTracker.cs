using AmitalCloud.Infrastructure.Application.EntityQueryServices;
using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using UAParser;

namespace AmitalCloud.Infrastructure.Web.Helpers.MixPanel
{
    public class MixPanelEventTracker
    {
        private static readonly HttpClient client = new HttpClient();
        private const string trackEventAPIURI = "https://api.mixpanel.com/track#live-event";
        string userEmail;
        string projectToken;
        private int tenant;
        private TenantManagementPM tenantManagement;

        public MixPanelEventTracker(string projectToken, string email, int tenant)
        {
            this.projectToken = projectToken;
            this.userEmail = email;
            this.tenant = tenant;
        }

        public async void TrackEvent(MixPanelEvent mixPanelEvent)
        {
            AddPrimaryEventFields(mixPanelEvent);

            FormUrlEncodedContent requestParameters = BuildHttpRequestParameters(mixPanelEvent);

            var trackEventResponse = await client.PostAsync(trackEventAPIURI, requestParameters);
            await trackEventResponse.Content.ReadAsStringAsync();
        }

        private void AddPrimaryEventFields(MixPanelEvent mixPanelEvent)
        {
            if (userEmail != null)
                mixPanelEvent.Properties.Add(new TrackingEventProperty("distinct_id", userEmail));
            mixPanelEvent.Properties.Add(new TrackingEventProperty("token", projectToken));
            mixPanelEvent.Properties.Add(new TrackingEventProperty("time", DateTimeOffset.Now.ToUnixTimeSeconds().ToString()));
            mixPanelEvent.Properties.Add(new TrackingEventProperty("browser", GetBrowserName()));
            mixPanelEvent.Properties.Add(new TrackingEventProperty("ip_address", GetIPAddress()));
            AddTenantProperties(mixPanelEvent);
        }

        private void AddTenantProperties(MixPanelEvent mixPanelEvent)
        {
            GetTenantManagement(tenant);
            mixPanelEvent.Properties.Add(new TrackingEventProperty("tenant-number", tenant.ToString()));
            mixPanelEvent.Properties.Add(new TrackingEventProperty("tenant-name", tenantManagement.Name));
            mixPanelEvent.Properties.Add(new TrackingEventProperty("tenant-url", tenantManagement.CustomerURL));
        }

        private static FormUrlEncodedContent BuildHttpRequestParameters(MixPanelEvent mixPanelEvent)
        {
            var parametersDictionary = new Dictionary<string, string> { { "data", mixPanelEvent.ToString() } };
            return new FormUrlEncodedContent(parametersDictionary);
        }

        private static string GetBrowserName()
        {
            if (HttpContextHelper.HttpContext == null)
                return null;

            var userAgent = HttpContextHelper.Request.Headers["User-Agent"].ToString();
            if (string.IsNullOrEmpty(userAgent))
                return null;

            var uaParser = Parser.GetDefault();
            ClientInfo clientInfo = uaParser.Parse(userAgent);
            return $"{clientInfo.UA.Family} {clientInfo.UA.Major}";
        }

        private static string GetIPAddress() => AuthenticationUtil.GetIP4Address();

        private void GetTenantManagement(int tenant)
        {
            TenantManagementQueryService tenantManagementQuery = new TenantManagementQueryService(tenant);
            tenantManagement = tenantManagementQuery.GetSingle(tenant, false, false);
        }
    }
}
