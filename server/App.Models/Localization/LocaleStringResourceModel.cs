using App.Framework.Models;

namespace App.Models.Logging
{
    public partial record LocaleStringResourceSearchModel : BaseSearchModel
    {
        public LocaleStringResourceSearchModel() : base("resourceName") { }
    }
    public partial record LocaleStringResourceListModel : BasePagedListModel<LocaleStringResourceModel>
    {
    }
    public partial record LocaleStringResourceModel : BaseNopEntityModel
    {
        public int LanguageId { get; set; }
        public string ResourceName { get; set; }
        public string ResourceValue { get; set; }
    }
    public partial record LocaleStringResourceFormModel : BaseNopModel
    {
    }
}