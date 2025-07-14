using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Data.EntityDataMappings;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using System.Collections.Generic;
using System.Linq;

namespace AmitalCloud.Infrastructure.Data.Queries
{
    public class ScreenFieldQuery
    {
        private readonly Repository<ScreenField> repository;
        private readonly int tenant;
        public ScreenFieldQuery(int tenant)
        {
            this.tenant = tenant;
            repository = new Repository<ScreenField>(AmitalCloudContext.GetContext(tenant));
        }
        public List<ScreenFieldPM> GetScreenFieldPMsByTenant()
        {
            List<ScreenField> screenFields = repository.GetMultiFromCache($"GetScreenFieldPMsByTenant{tenant}", a => a.Tenant == tenant || a.Tenant == 0, "ObjectField,ObjectField.ObjectTable", a => a);
            var config = new MapperConfiguration(cfg => cfg.AddProfile(new ScreenFieldDataMapping()), new NullLoggerFactory());
            var mapper = config.CreateMapper();
            List<ScreenFieldPM> screenFieldPMs = mapper.Map<List<ScreenFieldPM>>(screenFields);

            List<ScreenFieldPM> selectedScreenFields = new List<ScreenFieldPM>();
            foreach (ScreenFieldPM field in screenFieldPMs)
            {
                ScreenFieldPM existedField = selectedScreenFields.Where(a => a.ScreenId == field.ScreenId && a.ObjectFieldCode == field.ObjectFieldCode).FirstOrDefault();

                if (existedField == null)
                {
                    selectedScreenFields.Add(field);
                }
                else if (existedField.Tenant == 0)
                {
                    selectedScreenFields.Remove(existedField);
                    selectedScreenFields.Add(field);
                }
            }

            return selectedScreenFields;
        }
    }
}
