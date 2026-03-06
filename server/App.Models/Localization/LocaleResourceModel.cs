using App.Framework.Models;

namespace App.Models.Localization
{
    public partial record LocaleResourceSearchModel : BaseSearchModel
    {
        public LocaleResourceSearchModel() : base("searchResourceName")
        {
            AddResourceString = new LocaleResourceModel();
        }

        public int LanguageId { get; set; }
        public string SearchResourceName { get; set; }
        public string SearchResourceValue { get; set; }

        public LocaleResourceModel AddResourceString { get; set; }
    }
    public partial record LocaleResourceModel : BaseNopEntityModel
    {
        public string ResourceName { get; set; }
        public string ResourceValue { get; set; }
        public int LanguageId { get; set; }
    }
    public partial record LocaleResourceListModel : BasePagedListModel<LocaleResourceModel>
    {
    }
}