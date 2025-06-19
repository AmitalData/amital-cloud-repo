using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Domain.Interfaces;

namespace AmitalCloud.Infrastructure.Data.Helpers
{
    public class LoggedContactResolver
    {
        private readonly ILoggedContactUtil _loggedContactUtil;
        public LoggedContactResolver(ILoggedContactUtil loggedContactUtil)
        {
            _loggedContactUtil = loggedContactUtil;
        }
        public ContactPM GetLoggedContact(int tenant)
        {
            return _loggedContactUtil.GetLoggedContact(tenant);
        }
    }
}
