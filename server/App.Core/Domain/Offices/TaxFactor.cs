namespace App.Core.Domain.Offices
{
    public partial class TaxFactor : BaseEntity
    {
        public int Year { get; set; }
        public int TaxIncome { get; set; }
        public int TaxAdvance { get; set; }
        public int TaxAdvanceBCategory { get; set; }
    }
}
