using AmitalCloud.Infrastructure.Domain.BaseClasses;
using AmitalCloud.Invoice.Domain.EntityPMs;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace AmitalCloud.Shipment.Domain.EntityPMs
{
    public partial class ShipmentARInvoiceMoneyPM : BaseEntityPM
    {
        public string Id { get; set; }

        private List<ShipmentARInvoicePM> aRInvoices;
        
        [Association("ShipmentARInvoiceShipmentARMoney", "Id", "ShipmentARInvoiceMoneyId")]
        public List<ShipmentARInvoicePM> ARInvoices
        {
            get
            {
                if (aRInvoices == null)
                {
                    aRInvoices = new List<ShipmentARInvoicePM>();
                }

                return aRInvoices;
            }

            set
            {
                if (value != null)
                {
                    aRInvoices = value;
                }
            }
        }

        private List<ARInvoiceChargePM> aRCharges;
        
        [Association("ShipmentARChargeShipmentARMoney", "Id", "ShipmentARChargeMoneyId")]
        public List<ARInvoiceChargePM> ARCharges
        {
            get
            {
                if (aRCharges == null)
                {
                    aRCharges = new List<ARInvoiceChargePM>();
                }

                return aRCharges;
            }

            set
            {
                if (value != null)
                {
                    aRCharges = value;
                }
            }
        }




        public bool IsShowAmountLocalCurrencyColumnInSharedLogistics { get; set; }

    }
}