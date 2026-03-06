using App.Framework.Models;
using System;
using System.Collections.Generic;

namespace App.Models.Financial
{
    public partial record FinancialObligationFilterModel : BaseNopModel
    {
        public FinancialObligationFilterModel()
        {
            TraderIds = new List<int>();
        }

        public IList<int> TraderIds { get; set; }
        public string Institution { get; set; } = null;
        public string PaymentType { get; set; } = null;
        //public DateTime? PaymentDate { get; set; } = null;
        public int IsSent { get; set; } = 0;
        public int? CustomerTypeId { get; set; } = -1;
        public int? LegalFormTypeId { get; set; } = -1;
        public int? CategoryBookTypeId { get; set; } = -1;
        public int? CustomerId { get; set; } = 0;
        public DateTime? PeriodDate { get; set; } = null;
    }
    public partial record FinancialObligationFilterFormModel : BaseNopModel
    {
    }
    public partial record FinancialObligationRequestModel : BaseNopModel
    {
        public FinancialObligationRequestModel()
        {
            TraderIds = new List<int>();
            ServiceIds = new List<int>();
        }

        public IList<int> TraderIds { get; set; }
        public IList<int> ServiceIds { get; set; }
        public int ChoiceId { get; set; }
        public string Progress { get; set; }
    }
    public partial record FinancialObligationRequestFormModel : BaseNopModel
    {
    }
    public partial record FinancialObligationSearchModel : BaseSearchModel
    {
        public FinancialObligationSearchModel() : base("createdOn", "desc") { }
    }
    public partial record FinancialObligationListModel : BasePagedListModel<FinancialObligationModel>
    {
    }
    public partial record FinancialObligationModel : BaseNopEntityModel
    {
        public int TraderId { get; set; }
        public string Institution { get; set; }      // Φορέας 
        public string PaymentType { get; set; }      //Είδος πληρωμής
        public decimal PaymentValue { get; set; }    // Ποσό οφειλής
        public string PaymentIdentity { get; set; }  // Ταυτότητα πληρωμής
        public DateTime PaymentDate { get; set; }     // Ημερομηνία πληρωμής
        public bool IsSent { get; set; }
        public int CustomerId { get; set; }
        public int Period { get; set; }
        public DateTime CreatedOnUtc { get; set; }

        public string TraderName { get; set; }
        public int CustomerTypeId { get; set; }
        public int LegalFormTypeId { get; set; }
        public int CategoryBookTypeId { get; set; }
        public string CustomerTypeName { get; set; }
        public string LegalFormTypeName { get; set; }
        public string CategoryBookTypeName { get; set; }
        public string CustomerName { get; set; }
        public string PeriodName { get; set; }
        public DateTime CreatedOn { get; set; }
    }
    public partial record FinancialObligationFormModel : BaseNopModel
    {
    }
    public class EfkaTekaResult
    {
        public decimal EfkaEmployer { get; set; }
        public decimal Teka { get; set; }
        public int Periodos { get; set; }
    }
    public class PaymentIdentity
    {
        public int CompanyId { get; set; }
        public string TpteEfka { get; set; }
        public string TpteEdoeap { get; set; }
        public string TpteTeka { get; set; }
    }
}