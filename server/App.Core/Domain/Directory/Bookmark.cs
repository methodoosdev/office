namespace App.Core.Domain.Directory
{
    public partial class Bookmark : BaseEntity
    {
        public string UrlPath { get; set; }
        public string Description { get; set; }
        public int CustomerId { get; set; }
        public int DisplayOrder { get; set; }
    }
}
