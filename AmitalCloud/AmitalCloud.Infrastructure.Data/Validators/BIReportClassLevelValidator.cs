using AmitalCloud.Infrastructure.Domain.Interfaces;
using System;
using System.Linq;
namespace AmitalCloud.Infrastructure.Data.Validators
{
    public partial class BIReportClassLevelValidator : IInfrastructureClassLevelValidator
    {
        int tenant;
        string objectTableName;
        string errorMessage = "";

        public BIReportClassLevelValidator(string objectTableName, int tenant)
        {
            this.objectTableName = objectTableName;
            this.tenant = tenant;
        }

        public bool IsValid(object value, object instance, string propertyName)
        {
            errorMessage = "";
            if (value != null)
            {
                string biReportName = (string)value.GetType().GetProperty("Name").GetValue(value);
                string[] speaceCharacters = new string[] { "+", "#", "&", "|", "/", "\\", ":", "*", "?", "<", ">", ('"').ToString() };
                if (speaceCharacters.Any(biReportName.Contains))
                {
                    errorMessage = "Name field can't contain the following special characters:" + string.Join("", speaceCharacters.ToArray());
                    return false;

                }
            }

            return true;
        }

        public string GetErrorMessage(object value, object instance, string property)
        {
            if (!String.IsNullOrEmpty(errorMessage))
            {
                return errorMessage;
            }
            return "";
        }

        public System.Collections.Generic.List<string> GetErrorsInObject(object instance)
        {
            throw new NotImplementedException();
        }

        public string GetErrorMessage(object instance, string property)
        {
            return "";
        }
    }
}
