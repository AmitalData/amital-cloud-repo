using AmitalCloud.Infrastructure.Domain.Interfaces;
using POCO = AmitalCloud.Infrastructure.Model.EntityClasses;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Domain.EntityLists;
using AutoMapper;
using AmitalCloud.Infrastructure.Data.BaseClasses;
using AmitalCloud.Infrastructure.Model.EntityClasses;

namespace AmitalCloud.Infrastructure.Data.EntityDataMappings
{
    public partial class GlobalContactDataMapping : BaseMappingProfile<GlobalContactPM, POCO.GlobalContact>, IMapping<GlobalContactPM, POCO.GlobalContact, GlobalContactList>
    {
        protected override void ApplyCustomMapping(IMappingExpression<POCO.GlobalContact, GlobalContactPM> map)
        {
            base.ApplyCustomMapping(map);

            map.ForMember(dest => dest.GlobalTenant, opt => opt.Ignore());
        }
    }
}
