using System;

namespace App.Core.Domain.Payroll
{
    public partial class WorkerScheduleLog : BaseEntity // Ιστορικότητα
    {
        public int WorkerScheduleId { get; set; }
        public int TraderId { get; set; }
        public string Period { get; set; } // Περίοδος
        public DateTime SubmitDate { get; set; } // Ημ.Ενημέρωσης
        public string Notes { get; set; } // Σχόλια   
    }
}
