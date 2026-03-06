using System;

namespace App.Core.Domain.Payroll
{
    public partial class WorkerScheduleDate : BaseEntity // Ωράρια εργασίας
    {
        public int WorkerScheduleId { get; set; }
        public int TraderId { get; set; }
        public int WorkerId { get; set; }
        public string WorkerVat { get; set; } // ΑΦΜ εργαζομένου
        public DateTime WorkingDate { get; set; } // Ημέρα
        public DateTime NonstopFromDate { get; set; } // Συνεχόμενο από
        public DateTime NonstopToDate { get; set; } // Συνεχόμενο εώς
        public DateTime SplitFromDate { get; set; } // Διακεκομμένο από
        public DateTime SplitToDate { get; set; } // Διακεκομμένο εώς
        public DateTime BreakNonstopFromDate { get; set; } // Διάλειμμα από
        public DateTime BreakNonstopToDate { get; set; } // Διάλειμμα εώς    
        public DateTime BreakSplitFromDate { get; set; } // Διάλειμμα από
        public DateTime BreakSplitToDate { get; set; } // Διάλειμμα εώς    
        public DateTime OvertimeFromDate { get; set; } // Υπερωρία από
        public DateTime OvertimeToDate { get; set; } // Υπερωρία εώς     
        public int OvertimeTypeId { get; set; } // Τύπος υπερωρίας        
        public bool Active { get; set; }           
        public bool IsSplit { get; set; } // Διακεκομμένο                  
        public bool Leave { get; set; } // Άδεια           
        public bool SickLeave { get; set; } // Ασθένεια
        public int WeekOfYear { get; set; } // Εβδομάδα του έτους
        public bool IsSaturday { get; set; } // Σάββατο
        public bool IsSunday { get; set; } // Κυριακή

        //migration
        public DateTime BreakNonstop2FromDate { get; set; } // Διάλειμμα 2o από
        public DateTime BreakNonstop2ToDate { get; set; } // Διάλειμμα 2o εώς    
    }
}
