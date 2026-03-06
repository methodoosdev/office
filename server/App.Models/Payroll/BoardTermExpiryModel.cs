using App.Framework.Models;
using System;

namespace App.Models.Payroll
{
    public partial record BoardTermExpiryListModel : BaseNopModel
    {
	}

	public partial record BoardTermExpiryModel
    {
        public string Email { get; set; }
        public string CompanyName { get; set; } // Επωνυμία
        public DateTime ExpirationDate { get; set; } // Ημ.Λήξης ΔΣ
        public int DateStatusId { get; set; } // Έλεγχος λήξης ΔΣ
        public string DateStatus { get; set; } // Έλεγχος λήξης ΔΣ
        public string DayStatus { get; set; } // Έλεγχος λήξης ΔΣ σε ημέρες
    }
    public partial record BoardTermExpiryTableModel : BaseNopModel
    {
    }
}