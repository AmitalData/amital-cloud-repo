using System.Net;
using AmitalCloud.Infrastructure.Data.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AmitalCloud.Infrastructure.Web.Helpers
{
    public class AuthenticationExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is AutenticationException)
            {
                var response = new
                {
                    Message = "Access Denied",
                    ErrorType = nameof(AutenticationException),
                    ExceptionMessage = context.Exception.Message,
                };

                context.Result = new ObjectResult(response)
                {
                    StatusCode = (int)HttpStatusCode.Unauthorized
                };


                NetCommonHelper.Logger.DevLog.Instance.WriteError($"access denied: {context.Exception.Message}");

                context.ExceptionHandled = true;
            }
        }
    }
}