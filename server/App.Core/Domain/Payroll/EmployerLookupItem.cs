using App.Core.Domain.Common;

namespace App.Core.Domain.Payroll
{
    public class EmployerLookupItem : IFullName
    {
        public virtual int CompanyId { get; set; }
        public string AmIka { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Vat { get; set; }
    }
}