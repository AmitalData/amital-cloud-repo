using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace AmitalCloud.Infrastructure.Data.Validators
{
    public partial class ValidationClass : IValidateContext
    {
        public static ValidationResult ValidateClass(object value, ValidationContext context)
        {
            string objectTableName = context.ObjectType.Name.Substring(0, context.ObjectType.Name.Length - 2);
            Type type = Type.GetType(context.ObjectType.FullName);

            int tenant = 0;
            PropertyInfo tenantProp = null;

            if (objectTableName == "ShipmentPickUp" || objectTableName == "ShipmentDelivery")
            {
                objectTableName = "ShipmentPickUpDelivery";
            }

            switch (objectTableName)
            {
                case "MessagingStock":
                    {
                        tenantProp = type.GetProperty("TenantNumber");
                        tenant = (int)tenantProp.GetValue(context.ObjectInstance, null);
                        break;
                    }

                case "Tenant":
                case "TenantManagement":
                case "AccountingSetting":
                    {
                        tenantProp = type.GetProperty("Id");
                        tenant = (int)tenantProp.GetValue(context.ObjectInstance, null);
                        break;
                    }

                case "Package":
                case "PackageConnectedPackage":
                case "Distributor":
                case "ComputingPartner":
                case "AccountingSystem":
                case "IATACode":
                case "AWBSpecialHandlingCode":
                case "PartnerService":
                    {
                        break;
                    }

                default:
                    {
                        tenantProp = type.GetProperty("Tenant");
                        tenant = (int)tenantProp.GetValue(context.ObjectInstance, null);
                        break;
                    }
            }

            /* by islam
             */
            Validators.ValidationClass temp = new Validators.ValidationClass(objectTableName, tenant);
            bool valid = temp.IsValid(value, context.ObjectInstance, context.MemberName);


            if (!valid)
            {
                List<string> d = new List<string>();
                d.Add(context.MemberName);
                ValidationResult v = new ValidationResult(temp.GetErrorMessage(value, context.ObjectInstance, context.MemberName), new string[] { context.MemberName });

                return v;
            }

            return ValidationResult.Success;
        }
    }

    public interface IValidateContext
    {
        bool IsValid(object value, object objectInstance, string propertyName);
        string GetErrorMessage(object value, object instance, string propertyName);
    }
}
