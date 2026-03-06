using App.Framework.Models;

namespace App.Models.Traders
{
    public partial record TraderRatingCategorySearchModel : BaseSearchModel
    {
        public TraderRatingCategorySearchModel() : base("displayOrder") { }
    }
    public partial record TraderRatingCategoryListModel : BasePagedListModel<TraderRatingCategoryModel>
    {
    }
    public partial record TraderRatingCategoryModel : BaseNopEntityModel // Κέντρα κόστους - Κατηγορίες
    {
        public string Description { get; set; } // Περιγραφή
        public int DisplayOrder { get; set; } // Κατάταξη 
    }
    public partial record TraderRatingCategoryFormModel : BaseNopModel
    {
    }
}