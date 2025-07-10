using AmitalCloud.Infrastructure.Domain.Interfaces;

namespace AmitalCloud.Infrastructure.Web.BaseClasses
{
	public class TenantProvider : ITenantProvider
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public TenantProvider(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public int GetTenantId()
		{
			var tenantHeader = _httpContextAccessor.HttpContext?.Request.Headers["X-Tenant-Id"].FirstOrDefault();
			return int.TryParse(tenantHeader, out var tenantId) ? tenantId : 0;
		}
	}
}
