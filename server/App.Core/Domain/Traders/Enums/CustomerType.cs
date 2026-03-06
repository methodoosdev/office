namespace App.Core.Domain.Traders
{
    public enum CustomerType //Είδος
    {
        NaturalPerson = 0, //Φυσικό Πρόσωπο
        PersonalCompany = 1,//Προσωπική Εταιρεία
        LegalPerson = 2,//Νομικό Πρόσωπο
        Consortium = 3,//Κοινοπραξία
        Society = 4,//Κοινωνία
        Other = 5,//Άλλο
        IndividualCompany = 6//Ατομική Επιχείρηση
    }
}
