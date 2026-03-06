using App.Framework.Models;

namespace App.Models.Traders
{
    public partial record TraderRatingSearchModel : BaseSearchModel
    {
        public TraderRatingSearchModel() : base("displayOrder") { }
    }
    public partial record TraderRatingListModel : BasePagedListModel<TraderRatingModel>
    {
    }
    public partial record TraderRatingModel : BaseNopEntityModel // Κέντρα κόστους - Βάρος
    {
        public int DepartmentId { get; set; } // Τμήμα
        public int TraderRatingCategoryId { get; set; } // Κατηγορία
        public string Description { get; set; } // Περιγραφή
        public int Gravity { get; set; } // Βάρος
        public int DisplayOrder { get; set; } // Κατάταξη

        public string CategoryName { get; set; } // Κατηγορία
        public string DepartmentName { get; set; } // Τμήμα

    }
    public partial record TraderRatingFormModel : BaseNopModel
    {
    }

    public partial class TraderRatingByDepartmentModel
    {
        public int Id { get; set; }
        public string Trader { get; set; } // Συναλλασσόμενος
        public string Department { get; set; } // Τμήμα
        public string Category { get; set; } // Κατηγορία
        public string Gravity { get; set; } // Βαρύτητα
        public string Employee { get; set; } // Υπεύθυνος
        public int Value { get; set; } // Βάρος
    }

    public partial class TraderRatingByEmployeeModel
    {
        public int DepartmentId { get; set; } // Τμήμα
        public int CategoryBookTypeId { get; set; } // Κατηγορία βιβλίων
        public string Employee { get; set; } // Υπεύθυνος
        public int TraderCount { get; set; } // Πλήθος Συναλ/νων
        public int Value { get; set; } // Βαρύτητα
    }

    public partial class TraderRatingByTraderModel
    {
        public string Employee { get; set; } // Υπεύθυνος
        public string Trader { get; set; } // Συναλλασσόμενος
        public int Value { get; set; } // Βαρύτητα
    }

    public partial class SummaryTableModel
    {
        public string Vat { get; set; } // ΑΦΜ
        public string Trader { get; set; } // Συναλλασσόμενος
        public string CategoryBook { get; set; } // Κατηγορία βιβλίων
        public int CategoryBookTypeId { get; set; } // Κατηγορία βιβλίων
        public string Employee_Dep2 { get; set; } // Υπεύθυνος (Λογ.)
        public int Gravity_Dep2 { get; set; } // Βαρύτητα πελάτη (Λογ.)
        public int TotalGravity_Dep2 { get; set; } // Σύνολο βαρύτητας (Λογ.)
        public string Employee_Dep3 { get; set; } // Υπεύθυνος (Μισθ.)
        public int Gravity_Dep3 { get; set; } // Βαρύτητα πελάτη (Μισθ.)
        public int TotalGravity_Dep3 { get; set; } // Σύνολο βαρύτητας (Μισθ.)
    }
    public partial class ValuationTableModel
    {
        public int Id { get; set; }
        public string Vat { get; set; } // ΑΦΜ
        public string Trader { get; set; } // Συναλλασσόμενος
        public int TraderPayment { get; set; } // Έσοδο
        public string SpecialtyName { get; set; } // Κατηγορία βιβλίων
        public string Employee_Dep2 { get; set; } // Υπεύθυνος (Λογ.)
        public int EmployeeSalary_Dep2 { get; set; } // Έξοδο (Λογ.)
        public int Gravity_Dep2 { get; set; } // Βαρύτητα πελάτη (Λογ.)
        public int TotalGravity_Dep2 { get; set; } // Σύνολο βαρύτητας (Λογ.)
        public string Employee_Dep3 { get; set; } // Υπεύθυνος (Μισθ.)
        public int EmployeeSalary_Dep3 { get; set; } // Έξοδο (Μισθ.)
        public int Gravity_Dep3 { get; set; } // Βαρύτητα πελάτη (Μισθ.)
        public int TotalGravity_Dep3 { get; set; } // Σύνολο βαρύτητας (Μισθ.)
        public string Employee_Dep7 { get; set; } // Υπεύθυνος (Μηχ.)
        public int EmployeeSalary_Dep7 { get; set; } // Έξοδο (Μηχ.)
        public int Gravity_Dep7 { get; set; } // Βαρύτητα πελάτη (Μηχ.)
        public int TotalGravity_Dep7 { get; set; } // Σύνολο βαρύτητας (Μηχ.)
        public string Employee_Dep8 { get; set; } // Υπεύθυνος (Δοικ.)
        public int EmployeeSalary_Dep8 { get; set; } // Έξοδο (Δοικ.)
        public int Gravity_Dep8 { get; set; } // Βαρύτητα πελάτη (Δοικ.)
        public int TotalGravity_Dep8 { get; set; } // Σύνολο βαρύτητας (Δοικ.)
    }

    public partial class ValuationTableResult
    {
        public decimal Turnover { get; set; } // Τζίρος Γ'Κατ.
        public decimal TurnoverRate { get; set; } // Ποσοστό Γ'Κατ.
        public int TraderPayment { get; set; }
        public string Employee { get; set; } // Υπεύθυνος
        public string SpecialtyName { get; set; } // Κατ.Βιβλίων
        public decimal RateDep2 { get; set; } // Κόστος λογ.
        public decimal SalaryDep2 { get; set; } // Ποσοστό λογ.
        public decimal RateDep3 { get; set; } // Κόστος Μισθ.
        public decimal SalaryDep3 { get; set; } // Ποσοστό Μισθ.
        public decimal RateDep7 { get; set; } // Κόστος Πληρ.
        public decimal SalaryDep7 { get; set; } // Ποσοστό Πληρ.
        public decimal RateDep8 { get; set; } // Κόστος Διοικ..
        public decimal SalaryDep8 { get; set; } // Ποσοστό Διοικ.
        public decimal Total { get; set; } // Μεικτό κέρδος
        public decimal TotalRate { get; set; } // Ποσοστό ΜΚ
    }

    public partial class ValuationTraderResult
    {
        public string Trader { get; set; }
        public int Income { get; set; }
        public decimal Expences { get; set; }
        public decimal Total { get; set; } // Μεικτό κέρδος
        public decimal TotalRate { get; set; } // Ποσοστό ΜΚ
    }


}