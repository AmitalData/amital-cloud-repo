using AmitalCloud.Infrastructure.Domain.EntityKeys;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AmitalCloud.Infrastructure.Domain.Interfaces
{
    public interface IBaseQueryService<TEntityPM, TEntityPOCO ,TType> 
        where TEntityPM : IEntityPM, new()
        where TEntityPOCO : IEntity
    {
		TEntityPM GetSingle(TType id, bool getComposition, bool getFromCache);

    }
	
}
