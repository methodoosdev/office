namespace App.Core.Domain.Common
{
    public partial class PersistState : BaseEntity
    {
        public string ModelType { get; set; }
        public string JsonValue { get; set; }
        public int CustomerId { get; set; }
    }
}
