using App.Framework.Models;
using System;

namespace App.Models.Payroll
{
    public partial record WorkerScheduleLogFilterModel : BaseNopModel
    {
        public int TraderId { get; set; } = 0; // Εργοδοτης
        public int WorkerScheduleModeTypeId { get; set; } = 0; // Κατάσταση ωραρίου
        public DateTime? PeriodFrom { get; set; } = null; // Περίοδος από
        public DateTime? PeriodTo { get; set; } = null; // Περίοδος εώς
    }
    public partial record WorkerScheduleLogFilterFormModel : BaseNopModel
    {
    }
    public partial record WorkerScheduleLogSearchModel : BaseSearchModel
    {
        public WorkerScheduleLogSearchModel() : base("submitDate", "desc") { }
    }
    public partial record WorkerScheduleLogListModel : BasePagedListModel<WorkerScheduleLogModel>
    {
    }
    public partial record WorkerScheduleLogModel : BaseNopEntityModel // Ιστορικότητα
    {
        public int WorkerScheduleId { get; set; }
        public int TraderId { get; set; }
        public string Period { get; set; } // Περίοδος
        public DateTime SubmitDate { get; set; } // Ημ.Ενημέρωσης
        public string Notes { get; set; } // Σχόλια   

        //
        public string TraderName { get; set; } // Περίοδος
        public DateTime SubmitDateValue { get; set; } // Ημ.Ενημέρωσης
        public int WorkerScheduleModeTypeId { get; set; } // Κατάσταση ωραρίου
        public string WorkerScheduleModeTypeName { get; set; } // Κατάσταση ωραρίου
    }
    public partial record WorkerScheduleLogFormModel : BaseNopModel
    {
    }
}