using System;

namespace App.Core.Domain.Payroll
{
    public partial class WorkerSchedule : BaseEntity // Υποβολή ωραρίων
    {
        public int TraderId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime PeriodFromDate { get; set; } // Περίοδος από
        public DateTime PeriodToDate { get; set; } // Περίοδος εώς
        public DateTime DeliveryDate { get; set; } // Ημ.Παραλαβής
        public DateTime SubmitDate { get; set; } // Ημ.Υποβολής
        public string Protocol { get; set; } // Πρωτόκολο
        public string Notes { get; set; } // Σχόλια
        public int WorkerScheduleTypeId { get; set; } //Τύπος ωραρίου
        public int WorkerScheduleModeTypeId { get; set; } //Κατάσταση ωραρίου
    }
}
