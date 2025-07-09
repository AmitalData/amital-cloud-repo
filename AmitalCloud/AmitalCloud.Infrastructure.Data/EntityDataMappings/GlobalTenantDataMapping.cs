using AmitalCloud.Infrastructure.Domain.Interfaces;
using POCO = AmitalCloud.Infrastructure.Model.EntityClasses;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Domain.EntityLists;
using AutoMapper;
using AmitalCloud.Infrastructure.Data.BaseClasses;
using AmitalCloud.Infrastructure.Model.EntityClasses;

namespace AmitalCloud.Infrastructure.Data.EntityDataMappings
{
    public partial class GlobalTenantDataMapping : BaseMappingProfile<GlobalTenantPM, POCO.GlobalTenant>, IMapping<GlobalTenantPM, POCO.GlobalTenant, GlobalTenantList>
    {
        protected override void ApplyCustomMapping(IMappingExpression<POCO.GlobalTenant, GlobalTenantPM> map)
        {
            base.ApplyCustomMapping(map);

            map.ForMember(dest => dest.GlobalDB, opt => opt.Ignore());
            map.ForMember(dest => dest.TenantManagmentPrivateLabels, opt => opt.Ignore());
            map.ForMember(dest => dest.GlobalContacts, opt => opt.Ignore());
        }
    }
}
