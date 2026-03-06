using App.Framework.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace App.Models.Stores
{
    public partial record StoreModel : BaseNopEntityModel, ILocalizedModel<StoreLocalizedModel>
    {
        public StoreModel()
        {
            Locales = new List<StoreLocalizedModel>();
            AvailableLanguages = new List<SelectListItem>();
        }

        public string Name { get; set; }
        public string Url { get; set; }
        public virtual bool SslEnabled { get; set; }
        public string Hosts { get; set; }
        public int DefaultLanguageId { get; set; }

        public IList<SelectListItem> AvailableLanguages { get; set; }
        public int DisplayOrder { get; set; }
        public string CompanyName { get; set; }

        public string CompanyAddress { get; set; }

        public string CompanyPhoneNumber { get; set; }

        public string CompanyVat { get; set; }

        public string DefaultMetaKeywords { get; set; }
        public string DefaultMetaDescription { get; set; }

        public string DefaultTitle { get; set; }

        public string HomepageTitle { get; set; }

        public string HomepageDescription { get; set; }

        public IList<StoreLocalizedModel> Locales { get; set; }
    }

    public partial record StoreLocalizedModel : ILocalizedLocaleModel
    {
        public int LanguageId { get; set; }

        public string Name { get; set; }

        public string DefaultMetaKeywords { get; set; }

        public string DefaultMetaDescription { get; set; }

        public string DefaultTitle { get; set; }

        public string HomepageTitle { get; set; }

        public string HomepageDescription { get; set; }
    }
    public partial record StoreListModel : BasePagedListModel<StoreModel>
    {
    }
}