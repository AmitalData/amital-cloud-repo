
using AmitalCloud.Shipment.Domain.EntityPMs;
using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Shipment.Domain.Validators
{
    public class ShipmentPayableValidator
    {
        public static ValidationResult IsShipmentPayableValid(ShipmentPayablePM shipmentPayablePM, ValidationContext context)
        {
            //if (shipmentPayablePM.TotalAmount != null && shipmentPayablePM.UnitPrice != null && shipmentPayablePM.Quantity != null)
            //{
            //    if (shipmentPayablePM.VendorId == null)
            //    {
            //        return new ValidationResult("The vendor is required!");
            //    }
            //}
            return ValidationResult.Success;

        }
    }
}
