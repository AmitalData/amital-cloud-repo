using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AmitalCloud.Infrastructure.Data.Validators
{
    public partial class ClassLevelValidator : IClassLevelValidator
    {
        static string validationOutputMessage = "";
        List<ObjectField> objectFieldList;
        string objectTableName;
        int tenant;
        string errorMessage = "";
        IRulesValidator ruleValidator;
        public bool IsHybrid { get; set; }
        public ClassLevelValidator(IRulesValidator ruleValidator, string objectTableName, int tenant)
        {
            ruleValidator.Initialize(tenant);

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

            if (!IsHybrid)
            {
                if (!ruleValidator.ValidateEntityRules(value, objectTableName, tenant, ref validationOutputMessage))
                {
                    return false;
                }

                List<ObjectTableRuleField> requiredObjectFields = ruleValidator.ValidateAllRequiredFieldRules(value, objectTableName, tenant);

                if (requiredObjectFields.Count > 0)
                {
                    foreach (ObjectTableRuleField field in requiredObjectFields)
                    {
                        ObjectField f = objectFieldList.FirstOrDefault(fd => fd.FieldCode == field.ObjectFieldCode);
                        errorMessage = errorMessage + "," + TranslateTextsClass.GetTranslation("General.M.FieldIsRequired", f.FullNameTextCode.Code, null, null, field.Tenant);
                    }

                    return false;
                }

                bool isValid = ruleValidator.ApplyDuplicationRules(instance, objectTableName, tenant, ref validationOutputMessage);
                if (!isValid)
                {
                    return false;
                }
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

                    if (objectfeildprop.DataTypeCode == "Text" || objectfeildprop.DataTypeCode == "nText" || objectfeildprop.DataTypeCode == "LookUp")
                    {
                        if (objectfeildprop.IsCustom && propertyValue != null)
                        {
                            CustomFieldClass fieldClass = propertyValue as CustomFieldClass;
                            propertyValue = fieldClass.Value;
                        }

                        string valueString = propertyValue != null ? propertyValue.ToString() : "";
                        if (propertyValue != null && FieldValueValidator.IsNotValidMinMaxValue(objectfeildprop, propertyValue.ToString()) && !(objectfeildprop.DataTypeCode == "LookUp" && objectfeildprop.IsCustom))
                        {
                            return false;
                        }
                    }
                    //    }


                    //}

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
                            requiredError = requiredError + "," + TranslateTextsClass.GetTranslation("General.M.FieldIsRequired", objectfeildprop.FullNameTextCode.Code, null, null, objectfeildprop.Tenant);
                        }

                        else if (propertyValue is string)
                        {

                            if (string.IsNullOrEmpty(propertyValue.ToString()))
                            {
                                requiredError = requiredError + "," + TranslateTextsClass.GetTranslation("General.M.FieldIsRequired", objectfeildprop.FullNameTextCode.Code, null, null, objectfeildprop.Tenant);
                            }
                        }
                    }

                    if (objectfeildprop.DataTypeCode == "Text" || objectfeildprop.DataTypeCode == "nText" || objectfeildprop.DataTypeCode == "LookUp")
                    {
                        string valueString = propertyValue != null ? propertyValue.ToString() : "";
                        if (propertyValue != null && FieldValueValidator.IsNotValidMinMaxValue(objectfeildprop, propertyValue.ToString()) && !(objectfeildprop.DataTypeCode == "LookUp" && objectfeildprop.IsCustom))
                        {
                            stringLengthError = stringLengthError + "," + TranslateTextsClass.GetTranslation("General.M.MinMax", objectfeildprop.FullNameTextCode.Code, objectfeildprop.MinLength.ToString(), objectfeildprop.MaxLength.ToString(), objectfeildprop.Tenant);
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
                                        requiredError = requiredError + "," + TranslateTextsClass.GetTranslation("General.M.FieldIsRequired", objectfeildprop.FullNameTextCode.Code, null, null, objectfeildprop.Tenant);
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
    }



}
