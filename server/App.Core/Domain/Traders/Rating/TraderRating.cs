namespace App.Core.Domain.Traders
{
    public partial class TraderRating : BaseEntity
    {
        public int DepartmentId { get; set; }
        public int TraderRatingCategoryId { get; set; }
        public string Description { get; set; }
        public int Gravity { get; set; }
        public int DisplayOrder { get; set; }
    }
}
