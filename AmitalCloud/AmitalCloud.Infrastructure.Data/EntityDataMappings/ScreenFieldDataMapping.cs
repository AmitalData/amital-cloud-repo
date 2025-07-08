using AmitalCloud.Infrastructure.Domain.Interfaces;
using POCO = AmitalCloud.Infrastructure.Model.EntityClasses;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Domain.EntityLists;
using AutoMapper;
using AmitalCloud.Infrastructure.Data.BaseClasses;

namespace AmitalCloud.Infrastructure.Data.EntityDataMappings
{
    public partial class ScreenFieldDataMapping : BaseMappingProfile<ScreenFieldPM, POCO.ScreenField>, IMapping<ScreenFieldPM, POCO.ScreenField, ScreenFieldList>
    {
        protected override void ApplyCustomMapping(IMappingExpression<POCO.ScreenField, ScreenFieldPM> map)
        {
            base.ApplyCustomMapping(map);

            map.ForMember(dest => dest.ObjectFieldObjectTableName, opt => opt.MapFrom(src => src.ObjectField.ObjectTable.Name));
        }
    }
}
