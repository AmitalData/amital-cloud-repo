using AmitalCloud.Infrastructure.Domain.Interfaces;
using POCO = AmitalCloud.Infrastructure.Model.EntityClasses;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Domain.EntityLists;
using AutoMapper;
using AmitalCloud.Infrastructure.Data.BaseClasses;

namespace AmitalCloud.Infrastructure.Data.EntityDataMappings
{
    public partial class TenantLoginPolicyDataMapping : BaseMappingProfile<TenantLoginPolicyPM, POCO.TenantLoginPolicy>, IMapping<TenantLoginPolicyPM, POCO.TenantLoginPolicy, TenantLoginPolicyList>
    {
        protected override void ApplyCustomMapping(IMappingExpression<POCO.TenantLoginPolicy, TenantLoginPolicyPM> map)
        {
            base.ApplyCustomMapping(map);

            map.ForMember(dest => dest.LoginPolicy, opt => opt.Ignore());
        }
    }
}
