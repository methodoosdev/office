namespace App.Core.Domain.Customers
{
    public partial class CustomerPermission : BaseEntity
    {
        public string Name { get; set; }

        public string SystemName { get; set; }

        public string Category { get; set; }

        public string Menu { get; set; }

        public string Area { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }
    }
}
