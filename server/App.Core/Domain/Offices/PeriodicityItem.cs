namespace App.Core.Domain.Offices
{
    public partial class PeriodicityItem : BaseEntity
    {
        public string Paragraph { get; set; }
        public string Notes { get; set; }

        public int PeriodicityItemTypeId { get; set; }
        public PeriodicityItemType PeriodicityItemType
        {
            get => (PeriodicityItemType)PeriodicityItemTypeId;
            set => PeriodicityItemTypeId = (int)value;
        }

    }
}
