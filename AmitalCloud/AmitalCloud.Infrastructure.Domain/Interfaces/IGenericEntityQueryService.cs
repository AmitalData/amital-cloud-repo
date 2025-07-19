using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmitalCloud.Infrastructure.Domain.Interfaces
{
    public interface IGenericEntityQueryService
    {
        object? GetSingle(string entityName, Dictionary<string, string> keyParams, int tenant);
    }
}
