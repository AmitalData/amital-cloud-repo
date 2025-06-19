using AmitalCloud.Infrastructure.Data.Helpers;

namespace AmitalCloud.Infrastructure.Web.Middlewares
{
    public class HttpContextHelperMiddleware
    {
        private readonly RequestDelegate _next;

        public HttpContextHelperMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IHttpContextAccessor contextAccessor)
        {
            HttpContextHelper.Initialize(contextAccessor);

            await _next(context);
        }
    }
}
