using AmitalCloud.Infrastructure.Domain.BaseClasses;
using AmitalCloud.Shipment.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;
namespace AmitalCloud.Shipment.Domain.EntityPMs
{
    [CustomValidation(typeof(IShipmentClassLevelValidator), "ValidateClass")]
    public partial class CommodityPackagePM : BaseEntityPM
    {
        [Key]
        public string Id { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public string PackageTypeId { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public int? Quantity { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public double? Weight { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public double? Volume { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public double? VolumetricWeight { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public double? Height { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public double? Width { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public double? Length { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public string Description { get; set; }

        public string ShipmentNumber { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public string ShipmentId { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public string CommodityId { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public int NumberOfInsidePackages { get; set; }

        [CustomValidation(typeof(IShipmentValidationClass), "ValidateClass")]
        public string NumberOfInsidePackagesDetails { get; set; }
    }
}