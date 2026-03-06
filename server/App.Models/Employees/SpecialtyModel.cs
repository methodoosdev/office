using App.Framework.Models;

namespace App.Models.Employees
{
    public partial record SpecialtySearchModel : BaseSearchModel
    {
        public SpecialtySearchModel() : base("displayOrder") { }
    }
    public partial record SpecialtyListModel : BasePagedListModel<SpecialtyModel>
    {
    }
    public partial record SpecialtyModel : BaseNopEntityModel
    {
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
    }
    public partial record SpecialtyFormModel : BaseNopModel
    {
    }
}