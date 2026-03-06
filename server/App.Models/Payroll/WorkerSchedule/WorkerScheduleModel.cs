using App.Framework.Models;
using System;
using System.Collections.Generic;

namespace App.Models.Payroll
{
    public partial record WorkerScheduleFilterModel : BaseNopModel
    {
        public WorkerScheduleFilterModel()
        {
            WorkerScheduleModeTypeId = new List<int>();
        }
        public List<int> WorkerScheduleModeTypeId { get; set; }  // Κατάσταση ωραρίου  
        public int TraderId { get; set; } = 0; // Επωνυμία Εργοδότη   
    }
    public partial record WorkerScheduleFilterFormModel : BaseNopModel
    {
    }
    public partial record WorkerScheduleSearchModel : BaseSearchModel
    {
        public WorkerScheduleSearchModel() : base("deliveryDate", "desc") { }
    }
    public partial record WorkerScheduleListModel : BasePagedListModel<WorkerScheduleModel>
    {
    }
    public partial record WorkerScheduleModel : BaseNopEntityModel
    {
        public WorkerScheduleModel()
        {
            Workers = new List<int>();
        }
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
        //
        public string Description { get; set; } // Περίοδος
        public List<int> Workers { get; set; } // Εργαζόμενος
        public string WorkerCardNames { get; set; } // Εργαζόμενος
        public string TraderName { get; set; } // Συναλλασσόμενος // Επωνυμία Εργοδότη     
        public string EmployeeName { get; set; } // Υπεύθυνος διαχείρησης  
        public string WorkerScheduleTypeName { get; set; } //Τύπος ωραρίου
        public string WorkerScheduleModeTypeName { get; set; } //Κατάσταση ωραρίου    
    }
    public partial record WorkerScheduleFormModel : BaseNopModel
    {
    }
    public class WorkerScheduleQueryResult
    {
        public int WorkerId { get; set; }
        public int CompanyId { get; set; }
        public bool ActiveCard { get; set; }
        public double WorkingHours { get; set; }
        public string WorkerCardName { get; set; }
        public string WorkerName { get; set; }
        public string WorkerVat { get; set; }
    }
}