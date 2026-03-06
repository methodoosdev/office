using App.Framework.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace App.Models.Directory
{
    public partial record CurrencySearchModel : BaseSearchModel
    {
        public CurrencySearchModel() : base("name") { }
    }
    public partial record CurrencyModel : BaseNopEntityModel, ILocalizedModel<CurrencyLocalizedModel>
    {
        public CurrencyModel()
        {
            Locales = new List<CurrencyLocalizedModel>();

            SelectedStoreIds = new List<int>();
            AvailableStores = new List<SelectListItem>();
        }

        public string Name { get; set; }
        public string CurrencyCode { get; set; }
        public string DisplayLocale { get; set; }
        public decimal Rate { get; set; }
        public string CustomFormatting { get; set; }
        public bool Published { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsPrimaryExchangeRateCurrency { get; set; }
        public bool IsPrimaryStoreCurrency { get; set; }

        public IList<CurrencyLocalizedModel> Locales { get; set; }

        public IList<int> SelectedStoreIds { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }

        public int RoundingTypeId { get; set; }
    }

    public partial record CurrencyListModel : BasePagedListModel<CurrencyModel>
    {
    }
    public partial record CurrencyLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }
        public string Name { get; set; }
    }
}