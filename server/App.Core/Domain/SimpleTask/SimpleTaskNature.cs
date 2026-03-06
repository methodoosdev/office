namespace App.Core.Domain.SimpleTask
{
    public partial class SimpleTaskNature : BaseEntity // Φύση αντικειμένου
    {
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
    }
}
