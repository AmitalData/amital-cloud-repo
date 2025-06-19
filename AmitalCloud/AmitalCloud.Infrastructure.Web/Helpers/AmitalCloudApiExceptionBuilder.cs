using Marvin.JsonPatch.Exceptions;
using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;

namespace AmitalCloud.Infrastructure.Web.Helpers
{
    public class AmitalCloudApiExceptionBuilder
    {
        public static APIException BuildException(Exception ex)
        {

            APIException apiException = new APIException();
            string ErrorMessage = "";
            string ShortErrorMessage = "";
            if (ex.GetType().Name == "DbEntityValidationException")
            {
                if (ex.InnerException != null)
                {
                    ErrorMessage += ex.InnerException.Message + ";";
                    ShortErrorMessage += ex.InnerException.Message + ";";
                }
                else
                {
                    ErrorMessage += ex.Message + ";";
                    ShortErrorMessage += ex.Message + ";";
                }

                apiException = new APIException()
                {
                    ErrorType = ex.GetType().Name,
                    ErrorMessage = ErrorMessage,
                    ShortErrorMessage = ShortErrorMessage,
                };
            }
            else
            {
                string errorMessage = ex.Message + Environment.NewLine;
                string shortErrorMessage = ex.Message + Environment.NewLine;
                if (ex.InnerException != null)
                {

                    //errorMessage = errorMessage + " (" + (ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message) + ")" + Environment.NewLine;
                    //shortErrorMessage = shortErrorMessage + " (" + (ex.InnerException.InnerException != null ? ex.InnerException.InnerException.Message : ex.InnerException.Message) + ")" + Environment.NewLine;
                }

                apiException = new APIException()
                {
                    ErrorType = ex.GetType().Name,
                    ErrorMessage = errorMessage,    //    + " (" + ex.StackTrace + ")",
                    ShortErrorMessage = shortErrorMessage
                };
            }

            return apiException;
        }


        public static APIException BuildModelException(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary modelState)
        {
            APIException apiException = new APIException();
            string ErrorMessage = "";
            string ShortErrorMessage = "";

            foreach (var modValue in modelState.Values)
            {
                foreach (var error in modValue.Errors)
                {
                    //ErrorMessage += error.ErrorMessage + Environment.NewLine;
                    ErrorMessage += (String.IsNullOrWhiteSpace(error.ErrorMessage) ? error.Exception.Message : error.ErrorMessage) + Environment.NewLine;
                    ShortErrorMessage += error.ErrorMessage + Environment.NewLine;
                }
            }

            apiException = new APIException()
            {
                ErrorType = "ModelStateError",
                ErrorMessage = ErrorMessage,
                ShortErrorMessage = ShortErrorMessage,
            };

            return apiException;
        }

        public static object BuildJsonPatchException(JsonPatchException exception, string entityId, object jsonPatch)
        {
            string errorMessage = "Error Message: " + exception.Message + Environment.NewLine +
                                  "Entity Id: " + entityId + Environment.NewLine +
                                  "Json Patch: " + JsonConvert.SerializeObject(jsonPatch);

            string shortErrorMessage = GetShortErrorMessage(exception);

            return new APIException()
            {
                ErrorType = exception.GetType().Name,
                ErrorMessage = errorMessage,
                ShortErrorMessage = shortErrorMessage
            };
        }

        private static string GetShortErrorMessage(JsonPatchException exception)
        {
            if (Regex.IsMatch(exception.Message, @"The current value '.*' at path '.*' is not equal to the test value '.*'\."))
            {
                return "Some of the " + GetChildEntityName(exception) + " were deleted.";
            }
            else
            {
                return "Invalid update";
            }
        }

        private static string GetChildEntityName(JsonPatchException exception)
        {
            var failedOperation = exception.FailedOperation;
            string entityNameInCamelCase = failedOperation.path.Split('/')[1];

            string returnedEntityName = Regex.Replace(entityNameInCamelCase, @"\p{Lu}", m => " " + m.Value.ToLowerInvariant());
            return returnedEntityName;
        }
    }
}
