using AmitalCloud.Infrastructure.Domain.Interfaces;
using POCO = AmitalCloud.Infrastructure.Model.EntityClasses;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Domain.EntityLists;
using AutoMapper;
using AmitalCloud.Infrastructure.Data.BaseClasses;
using AmitalCloud.Infrastructure.Model.EntityClasses;

namespace AmitalCloud.Infrastructure.Data.EntityDataMappings
{
    public partial class ObjectTableDataMapping : BaseMappingProfile<ObjectTablePM, POCO.ObjectTable>, IMapping<ObjectTablePM, POCO.ObjectTable, ObjectTableList>
    {
        protected override void ApplyCustomMapping(IMappingExpression<POCO.ObjectTable, ObjectTablePM> map)
        {
            base.ApplyCustomMapping(map);

            map.ForMember(dest => dest.FullNameTextCodeDefaultText,
               opt => opt.MapFrom(src =>
                   src.FullNameTextCode != null && !string.IsNullOrWhiteSpace(src.FullNameTextCode.DefaultText)
                       ? src.FullNameTextCode.DefaultText
                       : src.Name
               ));
        }
    }
}
