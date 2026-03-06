using App.Framework.Models;

namespace App.Models.Offices
{
    public partial record AccountingOfficeModel : BaseNopEntityModel
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
    public partial record AccountingOfficeFormModel : BaseNopModel
    {
    }
    /*
     {
  "Id": "1",
  "FirstName": "ΔΗΜΗΤΡΙΟΣ",
  "LastName": "ΣΕΡΕΦΙΔΗΣ",
  "Address": "",
  "City": "",
  "ZipPostalCode": "",
  "PhoneNumber": "",
  "Doy": "",
  "Vat": "801408066",
  "Am": "",
  "LicenseCategory": "",
  "Adt": "",
  "Comment": "",
  "TaxisNetUserName": "WW33630U920",
  "TaxisNetPassword": "S!@dim0207",
  "LegalStatusTypeId": "0",
  "AadeRegistryUsername": "0038066412",
  "AadeRegistryPassword": "0038066412",
  "OfficeDataBaseName": "OfficeDB",
  "OfficeUsername": "srf_app",
  "OfficePassword": "NQcuBMrDy7aW5hTR",
  "OfficeIpAddress": "192.168.101.12",
  "OfficePort": "48000",
  "SrfDataBaseName": "Finacial_DB",
  "SrfUsername": "srf_app",
  "SrfPassword": "NQcuBMrDy7aW5hTR",
  "SrfIpAddress": "192.168.101.12",
  "SrfPort": "48000",
  "TaxSystemDataBaseName": "TaxSystem",
  "TaxSystemUsername": "srf",
  "TaxSystemPassword": "ZE8VrZStU6meaPfD",
  "TaxSystemIpAddress": "192.168.101.12",
  "TaxSystemPort": "48001",
  "HyperPayrollDataBaseName": "HyperM",
  "HyperPayrollUsername": "srf",
  "HyperPayrollPassword": "ZE8VrZStU6meaPfD",
  "HyperPayrollIpAddress": "192.168.101.12",
  "HyperPayrollPort": "48001",
  "HyperLogDataBaseName": "hlog",
  "HyperLogUsername": "srf",
  "HyperLogPassword": "ZE8VrZStU6meaPfD",
  "HyperLogIpAddress": "192.168.101.12",
  "HyperLogPort": "48001",
  "ProsvasisDataBaseName": "PBSONES1",
  "ProsvasisUsername": "pbs",
  "ProsvasisPassword": "b7db8R8Q8n8UHffD",
  "ProsvasisIpAddress": "192.168.101.12",
  "ProsvasisPort": "48003"
}
     */
}