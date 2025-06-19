using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public static class HttpContextHelper
    {
        private static IHttpContextAccessor? _contextAccessor;
        private static ClaimsPrincipal? _overriddenUser;

        public static void Initialize(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public static HttpContext HttpContext => _contextAccessor?.HttpContext;
        public static ClaimsPrincipal User
        {
            get
            {
                return _overriddenUser ?? _contextAccessor?.HttpContext?.User;
            }
            set
            {
                _overriddenUser = value;
            }
        }
        public static HttpRequest Request => _contextAccessor?.HttpContext?.Request;
        public static HttpResponse Response => _contextAccessor?.HttpContext?.Response;
        public static ISession Session => _contextAccessor?.HttpContext?.Session;

        public static void SetCookie(string userName, string userId, string tenant, bool isAuthenticated, string computerId)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim("UserId", userId),
                new Claim("CurrentTenant", tenant),
                new Claim("IsAuthenticated", isAuthenticated.ToString()),
                new Claim("ComputerId", computerId)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20160),
                IsPersistent = true,
                AllowRefresh = true
            };
            _contextAccessor?.HttpContext?.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);
        }
    }
}
