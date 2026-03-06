namespace App.Core.Domain.Traders
{
    public partial class AccountingWork : BaseEntity
    {
        public string SortDescription { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public int Price { get; set; }
    }
}
