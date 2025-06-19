using AmitalCloud.Infrastructure.Domain.EntityPMs;

namespace AmitalCloud.Infrastructure.Domain.Interfaces
{
    public interface ILoggedContactUtil
    {
        ContactPM GetLoggedContact(int tenant);
    }
}
