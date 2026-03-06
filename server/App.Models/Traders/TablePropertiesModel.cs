namespace App.Models.Traders
{
    public class TablePropertiesModel
    {
        public int Id { get; set; }
        public string FieldName { get; set; }
        public string FieldLabel { get; set; }
        public string FieldValue { get; set; }
        public string RegistryValue { get; set; }
        public string RegistryType { get; set; }
        public bool IsEquals { get; set; }
    }
}
