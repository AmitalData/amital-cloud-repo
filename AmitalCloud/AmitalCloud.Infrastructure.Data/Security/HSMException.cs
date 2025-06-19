using System;

namespace AmitalCloud.Infrastructure.Data.Security
{
    public class HSMException : Exception
    {
        public HSMException()
        {
        }
        public HSMException(string errorMessage, string errorCode) : base(errorMessage)
        {
        }
    }
}
