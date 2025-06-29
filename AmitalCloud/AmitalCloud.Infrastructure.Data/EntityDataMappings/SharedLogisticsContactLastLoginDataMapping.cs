using AmitalCloud.Infrastructure.Domain.Interfaces;
using POCO = AmitalCloud.Infrastructure.Model.EntityClasses;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Domain.EntityLists;
using AutoMapper;
using AmitalCloud.Infrastructure.Data.BaseClasses;

namespace AmitalCloud.Infrastructure.Data.EntityDataMappings
{
    public partial class SharedLogisticsContactLastLoginDataMapping : BaseMappingProfile<SharedLogisticsContactLastLoginPM, POCO.SharedLogisticsContactLastLogin>, IMapping<SharedLogisticsContactLastLoginPM, POCO.SharedLogisticsContactLastLogin, SharedLogisticsContactLastLoginList>
    {
        protected override void ApplyCustomMapping(IMappingExpression<SharedLogisticsContactLastLoginPM, POCO.SharedLogisticsContactLastLogin> map)
        {
            base.ApplyCustomMapping(map);

            map.ForMember(dest => dest.PartnerTypeId, opt => opt.Condition((src, dest, srcMember, destMember) => destMember == null))
                .ForMember(dest => dest.ContactId, opt => opt.Condition((src, dest, srcMember, destMember) => destMember == null))
                .ForMember(dest => dest.CardId, opt => opt.Condition((src, dest, srcMember, destMember) => destMember == null))
                .ForMember(dest => dest.Via, opt => opt.Condition((src, dest, srcMember, destMember) => destMember == null));
        }
    }
}
