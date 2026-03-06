using App.Framework.Models;
using System;

namespace App.Models.Payroll
{
    public partial record PersonTermExpiryListModel : BaseNopModel
    {
	}

	public partial record PersonTermExpiryModel
    {
        public int CompanyId { get; set; }
        public string Vat { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; } // Επωνυμία
        public string BranchCode { get; set; } // Κωδικός υποκ/τος
        public string BranchName { get; set; } // Υποκατάστημα
        public string PersonName { get; set; } // Εκπρόσωπος
        public string PersonType { get; set; } // Είδος εκπ/που
        public DateTime ExpirationDate { get; set; } // Ημ.Λήξης 
        public int DateStatusId { get; set; } // Έλεγχος σύμβασης
        public string DateStatus { get; set; } // Έλεγχος σύμβασης
        public string DayStatus { get; set; } // Έλεγχος σύμβασης σε ημέρες
    }
    public partial record PersonTermExpiryTableModel : BaseNopModel
    {
    }
}