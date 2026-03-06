using App.Core.Domain.Common;

namespace App.Core.Domain.Employees
{
    public partial class Employee : BaseEntity, ISoftDeletedEntity, IFullName
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FatherName { get; set; }
        public string EmailContact { get; set; }
        public string Mobile { get; set; }
        public string InternalPhoneNumber { get; set; }
        public int PictureId { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }

        public int? EducationId { get; set; }
        public int? DepartmentId { get; set; }
        public int? SpecialtyId { get; set; }
        public int? JobTitleId { get; set; }
        public int? SupervisorId { get; set; }

        public bool PayrollInfoEmail { get; set; }
        public int EmployeeSalary { get; set; }
    }
}
