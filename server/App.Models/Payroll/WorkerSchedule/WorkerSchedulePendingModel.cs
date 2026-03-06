using App.Core.Domain.Common;
using App.Framework.Models;
using System;

namespace App.Models.Payroll
{
    public partial record WorkerSchedulePendingSearchModel : BaseSearchModel
    {
        public WorkerSchedulePendingSearchModel() : base("submited") { }
    }
    public partial record WorkerSchedulePendingListModel : BasePagedListModel<WorkerSchedulePendingModel>
    {
    }
    public partial record WorkerSchedulePendingModel : BaseNopEntityModel, IFullName
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string TraderName { get; set; } // Επωνυμία Εργοδότη  
        public DateTime? PeriodToDate { get; set; } // Περίοδος  
        public bool Submited { get; set; } // Υποβολή  
    }
}