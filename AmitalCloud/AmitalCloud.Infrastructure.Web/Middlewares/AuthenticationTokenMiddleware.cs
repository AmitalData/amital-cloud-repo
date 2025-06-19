using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using System.Security.Principal;

namespace AmitalCloud.Infrastructure.Web.Middlewares
{
    public class AuthenticationTokenMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenticationTokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var headers = context.Request.Headers;

            if (headers.TryGetValue("Token", out var tokenValues))
            {
                string token = tokenValues.FirstOrDefault();

                if (!string.IsNullOrEmpty(token))
                {
                    AuthenticationToken authToken = AuthenticationTokenRepository.GetSingleTokenFromCache(token);

                    if (authToken != null)
                    {
                        context.Items["authToken"] = authToken;
                        context.Items["Tenant"] = authToken.Tenant;

                        if (!authToken.APIToken && !authToken.InActive)
                        {
                            var identity = new GenericIdentity(authToken.Email);
                            var principal = new GenericPrincipal(identity, roles: null);
                            context.User = principal;
                        }
                    }
                }
            }

            await _next(context);
        }
    }
}
