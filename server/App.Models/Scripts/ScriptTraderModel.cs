namespace App.Models.Scripts
{
    public class ScriptTraderModel
    {
        public int Id { get; set; }
        public int TraderId { get; set; }
        public int CategoryBookTypeId { get; set; }
        public string TraderName { get; set; }
        public int Year { get; set; }
        public int Period { get; set; }
        public int ShowTypeId { get; set; }
        public bool Inventory { get; set; }
    }

    public class ScriptCustomerInfo
    {
        public string TraderName { get; set; }
        public string Address { get; set; }
        public string Postcode { get; set; }
        public string City { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}