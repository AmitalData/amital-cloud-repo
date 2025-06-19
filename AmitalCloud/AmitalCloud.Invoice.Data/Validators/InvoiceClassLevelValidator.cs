using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Data.Validators;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Model.EntityClasses;
using AmitalCloud.Invoice.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AmitalCloud.Invoice.Domain.Validators
{
    public partial class InvoiceClassLevelValidator : IInvoiceClassLevelValidator
    {
        bool ready;
        List<ObjectField> objectFieldList;
        private string errorMessage = "";
        int tenant = 0;

        public InvoiceClassLevelValidator(string objectTableName, int tenant)
        {
            //}
            this.tenant = tenant;
            objectFieldList = ObjectFieldRepository.GetObjectFieldsByObjectTableName(objectTableName, tenant).ToList();
        }

        public bool IsValid(object value, object instance, string propertyName)
        {
            bool valid = true;
            errorMessage = "";
            Type type = instance.GetType();
            object typeProp = null;
            FieldValidator fieldValidator = new FieldValidator(tenant);
            string objectType = "";

            if (typeProp != null)
            {
                objectType = typeProp.ToString();
            }
            else
            {
                objectType = type.Name.ToString();
                if (objectType.Contains("FollowUp"))
                {
                    objectType = "FollowUp";
                }
                else if (objectType.Contains("PM"))
                {
                    objectType = objectType.Substring(0, objectType.Length - 2);
                }
            }


            ObjectField field = (from a in objectFieldList
                                 where a.ObjectTable.Name == objectType && a.FieldName == propertyName
                                 select a).FirstOrDefault();

            if (field != null)
            {
                if (field.IsRequiered)
                {
                    if (field.IsCustom && value != null)
                    {
                        CustomFieldClass fieldClass = value as CustomFieldClass;
                        value = fieldClass.Value;
                    }

                    if (value == null)
                    {
                        valid = false;
                    }
                    else if (value is string)
                    {
                        if (String.IsNullOrEmpty(value.ToString()))
                        {
                            valid = false;
                        }
                    }
                    else
                    {
                        valid = true;
                    }
                }

                if (field.DataTypeCode == "Text") // server side
                {
                    if (field.IsCustom && value != null)
                    {
                        CustomFieldClass fieldClass = value as CustomFieldClass;
                        value = fieldClass.Value;
                    }

                    string valueString = value != null ? value.ToString() : "";
                    if (FieldValueValidator.IsNotValidMinMaxValue(field, valueString))
                    {
                        valid = false;
                    }
                }

                if (value != null)
                {
                    ValidationFieldResult result = fieldValidator.ValidateField(field, value, instance, tenant);

                    if (!result.Valid)
                    {
                        valid = result.Valid;
                        errorMessage = result.ErrorMessage;
                    }
                }
            }

            return valid;
        }

        public string GetErrorMessage(object value, object instance, string propertyName)
        {
            string error = "";
            Type type = instance.GetType();
            object typeProp = null;

            string objectType = "";
            if (typeProp != null)
            {
                objectType = typeProp.ToString();
            }
            else
            {
                objectType = type.Name.ToString();
                if (objectType.Contains("FollowUp"))
                {
                    objectType = "FollowUp";
                }
                else if (objectType.Contains("PM"))
                {
                    objectType = objectType.Substring(0, objectType.Length - 2);
                }
            }

            ObjectField field = (from a in objectFieldList
                                 where a.ObjectTable.Name == objectType && a.FieldName == propertyName
                                 select a).FirstOrDefault();

            if (field.IsRequiered)
            {
                if (value == null)
                {
                    error = TranslateTextsClass.GetTranslation("General.M.FieldIsRequired", field.FullNameTextCode.Code, null, null, field.Tenant);
                }
                if (string.IsNullOrEmpty(error) && value != null)
                {
                    if (value is string)
                    {
                        string valueString = value as string;
                        if (String.IsNullOrEmpty(value.ToString()))
                        {
                            error = TranslateTextsClass.GetTranslation("General.M.FieldIsRequired", field.FullNameTextCode.Code, null, null, field.Tenant);
                        }
                    }
                }
            }

            if (field.DataTypeCode == "Text")
            {

                if (field.IsCustom && value != null)
                {
                    CustomFieldClass fieldClass = value as CustomFieldClass;
                    value = fieldClass.Value;
                }

                string valueString = value != null ? value.ToString() : "";
                if (FieldValueValidator.IsNotValidMinMaxValue(field, valueString))
                {
                    error = TranslateTextsClass.GetTranslation("General.M.MinMax", field.FullNameTextCode.Code, field.MinLength.ToString(), field.MaxLength.ToString(), field.Tenant);
                }
            }

            if (!String.IsNullOrEmpty(errorMessage))
            {
                error = errorMessage;
            }

            return error;
        }
    }

}
