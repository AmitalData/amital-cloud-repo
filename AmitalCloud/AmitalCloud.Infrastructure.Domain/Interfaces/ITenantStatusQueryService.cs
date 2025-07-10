using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmitalCloud.Infrastructure.Domain.Interfaces
{
	public interface ITenantStatusQueryService
	{
		TenantStatusPM GetTenantStatusPM(int tenant, string userId);
	}
}
