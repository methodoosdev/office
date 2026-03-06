using App.Framework.Models;

namespace App.Models.Directory
{
    public partial record BookmarkSearchModel : BaseSearchModel
    {
        public BookmarkSearchModel() : base("displayOrder") { }
    }
    public partial record BookmarkListModel : BasePagedListModel<BookmarkModel>
    {
    }
    public partial record BookmarkModel : BaseNopEntityModel
    {
        public string UrlPath { get; set; }
        public string Description { get; set; }
        public int CustomerId { get; set; }
        public int DisplayOrder { get; set; }
    }
    public partial record BookmarkFormModel : BaseNopModel
    {
    }
}