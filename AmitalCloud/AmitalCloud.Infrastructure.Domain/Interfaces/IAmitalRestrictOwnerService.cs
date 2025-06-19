using AmitalCloud.Infrastructure.Domain.DataContracts;

namespace AmitalCloud.Infrastructure.Domain.Interfaces
{
    public interface IAmitalRestrictOwnerService
    {
        AmitalRestrictOwnerModel GetAmitalRestrictOwnerModel(bool getFromCache, int tenant = 1, string UnifreightUserId = null);
    }
}
