using App.Framework.Models;
using System;
using System.Collections.Generic;

namespace App.Models.Payroll
{
    public partial record WorkerScheduleDateSearchModel : BaseSearchModel
    {
        public WorkerScheduleDateSearchModel() : base("workingDate") { }
    }
    public partial record WorkerScheduleDateListModel : BasePagedListModel<WorkerScheduleDateModel>
    {
    }
    public partial record WorkerScheduleDateModel : BaseNopEntityModel
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
        public bool Leave { get; set; } // Άδεια           
        public bool SickLeave { get; set; } // Ασθένεια
        public int WeekOfYear { get; set; } // Εβδομάδα του έτους
        public bool IsSaturday { get; set; } // Σάββατο
        public bool IsSunday { get; set; } // Κυριακή
        public DateTime BreakNonstop2FromDate { get; set; } // Διάλειμμα 2o από
        public DateTime BreakNonstop2ToDate { get; set; } // Διάλειμμα 2o εώς           
        public bool IsSplit { get; set; } // Διακεκομμένο                
        //
        public string OvertimeTypeName { get; set; } // Τύπος υπερωρίας       
        public string WorkerCardName { get; set; } // Εργαζόμενος   
        public string WorkerName { get; set; } // Ονοματεπώνυμο
        public double DailyNonstop { get; set; } // Ημερήσιο συνεχόμενο ωράριο     
        public double DailySplit { get; set; } // Ημερήσιο διακεκομμένο ωράριο     
        public double DailyBreak { get; set; } // Ημερήσιο διάλειμμα     
        public double DailyTotalHours { get; set; } // Ημερήσιο ωράριο άθροισμα  
    }
    public partial record WorkerScheduleDateFormModel : BaseNopModel
    {
    }

    public partial record WorkerScheduleResult
    {
        public string WorkerName { get; set; } // Ονοματεπώνυμο
        public string Period { get; set; } // Περίοδος
        public bool MaxFortyHoursPerWeekError { get; set; } // Όριο εργάσημων ωρών εβδομάδας
        public bool MaxSixDaysPerWeekError { get; set; } // Όριο εργάσημων ημερών εβδομάδας
        public bool ContractChangeError { get; set; } // Αλλαγή σύμβασης
        public bool Leave { get; set; } // Άδεια           
        public bool SickLeave { get; set; } // Ασθένεια
        public double WorkingHours { get; set; } // Σύμβαση εργασίας
        public double WeeklyTotalHours { get; set; } // Σύνολο εβδομαδιαίου ωραρίου
        public string WorkingHoursValue { get; set; } // Σύμβαση εργασίας
        public string WeeklyTotalHoursValue { get; set; } // Σύνολο εβδομαδιαίου ωραρίου
        public IList<WorkerScheduleDetailsResult> Details { get; set; }

    }
    public partial record WorkerScheduleDetailsResult
    {
        public string WorkerVat { get; set; } // ΑΦΜ εργαζομένου
        public string WorkerName { get; set; } // Ονοματεπώνυμο
        public DateTime WorkingDate { get; set; } // Ημέρα
        public double DailyNonstop { get; set; } // Ημερήσιο συνεχόμενο ωράριο     
        public double DailySplit { get; set; } // Ημερήσιο διακεκομμένο ωράριο     
        public double DailyBreak { get; set; } // Ημερήσιο διάλειμμα     
        public double DailyTotalHours { get; set; } // Ημερήσιο ωράριο άθροισμα
        public string DailyNonstopValue { get; set; } // Ημερήσιο συνεχόμενο ωράριο     
        public string DailySplitValue { get; set; } // Ημερήσιο διακεκομμένο ωράριο     
        public string DailyBreakValue { get; set; } // Ημερήσιο διάλειμμα     
        public string DailyTotalHoursValue { get; set; } // Ημερήσιο ωράριο άθροισμα
        public bool Leave { get; set; } // Άδεια           
        public bool SickLeave { get; set; } // Ασθένεια

    }
}