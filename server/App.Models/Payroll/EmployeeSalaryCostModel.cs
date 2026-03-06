using App.Framework.Models;
using System;
using System.Collections.Generic;

namespace App.Models.Payroll
{
    public partial record EmployeeSalaryCostSearchModel : BaseNopModel
    {
        public int? CompanyId { get; set; } // Εργοδότης

        public decimal NetAmountPayable { get; set; } // Καθαρό πληρωτέο ποσό
        public int? InsurancePackageId { get; set; } // Πακέτα κάλυψης
        public int NumberOfChildren { get; set; } // Αριθμός τέκνων
        public decimal Undeclared { get; set; } // Αδήλωτο
        //public int InsurancePackageAllId { get; set; } // Πακέτα κάλυψης
        public bool AllPackages { get; set; } // Όλα τα πακέτα
    }
    public partial record EmployeeSalaryCostSearchFormModel : BaseNopModel
    {
    }
    public partial record EmployeeSalaryCostModel : BaseNopModel
    {
        public decimal NetAmountPayable { get; set; } // Καθαρό πληρωτέο ποσό
        public decimal EmployeeContributionsRate { get; set; } // Εισφορές εργαζόμενου
        public decimal EmployerContributionsRate { get; set; } // Εισφορές εργοδότη
        public decimal TotalContributionsRate { get; set; } // Σύνολο κρατήσεων

        public decimal AnualIncome { get; set; } // Ετήσιο εισόδημα
        public decimal TaxIncome { get; set; } // Φόρος εισοδήματος
        public decimal TaxDeduction { get; set; } // Έκπτωση φόρου
        public decimal TaxDeductionMinus { get; set; } // Μείωση έκπτωσης φόρου
        public decimal FinalTax { get; set; } // Τελικός φοροσ εισοδήματος

        public decimal MixedEarnings { get; set; }  // Μικτές αποδοχές
        public decimal EmployeeContributions { get; set; } // Εισφορές εργαζόμενου
        public decimal EmployerContributions { get; set; } // Εισφορές εργοδότη
        public decimal TotalContributions { get; set; } // Σύνολο κρατήσεων
        public decimal NetTaxable { get; set; } // Καθαρο φορολογητέο
        public decimal Fmy { get; set; } // Φ.Μ.Υ.
        public decimal CompanyCost { get; set; } // Κόστος επιχείρησης
        public decimal UndeclaredCost { get; set; } // Κόστος αδήλωτο
        public decimal UndeclaredBenefit { get; set; } // Όφελος ως αδήλωτο όταν αυτό δηλώνεται
        public decimal TaxDifference { get; set; } // μείον διαφορά φόρου
        public decimal ChargeUndeclared { get; set; } // Επιβάρυνση  σε € επι του αδήλωτου
        public decimal ChargeUndeclaredPayee { get; set; } // Επιβάρυνση % επι του αδήλωτου καταβαλλόμενου
        public decimal FinalPayable { get; set; } // Τελικό πληρωτέο
    } 

    public partial record EmployeeSalaryCostFormModel : BaseNopModel
    {       
    }
    public class InsurancePackageModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public decimal Employee { get; set; }
        public decimal Employer { get; set; }
    }
}
