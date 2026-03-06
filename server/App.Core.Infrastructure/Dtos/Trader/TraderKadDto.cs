namespace App.Core.Infrastructure.Dtos.Trader
{
    public partial class TraderKadDto
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool Activity { get; set; }
        public int TraderId { get; set; }
    }
}
