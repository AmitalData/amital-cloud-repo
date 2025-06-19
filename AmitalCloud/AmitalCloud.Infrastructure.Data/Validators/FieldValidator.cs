using AmitalCloud.Infrastructure.Data.Queries;
using AmitalCloud.Infrastructure.Data.Repositories;
using AmitalCloud.Infrastructure.Domain.DataContracts;
using AmitalCloud.Infrastructure.Model.EntityClasses ;
using AmitalCloud.Infrastructure.Domain.EntityPMs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AmitalCloud.Infrastructure.Data.Validators
{
    public class FieldValidator
    {
        Dictionary<string, object> definedObjects = new Dictionary<string, object>();
        List<ObjectFieldValidationPM> objectFieldValidations = new List<ObjectFieldValidationPM>();
        public FieldValidator(int tenant)
        {
            //ObjectTableRep = new ObjectTabelRepository();
            //ObjectFieldsRep = new ObjectFieldsRepository();
            //ObjectFieldsValidationsRep = new ObjectFieldValidationRepository(tenant);
            //ObjectFieldValidationRepository objectFieldValidationRepository = new ObjectFieldValidationRepository(tenant);
            //ObjectFieldValidationQuery objectFieldValidationQuery = new ObjectFieldValidationQuery(objectFieldValidationRepository);
            objectFieldValidations = ObjectFieldValidationQuery.GetObjectFieldValidationPMs(tenant).ToList();
        }

        public ValidationFieldResult ValidateField(ObjectField field, object fieldValue, object entity, int tenant)
        {
            if (AmitalCloudSettings.DeploymentStage == "logboxwe1" || AmitalCloudSettings.DeploymentStage == "Test2" || AmitalCloudSettings.DeploymentStage == "Dev")
            {
                return new ValidationFieldResult(true, "");
            }
            List<ObjectFieldValidationPM> validations = objectFieldValidations.Where(f => f.ObjectFieldCode == field.FieldCode).ToList();
            if (validations.Count > 0)
            {
                ExpressionValidation expressionValidation = null;
                foreach (ObjectFieldValidationPM validationPm in validations.OrderBy(f => f.ValidationOrder).ToList())
                {
                    if (expressionValidation == null)
                    {
                        expressionValidation = new ExpressionValidation();
                    }

                    if (validationPm.Condition != null)
                    {
                        Dictionary<string, object> conditionFieldsDic = new Dictionary<string, object>();

                        Type type1 = entity.GetType();
                        PropertyInfo propertyInf = null;
                        object propertyValue = null;

                        string[] fieldNames = { };
                        string fieldValidationCondition = validationPm.Condition;
                        string[] level1 = fieldValidationCondition.Split('[');

                        foreach (string s in level1)
                        {
                            if (s.Contains("]"))
                            {
                                string[] level2 = s.Split(']');
                                string fieldName = level2[0].Trim();

                                if (!fieldName.Contains("."))
                                {
                                    fieldValidationCondition = fieldValidationCondition.Replace("[" + fieldName + "]", fieldName);

                                    propertyInf = type1.GetProperty(fieldName);
                                    if (propertyInf != null)
                                    {
                                        propertyValue = propertyInf.GetValue(entity, null);
                                        conditionFieldsDic.Add(fieldName, propertyValue);
                                    }
                                }

                                else
                                {
                                    fieldValidationCondition = fieldValidationCondition.Replace("[" + fieldName + "]", fieldName.Replace(".", ""));
                                    propertyValue = GetObjectFieldValue(fieldName, field.ObjectTableId, entity, tenant);
                                    conditionFieldsDic.Add(fieldName.Replace(".", ""), propertyValue);
                                }
                            }
                        }

                        conditionFieldsDic.Add("value", fieldValue);

                        if (conditionFieldsDic.Count != 0)
                        {
                            bool enableRun = expressionValidation.ExecuteExpression(fieldValidationCondition, conditionFieldsDic);

                            if (!enableRun)
                            {
                                continue;
                            }
                        }
                    }
                    bool valid = ValidateExpression(validationPm.ValidationExpression, field.ObjectTableId, entity, fieldValue, tenant);

                    if (!valid)
                    {
                        return new ValidationFieldResult(false, validationPm.ErrorMessage);
                    }
                }
            }

            return new ValidationFieldResult(true, "");
        }

        private bool ValidateExpression(string expression, string objectTableId, object entity, object fieldValue, int tenant)
        {
            if (fieldValue != null)
            {
                if (fieldValue.ToString().Trim() == String.Empty)
                {
                    return true;
                }

                ExpressionValidation expressionValidation = new ExpressionValidation();
                Dictionary<string, object> conditionFieldsDic = new Dictionary<string, object>();

                Type type1 = entity.GetType();
                PropertyInfo propertyInf = null;
                object propertyValue = null;

                string[] fieldNames = { };

                string[] level1 = expression.Split('[');

                foreach (string s in level1)
                {
                    if (s.Contains("]"))
                    {
                        string[] level2 = s.Split(']');

                        string fieldName = level2[0].Trim();

                        if (!fieldName.Contains("."))
                        {
                            expression = expression.Replace("[" + fieldName + "]", fieldName);

                            propertyInf = type1.GetProperty(fieldName);
                            if (propertyInf != null)
                            {
                                propertyValue = propertyInf.GetValue(entity, null);
                                conditionFieldsDic.Add(fieldName, propertyValue);
                            }
                        }
                        else
                        {
                            expression = expression.Replace("[" + fieldName + "]", fieldName.Replace(".", ""));
                            propertyValue = GetObjectFieldValue(fieldName, objectTableId, entity, tenant);
                            conditionFieldsDic.Add(fieldName.Replace(".", ""), propertyValue);
                        }
                    }
                }

                conditionFieldsDic.Add("value", fieldValue);

                object result = expressionValidation.ExecuteValueExpression(expression, conditionFieldsDic);
                bool valid = (bool)result;

                return valid;
            }
            else
            {
                return true;
            }
        }

        #region GetObjectFieldValue
        private object GetObjectFieldValue(string propertyName, string objectTableName, object entity, int tenant)
        {
            object value = null;
            if (propertyName.Contains("."))
            {
                string[] fields = propertyName.Split('.');

                List<ObjectField> entityObjectFields = ObjectFieldRepository.GetObjectFieldsByObjectTableName(objectTableName, tenant).ToList();
                value = GetInsideEntityFieldValue(entity, entityObjectFields, fields, tenant);
            }

            else
            {
                value = GetEntityFieldValue(entity, propertyName);
            }
            return value;
        }
        #endregion

        #region GetInsideEntityFieldValue
        public string GetInsideEntityFieldValue(object entity, List<ObjectField> entityObjectFields, string[] fields, int tenant)
        {
            int i = 0;
            string resultValue = " ";
            //object currentEntity = entity;
            //List<ObjectField> currentEntityObjectFields = entityObjectFields;

            //while (true)
            //{
            //    PropertyInfo propertyPathPi = currentEntity.GetType().GetProperty(fields[i].Trim());
            //    if (propertyPathPi != null)
            //    {
            //        object value = propertyPathPi.GetValue(currentEntity, null);

            //        if (value == null)
            //        {
            //            break;
            //        }
            //        ObjectField objectField = currentEntityObjectFields.Where(f => f.FieldName == fields[i]).FirstOrDefault();
            //        if (objectField != null)
            //        {
            //            string insideEntityName = objectField.ObjectTable_LookUpTable.Name;
            //            if (insideEntityName == "Carrier")
            //            {
            //                insideEntityName = "Card";
            //            }
            //            // ==================================================================================

            //            List<ObjectField> insideEntityObjectFields = ObjectFieldRepository.GetObjectFieldsByObjectTableName(insideEntityName, tenant).ToList();
            //            ObjectField insideObjectField = insideEntityObjectFields.Where(f => f.FieldName == fields[i + 1]).FirstOrDefault();

            //            //===============================================================================================================
            //            string insideTypePath = "Logitude.BL.ShipmentsModel.Repositories." + insideEntityName + "Repository";

            //            Type insideEntityType = Type.GetType(insideTypePath);
            //            if (insideEntityType == null)
            //            {
            //                insideTypePath = "Logitude.BL.CommonDataModel.Repositories." + insideEntityName + "Repository";
            //                insideEntityType = Type.GetType(insideTypePath);
            //            }

            //            if (insideEntityType == null)
            //            {
            //                insideTypePath = "Logitude.BL." + insideEntityName + "Repository";
            //                insideEntityType = Type.GetType(insideTypePath);
            //            }

            //            if (insideEntityType == null)
            //            {

            //                insideTypePath = "Logitude.BL.InfrastructureModel.Repositories." + insideEntityName + "Repository";
            //                insideEntityType = Type.GetType(insideTypePath);
            //            }

            //            if (insideEntityType == null)
            //            {

            //                insideTypePath = "Logitude.BL.QuoteModel.Repositories." + insideEntityName + "Repository";
            //                insideEntityType = Type.GetType(insideTypePath);
            //            }

            //            object insideEntityRepository = null;
            //            if (insideEntityType != null)
            //            {
            //                if (definedObjects.Keys.Contains(insideTypePath))
            //                {
            //                    insideEntityRepository = definedObjects[insideTypePath];
            //                }

            //                if (insideEntityRepository == null)
            //                {
            //                    insideEntityRepository = Activator.CreateInstance(insideEntityType, tenant);///??????
            //                                                                                                ///
            //                    definedObjects.Add(insideTypePath, insideEntityRepository);
            //                }

            //                MethodInfo insideMethodInfo = insideEntityRepository.GetType().GetMethod("GetSinglePM");
            //                object insideEntity = null;
            //                if (insideMethodInfo != null)
            //                {
            //                    ParameterInfo[] parametersInfo = insideMethodInfo.GetParameters();
            //                    object[] parameters = new object[] { };
            //                    switch (parametersInfo.Count())
            //                    {
            //                        case 1:
            //                            parameters = new object[] { value };
            //                            break;
            //                        case 2:
            //                            parameters = new object[] { value, tenant };
            //                            break;
            //                        case 3:
            //                            parameters = new object[] { value, tenant, false };
            //                            break;
            //                        default:
            //                            parameters = new object[] { value, tenant };
            //                            break;
            //                    }

            //                    insideEntity = insideMethodInfo.Invoke(insideEntityRepository, parameters);
            //                    if (insideEntity != null)
            //                    {
            //                        if ((i + 1) < fields.Count())
            //                        {
            //                            PropertyInfo insidePropertyPathPi = insideEntity.GetType().GetProperty(fields[i + 1].Trim());
            //                            if (insidePropertyPathPi != null)
            //                            {
            //                                object insideValue = insidePropertyPathPi.GetValue(insideEntity, null);
            //                                if (insideValue != null)
            //                                {
            //                                    if (insideValue is DateTime)
            //                                    {
            //                                        DateTime date = (DateTime)insideValue;
            //                                        insideValue = date.ToShortDateString();
            //                                    }
            //                                }

            //                                resultValue = (insideValue != null ? insideValue.ToString() : " ");

            //                            }
            //                            else
            //                            {
            //                                break;
            //                            }
            //                        }
            //                        else
            //                        {

            //                            break;
            //                        }

            //                        if (insideObjectField != null)
            //                        {
            //                            if (insideObjectField.DataTypeCode == "LookUp")
            //                            {
            //                                currentEntity = insideEntity;
            //                                currentEntityObjectFields = insideEntityObjectFields;
            //                                i++;
            //                            }
            //                            else
            //                            {
            //                                // check if its a multi value
            //                                // ResolveObjectFieldValue
            //                                break;
            //                            }
            //                        }
            //                        else
            //                        {

            //                            break;
            //                        }
            //                    }
            //                    else
            //                    {
            //                        break;

            //                    }
            //                }
            //                else
            //                {
            //                    break;
            //                }
            //            }
            //            else
            //            {

            //                break;
            //            }

            //        }
            //        else
            //        {

            //            break;
            //        }

            //    }
            //    else
            //    {

            //        break;
            //    }
            //}

            return resultValue;
        }
        #endregion

        #region GetEntityFieldValue
        private object GetEntityFieldValue(object entity, string propertyName)
        {
            object resultValue = " ";
            PropertyInfo propertyPathPi = entity.GetType().GetProperty(propertyName.Trim());
            if (propertyPathPi != null)
            {
                object value = propertyPathPi.GetValue(entity, null);

                resultValue = value;
            }

            return resultValue;
        }
        #endregion
    }

    public class ValidationFieldResult
    {
        public ValidationFieldResult(bool valid, string errorMessage)
        {
            this.Valid = valid;
            this.ErrorMessage = errorMessage;
        }

        public bool Valid { get; set; }
        public string ErrorMessage { get; set; }
    }
}
