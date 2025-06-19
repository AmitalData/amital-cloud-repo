using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmitalCloud.Shipment.Domain.EntityLists
{
    public class ShipmentsReceivablesPayablesList
    {
        [Key]
        public string Id { get; set; }
        //public string LineTypeCode { get; set; }
        public string ShipmentId { get; set; }  
        public string ShipmentNumber { get; set; }        
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public DateTime? CreateDateTime { get; set; }
        public DateTime? MainCarriageATA { get; set; }
        public DateTime? MainCarriageATD { get; set; }
        public DateTime? OperationalDate { get; set; }
        public string Master { get; set; }
        public string House { get; set; }
        public bool IsOperationalClosed { get; set; }
        public bool IsAccountingClosed { get; set; }

        public string ChargeTypeId { get; set; }
        public string ChargeTypeCode { get; set; }
        public string ChargeTypeName { get; set; }
        public double? Payables_OPEN { get; set; }
        public double? Payables_ACCT { get; set; }
        public double? Receivables_OPEN { get; set; }
        public double? Receivables_ACCT { get; set; }

        public double? GrossWeight { get; set; }
        public double? ChargeableWeight { get; set; }
        public int? Quantity { get; set; }
        public double? TEU { get; set; }

        public DateTime? FirstPickupETD { get; set; }
        public DateTime? MainCarriageETD { get; set; }
        public string TransportMode { get; set; }
        public string MainCarriagePortCode { get; set; }
        public string FinalDestinationPortCode { get; set; }
        public double? ValueOfGoods { get; set; }
        public string FlightNumber { get; set; }
        public string ChargeGroupName { get; set; }
        public string ChargeGroupCode { get; set; }
        public double? ExchangeRate { get; set; }

    }
}
