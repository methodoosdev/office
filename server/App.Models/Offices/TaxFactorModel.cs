using App.Framework.Models;

namespace App.Models.Offices
{
    public partial record TaxFactorSearchModel : BaseSearchModel
    {
        public TaxFactorSearchModel() : base("year") { }
    }
    public partial record TaxFactorListModel : BasePagedListModel<TaxFactorModel>
    {
    }
    public partial record TaxFactorModel : BaseNopEntityModel
    {
        public int Year { get; set; }
        public int TaxIncome { get; set; }
        public int TaxAdvance { get; set; }
    }
    public partial record TaxFactorFormModel : BaseNopModel
    {

    }
}
