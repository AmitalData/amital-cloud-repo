using System.Collections.Generic;

namespace AmitalCloud.Infrastructure.Domain.DataContracts
{
    public class AmitalRestrictOwnerModel
    {
        public AmitalRestrictOwnerModel()
        {
            Cards = new List<string>();
        }
        //key
        public int Tenant { get; set; }
        public string UnifreightUserId { get; set; }

        /// vAlue


        public bool IsRestrictedOwner { get; set; }
        public List<string> Cards { get; set; }
    }


}
