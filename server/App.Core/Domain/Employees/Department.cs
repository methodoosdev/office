namespace App.Core.Domain.Employees
{
    public partial class Department : BaseEntity
    {
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public string Background { get; set; }
        public string Color { get; set; }
        public string SystemName { get; set; }
    }
}
