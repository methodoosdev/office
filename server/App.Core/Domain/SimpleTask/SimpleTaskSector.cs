namespace App.Core.Domain.SimpleTask
{
    public partial class SimpleTaskSector : BaseEntity // Τομέας
    {
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
    }
}
