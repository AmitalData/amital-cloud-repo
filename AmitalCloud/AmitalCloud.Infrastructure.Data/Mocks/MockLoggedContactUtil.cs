using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Infrastructure.Domain.Interfaces;

namespace AmitalCloud.Infrastructure.Data.Mocks
{
    public class MockLoggedContactUtil : ILoggedContactUtil
    {
        public ContactPM GetLoggedContact(int tenant)
        {
            string expectedLoggedUserId = "myUser";
            ContactPM loggedcontact = new ContactPM()
            {
                Id = expectedLoggedUserId,
                //DontShowLocal = true,
            };

            return loggedcontact;
        }
    }
}
