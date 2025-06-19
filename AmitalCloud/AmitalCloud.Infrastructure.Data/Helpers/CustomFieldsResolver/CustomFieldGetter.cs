using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using System;
using System.Linq;
using System.Reflection;


namespace AmitalCloud.Infrastructure.Data.Helpers.CustomFieldsResolver
{
    public class CustomFieldGetter
    {
        public static string Get(CustomFieldGetterArgs customFieldGetterArgs)
        {
            ObjectField objectField = customFieldGetterArgs.objectField;
            if (objectField == null) return null;

            object currentEntity = customFieldGetterArgs.entity;
            PropertyInfo propertyPathPi = currentEntity.GetType().GetProperty(objectField.FieldName);
            if (propertyPathPi == null) return null;
            object value = GetEntityFieldValue(currentEntity, propertyPathPi);
            if (value == null) return null;
            customFieldGetterArgs.value = value;

            if (objectField.DataTypeCode == "LookUp")
            {
                return GetLookUpFieldValue(customFieldGetterArgs);
            }

            if (objectField.DataTypeCode == "PickList")
            {
                return GetPickListFieldValue(customFieldGetterArgs.customFieldResolver, value);
            }

            if (!String.IsNullOrEmpty(value.ToString()))
            {
                return GetFieldDataType(objectField, value.ToString(), customFieldGetterArgs.customFieldResolver);
            }

            return null;
        }

        private static object GetEntityFieldValue(object currentEntity, PropertyInfo propertyPathPi)
        {
            object value = propertyPathPi.GetValue(currentEntity, null);
            if (value == null) return null;

            if (value.GetType() == typeof(CustomFieldClass))
            {
                value = (value as CustomFieldClass).Value;
            }

            return value;
        }

        private static string GetLookUpFieldValue(CustomFieldGetterArgs customFieldGetterArgs)
        {
            LookUpFieldValueGetterArgs lookUpFieldValueGetterArgs = GetLookUpFieldValueGetterArgs(customFieldGetterArgs);
            CustomFieldResolver customFieldResolver = customFieldGetterArgs.customFieldResolver;

            if (customFieldResolver.UsingParallelMechanisim)
            {
                lock (customFieldResolver.lockMe)
                {
                    return GetLookUpFieldValueAfterReflection(lookUpFieldValueGetterArgs, customFieldResolver);
                }
            }
            else
            {
                return GetLookUpFieldValueAfterReflection(lookUpFieldValueGetterArgs, customFieldResolver);
            }
        }

        private static string GetLookUpFieldValueAfterReflection(LookUpFieldValueGetterArgs lookUpFieldValueGetterArgs, CustomFieldResolver customFieldResolver)
        {
            LookUpFieldDataStorage lookUpFieldDataStorage = customFieldResolver.LookUpFieldsDataStorage.Where(LookUpFieldData => IsLookUpFieldExist(LookUpFieldData, lookUpFieldValueGetterArgs)).FirstOrDefault();

            if (lookUpFieldDataStorage != null) return lookUpFieldDataStorage.FieldValue;

            lookUpFieldDataStorage = ReflectLookUpFieldValue(lookUpFieldValueGetterArgs, customFieldResolver);
            customFieldResolver.LookUpFieldsDataStorage.Add(lookUpFieldDataStorage);
            return lookUpFieldDataStorage.FieldValue;
        }

        private static bool IsLookUpFieldExist(LookUpFieldDataStorage LookUpFieldData, LookUpFieldValueGetterArgs lookUpFieldValueGetterArgs)
        {
            ObjectField objectField = lookUpFieldValueGetterArgs.ObjectField;
            object value = lookUpFieldValueGetterArgs.Value;
            if (LookUpFieldData.LookUpTableId != objectField.LookUpTableId) return false;
            if (LookUpFieldData.Tenant != objectField.Tenant) return false;
            if (LookUpFieldData.Value == null || value == null) return false;
            return LookUpFieldData.Value.ToString() == value.ToString();
        }

        private static LookUpFieldValueGetterArgs GetLookUpFieldValueGetterArgs(CustomFieldGetterArgs customFieldGetterArgs)
        {
            LookUpFieldTypeReflectionDetails lookUpFieldTypeReflectionDetails = GetLookUpTypeReflectionDetails(customFieldGetterArgs.objectField);

            return new LookUpFieldValueGetterArgs
            {
                ObjectField = customFieldGetterArgs.objectField,
                Tenant = customFieldGetterArgs.tenant,
                ExternalAPICall = customFieldGetterArgs.externalAPICall,
                Value = customFieldGetterArgs.value,
                InsideTypePath = lookUpFieldTypeReflectionDetails.InsideTypePath,
                InsideEntityType = lookUpFieldTypeReflectionDetails.InsideEntityType
            };
        }

        private static LookUpFieldTypeReflectionDetails GetLookUpTypeReflectionDetails(ObjectField objectField)
        {
            LookUpFieldTypeReflectionDetails lookUpFieldTypeReflectionDetails = new LookUpFieldTypeReflectionDetails();

            //Assembly blAssembly = Assembly.Load("Logitude.BL");
            //string insideEntityName = ObjectTableRepository.GetSingleObjectTableById(objectField.LookUpTableId, objectField.Tenant).Name;
            //if (insideEntityName == "Carrier") insideEntityName = "Card";

            //lookUpFieldTypeReflectionDetails.InsideTypePath = "Logitude.BL.ShipmentsModel.EntityQueries." + insideEntityName + "Query";
            //lookUpFieldTypeReflectionDetails.InsideEntityType = blAssembly.GetType(lookUpFieldTypeReflectionDetails.InsideTypePath);
            //if (lookUpFieldTypeReflectionDetails.InsideEntityType != null) return lookUpFieldTypeReflectionDetails;

            //lookUpFieldTypeReflectionDetails.InsideTypePath = "Logitude.BL.CommonDataModel.EntityQueries." + insideEntityName + "Query";
            //lookUpFieldTypeReflectionDetails.InsideEntityType = blAssembly.GetType(lookUpFieldTypeReflectionDetails.InsideTypePath);
            //if (lookUpFieldTypeReflectionDetails.InsideEntityType != null) return lookUpFieldTypeReflectionDetails;

            //lookUpFieldTypeReflectionDetails.InsideTypePath = "Logitude.BL." + insideEntityName + "Query";
            //lookUpFieldTypeReflectionDetails.InsideEntityType = blAssembly.GetType(lookUpFieldTypeReflectionDetails.InsideTypePath);
            //if (lookUpFieldTypeReflectionDetails.InsideEntityType != null) return lookUpFieldTypeReflectionDetails;

            //lookUpFieldTypeReflectionDetails.InsideTypePath = "Logitude.BL.InfrastructureModel.EntityQueries." + insideEntityName + "Query";
            //lookUpFieldTypeReflectionDetails.InsideEntityType = blAssembly.GetType(lookUpFieldTypeReflectionDetails.InsideTypePath);
            //if (lookUpFieldTypeReflectionDetails.InsideEntityType != null) return lookUpFieldTypeReflectionDetails;

            //lookUpFieldTypeReflectionDetails.InsideTypePath = "Logitude.BL.QuoteModel.EntityQueries." + insideEntityName + "Query";
            //lookUpFieldTypeReflectionDetails.InsideEntityType = blAssembly.GetType(lookUpFieldTypeReflectionDetails.InsideTypePath);
            //if (lookUpFieldTypeReflectionDetails.InsideEntityType != null) return lookUpFieldTypeReflectionDetails;

            return lookUpFieldTypeReflectionDetails;
        }

        private static LookUpFieldDataStorage ReflectLookUpFieldValue(LookUpFieldValueGetterArgs lookUpFieldValueGetterArgs, CustomFieldResolver customFieldResolver)
        {
            ObjectField objectField = lookUpFieldValueGetterArgs.ObjectField;
            object value = lookUpFieldValueGetterArgs.Value;

            LookUpFieldDataStorage lookUpFieldDataStorage = new LookUpFieldDataStorage
            {
                Value = value,
                LookUpTableId = objectField.LookUpTableId,
                Tenant = objectField.Tenant,
                FieldValue = null
            };

            int tenant = lookUpFieldValueGetterArgs.Tenant;
            bool externalAPICall = lookUpFieldValueGetterArgs.ExternalAPICall;
            string insideTypePath = lookUpFieldValueGetterArgs.InsideTypePath;
            Type insideEntityType = lookUpFieldValueGetterArgs.InsideEntityType;

            if (insideEntityType == null) return lookUpFieldDataStorage;

            object insideEntityRepository = null;
            if (customFieldResolver.definedObjects.Keys.Contains(insideTypePath))
            {
                insideEntityRepository = customFieldResolver.definedObjects[insideTypePath];
            }

            if (insideEntityRepository == null)
            {
                insideEntityRepository = Activator.CreateInstance(insideEntityType, tenant);
                customFieldResolver.definedObjects.Add(insideTypePath, insideEntityRepository);
            }

            MethodInfo insideMethodInfo = insideEntityRepository.GetType().GetMethod("GetCustomSinglePM");
            if (insideMethodInfo == null)
            {
                insideMethodInfo = insideEntityRepository.GetType().GetMethod("GetSinglePM");
            }
            if (insideMethodInfo == null)
            {
                insideMethodInfo = insideEntityRepository.GetType().GetMethod("GetSingle");
            }

            if (insideMethodInfo == null) return lookUpFieldDataStorage;

            object[] parameters = GetMethodParameters(tenant, value, insideMethodInfo);

            object insideEntity = insideMethodInfo.Invoke(insideEntityRepository, parameters);

            if (insideEntity == null) return lookUpFieldDataStorage;

            ObjectTable lookupTable = ObjectTableRepository.GetSingleObjectTableById(objectField.LookUpTableId, objectField.Tenant);
            string lookupProperty = lookupTable.LookUp2 != null ? lookupTable.LookUp2 : lookupTable.LookUp1;
            if (externalAPICall)
            {
                lookupProperty = "Code";
            }

            PropertyInfo insidePropertyPathPi = insideEntity.GetType().GetProperty(lookupProperty);
            if (insidePropertyPathPi == null) return lookUpFieldDataStorage;

            object insideValue = insidePropertyPathPi.GetValue(insideEntity, null);

            if (insideValue == null) return lookUpFieldDataStorage;

            if (insideValue is DateTime)
            {
                insideValue = ((DateTime)insideValue).ToShortDateString();
            }

            lookUpFieldDataStorage.FieldValue = insideValue?.ToString();

            return lookUpFieldDataStorage;
        }

        private static string GetPickListFieldValue(CustomFieldResolver customFieldResolver, object value)
        {
            CustomPickList picklist = customFieldResolver.customPickLists.Where(c => c.Id == value.ToString()).FirstOrDefault();
            if (picklist != null)
            {
                return picklist.Value;
            }

            return null;
        }

        private static object[] GetMethodParameters(int tenant, object value, MethodInfo insideMethodInfo)
        {
            ParameterInfo[] parametersInfo = insideMethodInfo.GetParameters();
            switch (parametersInfo.Count())
            {
                case 1:
                    return new object[] { value };
                case 2:
                    return new object[] { value, tenant };
                case 3:
                    return (IsStringParameter(parametersInfo[2])) ? new object[] { value, tenant, null } : new object[] { value, tenant, false };
                case 4:
                    return new object[] { value, tenant, null, false };
                default:
                    return new object[] { value, tenant };
            }
        }

        private static bool IsStringParameter(ParameterInfo parametersInfo)
        {
            return parametersInfo?.ParameterType?.Name == "String";
        }

        private static string GetFieldDataType(ObjectField field, string customField, CustomFieldResolver customFieldResolver)
        {
            if (field == null || customField == null) return null;

            switch (field.DataTypeCode.Trim())
            {
                case "Text":
                case "Emails":
                case "nText":
                    {
                        return customField.ToString();
                    }

                case "DateTime":
                    {
                        return GetFieldDataDateType(customField, customFieldResolver, true);
                    }

                case "Date":
                    {
                        return GetFieldDataDateType(customField, customFieldResolver, false);
                    }

                case "UnsDecimal":
                case "Decimal":
                    {
                        return GetFieldDataDecimalType(field, customField);
                    }

                case "Integer":
                case "UnsInteger":
                    {
                        return GetFieldDataIntegerType(customField);
                    }

                case "Double":
                case "SigDouble":
                    {
                        return GetFieldDataDoubleType(customField);
                    }

                case "Boolean":
                    {
                        return GetFieldDataBooleanType(customField);
                    }

                default:
                    {
                        return null;
                    }
            }
        }

        private static string GetFieldDataDateType(string customField, CustomFieldResolver customFieldResolver, bool isDateTime)
        {
            string dateFormat = isDateTime ? @"dd MMM yyyy HH':'mm':'ss" : @"dd MMM yyyy";
            DateTime? date = ConvertToDate(customField);
            return date != null ? date.Value.ToString(dateFormat, customFieldResolver.en.DateTimeFormat) : null;
        }

        private static DateTime? ConvertToDate(string arg)
        {
            DateTime? date = null;

            if (!string.IsNullOrEmpty(arg) && arg.Length >= 14)
            {
                date = new DateTime(System.Convert.ToInt32(arg.Substring(0, 4)), System.Convert.ToInt32(arg.Substring(4, 2)), System.Convert.ToInt32(arg.Substring(6, 2)), System.Convert.ToInt32(arg.Substring(8, 2)), System.Convert.ToInt32(arg.Substring(10, 2)), System.Convert.ToInt32(arg.Substring(12, 2)));
            }

            return date;
        }

        private static string GetFieldDataDecimalType(ObjectField field, string customField)
        {
            if (customField.Length >= 15)
            {
                customField = customField.Insert(customField.Length - 3, ".");
            }

            decimal.TryParse(customField, out decimal value);
            string result = GetFormatedDecimalVlue(field, value);

            return result;
        }

        private static string GetFieldDataIntegerType(string customField)
        {
            if (!string.IsNullOrEmpty(customField))
            {
                customField = customField.Trim().Replace(" ", "").Split('.')[0];
            }

            int.TryParse(customField, out int value);

            return value.ToString();
        }

        private static string GetFieldDataDoubleType(string customField)
        {
            if (!string.IsNullOrEmpty(customField))
            {
                customField = customField.Trim().Replace(" ", "");
            }

            if (customField.Length >= 15)
            {
                customField = customField.Insert(customField.Length - 3, ".");
            }

            double.TryParse(customField, out double value);
            return value.ToString();
        }

        private static string GetFieldDataBooleanType(string customField)
        {
            bool.TryParse(customField, out bool value);
            return value.ToString();
        }

        private static string GetFormatedDecimalVlue(ObjectField field, decimal value)
        {
            if (field.DigitsAfterPoint > 0)
            {
                return value.ToString("#,##0." + new string('0', field.DigitsAfterPoint));
            }

            return value.ToString("#,##0.");
        }




        public static string Get2(object value, ObjectField objectField, int tenant, CustomFieldResolver customFieldResolver)
        {
            //Old Code will keep it as is 
            string resultValue = null;
            //if (value != null)
            //{
            //    if (objectField != null)
            //    {
            //        Assembly blAssembly = Assembly.Load("Logitude.BL");

            //        if (objectField.DataTypeCode == "LookUp" && value != null)
            //        {
            //            string insideEntityName = ObjectTableRepository.GetSingleObjectTableById(objectField.LookUpTableId, objectField.Tenant).Name;
            //            if (insideEntityName == "Carrier")
            //            {
            //                insideEntityName = "Card";
            //            }

            //            string insideTypePath = "Logitude.BL.ShipmentsModel.EntityQueries." + insideEntityName + "Query";

            //            Type insideEntityType = blAssembly.GetType(insideTypePath);

            //            if (insideEntityType == null)
            //            {
            //                insideTypePath = "Logitude.BL.CommonDataModel.EntityQueries." + insideEntityName + "Query";
            //                insideEntityType = blAssembly.GetType(insideTypePath);
            //            }

            //            if (insideEntityType == null)
            //            {
            //                insideTypePath = "Logitude.BL." + insideEntityName + "Query";
            //                insideEntityType = blAssembly.GetType(insideTypePath);
            //            }

            //            if (insideEntityType == null)
            //            {
            //                insideTypePath = "Logitude.BL.InfrastructureModel.EntityQueries." + insideEntityName + "Query";
            //                insideEntityType = blAssembly.GetType(insideTypePath);
            //            }

            //            if (insideEntityType == null)
            //            {
            //                insideTypePath = "Logitude.BL.QuoteModel.EntityQueries." + insideEntityName + "Query";
            //                insideEntityType = blAssembly.GetType(insideTypePath);
            //            }

            //            if (insideEntityType == null)
            //            {
            //                insideTypePath = "Logitude.BL.InvoiceModel.EntityQueries." + insideEntityName + "Query";
            //                insideEntityType = blAssembly.GetType(insideTypePath);
            //            }





            //            bool isGeneratedQuery = false;
            //            if (insideEntityType == null)
            //            {
            //                //Logitude.CRM.BL.EntityQueryServices
            //                blAssembly = Assembly.Load("Logitude.CRM.BL");
            //                insideTypePath = "Logitude.CRM.BL.EntityQueryServices." + insideEntityName + "QueryService";
            //                insideEntityType = blAssembly.GetType(insideTypePath);
            //                if (insideEntityType != null)
            //                {
            //                    isGeneratedQuery = true;
            //                }

            //            }

            //            object insideEntityRepository = null;

            //            if (insideEntityType != null)
            //            {
            //                if (customFieldResolver.definedObjects.Keys.Contains(insideTypePath))
            //                {
            //                    insideEntityRepository = customFieldResolver.definedObjects[insideTypePath];
            //                }

            //                if (insideEntityRepository == null)
            //                {
            //                    insideEntityRepository = Activator.CreateInstance(insideEntityType, tenant);

            //                    customFieldResolver.definedObjects.Add(insideTypePath, insideEntityRepository);
            //                }

            //                MethodInfo insideMethodInfo = insideEntityRepository.GetType().GetMethod("GetCustomSinglePM");

            //                if (insideMethodInfo == null)
            //                {
            //                    // insideMethodInfo = insideEntityRepository.GetType().GetMethod("GetSinglePM");


            //                    insideMethodInfo = insideEntityRepository.GetType().GetMethods().Where(d => d.Name == "GetSinglePM").FirstOrDefault();

            //                }
            //                if (insideMethodInfo == null)
            //                {
            //                    insideMethodInfo = insideEntityRepository.GetType().GetMethod("GetSingle");
            //                }
            //                object insideEntity = null;

            //                if (insideMethodInfo != null)
            //                {
            //                    ParameterInfo[] parametersInfo = insideMethodInfo.GetParameters();
            //                    object[] parameters = new object[] { };
            //                    if (!isGeneratedQuery)
            //                    {
            //                        switch (parametersInfo.Count())
            //                        {
            //                            case 1:
            //                                parameters = new object[] { value };
            //                                break;
            //                            case 2:
            //                                parameters = new object[] { value, tenant };
            //                                break;
            //                            case 3:
            //                                parameters = new object[] { value, tenant, false };
            //                                break;
            //                            default:
            //                                parameters = new object[] { value, tenant };
            //                                break;
            //                        }
            //                    }
            //                    else parameters = new object[] { value, false, false };



            //                    insideEntity = insideMethodInfo.Invoke(insideEntityRepository, parameters);

            //                    if (insideEntity != null)
            //                    {
            //                        ObjectTable lookupTable = ObjectTableRepository.GetSingleObjectTableById(objectField.LookUpTableId, objectField.Tenant);
            //                        string lookupProperty = lookupTable.LookUp2 != null ? lookupTable.LookUp2 : lookupTable.LookUp1;
            //                        PropertyInfo insidePropertyPathPi = insideEntity.GetType().GetProperty(lookupProperty);
            //                        if (insidePropertyPathPi != null)
            //                        {
            //                            object insideValue = insidePropertyPathPi.GetValue(insideEntity, null);
            //                            if (insideValue != null)
            //                            {
            //                                if (insideValue is DateTime)
            //                                {
            //                                    DateTime date = (DateTime)insideValue;
            //                                    insideValue = date.ToShortDateString();
            //                                }
            //                            }

            //                            resultValue = (insideValue != null ? insideValue.ToString() : null);
            //                        }
            //                    }
            //                }
            //            }
            //        }

            //        if (objectField.DataTypeCode == "PickList")
            //        {
            //            if (value != null)
            //            {
            //                CustomPickList picklist = customFieldResolver.customPickLists.Where(c => c.Id == value.ToString()).FirstOrDefault();

            //                if (picklist != null)
            //                {
            //                    resultValue = picklist.Value;
            //                }
            //            }
            //        }

            //        if (objectField.DataTypeCode != "LookUp" && objectField.DataTypeCode != "PickList")
            //        {
            //            string valueString = value != null ? value.ToString() : null;
            //            if (!String.IsNullOrEmpty(valueString))
            //            {
            //                resultValue = GetFieldDataType(objectField, valueString, customFieldResolver);
            //            }
            //        }
            //    }
            //}

            //if (resultValue == "")
            //{
            //    resultValue = null;
            //}

            return resultValue;
        }

    }

    public class CustomFieldGetterArgs
    {
        public object entity { get; set; }
        public ObjectField objectField { get; set; }
        public int tenant { get; set; }
        public CustomFieldResolver customFieldResolver { get; set; }
        public bool externalAPICall { get; set; }
        public object value { get; set; }
    }

    public class LookUpFieldTypeReflectionDetails
    {
        public string InsideTypePath { get; set; }
        public Type InsideEntityType { get; set; }
    }

}
