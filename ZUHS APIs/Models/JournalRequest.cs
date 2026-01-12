namespace ZUHS_APIs.Models
{
    public class JournalRequest
    {

        public string Date { get; set; }
        public string AccountNumber { get; set; }
        public string Reference { get; set; }
        public string Description { get; set; }
        public string Amount { get; set; }
        public string TaxType { get; set; }
        public string TaxAmount { get; set; }
        public string Contra { get; set; }
        public string ExchangeRate { get; set; }
        public string Currency { get; set; }
    }
}
