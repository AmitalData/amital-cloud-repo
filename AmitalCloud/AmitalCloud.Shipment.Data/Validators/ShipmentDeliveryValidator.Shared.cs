using AmitalCloud.Infrastructure.Data.Helpers;
using AmitalCloud.Shipment.Domain.EntityPMs;
using AmitalCloud.Shipment.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;
namespace AmitalCloud.Shipment.Domain.Validators
{
    public class ShipmentDeliveryValidator : IShipmentDeliveryValidator
    {
        public static ValidationResult IsShipmentPickUpValid(ShipmentPickUpPM shipmentPickUpPM, ValidationContext context)
        {
            string errorMessage = "";
            if (shipmentPickUpPM.FullResponsibility)
            {
                switch (shipmentPickUpPM.PickUpDeliveryFromTypeCode)
                {
                    case "PORT":
                        {
                            if (string.IsNullOrEmpty(shipmentPickUpPM.FromPortId))
                            {
                                errorMessage = errorMessage + " , " + TextCodesTranslator.TranslateText("ShipmentPickUpDelivery.M.FromPortIsRequired", shipmentPickUpPM.Tenant);
                            }
                            break;
                        }

                    case "PART":
                        {
                            if (string.IsNullOrEmpty(shipmentPickUpPM.FromPartnerCardId))
                            {
                                errorMessage = errorMessage + " , " + TextCodesTranslator.TranslateText("ShipmentPickUpDelivery.M.FromPartnerIsRequired", shipmentPickUpPM.Tenant);
                            }
                            break;
                        }
                }


                switch (shipmentPickUpPM.PickUpDeliveryToTypeCode)
                {
                    case "PORT":
                        {
                            if (string.IsNullOrEmpty(shipmentPickUpPM.ToPortId))
                            {
                                errorMessage = errorMessage + " , " + TextCodesTranslator.TranslateText("ShipmentPickUpDelivery.M.ToPortIsRequired", shipmentPickUpPM.Tenant);
                            }
                            break;
                        }

                    case "PART":
                        {
                            if (string.IsNullOrEmpty(shipmentPickUpPM.ToPartnerCardId))
                            {
                                errorMessage = errorMessage + " , " + TextCodesTranslator.TranslateText("ShipmentPickUpDelivery.M.ToPartnerIsRequired", shipmentPickUpPM.Tenant);
                            }
                            break;
                        }
                }
            }


            if (!string.IsNullOrEmpty(errorMessage))
            {
                errorMessage = errorMessage.TrimStart(',');
                return new ValidationResult(errorMessage);
            }

            else
            {
                return ValidationResult.Success;
            }
        }

        public static ValidationResult IsShipmentDeliveryValid(ShipmentDeliveryPM shipmentDeliveryPM, ValidationContext context)
        {
            string errorMessage = "";
            if (shipmentDeliveryPM.FullResponsibility)
            {
                switch (shipmentDeliveryPM.PickUpDeliveryFromTypeCode)
                {
                    case "PORT":
                        {
                            if (string.IsNullOrEmpty(shipmentDeliveryPM.FromPortId))
                            {
                                errorMessage = errorMessage + " , " + TextCodesTranslator.TranslateText("ShipmentPickUpDelivery.M.FromPortIsRequired", shipmentDeliveryPM.Tenant);
                            }
                            break;
                        }

                    case "PART":
                        {
                            if (string.IsNullOrEmpty(shipmentDeliveryPM.FromPartnerCardId))
                            {
                                errorMessage = errorMessage + " , " + TextCodesTranslator.TranslateText("ShipmentPickUpDelivery.M.FromPartnerIsRequired", shipmentDeliveryPM.Tenant);
                            }
                            break;
                        }
                }


                switch (shipmentDeliveryPM.PickUpDeliveryToTypeCode)
                {
                    case "PORT":
                        {
                            if (string.IsNullOrEmpty(shipmentDeliveryPM.ToPortId))
                            {
                                errorMessage = errorMessage + " , " + TextCodesTranslator.TranslateText("ShipmentPickUpDelivery.M.ToPortIsRequired", shipmentDeliveryPM.Tenant);
                            }
                            break;
                        }

                    case "PART":
                        {
                            if (string.IsNullOrEmpty(shipmentDeliveryPM.ToPartnerCardId))
                            {
                                errorMessage = errorMessage + " , " + TextCodesTranslator.TranslateText("ShipmentPickUpDelivery.M.ToPartnerIsRequired", shipmentDeliveryPM.Tenant);
                            }
                            break;
                        }
                }
            }


            if (!string.IsNullOrEmpty(errorMessage))
            {
                errorMessage = errorMessage.TrimStart(',');
                return new ValidationResult(errorMessage);
            }

            else
            {
                return ValidationResult.Success;
            }
        }
    }
}
