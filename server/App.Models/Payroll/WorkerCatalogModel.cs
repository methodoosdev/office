using App.Framework.Models;
using System;

namespace App.Models.Payroll
{
    public partial record WorkerCatalogSearchModel : BaseSearchModel
	{
		public WorkerCatalogSearchModel() : base("surname") { }
	}
	public partial record WorkerCatalogListModel : BasePagedListModel<WorkerCatalogModel>
	{
	}
	public partial record WorkerCatalogModel : BaseNopEntityModel // Λίστα εργαζομένων
    {
        public int EmployeeId { get; set; } // 
        public int ActiveCard { get; set; } // 
        public int CompanyId { get; set; } // 
        public bool CompanyIsActive { get; set; } // 
        public DateTime? CompanyCalcDate { get; set; } // 
        public bool IsInActive { get; set; } // 
		public DateTime? BirthDate { get; set; } // Ημ.Γέννησης
		public string Amka { get; set; } // ΑΜΚΑ
		public string AmIka { get; set; } // ΑΜ ΙΚΑ
        public decimal Salary { get; set; } // Νόμιμος μισθός
        public decimal SalaryAgreed { get; set; } // Συμφ. μισθός/ημερ./ωρομ.
        public DateTime? OaedEpidomaDate { get; set; } // Ημ/νία Ένταξης Πρόγραμμα ΟΑΕΔ
        public string CompanyName { get; set; } // Εταιρία
        public decimal EpidotisiErgodPercent { get; set; } // Ποσοστό Εργοδοτικής Επιδότησης 6,66%
        public int Birthday { get; set; } // Ηλικία
        public string OaedEpidoma { get; set; } // Πρόγραμμα ΟΑΕΔ
        public string CndtDescr { get; set; } // Σύμβαση
        public string SepeOaedDesc { get; set; } // Ειδικότητα ΣΕΠΕ-ΟΑΕΔ
        public string SepeOaedCode { get; set; } // Κωδικός Ειδικότητας ΣΕΠΕ-ΟΑΕΔ
        public string AmKoinAsf { get; set; } // ΑΜ. Κοιν. Ασφάλισης
        public string OaedEpidotisi { get; set; } // Επιδότηση ΟΑΕΔ
        public int EidikotitaId { get; set; } // Κωδικός Ειδικότητας
        public int PackageId { get; set; } // Πακέτο Κάλυψης
        public string EidikotitaDesc { get; set; } // Περιγραφή Κωδικού Ειδικότητας ΙΚΑ
        public string UpokCode { get; set; } // Κωδικός Υποκαταστήματος
        public string EmployeeType { get; set; } // Τύπος Εργαζομένου
        public string OresErgasias { get; set; } // Ωράριο Εργασίας
        public string EmployeeSpc { get; set; } // Ειδικότητα
        public string EmployeeKind { get; set; } // Είδος
        public DateTime? Orismenou { get; set; } // Ημ/νία Έναρξης Ορισμένου
        public DateTime? OrismenouExpire { get; set; } // Ημ/νία Λήξης Ορισμένου
        public string MerikiApasx { get; set; } // Μερική απασχόληση
        public string EmployeeSpcDesc { get; set; } // Περιγραφή Ειδικότητας
        public string PliresOrario { get; set; } // Πλήρες Ωράριο
        public decimal DayHours { get; set; } // Ώρες Ανά Ημέρα
        public string Vat { get; set; } // Α.Φ.Μ.
        public string Surname { get; set; } // Επώνυμο
        public string EmployeeCode { get; set; } // Κωδικός
        public string EmployeeName { get; set; } // Όνομα
        public string FrReason { get; set; } // Αποχώρηση
        public DateTime? HrDate { get; set; } // Ημ/νία Πρόσληψης
        public DateTime? FrDate { get; set; } // Ημ/νία Αποχώρησης
    }
}