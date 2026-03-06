using App.Framework.Models;

namespace App.Models.Employees
{
    public partial record JobTitleSearchModel : BaseSearchModel
    {
        public JobTitleSearchModel() : base("displayOrder") { }
    }
    public partial record JobTitleListModel : BasePagedListModel<JobTitleModel>
    {
    }
    public partial record JobTitleModel : BaseNopEntityModel
    {
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
    }
    public partial record JobTitleFormModel : BaseNopModel
    {
    }
}