using App.Framework.Models;
using System;

namespace App.Models.Accounting
{
    public partial record SoftoneProjectSearchModel : BaseSearchModel
    {
        public SoftoneProjectSearchModel() : base("createdDate", "desc") { }
    }
    public class SoftoneProjectInfoModel
    {
        public int TraderId { get; set; }
    }
    public partial record SoftoneProjectInfoFormModel : BaseNopModel
    {
    }
    public partial record SoftoneProjectListModel : BasePagedListModel<SoftoneProjectModel>
    {
    }
    public partial record SoftoneProjectModel : BaseNopEntityModel
    {
        public string Code { get; set; } // Κωδικός
        public string Description { get; set; } // Περιγραφή
        public bool Active { get; set; } // Ενεργό
        public string Customer { get; set; } // Παλάτης
        public DateTime? StartingDate { get; set; } // Ημερ/νία έναρξης
        public DateTime? EndingDate { get; set; } // Ημερ/νία λήξης
        public DateTime? CreatedDate { get; set; } // Ημερομηνία δημιουργίας
        public string StartingDateValue { get; set; } // Ημερ/νία έναρξης
        public string EndingDateValue { get; set; } // Ημερ/νία λήξης
        public string CreatedDateValue { get; set; } // Ημερομηνία δημιουργίας
    }
    public class SoftoneProjectQueryResult
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public string Customer { get; set; }
        public DateTime? StartingDate { get; set; }
        public DateTime? EndingDate { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}