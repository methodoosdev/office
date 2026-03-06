namespace App.Core.Domain.Traders
{
    public partial class TraderGroup : BaseEntity
    {
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
    }
}
