using AmitalCloud.Infrastructure.Domain.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmitalCloud.Infrastructure.Domain.Interfaces
{
	public interface ISystemMetadataLastUpdateQuery
	{
		MetaDataLastUpdateDates GetSystemMetadataLastUpdatesCacheHandle();
	}
}
