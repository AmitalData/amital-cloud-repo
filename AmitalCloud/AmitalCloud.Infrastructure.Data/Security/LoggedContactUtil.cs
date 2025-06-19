using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Data.Queries;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System.Web;

namespace AmitalCloud.Infrastructure.Data.Security
{
    public class LoggedContactUtil : ILoggedContactUtil
    {

        public ContactPM GetLoggedContact(int tenant)
        {
            string userIdentityName;

            string userEmailSetByReportWR = AuthenticationUtil.AuthenticatedUserEmail;
            if (userEmailSetByReportWR != null)
                userIdentityName = userEmailSetByReportWR;
            else
                userIdentityName = AuthenticationUtil.ResolveUserIdentityName(tenant); // fix by Islam for 69295 Wrong user name recorded in events of AR invoices

            string key = $"GetLoggedContact({userIdentityName}{tenant})";
            var loggedContact = CacheManager.GetOrInsertNewObject<ContactPM>(key, () =>
            {
                var res = GetLoggedContactNoValidCache(tenant);
                return res;
            }, true);
            return loggedContact;

        }
        ContactPM GetLoggedContactNoValidCache(int tenant)
        {
            ContactPM loggedContact = null;
            try
            {
                if (HttpContextHelper.HttpContext != null)
                {
                    loggedContact = new ContactQuery(tenant).GetContactByEmailOnly(AuthenticationUtil.ResolveUserIdentityName(tenant), tenant);


                    // add by Abdullah & Mohammad to fix customer care contact issue, BUG 48520
                    // if (null and tenant <> 0) - customer care
                    //      get it by tenant 0 
                    if (tenant != 0 && loggedContact == null)
                    {
                        loggedContact = new ContactQuery(tenant).GetContactByEmailOnly(AuthenticationUtil.ResolveUserIdentityName(tenant), 0);
                    }
                    // note: the user will be found by method (GetContactByEmailOnly) in the cache 

                }
                else
                {
                    if (!string.IsNullOrEmpty(AuthenticationUtil.AuthenticatedUserEmail))
                        loggedContact = new ContactQuery(tenant).GetSingleByEmail(AuthenticationUtil.AuthenticatedUserEmail, tenant);
                    else
                        loggedContact = new ContactQuery(tenant).GetSingleByEmail("system@tenant" + tenant + ".com", tenant);
                }
            }
            catch { }


            loggedContact = loggedContact ?? new ContactPM();// { DontShowLocal = true };
            return loggedContact;
        }
    }
}
