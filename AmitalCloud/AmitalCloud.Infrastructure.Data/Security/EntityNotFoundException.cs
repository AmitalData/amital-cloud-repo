using System;

namespace AmitalCloud.Infrastructure.Data.Security
{
    public class EntityNotFoundException : Exception
    {
        public EntityNotFoundException()
        {
        }
        public EntityNotFoundException(string errorMessage) : base(errorMessage)
        {
        }
    }
}

