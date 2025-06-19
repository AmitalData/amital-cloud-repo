using System;
using System.Collections.Generic;

namespace AmitalCloud.Infrastructure.Domain.Interfaces
{
    public class Response
    {
        public Response()
        {
            this.ValidationErrors = new List<string>();
        }

        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }
        public string InnerErrorMessage { get; set; }
        public string Result { get; set; }
        public string Result2 { get; set; }
        public bool IsAuthenticationError { get; set; }

        List<String> validationErrors;

        public List<String> ValidationErrors
        {
            get
            {
                if (validationErrors == null)
                {
                    validationErrors = new List<string>();
                }
                return validationErrors;
            }

            set { validationErrors = value; }
        }


    }
}
