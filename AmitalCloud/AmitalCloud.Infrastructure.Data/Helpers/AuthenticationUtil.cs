using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.Exceptions;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Threading;
using System.Web;
using AmitalCloud.Infrastructure.Model.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public class AuthenticationUtil
    {

        [ThreadStatic] public static string AuthenticatedUserEmail = string.Empty;

        public static string GetIP4Address()
        {
            string IP4Address = String.Empty;

            string currentIP = HttpContextHelper.Request.Headers["X-Real-IP"];
            if (string.IsNullOrEmpty(currentIP))
            {
                currentIP = HttpContextHelper.HttpContext?.Connection?.RemoteIpAddress?.ToString();
            }
            foreach (IPAddress IPA in Dns.GetHostAddresses(currentIP))
            {
                if (IPA.AddressFamily.ToString() == "InterNetwork")
                {
                    IP4Address = IPA.ToString();
                    break;
                }
            }

            if (IP4Address != String.Empty)
            {
                return IP4Address;
            }

            foreach (IPAddress IPA in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (IPA.AddressFamily.ToString() == "InterNetwork")
                {
                    IP4Address = IPA.ToString();
                    break;
                }
            }

            return IP4Address;
        }


        public static string GetLoggedUserEmail(int tenant)
        {
            string email = "";
            if (!string.IsNullOrEmpty(AuthenticatedUserEmail))
            {
                email = AuthenticatedUserEmail;
                return email;
            }
            if (HttpContextHelper.User != null && HttpContextHelper.User.Identity != null && !string.IsNullOrEmpty(HttpContextHelper.User.Identity.Name))
            {
                email = HttpContextHelper.User.Identity.Name;
            }
            else
            {
                email = "system@tenant" + tenant.ToString() + ".com";
            }

            return email;
        }



        public static string GetAuthenticatedUser()
        {
            if (!string.IsNullOrEmpty(HttpContextHelper.User?.Identity?.Name))
            {
                return HttpContextHelper.User.Identity.Name;
            }
            else
            {
                string token = HttpContextHelper.Request.Headers["Token"];
                if (!string.IsNullOrEmpty(token))
                {
                    AuthenticationToken authToken = AuthenticationTokenRepository.GetSingleTokenFromCache(token);
                    if (authToken != null)
                        return authToken.Email;
                }

                throw new Exception("Sorry! this user is not authorized!");
            }
        }
        private string ResolveLoggingUserId1(int Tenant)
        {
            string systemEmail = SystemIdentityName(Tenant);
            //var contactQuery = new ContactQuery(tenant);
            var loggedContact = "";
            //contactQuery.GetContactByNameAndTenant(serviceContextUser, tenant, true);
            return loggedContact;
        }
        public static string SystemIdentityName(int Tenant)
        {
            string systemEmail = "system@tenant" + Tenant.ToString() + ".com";
            return systemEmail;
        }

        public static string ResolveSystemUserId(int Tenant)
        {
            string systemEmail = SystemIdentityName(Tenant);
            if (String.IsNullOrWhiteSpace(systemEmail))
            {
                return null;
            }

            IRepository<Contact> contactRep = new Repository<Contact>(Tenant);
            var contact = contactRep.GetMulti(d => d.Email == systemEmail && d.Tenant == Tenant).FirstOrDefault();
            if (contact == null)
            {
                throw new BusinessErrorException("could not ResolveUserId from  Tenant");
            }
            return contact.Id;
        }

        public static void test()
        {
            var i = System.Threading.Thread.CurrentPrincipal.Identity;
            GenericIdentity identity = new GenericIdentity("M.Brown", "gg");
            //identity.IsAuthenticated = true;

            // ...

            System.Threading.Thread.CurrentPrincipal =
                new GenericPrincipal(
                    identity,
                    new string[] { "Role1", "Roll2" }
                    );

            //http://msdn.microsoft.com/en-us/library/ff647404.aspx
            var name = WindowsIdentity.GetCurrent().Name;

            //WindowsIdentity.Impersonate 
            //IClaimsPrincipal claimsPrincipal = HttpContextHelper.User as IClaimsPrincipal;
            HttpContextHelper.User = new GenericPrincipal(
                    identity,
                    new string[] { "Role1", "Roll2" }
                    ); ;
        }

        public static string ResolveUserIdentityName(int Tenant)
        {
            string systemEmail = SystemIdentityName(Tenant);
            string defaultName = "";
            if (HttpContextHelper.User != null)
            {
                if (!string.IsNullOrEmpty(HttpContextHelper.User.Identity.Name))
                {
                    defaultName = HttpContextHelper.User.Identity.Name;
                }

            }

            if (String.IsNullOrWhiteSpace(defaultName))
            {
                defaultName = GetThreadCurrentPrincipalIdentityName(); ;// Thread.CurrentPrincipal.Identity.Name;
            }
            if (String.IsNullOrWhiteSpace(defaultName))
            {
                defaultName = systemEmail;
            }
            return defaultName;
        }
        public static string ResolveUserId(int Tenant, bool fromSign = false)
        {
            IRepository<Contact> contactRep = new Repository<Contact>(Tenant);

            string resolveUserIdentityName = ResolveUserIdentityName(Tenant);


            var contact = contactRep.GetMulti(d => d.Email == resolveUserIdentityName && d.Tenant == Tenant).FirstOrDefault();                                    //GetSingleContactByEmail(resolveUserIdentityName, Tenant, true);
            if (contact == null && fromSign)
            {
                string systemEmail = SystemIdentityName(Tenant);
                contact = contactRep.GetMulti(d => d.Email == systemEmail && d.Tenant == Tenant).FirstOrDefault();   //.GetSingleContactByEmail(systemEmail, Tenant, false);
            }
            if (contact == null)
            {
                throw new BusinessErrorException($" resolveUserIdentityName :{resolveUserIdentityName} could not ResolveUserId from  Tenant:{Tenant}");
            }
            return contact.Id;
        }
        public static string ResolveUnifreightUserId(int Tenant)
        {
            //<--- Yuval Chalup 16.06.2015 TASK-13974 - CHANGED (client to user) FROM:
            //ContactRepository contactRep = new ContactRepository(Tenant);

            //string resolveUserIdentityName = AuthenticationUtil.ResolveUserIdentityName(Tenant);

            //var contact = contactRep.GetSingleContactByEmail(resolveUserIdentityName, Tenant, true);
            //if (contact == null)
            //{
            //    return null;
            //}
            //if (!String.IsNullOrWhiteSpace(contact.ExternalId))
            //{
            //    return contact.ExternalId;
            //}
            //return null;//system
            //TO:
            string resolveUserIdentityName = AuthenticationUtil.ResolveUserIdentityName(Tenant);

            IRepository<User> userRep = new Repository<User>(Tenant);
            User user = userRep.GetMulti(d => d.Contact.Email == resolveUserIdentityName && d.Tenant == Tenant).FirstOrDefault();             //.GetSingleUserByEmail(resolveUserIdentityName, Tenant, true);
            if (user == null)
            {
                return null;
            }
            if (!String.IsNullOrWhiteSpace(user.Code))
            {
                return user.Code;
            }
            return null;//system
            //Yuval Chalup 16.06.2015 TASK-13974 --->
        }

        public static string ResolveUserIdOfUnifreightUser(int Tenant)//Eitan H task 35737
        {
            string resolveUserIdentityName = AuthenticationUtil.ResolveUserIdentityName(Tenant);

            IRepository<User> userRep = new Repository<User>(Tenant);
            User user = userRep.GetMulti(d => d.Contact.Email == resolveUserIdentityName && d.Tenant == Tenant).FirstOrDefault();
            if (user == null)
            {
                return null;
            }
            if (!String.IsNullOrWhiteSpace(user.Id))
            {
                return user.Id;
            }
            return null;//system
        }
        public static string ResolveUnifreightUserById(string Id, int Tenant)
        {
            IRepository<Contact> contactRep = new Repository<Contact>(Tenant);
            var contact = contactRep.GetMulti(d => d.Id == Id && d.Tenant == Tenant).FirstOrDefault();
            if (contact == null)
            {
                return null;
            }
            if (!String.IsNullOrWhiteSpace(contact.ExternalId))
            {
                return contact.ExternalId;
            }
            return null;//system
        }

        //[PrincipalPermission(SecurityAction.Demand,Role = "Role1")]
        public static bool UnifreightImpersonateOld(string MoreParams, out int iTenanat, out string contactEmail)
        {
            iTenanat = -999;
            contactEmail = "";
            try
            {
                //test();
                var unifreightListsParams = UnifreightListsUtil.Deserialize(MoreParams);
                var tenant = UnifreightListsUtil.GetValue(ref unifreightListsParams, "tenant");
                var UniUser = UnifreightListsUtil.GetValue(ref unifreightListsParams, "UNIFREIGHT_USER_ID");
                if (String.IsNullOrWhiteSpace(tenant) || string.IsNullOrWhiteSpace(UniUser))
                {
                    return false;
                }

                if (!int.TryParse(tenant, out iTenanat))
                {
                    return false;
                }

                int tenant1 = iTenanat;
                IRepository<Contact> contactRep = new Repository<Contact>(iTenanat);
                var contact = contactRep.GetMulti(a => a.Tenant == tenant1 && a.ExternalId == UniUser).FirstOrDefault();    //GetSingleContactByExternalId(UniUser, iTenanat);
                if (contact == null)
                {
                    contactEmail = SystemIdentityName(iTenanat);
                }
                else
                {
                    contactEmail = contact.Email;
                }



                GenericIdentity identity = new GenericIdentity(contactEmail, "UnifreightImpersonate");




                var myPrincipal = new GenericPrincipal(
                    identity,
                    new string[] { "UnifreightRole", "TenantRole=" + tenant }
                    );




                AppDomain.CurrentDomain.SetThreadPrincipal(myPrincipal);



                System.Threading.Thread.CurrentPrincipal = myPrincipal; //myPrincipal;
                if (HttpContextHelper.HttpContext != null)
                {
                    HttpContextHelper.User = myPrincipal;

                }

                return true;
            }
            catch (Exception)
            {

                return false;
            }

        }
        public static bool UnifreightImpersonate(string MoreParams, out int iTenanat, out string contactEmail)
        {
            iTenanat = -999;
            contactEmail = "";
            try
            {
                //test();
                var unifreightListsParams = UnifreightListsUtil.Deserialize(MoreParams);
                var tenant = UnifreightListsUtil.GetValue(ref unifreightListsParams, "tenant");
                var UniUser = UnifreightListsUtil.GetValue(ref unifreightListsParams, "UNIFREIGHT_USER_ID");
                if (String.IsNullOrWhiteSpace(tenant) || string.IsNullOrWhiteSpace(UniUser))
                {
                    return false;
                }

                if (!int.TryParse(tenant, out iTenanat))
                {
                    return false;
                }


                ///ContactRepository contactRep = new ContactRepository(iTenanat);
                ///
                int tenant1 = iTenanat;
                var userRepository = new Repository<User>(tenant1);
                var //contact =
                    user =
                    //contactRep.GetSingleContactByExternalId(UniUser, iTenanat);
                    userRepository.GetMulti(d => d.Contact.ExternalId == UniUser && d.Tenant == tenant1).FirstOrDefault();            //.GetSingleUserByCode(UniUser, iTenanat, false);

                if (user == null)
                {
                    //yaron say int system !!!   ///throw new BusinessErrorException("UniUser, iTenanat not exist ");
                    //yaron say int system !!!   return false;
                    contactEmail = SystemIdentityName(iTenanat);

                }
                else
                {
                    if (user.Contact != null)
                    {
                        contactEmail = user.Contact.Email;
                    }
                    else
                    {
                        contactEmail = SystemIdentityName(iTenanat);
                    }

                }


                return Impersonate(iTenanat, contactEmail, UniUser);
            }
            catch (Exception)
            {

                return false;
            }

        }

        public static bool Impersonate(int iTenanat, string contactEmail, string UniUser)
        {
            var claimsPrincipal = new ClaimsPrincipal();
            var claims = new List<Claim>
                {
                    new Claim("UniUser", UniUser),
                    new Claim("Tenant", iTenanat.ToString()  ),
                    new Claim(ClaimTypes.Name, contactEmail),
                    new Claim(ClaimTypes.Email, contactEmail),

                };
            var claimIdentity = new ClaimsIdentity(claims);

            claimsPrincipal.AddIdentity(claimIdentity);
            Thread.CurrentPrincipal = claimsPrincipal;






            try
            {
                //AppDomain.CurrentDomain.SetThreadPrincipal(null);
                //AppDomain.CurrentDomain.SetThreadPrincipal(claimsPrincipal);
            }
            catch (Exception)
            {

                //throw;
            }




            System.Threading.Thread.CurrentPrincipal = claimsPrincipal; //myPrincipal;
            if (HttpContextHelper.User != null)
            {
                HttpContextHelper.User = claimsPrincipal;

            }

            return true;
        }
        public static int? GetThreadCurrentPrincipalTenant()
        {

            if (Thread.CurrentPrincipal.Identity == null)
            {
                return null;
            }
            Claim tenant = ClaimsPrincipal.Current.FindFirst("Tenant");
            if (tenant == null)
            {
                return null;
            }
            var sTenant = tenant.Value;
            var i = int.Parse(sTenant);
            return i;
        }

        public static string GetThreadCurrentPrincipalIdentityName()
        {

            if (Thread.CurrentPrincipal.Identity == null)
            {
                return null;
            }
            return Thread.CurrentPrincipal.Identity.Name;
        }

        public static void DebugUsers()
        {

            var principal = ClaimsPrincipal.Current;

            //Claim userName = principal.FirstOrDefault("UserName");

            var claimsEmail = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            bool hasEmailClaim = principal.HasClaim(c => c.Type == ClaimTypes.Email);
            if (hasEmailClaim)
            {
                NetCommonHelper.Logger.DevLog.Instance.WriteDebug("hasEmailClaim:" + claimsEmail.Value);
            }

            NetCommonHelper.Logger.DevLog.Instance.WriteDebug(string.Format("GetCurrentPrincipalTenant():{0}", GetThreadCurrentPrincipalTenant()));


            NetCommonHelper.Logger.DevLog.Instance.WriteDebug("UserName:" + Environment.UserName);
            NetCommonHelper.Logger.DevLog.Instance.WriteDebug("Thread.CurrentPrincipal.Identity.Name:" + Thread.CurrentPrincipal.Identity.Name);
            if (HttpContextHelper.User != null)
            {
                NetCommonHelper.Logger.DevLog.Instance.WriteDebug("HttpContextHelper.User.Identity.Name:" + HttpContextHelper.User.Identity.Name);
            }

            NetCommonHelper.Logger.DevLog.Instance.WriteDebug("WindowsIdentity.GetCurrent().Name:" + WindowsIdentity.GetCurrent().Name);
        }

        public static bool IsResolveUserIdentityNameEqualSystem(int tenant)
        {
            var resolveUserIdentityName = ResolveUserIdentityName(tenant);
            var systemIdentityName = SystemIdentityName(tenant);
            return resolveUserIdentityName.Equals(systemIdentityName, StringComparison.OrdinalIgnoreCase);
        }

        public static string ResolveLoggingUserId(int Tenant)
        {
            string systemEmail = "system@tenant" + Tenant.ToString() + ".com";
            string defaultName = "";
            if (HttpContextHelper.User != null)
            {
                if (!string.IsNullOrEmpty(HttpContextHelper.User.Identity.Name))
                {
                    defaultName = HttpContextHelper.User.Identity.Name;
                }
            }
            if (String.IsNullOrWhiteSpace(defaultName))
            {
                defaultName = systemEmail;
            }
            return defaultName;
        }

        public static bool IsAuthenticatedUserExists()
        {
            bool exists = false;
            if (HttpContextHelper.User != null)
            {
                if (!string.IsNullOrEmpty(HttpContextHelper.User.Identity.Name))
                {
                    exists = true;
                }
            }

            return exists;
        }

        public static ContactPassword? VerifyContactPassword(string email, string? password, bool isHashPassword = false)
        {
            IGlobalContext globalContext = GlobalContext.GetContext();
            ContactPassword? contactPassword = null;
            if (globalContext != null && !string.IsNullOrEmpty(email))
            {
                email = email.ToLower();
                string? hashedOldPassword = !string.IsNullOrEmpty(password) ? !isHashPassword ? PasswordGenerator.GetHashedPassword(email, password) : password : null;
                contactPassword = globalContext.ContactPasswords.Where(c => c.Email == email && !c.IsBCrypt && (string.IsNullOrEmpty(password) || c.Password == hashedOldPassword)  ).FirstOrDefault();

                if (contactPassword == null)
                {
                    contactPassword = globalContext.ContactPasswords.Where(c => c.Email.ToLower() == email && c.IsBCrypt).FirstOrDefault();
                    if (contactPassword != null && !string.IsNullOrEmpty(password))
                    {
                        var isValid = false;
                        if (!isHashPassword)
                        {
                            isValid = PasswordGenerator.VerifyBCryptHashedPassword(contactPassword.Email, password, contactPassword.Password);
                        }

                        else
                        {
                            isValid = contactPassword.Password == password ? true : false;
                        }
                        if (!isValid)
                        {
                            contactPassword = null;
                        }
                    }
                }
            }

            return contactPassword;
        }

        public static string GenerateToken()
        {
            int length = 26;
            string token = "";
            RandomNumberGenerator generator = RandomNumberGenerator.Create();
            byte[] rndArray = new byte[length];
            generator.GetBytes(rndArray);
            token = Convert.ToBase64String(rndArray);

            return token;
        }
    }
    public class SecurityException : Exception
    {
        public SecurityException(string message) : base(message)
        {

        }
    }
}
