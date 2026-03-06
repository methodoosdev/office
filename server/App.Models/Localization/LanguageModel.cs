using App.Framework.Models;

namespace App.Models.Localization
{
    public partial record LanguageSearchModel : BaseSearchModel
    {
        public LanguageSearchModel() : base("displayOrder") { }
    }
    public partial record LanguageListModel : BasePagedListModel<LanguageModel>
    {
    }
    public partial record LanguageModel : BaseNopEntityModel
    {
        public string Name { get; set; }
        public string LanguageCulture { get; set; }
        public string UniqueSeoCode { get; set; }
        public string FlagImageFileName { get; set; }
        public bool Rtl { get; set; }
        public int DefaultCurrencyId { get; set; }
        public bool Published { get; set; }
        public int DisplayOrder { get; set; }
    }
    public partial record LanguageFormModel : BaseNopModel
    {
    }
    public partial record LanguageTableModel : BaseNopModel
    {
    }
}