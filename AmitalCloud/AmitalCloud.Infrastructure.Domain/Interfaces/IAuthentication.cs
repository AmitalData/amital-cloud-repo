using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;


namespace AmitalCloud.Infrastructure.Domain.Interfaces
{
	public interface IAuthentication
	{
		 bool GetIsBlockingFromDB(HttpContext httpContext);
	}
}
