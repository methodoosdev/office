using App.Framework.Models;

namespace App.Models.Offices
{
    public partial record ChamberSearchModel : BaseSearchModel
    {
        public ChamberSearchModel() : base("chamberName") { }
    }
    public partial record ChamberListModel : BasePagedListModel<ChamberModel>
    {
    }
    public partial record ChamberModel : BaseNopEntityModel
    {
        public string ChamberName { get; set; }
        public string ChamberNumber { get; set; }
        public string Address { get; set; }
        public string Postcode { get; set; }
        public string City { get; set; }
        public string PhoneNumber { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string WebPage { get; set; }
        public int DisplayOrder { get; set; }
    }
    public partial record ChamberFormModel : BaseNopModel
    {
    }
}