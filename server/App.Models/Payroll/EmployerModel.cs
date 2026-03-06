using App.Core.Domain.Common;
using App.Framework.Models;

namespace App.Models.Payroll
{
    public partial record EmployerSearchModel : BaseSearchModel
	{
		public EmployerSearchModel() : base("lastName") { }
	}
	public partial record EmployerListModel : BasePagedListModel<EmployerModel>
	{
	}
	public partial record EmployerModel : BaseNopEntityModel, IFullName // Εργοδότες
    {
        //public bool Disabled { get; set; }
        public int CompanyId { get; set; }
		public string LastName { get; set; } // Επωνυμία
		public string FirstName { get; set; } // Όνομα
		public string Vat { get; set; } // ΑΦΜ
		public string Profession { get; set; } // Επάγγελμα
		public string LegalFormTypeName { get; set; } // Νομική μορφή
		public string Doy { get; set; } // ΔΟΥ
		public string AmIka { get; set; } // ΑΜ ΙΚΑ
		public string IkaDescription { get; set; } // Υποκ/μα ΙΚΑ
		public string Address { get; set; } // Διεύθυνση
		public string Number { get; set; } // Αριθμός
		public string City { get; set; } // Πόλη
		public string Phone { get; set; } // Τηλέφωνο
		public string Email { get; set; } // Email
		public string Representative { get; set; } // Εκπρόσωπος
		public bool Active { get; set; } // Ενεργός
		public string CompanyType { get; set; } // Είδος εταιρίας
		public string Kad { get; set; } // ΚΑΔ
		public string KadDescription { get; set; } // Περιγραφή ΚΑΔ
		public string EmployerIkaUserName { get; set; }
		public string EmployerIkaPassword { get; set; }
	}
    public partial record EmployerTableModel : BaseNopModel
    {
    }
}