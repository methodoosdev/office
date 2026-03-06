using App.Framework.Components;
using System.Collections.Generic;

namespace App.Models.Payroll
{
    public partial record WorkerScheduleCheckModel
    {
        public IList<ColumnConfig> Columns { get; set; }
        public List<ColumnConfig> DetailColumns { get; set; }
        public string Title { get; set; }
        public string TraderName { get; set; }
        public string FileName { get; set; }
        public bool IsTrader { get; set; }
    }
}