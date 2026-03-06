namespace App.Core.Domain.Traders
{
    public class TraderLookupItem
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Vat { get; set; }
        public string Email { get; set; }
        public int CustomerTypeId { get; set; }
        public string CustomerTypeName { get; set; }
        public int LegalFormTypeId { get; set; }
        public string LegalFormTypeName { get; set; }
        public int CategoryBookTypeId { get; set; }
        public string CategoryBookTypeName { get; set; }
        public int HyperPayrollId { get; set; }
        public bool ConnectionAccountingActive { get; set; }
        public bool Active { get; set; }
        public bool HasFinancialObligation { get; set; }
    }
}