namespace App.Core.Domain.Employees
{
    public partial class Specialty : BaseEntity
    {
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
    }
}
