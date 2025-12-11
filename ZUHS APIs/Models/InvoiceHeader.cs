using Partner_DLL;

namespace ZUHS_APIs.Models
{
    public class InvoiceHeader
    {
        public string DocNumber { get; set; }
        public string CustomerCode { get; set; }
        public string Date { get; set; } 
        public string OrderNumber { get; set; }
        public string InvoiceMessage1 { get; set; }
        public string DeliveryAddress1 { get; set; }
        public string HeaderDescription { get; set; }
        public string ExchangeRate { get; set; }
        public List<Partner_DLL.InvoiceLine> InvoiceLines { get; set; }
    }
}
