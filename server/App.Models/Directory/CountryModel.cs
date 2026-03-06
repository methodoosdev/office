using App.Framework.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace App.Models.Directory
{
    public partial record CountrySearchModel : BaseSearchModel
    {
        public CountrySearchModel() : base("name") { }
    }
    public partial record CountryModel : BaseNopEntityModel, ILocalizedModel<CountryLocalizedModel>
    {
        public CountryModel()
        {
            Locales = new List<CountryLocalizedModel>();
            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();
        }

        public string Name { get; set; }
        public bool AllowsBilling { get; set; }
        public bool AllowsShipping { get; set; }
        public string TwoLetterIsoCode { get; set; }
        public string ThreeLetterIsoCode { get; set; }
        public int NumericIsoCode { get; set; }
        public bool SubjectToVat { get; set; }
        public bool Published { get; set; }
        public int DisplayOrder { get; set; }

        public IList<CountryLocalizedModel> Locales { get; set; }

        public IList<int> SelectedStoreIds { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
    }

    public partial record CountryListModel : BasePagedListModel<CountryModel>
    {
    }
    public partial record CountryLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }
        public string Name { get; set; }
    }
}