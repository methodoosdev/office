using App.Framework.Models;
using System;

namespace App.Models.Payroll
{
    public partial record ApdTekaDialogModel : BaseNopModel
    {
        public int CompanyId { get; set; } // Εργοδότης
        public DateTime Period { get; set; } // Περίοδος
    }
    public partial record ApdTekaDialogFormModel : BaseNopModel
    {
    }
    public partial record ApdTekaFilterModel : BaseNopModel
    {
        public int? CompanyId { get; set; } // Εργοδότης
        public int? EmployeeId { get; set; } // Υπεύθυνος
        public DateTime? Period { get; set; } // Περίοδος
    }
    public partial record ApdTekaFilterFormModel : BaseNopModel
    {
    }
    public partial record ApdTekaSearchModel : BaseSearchModel
	{
		public ApdTekaSearchModel() : base("companyName") { }
	}
	public partial record ApdTekaListModel : BasePagedListModel<ApdTekaModel>
	{
	}
	public partial record ApdTekaModel : BaseNopEntityModel // Έλεγχος ΑΠΔ-ΤΕΚΑ
    {
        public int TraderId { get; set; } // Συναλλασσόμενος
        public int CompanyId { get; set; } // Επωνυμία
        public int? EmployeeId { get; set; } // Υπεύθυνος
        public bool CheckWorkers { get; set; } // Έλεγχος Εργ.με Μισθ.κατάσταση
        public decimal Contributions { get; set; } // Εισφορές
        public decimal Subsidiary { get; set; } // Επικουρικό
        public decimal Apd { get; set; } // ΑΠΔ
        public decimal Teka { get; set; } // ΤΕΚΑ
        public decimal ApdSubmit { get; set; } // ΑΠΔ Υποβολής
        public decimal TekaSubmit { get; set; } // ΤΕΚΑ Υποβολής
        public DateTime? ApdSubmitDateOnUtc { get; set; } // Ημερ.Υποβολής ΑΠΔ
        public DateTime? TekaSubmitDateOnUtc { get; set; } // Ημερ.Υποβολής ΤΕΚΑ
        public DateTime? InfoDateOnUtc { get; set; } // Ημερ.Ενημέρωσης
        public string Notes { get; set; } // Παρατηρήσεις
        public int Period { get; set; } // Περίοδος
        public int Year { get; set; } // Έτος

        public decimal DxApd { get; set; } // ΔΧ.ΑΠΔ
        public decimal DxTeka { get; set; } // ΔΧ.ΤΕΚΑ
        public decimal DpApd { get; set; } // ΔΠ.ΑΠΔ
        public decimal DpTeka { get; set; } // ΔΠ.ΤΕΚΑ
        public string WorkersError { get; set; } // Σφάλμα εργαζομένων
        public string ApdError { get; set; } // Σφάλμα ΑΠΔ
        public string TekaError { get; set; } // Σφάλμα ΤΕΚΑ

        public string CompanyName { get; set; } // Επωνυμία
        public string EmployeeName { get; set; } // Υπεύθυνος
        public DateTime? ApdSubmitDateOn { get; set; } // Ημερ.Υποβολής ΑΠΔ
        public DateTime? TekaSubmitDateOn { get; set; } // Ημερ.Υποβολής ΤΕΚΑ
        public DateTime? InfoDateOn { get; set; } // Ημερ.Ενημέρωσης
        public string PeriodName { get; set; } // Περίοδος
        public string ErrorMessage { get; set; } // Σφάλματα
    }
    public partial record ApdTekaFormModel : BaseNopModel
    {
    }
    public class ApdTekaEmployees
    {
        public int EmployeeId { get; set; }
        public int Period { get; set; }
        public int Year { get; set; }
        public string EmployeeName { get; set; }
    }
    public class ApdTekaPayrollStatus
    {
        public int PeriodId { get; set; }
        public decimal Contributions { get; set; }
        public decimal Subsidiary { get; set; }
    }
    public class ApdTekaEfka
    {
        public decimal TotalContribution { get; set; }
        public int Mode { get; set; }
    }
}