using App.Framework.Models;

namespace App.Models.Employees
{
    public partial record EmployeeSearchModel : BaseSearchModel
    {
        public EmployeeSearchModel() : base("fullName") { }
        public string FullName { get; set; }
    }
    public partial record EmployeeListModel : BasePagedListModel<EmployeeModel>
    {
    }
    public partial record EmployeeModel : BaseNopEntityModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FatherName { get; set; }
        public string EmailContact { get; set; }
        public string Mobile { get; set; }
        public string InternalPhoneNumber { get; set; }
        public bool PayrollInfoEmail { get; set; }
        public int PictureId { get; set; }
        public bool Active { get; set; }

        public int? EducationId { get; set; }
        public int? DepartmentId { get; set; }
        public int? SpecialtyId { get; set; }
        public int? JobTitleId { get; set; }
        public int? SupervisorId { get; set; }

        public string FullName { get; set; }
        public string EducationName { get; set; }
        public string DepartmentName { get; set; }
        public string SpecialtyName { get; set; }
        public string JobTitleName { get; set; }
        public string SupervisorName { get; set; }
        public int EmployeeSalary { get; set; }
    }
    public partial record EmployeeFormModel : BaseNopModel
    {
    }
}