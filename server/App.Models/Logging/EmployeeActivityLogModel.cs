using App.Framework.Models;
using System;

namespace App.Models.Logging
{
    public partial record EmployeeActivityLogSearchModel : BaseNopModel
    {
        public DateTime From { get; set; } // Ημερομηνία από
        public DateTime To { get; set; } // Ημερομηνία εως
    }
    public partial record EmployeeActivityLogSearchFormModel : BaseNopModel
    {
    }

    public partial record EmployeeActivityLogModel : BaseNopModel
    {
        public string UserName { get; set; }
        public string NickName { get; set; }
        public string EmployeeName { get; set; }
        public string ActivityLogType { get; set; }
        public int ActivityCount { get; set; }

    }

    public partial record EmployeeActivityLogTableModel : BaseNopModel
    {
    }
}
