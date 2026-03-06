using App.Framework.Models;

namespace App.Models.Payroll
{
    public partial record WorkerScheduleWorkerSearchModel : BaseSearchModel
    {
        public WorkerScheduleWorkerSearchModel() : base("workerCardName") { }
    }
    public partial record WorkerScheduleWorkerListModel : BasePagedListModel<WorkerScheduleWorkerModel>
    {
    }
    public partial record WorkerScheduleWorkerModel : BaseNopEntityModel // Εργαζόμενος
    {
        public int WorkerScheduleId { get; set; }
        public int TraderId { get; set; }
        public int WorkerId { get; set; }
        public double WorkingHours { get; set; } // Ώρες εργασίας
        public string WorkerCardName { get; set; } // Εργαζόμενος
        public string WorkerName { get; set; } // Ονοματεπώνυμο
        public string WorkerVat { get; set; } // ΑΦΜ εργαζομένου
        //
        public string TraderName { get; set; } // Συναλλασσόμενος // Επωνυμία Εργοδότη      
    }
    public partial record WorkerScheduleWorkerFormModel : BaseNopModel
    {
    }
}