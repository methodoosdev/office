using App.Framework.Models;
using System;

namespace App.Models.Banking
{
    public partial record UserConnectionBankConfigModel : BaseNopModel
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
    public partial record UserConnectionBankSearchModel : BaseSearchModel
    {
        public UserConnectionBankSearchModel() : base("bankBIC") { }
    }
    public partial record UserConnectionBankListModel : BasePagedListModel<UserConnectionBankModel>
    {
    }
    public partial record UserConnectionBankModel : BaseNopEntityModel
    {
        public string ConnectionId { get; set; } // Σύνδεση
        public string Country { get; set; } // Χώρα
        public string BankBIC { get; set; }  // BIC τράπεζας
        public string DisplayName { get; set; } // Τράπεζα
        public string Status { get; set; } // Status
        public DateTime ValidUntil { get; set; } // Ισχύει έως
    }
}