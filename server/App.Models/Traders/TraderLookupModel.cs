using App.Framework.Models;

namespace App.Models.Traders
{
    public partial record TraderLookupSearchModel : BaseSearchModel
    {
        public TraderLookupSearchModel() : base("fullName") { }
    }
    public partial record TraderLookupModel : BaseNopEntityModel
    {
        public string FullName { get; set; }
        public string Vat { get; set; }
        public string Email { get; set; }
        public int CustomerTypeId { get; set; }
        public string CustomerTypeName { get; set; }
        public int LegalFormTypeId { get; set; }
        public string LegalFormTypeName { get; set; }
        public int CategoryBookTypeId { get; set; }
        public string CategoryBookTypeName { get; set; }
        public int HyperPayrollId { get; set; }
        public bool ConnectionAccountingActive { get; set; }
        public bool Active { get; set; }
        public bool HasFinancialObligation { get; set; }
    }
    public partial record TraderLookupListModel : BasePagedListModel<TraderLookupModel>
    {
    }
}