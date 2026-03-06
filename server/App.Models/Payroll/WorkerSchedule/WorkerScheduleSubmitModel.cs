using App.Core.Domain.Payroll;
using App.Framework.Components;
using System.Collections.Generic;

namespace App.Models.Payroll
{
    public partial record WorkerScheduleSubmitModel
    {
        public Dictionary<string, ColumnConfig> Columns { get; set; }
        public string Title { get; set; }
        public string TraderName { get; set; }
        public int BreakLimit { get; set; }
        public bool CanEdit { get; set; }
        public bool IsTrader { get; set; }

        public IList<WorkerScheduleShift> Shifts { get; set; }
    }
}