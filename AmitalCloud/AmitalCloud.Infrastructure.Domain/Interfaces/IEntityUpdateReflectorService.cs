using AmitalCloud.Infrastructure.Domain.Helpers;

namespace AmitalCloud.Infrastructure.Domain.Interfaces
{
    public interface IEntityUpdateReflectorService
    {
        void UpdateEntity(object entityPM, string entityName, int tenant);
        void UpdateEntity(UpdateEntityArgs updateEntityArgs);
    }

}
