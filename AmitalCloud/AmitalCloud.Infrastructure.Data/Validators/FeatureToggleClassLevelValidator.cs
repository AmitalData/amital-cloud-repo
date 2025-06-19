using AmitalCloud.Infrastructure.Domain.Interfaces;
using System;
namespace AmitalCloud.Infrastructure.Data.Validators
{
    public class FeatureToggleClassLevelValidator : IInfrastructureClassLevelValidator
    {

        string errorMessage = "";
        public FeatureToggleClassLevelValidator()
        {

        }

        public bool IsValid(object value, object instance, string propertyName)
        {
            errorMessage = "";
            if (value != null)
            {
                bool isMultiTenant = (bool)value.GetType().GetProperty("IsMultiTenant").GetValue(value);
                if (isMultiTenant)
                {
                    int? fromTenantNumber = (int?)value.GetType().GetProperty("FromTenantNumber").GetValue(value);
                    int? toTenantNumber = (int?)value.GetType().GetProperty("ToTenantNumber").GetValue(value);
                    if (isMultiTenant && (fromTenantNumber > toTenantNumber))
                    {
                        errorMessage = "From Tenant cannot be greater than the To Tenant";
                        return false;
                    }
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
