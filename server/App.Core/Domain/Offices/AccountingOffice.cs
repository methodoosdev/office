namespace App.Core.Domain.Offices
{
    public partial class AccountingOffice : BaseEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string ZipPostalCode { get; set; }
        public string PhoneNumber { get; set; }
        public string Doy { get; set; }
        public string Vat { get; set; }
        public string Am { get; set; }
        public string LicenseCategory { get; set; }
        public string Adt { get; set; }
        public string Comment { get; set; }
        public string TaxisNetUserName { get; set; }
        public string TaxisNetPassword { get; set; }
        public int LegalStatusTypeId { get; set; }

        public LegalStatusType LegalStatusType
        {
            get => (LegalStatusType)LegalStatusTypeId;
            set => LegalStatusTypeId = (int)value;
        }

        //AadeRegistry
        public string AadeRegistryUsername { get; set; }
        public string AadeRegistryPassword { get; set; }

        //Office
        public string OfficeDataBaseName { get; set; }
        public string OfficeUsername { get; set; }
        public string OfficePassword { get; set; }
        public string OfficeIpAddress { get; set; }
        public string OfficePort { get; set; }

        //Srf
        public string SrfDataBaseName { get; set; }
        public string SrfUsername { get; set; }
        public string SrfPassword { get; set; }
        public string SrfIpAddress { get; set; }
        public string SrfPort { get; set; }

        //TaxSystem
        public string TaxSystemDataBaseName { get; set; }
        public string TaxSystemUsername { get; set; }
        public string TaxSystemPassword { get; set; }
        public string TaxSystemIpAddress { get; set; }
        public string TaxSystemPort { get; set; }

        //HyperPayroll
        public string HyperPayrollDataBaseName { get; set; }
        public string HyperPayrollUsername { get; set; }
        public string HyperPayrollPassword { get; set; }
        public string HyperPayrollIpAddress { get; set; }
        public string HyperPayrollPort { get; set; }

        //HyperLog
        public string HyperLogDataBaseName { get; set; }
        public string HyperLogUsername { get; set; }
        public string HyperLogPassword { get; set; }
        public string HyperLogIpAddress { get; set; }
        public string HyperLogPort { get; set; }

        //Prosvasis
        public string ProsvasisDataBaseName { get; set; }
        public string ProsvasisUsername { get; set; }
        public string ProsvasisPassword { get; set; }
        public string ProsvasisIpAddress { get; set; }
        public string ProsvasisPort { get; set; }
    }
}
