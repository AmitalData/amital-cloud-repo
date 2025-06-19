using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace AmitalCloud.Infrastructure.Data.Validators
{
    public partial class InfrastructureClassLevelValidator : IInfrastructureClassLevelValidator
    {
        int tenant;
        string objectTableName;
        string errorMessage = "";
        List<ObjectField> objectFieldList;
        static string validationOutputMessage = "";

        public InfrastructureClassLevelValidator(string objectTableName, int tenant)
        {
            this.objectTableName = objectTableName;
            this.tenant = tenant;

            objectFieldList = ObjectFieldRepository.GetObjectFieldsByObjectTableName(objectTableName, tenant).ToList();
        }

        public bool IsValid(object value, object instance, string propertyName)
        {
            errorMessage = "";
            validationOutputMessage = "";
            Type type = value.GetType();
            string objectName = type.Name;

            if (objectName.Contains("FollowUp"))
            {
                objectName = "FollowUp";
            }

            else if (objectName.Contains("PM"))
            {
                objectName = objectName.Substring(0, objectName.Length - 2);
            }

            PropertyInfo custom = type.GetProperty("CustomField");
            object cutomfield = null;
            Type customType = null;
            if (custom != null)
            {
                cutomfield = custom.GetValue(value, null);
                customType = custom.PropertyType;
            }

            foreach (ObjectField objectfeildprop in objectFieldList)
            {

                PropertyInfo propertyInf = type.GetProperty(objectfeildprop.FieldName);
                if (propertyInf != null)
                {
                    object propertyValue = propertyInf.GetValue(value, null);

                    if (objectfeildprop.IsRequiered)
                    {

                        if (propertyValue == null)
                        {
                            return false;
                        }

                        else if (propertyValue is string)
                        {

                            if (string.IsNullOrEmpty(propertyValue.ToString()))
                            {
                                return false;
                            }
                        }
                    }

                    if (objectfeildprop.DataTypeCode == "Text" || objectfeildprop.DataTypeCode == "nText")
                    {
                        if (objectfeildprop.IsCustom && propertyValue != null)
                        {
                            CustomFieldClass fieldClass = propertyValue as CustomFieldClass;
                            propertyValue = fieldClass.Value;
                        }
                        if (propertyValue != null && FieldValueValidator.IsNotValidMinMaxValue(objectfeildprop, propertyValue.ToString()))
                        {
                            return false;
                        }
                    }
                }

                else
                {
                    if (customType != null)
                    {
                        PropertyInfo field = customType.GetProperty(objectfeildprop.FieldName);
                        if (field != null)
                        {
                            if (objectfeildprop.IsRequiered)
                            {
                                if (cutomfield != null)
                                {
                                    object fieldValue = field.GetValue(cutomfield, null);
                                    if (fieldValue == null)
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }

        public string GetErrorMessage(object instance, string property)
        {
            if (!String.IsNullOrEmpty(errorMessage))
            {
                return errorMessage;
            }
            string requiredError = "";
            string stringLengthError = "";
            Type type = instance.GetType();
            string objectName = type.Name;

            if (objectName.Contains("FollowUp"))
            {
                objectName = "FollowUp";
            }

            else if (objectName.Contains("PM"))
            {
                objectName = objectName.Substring(0, objectName.Length - 2);
            }

            PropertyInfo custom = type.GetProperty("CustomField");
            object cutomfield = null;
            Type customType = null;
            if (custom != null)
            {
                cutomfield = custom.GetValue(instance, null);
                customType = custom.PropertyType;
            }

            foreach (ObjectField objectfeildprop in objectFieldList)
            {
                PropertyInfo propertyInf = type.GetProperty(objectfeildprop.FieldName);
                if (propertyInf != null)
                {
                    object propertyValue = propertyInf.GetValue(instance, null);

                    if (objectfeildprop.IsCustom && propertyValue != null)
                    {
                        CustomFieldClass fieldClass = propertyValue as CustomFieldClass;
                        propertyValue = fieldClass.Value;
                    }

                    if (objectfeildprop.IsRequiered)
                    {
                        if (propertyValue == null)
                        {
                            requiredError = requiredError + "," + InfrastructureTranslateTextsClass.GetTranslation("General.M.FieldIsRequired", objectfeildprop.FullNameTextCode.Code, null, null, objectfeildprop.Tenant);
                        }

                        else if (propertyValue is string)
                        {

                            if (string.IsNullOrEmpty(propertyValue.ToString()))
                            {
                                requiredError = requiredError + "," + InfrastructureTranslateTextsClass.GetTranslation("General.M.FieldIsRequired", objectfeildprop.FullNameTextCode.Code, null, null, objectfeildprop.Tenant);
                            }


                        }

                    }


                    if (objectfeildprop.DataTypeCode == "Text" || objectfeildprop.DataTypeCode == "nText")
                    {
                        if (objectfeildprop.IsCustom && propertyValue != null)
                        {
                            CustomFieldClass fieldClass = propertyValue as CustomFieldClass;
                            propertyValue = fieldClass.Value;
                        }
                        if (propertyValue != null && FieldValueValidator.IsNotValidMinMaxValue(objectfeildprop, propertyValue.ToString()))
                        {
                            stringLengthError = stringLengthError + "," + InfrastructureTranslateTextsClass.GetTranslation("General.M.MinMax", objectfeildprop.FullNameTextCode.Code, objectfeildprop.MinLength.ToString(), objectfeildprop.MaxLength.ToString(), objectfeildprop.Tenant);
                        }
                    }
                }

                else
                {
                    if (customType != null)
                    {
                        PropertyInfo field = customType.GetProperty(objectfeildprop.FieldName);
                        if (field != null)
                        {
                            if (cutomfield != null)
                            {
                                if (objectfeildprop.IsRequiered)
                                {
                                    object fieldValue = field.GetValue(cutomfield, null);
                                    if (fieldValue == null)
                                    {
                                        requiredError = requiredError + "," + InfrastructureTranslateTextsClass.GetTranslation("General.M.FieldIsRequired", objectfeildprop.FullNameTextCode.Code, null, null, objectfeildprop.Tenant);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            string errors = "";
            requiredError = requiredError.TrimStart(',');
            stringLengthError = stringLengthError.TrimStart(',');


            if (!string.IsNullOrEmpty(validationOutputMessage))
            {
                validationOutputMessage = validationOutputMessage.TrimStart(',');
                errors = validationOutputMessage;
            }

            if (!string.IsNullOrEmpty(requiredError))
            {
                errors = errors + "," + requiredError;
            }
            if (!string.IsNullOrEmpty(stringLengthError))
            {
                if (!string.IsNullOrEmpty(requiredError))
                {
                    errors = errors + "," + stringLengthError;
                }
                else
                {
                    errors = stringLengthError;
                }
            }

            errors = errors.TrimStart(',');
            return errors;
        }

        public System.Collections.Generic.List<string> GetErrorsInObject(object instance)
        {
            throw new NotImplementedException();
        }

        public static ValidationResult ValidateClass(object value, ValidationContext context)
        {
            //return ValidationResult.Success;
            string objectTableName = context.ObjectType.Name.Substring(0, context.ObjectType.Name.Length - 2);
            Type type = Type.GetType(context.ObjectType.FullName);

            if (context.ObjectType.FullName == "WebFreight.Web.DataContracts.BIReportXMLData")
            {
                return null;
            }

            PropertyInfo tenantProp = null;
            tenantProp = type.GetProperty("Tenant");
            int tenant = 0;

            if (objectTableName == "BookingProduct")
            {
                tenant = 0;
            }

            else if (objectTableName == "Tenant" && objectTableName == "TenantManagement")
            {
                tenantProp = type.GetProperty("Id");
                tenant = (int)tenantProp.GetValue(context.ObjectInstance, null);
            }

            else if (objectTableName == "BIReport")
            {
                BIReportClassLevelValidator bIReportClassLevelValidator = new BIReportClassLevelValidator(objectTableName, tenant);
                if (!bIReportClassLevelValidator.IsValid(value, context.ObjectInstance, context.MemberName))
                {
                    ValidationResult v = new ValidationResult(bIReportClassLevelValidator.GetErrorMessage(value, context.ObjectInstance, context.MemberName), new string[] { context.MemberName });
                    return v;
                }
            }
            else if (objectTableName == "FeatureToggle")
            {
                FeatureToggleClassLevelValidator featureToggleClassLevelValidator = new FeatureToggleClassLevelValidator();
                if (!featureToggleClassLevelValidator.IsValid(value, context.ObjectInstance, context.MemberName))
                {
                    ValidationResult v = new ValidationResult(featureToggleClassLevelValidator.GetErrorMessage(value, context.ObjectInstance, context.MemberName), new string[] { context.MemberName });
                    return v;
                }
            }
            else
            {
                tenantProp = type.GetProperty("Tenant");
                if (context.ObjectInstance.GetType() == type && tenantProp != null)
                {
                    object tenantvalue = tenantProp.GetValue(context.ObjectInstance, null);
                    if (tenantvalue != null)
                    {
                        tenant = (int)tenantvalue;
                    }
                }
            }

            Validators.InfrastructureClassLevelValidator temp = new Validators.InfrastructureClassLevelValidator(objectTableName, tenant);
            //Client client1 = Context.ObjectInstance as Client;
            bool valid = temp.IsValid(value, context.ObjectInstance, context.MemberName);

            List<ValidationResult> vList = new List<ValidationResult>();
            if (!valid)
            {


                ValidationResult v = new ValidationResult(temp.GetErrorMessage(context.ObjectInstance, context.MemberName), new string[] { context.MemberName });
                return v;
            }
            return null;


        }


    }

}
