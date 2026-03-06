using App.Framework.Models;
using System;

namespace App.Models.Logging
{
    public partial record TraderActivityLogSearchModel : BaseNopModel
    {
        public DateTime From { get; set; } // Ημερομηνία από
        public DateTime To { get; set; } // Ημερομηνία εως
    }
    public partial record TraderActivityLogSearchFormModel : BaseNopModel
    {
    }

    public partial record TraderActivityLogModel : BaseNopModel
    {
        public string UserName { get; set; }
        public string NickName { get; set; }
        public string TraderName { get; set; }
        public string ActivityLogType { get; set; }
        public int ActivityCount { get; set; }

    }

    public partial record TraderActivityLogTableModel : BaseNopModel
    {
    }
}
