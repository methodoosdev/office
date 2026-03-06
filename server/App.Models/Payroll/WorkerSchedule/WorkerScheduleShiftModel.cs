using App.Framework.Models;
using System;

namespace App.Models.Payroll
{
    public partial record WorkerScheduleShiftSearchModel : BaseSearchModel
    {
        public WorkerScheduleShiftSearchModel() : base("displayOrder") { }
    }
    public partial record WorkerScheduleShiftListModel : BasePagedListModel<WorkerScheduleShiftModel>
    {
    }
    public partial record WorkerScheduleShiftModel : BaseNopEntityModel // Βάρδιες
    {
        public int TraderId { get; set; }
        public int DisplayOrder { get; set; } // Κατάταξη
        public string Description { get; set; } // Περιγραφή
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
        public DateTime BreakNonstop2FromDate { get; set; } // Διάλειμμα 2o από
        public DateTime BreakNonstop2ToDate { get; set; } // Διάλειμμα 2o εώς           
        public bool IsSplit { get; set; } // Διακεκομμένο                
    }
    public partial record WorkerScheduleShiftFormModel : BaseNopModel
    {
    }
}