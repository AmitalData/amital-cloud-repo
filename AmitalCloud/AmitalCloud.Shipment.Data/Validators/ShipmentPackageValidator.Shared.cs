using AmitalCloud.Shipment.Domain.EntityPMs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Shipment.Domain.Validators
{
    public class ShipmentPackageValidator
    {
        public static ValidationResult IsShipmentPackageValid(ShipmentPackagePM shipmentPackagePM, ValidationContext context)
        {
            List<string> errorsList = new List<string>();


            if (errorsList.Count == 0)
            {
                return ValidationResult.Success;
            }

            else
            {
                string errorString = String.Empty;
                foreach (string error in errorsList)
                {
                    errorString = errorString + error + ",";
                }

                errorString = errorString.Remove(errorString.Length - 1);
                return new ValidationResult(errorString);
            }
        }
    }
}
