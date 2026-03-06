using System;

namespace App.Core.Infrastructure.Dtos.Accounting
{
    public class ESendDto
    {
        public string ZhtaNo { get; set; }
        public string Tameiaki { get; set; }
        public DateTime Date { get; set; }
        public decimal A6 { get; set; }
        public decimal B13 { get; set; }
        public decimal C24 { get; set; }
        public decimal D36 { get; set; }
        public decimal E0 { get; set; }
    }

    public class ESendFromBody
    {
        public string TraderName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public DateTime Date { get; set; }
        public string ConnectionId { get; set; }
    }

}