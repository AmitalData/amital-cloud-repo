using AmitalCloud.Infrastructure.Domain.Interfaces;
using POCO = AmitalCloud.Infrastructure.Model.EntityClasses;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Domain.EntityLists;
using AutoMapper;
using AmitalCloud.Infrastructure.Data.BaseClasses;
using AmitalCloud.Infrastructure.Model.EntityClasses;

namespace AmitalCloud.Infrastructure.Data.EntityDataMappings
{
    public partial class TextCodeDataMapping : BaseMappingProfile<TextCodePM, POCO.TextCode>, IMapping<TextCodePM, POCO.TextCode, TextCodeList>
    {
        protected override void ApplyCustomMapping(IMappingExpression<POCO.TextCode, TextCodePM> map)
        {
            base.ApplyCustomMapping(map);

            map.ForMember(dest => dest.SpellCheckedByUserName, opt => opt.MapFrom(src => src.SpellCheckedByUser.Contact.EnglishName));
        }
    }
}
