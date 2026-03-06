using App.Framework.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace App.Models.Settings
{
    public partial record SettingModel : BaseNopEntityModel
    {
        public SettingModel()
        {
            AvailableStores = new List<SelectListItem>();
        }

        public string Name { get; set; }
        public string Value { get; set; }
        public string Store { get; set; }
        public int StoreId { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
    }
    public partial record SettingListModel : BasePagedListModel<SettingModel>
    {
    }
}