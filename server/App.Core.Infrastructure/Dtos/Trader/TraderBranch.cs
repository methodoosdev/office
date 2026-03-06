namespace App.Core.Infrastructure.Dtos.Trader
{
    public partial class TraderBranchDto
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string Kind { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public string Doy { get; set; }
        public int TraderId { get; set; }
    }
}
