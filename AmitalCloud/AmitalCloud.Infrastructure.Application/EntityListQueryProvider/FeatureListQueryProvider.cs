using AmitalCloud.Infrastructure.Data.Context;
using AmitalCloud.Infrastructure.Domain.EntityLists;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

public class FeatureListQueryProvider : BaseEntityListQueryProvider<Feature, FeatureList>,
     IEntityListQueryProvider<Feature, FeatureList>
{
    public FeatureListQueryProvider(AmitalCloudContext context, IMapper mapper)
        : base(context, mapper)
    {
    }

    public override IQueryable<FeatureList> BuildQuery()
    {
        return _context.Set<Feature>()
                       .Include(f => f.FeatureType)
                       .Select(f => new FeatureList
                       {
                           Id = f.Id,
                           Tenant = f.Tenant,
                           Code = f.Code,
                           ObjectTableId = f.ObjectTableId,
                       });
    }
}
