using App.Framework.Models;
using System;

namespace App.Models.Banking
{
    public partial record AvailableBankSearchModel : BaseSearchModel
    {
        public AvailableBankSearchModel() : base("bankBIC") { }
    }
    public partial record AvailableBankListModel : BasePagedListModel<AvailableBankModel>
    {
    }
    public partial record AvailableBankModel : BaseNopEntityModel
    {
        public string BankBIC { get; set; } // BIC τράπεζας
        public string Country { get; set; } // Χώρα
        public string DisplayName { get; set; } // Δικαιούχος
    }
}