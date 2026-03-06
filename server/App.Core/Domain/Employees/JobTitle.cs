namespace App.Core.Domain.Employees
{
    public partial class JobTitle : BaseEntity
    {
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
    }
}
