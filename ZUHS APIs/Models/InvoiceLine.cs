namespace ZUHS_APIs.Models
{
    public class InvoiceLine
    {
        public decimal CostPrice { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitSellingPrice { get; set; }
        public decimal InclusivePrice { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
     

    }
}
