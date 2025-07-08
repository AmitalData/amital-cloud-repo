using AmitalCloud.Infrastructure.Domain.Interfaces;
using POCO = AmitalCloud.Infrastructure.Model.EntityClasses;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Domain.EntityLists;
using AutoMapper;
using AmitalCloud.Infrastructure.Data.BaseClasses;
using AmitalCloud.Infrastructure.Model.EntityClasses;

namespace AmitalCloud.Infrastructure.Data.EntityDataMappings
{
    public partial class AdvancedQueryFilterDataMapping : BaseMappingProfile<AdvancedQueryFilterPM, POCO.AdvancedQueryFilter>, IMapping<AdvancedQueryFilterPM, POCO.AdvancedQueryFilter, AdvancedQueryFilterList>
    {
        protected override void ApplyCustomMapping(IMappingExpression<POCO.AdvancedQueryFilter, AdvancedQueryFilterPM> map)
        {
            base.ApplyCustomMapping(map);

            map.ForMember(dest => dest.QueryObjectTableName, opt => opt.MapFrom(src => src.Query.ObjectTable.Name));
        }
    }
}
