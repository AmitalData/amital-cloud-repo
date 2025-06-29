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
            ApplyCustomMapping(mapPOCOtoPM);

            var mapPMtoPOCO = CreateMap<TEntityPM, TEntityPOCO>();
            ApplyCustomMapping(mapPMtoPOCO);

            CreateMap<TEntityPM, TEntityPM>();
        }

        public IMapper CreateMapper()
        {
            return new MapperConfiguration(cfg => cfg.AddProfile(this)).CreateMapper();
        }

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

        private readonly List<string> ProtectedFields = new List<string> { "Tenant", "SearchFields", "CreatedByUserId", "CreatedDate" };
        protected List<string> GetProtectedPropertyNames()
        {
            // get the entity keys
            var keys = typeof(TEntityPOCO)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => Attribute.IsDefined(p, typeof(KeyAttribute)))
                .Select(p => p.Name)
                .ToList();

            // add the protected fields to the keys list
            foreach ( var field in ProtectedFields)
            {
                if (typeof(TEntityPOCO).GetProperty(field) != null && !keys.Contains(field))
                    keys.Add(field);
            }

            return keys;
        }
    }
}

