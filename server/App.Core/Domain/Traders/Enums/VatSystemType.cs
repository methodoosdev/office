namespace App.Core.Domain.Traders
{
    public enum VatSystemType //Καθεστώς ΦΠΑ
    {
        None = 0, // Κενό
        Normal = 1, // Κανονικό
        Farmers = 2, // Αγροτών
        Exemption = 3, // Απαλλαγή
        LumpSum = 4 // Κατ' αποκοπή
    }
}
