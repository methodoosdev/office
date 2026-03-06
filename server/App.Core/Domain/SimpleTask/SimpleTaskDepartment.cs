namespace App.Core.Domain.SimpleTask
{
    public partial class SimpleTaskDepartment : BaseEntity // Τμήμα
    {
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
    }
}
