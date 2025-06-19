using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Shipment.Domain.EntityPMs;
using System.ComponentModel.DataAnnotations;

namespace AmitalCloud.Shipment.Domain.Validators
{
    public class ShipmentReceivableValidator
    {
        public static ValidationResult IsShipmentReceivableValid(ShipmentReceivablePM shipmentReceivablePM, ValidationContext context)
        {
            if (shipmentReceivablePM.TotalAmount != null && shipmentReceivablePM.UnitPrice != null && shipmentReceivablePM.Quantity != null)
            {
                if (shipmentReceivablePM.Rate == null)
                {
                    return new ValidationResult(TextCodesTranslator.TranslateText("ShipmentReceivable.M.ExchangeRateIsRequired", shipmentReceivablePM.Tenant));
                }
            }

            return ValidationResult.Success;
        }
    }
}
