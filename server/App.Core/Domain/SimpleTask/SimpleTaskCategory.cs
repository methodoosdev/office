namespace App.Core.Domain.SimpleTask
{
    public partial class SimpleTaskCategory : BaseEntity // Κατηγορία                                                     
    {
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
    }
}
