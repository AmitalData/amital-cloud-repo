using AutoMapper;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace AmitalCloud.Infrastructure.Data.BaseClasses
{
    public abstract class BaseMappingProfile<TEntityPM, TEntityPOCO> : Profile
    {
        public BaseMappingProfile()
        {
            var mapPOCOtoPM = CreateMap<TEntityPOCO, TEntityPM>();
            ApplyGeneratedMapping(mapPOCOtoPM);
            ApplyCustomMapping(mapPOCOtoPM);

            var mapPMtoPOCO = CreateMap<TEntityPM, TEntityPOCO>();
            ApplyCustomMapping(mapPMtoPOCO);

            CreateMap<TEntityPM, TEntityPM>();
        }

        protected virtual void ApplyGeneratedMapping(IMappingExpression<TEntityPOCO, TEntityPM> map) { }
        protected virtual void ApplyCustomMapping(IMappingExpression<TEntityPOCO, TEntityPM> map) { }
        protected virtual void ApplyCustomMapping(IMappingExpression<TEntityPM, TEntityPOCO> map)
        {
            foreach (var keyProp in GetProtectedPropertyNames())
            {
                var member = typeof(TEntityPOCO).GetProperty(keyProp);
                if (member == null) continue;

                map.ForMember(keyProp, opt => opt.Condition((src, destObj, srcMember, destMember) => destMember == null));

            }
        }

        private readonly HashSet<string> ProtectedFields = new(StringComparer.OrdinalIgnoreCase)
        {
            "Tenant", "SearchFields", "CreatedByUserId", "CreatedDate"
        };
        protected List<string> GetProtectedPropertyNames()
        {
            var props = typeof(TEntityPOCO).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);

            var keys = props.Where(p => Attribute.IsDefined(p.Value, typeof(KeyAttribute))).Select(p => p.Key).ToList();

            foreach (var field in ProtectedFields)
            {
                if (props.ContainsKey(field) && !keys.Contains(field))
                    keys.Add(field);
            }

            return keys;
        }
    }
}

