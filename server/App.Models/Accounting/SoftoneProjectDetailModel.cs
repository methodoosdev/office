using App.Framework.Models;
using System;

namespace App.Models.Accounting
{
    public partial record SoftoneProjectDetailSearchModel : BaseSearchModel
    {
        public SoftoneProjectDetailSearchModel() : base("createdOn") { }
        public int ProjectId { get; set; }
    }
    public partial record SoftoneProjectDetailListModel : BasePagedListModel<SoftoneProjectDetailModel>
    {
    }
    public partial record SoftoneProjectDetailModel : BaseNopEntityModel
    {
        public DateTime CreatedOn { get; set; } // Ημερομηνία
        public string Invoice { get; set; } // Παραστατικό
        public string InvoiceType { get; set; } // Κίνηση
        public string Comments { get; set; } // Αιτιολογία
        public string TraderId { get; set; } // Κωδικός
        public string TraderName { get; set; } // Επωνυμία
        public decimal IncomeFpa { get; set; } // Έσοδα χωρίς ΦΠΑ
        public decimal Income { get; set; } // Έσοδα με ΦΠΑ
        public decimal ExpensesFpa { get; set; }  // Έξοδα χωρίς ΦΠΑ
        public decimal Expenses { get; set; } // Έξοδα με ΦΠΑ
        public decimal Collection { get; set; } // Εισπράξεις
        public decimal Payment { get; set; } // Πληρωμές
        public decimal Fpa { get; set; } // ΦΠΑ
    }
    
    public class SoftoneProjectDetailQueryResult
    {
        //public int Id { get; set; }
        public int SodType { get; set; }
        public int SoSource { get; set; }
        public DateTime CreatedOnUtc { get; set; } // Ημερομηνία
        public string TraderId { get; set; } // Κωδικός
        public string TraderName { get; set; } // Επωνυμία
        public int InvoiceTypeId { get; set; }
        public string Invoice { get; set; } // Παραστατικό
        public string InvoiceType { get; set; } // Κίνηση
        public string Comments { get; set; } // Αιτιολογία
        public decimal FpaAmount { get; set; } // Αξία με ΦΠΑ
        public decimal Amount { get; set; } // Αξία
        public decimal VatAmount { get; set; } // ΦΠΑ
    }

    public class SoftoneProjectItemQueryResult
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }
}