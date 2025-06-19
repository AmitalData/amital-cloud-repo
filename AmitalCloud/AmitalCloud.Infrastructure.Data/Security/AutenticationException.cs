using System;

namespace AmitalCloud.Infrastructure.Data.Security
{
    public class AutenticationException : Exception
    {
        //string Message;
        public AutenticationException()
        {

        }
        public AutenticationException(string errorMessage) : base(errorMessage)
        {

        }
    }
}