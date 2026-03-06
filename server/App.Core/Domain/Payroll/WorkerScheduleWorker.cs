using System;

namespace App.Core.Domain.Payroll
{
    public partial class WorkerScheduleWorker : BaseEntity // Εργαζόμενοι
    {
        public int WorkerScheduleId { get; set; }
        public int TraderId { get; set; }
        public int WorkerId { get; set; }
        public bool ActiveCard { get; set; }
        public double WorkingHours { get; set; } // Ώρες εργασίας
        public string WorkerCardName { get; set; } // Εργαζόμενος
        public string WorkerName { get; set; } // Ονοματεπώνυμο
        public string WorkerVat { get; set; } // ΑΦΜ εργαζομένου
    }
}
