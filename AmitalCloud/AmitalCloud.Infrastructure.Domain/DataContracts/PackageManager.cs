using AmitalCloud.Infrastructure.Model.EntityClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmitalCloud.Infrastructure.Domain.DataContracts
{
    public class PackageManager
    {
        public int Tenant { get; set; }
        public string LoggedUserId { get; set; }=string.Empty;
        public bool IsCustomerCare { get; set; }
        public string MainPackageCode { get; set; } = string.Empty;
        public bool IsMultiPackage { get; set; }
        public bool MainAdditionalPackageApplied { get; set; }
        public bool IsUserAdditionalOnly { get; set; }

        public HashSet<string> BasePackagesCodes { get; set; } = new();
        public List<string> AddonsPackagesCodes { get; set; } = new();
        public List<string> AdditionalPackagesCodes { get; set; } = new();
    }

}
