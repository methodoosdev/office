using App.Framework.Models;

namespace App.Models.Traders
{
    public partial record SrfTraderSearchModel : BaseSearchModel
    {
        public SrfTraderSearchModel() : base("lastName") { }
    }
    public partial record SrfTraderListModel : BasePagedListModel<SrfTraderModel>
    {
    }
    public partial record SrfTraderModel : BaseNopEntityModel
    {
        public int SrfId { get; set; }
        public string LastName { get; set; } // Επώνυμο Α ή Επωνυμία
        public string FirstName { get; set; } // Όνομα
        public string Vat { get; set; } // ΑΦΜ
        public string Email { get; set; } // Email
        public int CategoryBookTypeId { get; set; }
        public string CategoryBookTypeName { get; set; }
        public int TaxSystemId { get; set; } // Id i
        public int HyperPayrollId { get; set; }// HyperM-ID i
        public string EmployerIkaUserName { get; set; } // Όνομα χρήστη(ΙΚΑ Εργοδότη)
        public string EmployerIkaPassword { get; set; } // Κωδικός πρόσβασης(ΙΚΑ Εργοδότη)
        public string TaxisUserName { get; set; } // Όνομα χρήστη(Τάξις)
        public string TaxisPassword { get; set; } // Κωδικός πρόσβασης(Τάξις)
        public string OaeeUserName { get; set; } // Όνομα χρήστη(ΟΑΕΕ)
        public string OaeePassword { get; set; } // Κωδικός πρόσβασης(ΟΑΕΕ)
        public string SepeUserName { get; set; } // Όνομα χρήστη(ΣΕΠΕ)
        public string SepePassword { get; set; } // Κωδικός πρόσβασης(ΣΕΠΕ)
        public string SpecialTaxisUserName { get; set; } // Ειδικό όνομα χρήστη(Τάξις)
        public string SpecialTaxisPassword { get; set; } // Ειδικός κωδικός πρόσβασης(Τάξις)
        public string EfkaUserName { get; set; } // Όνομα χρήστη(ΕΦΚΑ)
        public string EfkaPassword { get; set; } // Κωδικός πρόσβασης(ΕΦΚΑ)
        public int CompanyId { get; set; } // Κωδικός εταιρείας
        public string LogistikiDataBaseName { get; set; } // 'Όνομα βάσης δεδομένων
        public string LogistikiUsername { get; set; } // Όνομα χρήστη
        public string LogistikiPassword { get; set; } // Κωδικός πρόσβασης
        public string LogistikiIpAddress { get; set; } // Διεύθυνση IP
        public string LogistikiPort { get; set; } // Πόρτα
        public int LogistikiProgramTypeId { get; set; } // Πρόγραμμα λογιστικής
        public string LogistikiProgramTypeName { get; set; } // Πρόγραμμα λογιστικής
        public bool Active { get; set; }
        public int TaxesFee { get; set; }
        public int AccountingSchema { get; set; }
        public int EmployerBreakLimit { get; set; }
    }
}
