using AmitalCloud.Infrastructure.Domain.EntityPMs;
using AmitalCloud.Shipment.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace AmitalCloud.Shipment.Domain.EntityPMs
{
    [CustomValidation(typeof(IShipmentClassLevelValidator), "ValidateClass")]
    [CustomValidation(typeof(IShipmentDeliveryValidator), "IsShipmentPickUpValid")]
    public partial class ShipmentPickUpPM : ChildEntitiesCustomFieldPM
    {
        [Key]
        public string Id { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public string ShipmentId { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public string PickUpDeliveryNumber { get; set; }
        public int PickUpDeliveryIndex { get; set; }
        public int ChildIndex { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public string PickUpDeliveryTypeCode { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public bool FullResponsibility { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public string PickUpDeliveryFromTypeCode { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public string PickUpDeliveryToTypeCode { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public string FromPortId { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public string ToPortId { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public string FromPartnerCardId { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public string FromAddressId { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public string FromAddress { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public string FromAddressCity { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public string FromAddressZipCode { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public string FromAddressCountryId { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public string ToPartnerCardId { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public string ToAddressId { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public string ToAddress { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public string ToAddressCity { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public string ToAddressZipCode { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public string ToAddressCountryId { get; set; }

        #region Port Dummy fields
        public string FromPortCode { get; set; }
        public string FromPortName { get; set; }
        public string FromPortCountryCode { get; set; }
        public string FromPortCountryName { get; set; }

        public string ToPortCode { get; set; }
        public string ToPortName { get; set; }
        public string ToPortCountryCode { get; set; }
        public string ToPortCountryName { get; set; }
        #endregion

        #region Address Dummy fields
        public string FromAddressCity_Dummy { get; set; }
        public string FromAddressCountryCode { get; set; }
        public string FromAddressCountryName { get; set; }
        public string FromLocation { get; set; }

        public string ToAddressCity_Dummy { get; set; }
        public string ToAddressCountryCode { get; set; }
        public string ToAddressCountryName { get; set; }
        public string ToLocation { get; set; }
        #endregion

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public DateTime? ATD { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public DateTime? ATA { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public DateTime? ETD { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public DateTime? ETA { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public string CarrierId { get; set; }
        public string CarrierCode { get; set; }
        public string CarrierName { get; set; }
        public string CarrierTypeName { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public string CarrierNumber { get; set; }
        public string CarrierWebSite { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public string Driver { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public string TruckNumber { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public string TrailerNumber { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public string Notes { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public string EmptyPickupContainerPartnerId { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public string EmptyPickupDepotReference { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public string EmptyDeliveryContainerPartnerId { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public string EmptyDeliveryDepotReference { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public string TransportModeCode { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public string TransportModeName { get; set; }

        public string CustomerId { get; set; }
        public string DirectionId { get; set; }
        public string MasterNumber { get; set; }
        public string AgentName { get; set; }
        public string ShipmentNumber { get; set; }
        public string ShippingLine { get; set; }
        public double? PackageTEU { get; set; }
        public string AgentId { get; set; }
        public string ParentPickUpDeliveryId { get; set; }
        public int? ChildPickUpIndex { get; set; }
        public string BookingConfirmationNumber { get; set; }

        private List<ShipmentPickUpDeliveryPackagePM> shipmentPickUpDeliveryPackages;
        
        
        [Association("ShipmentDeliveryPackagePMShipmentPickUpPM", "Id", "ShipmentPickUpDeliveryId")]
        public virtual List<ShipmentPickUpDeliveryPackagePM> ShipmentPickUpDeliveryPackages
        {
            get
            {
                if (shipmentPickUpDeliveryPackages == null)
                {
                    shipmentPickUpDeliveryPackages = new List<ShipmentPickUpDeliveryPackagePM>();
                }

                return this.shipmentPickUpDeliveryPackages;
            }
            set
            {
                if (value != null)
                {
                    shipmentPickUpDeliveryPackages = value;
                }
            }
        }


        public List<ShipmentPickUpDeliveryPackagePM> ShipmentPickUpPackagesChangeSet { get; set; }
        public string StandaloneShipmentId { get; set; }
        public string StandaloneShipmentNumber { get; set; }
        public bool IsConnectedToStandalone { get; set; }
        public string CarrierLocalName { get; set; }
        public string ChangeSet { get; set; }
    }
}
