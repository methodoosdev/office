using System;

namespace App.Core
{
    [Flags]
    public enum SubmitModeType // Κατάσταση δήλωσης
    {
        None = 0, // Επιλογή
        Submited = 1, // Υποβλήθηκε
        Pending = 2 // Εκκρεμεί 
    }
    [Flags]
    public enum F523Type // Καταβολή ποσού κωδ.511
    {
        None = 0, // Επιλογή
        Yes = 1, // Ναι
        No = 2, // Όχι 
    }
    [Flags]
    public enum F507Type // Αιτία επιστροφής
    {
        Exemption = 1, // Απαλ.Πράξεις
        Assets = 2, // Πάγια 
        Deposit = 3, // Αναστ.κατάβ.
        TaxRateDiff = 4, // Διαφορά συντέλ. 
        OtherCouse = 5, // Άλλη αιτία
        DebitBalance = 6, // Μείωση χρεώστ.υπόλ 
    }
    public enum DisplayActorRoleType // Επιλογή ρόλου
    {
        MySelf = 1, // Για τον εαυτό μου
        Accountant = 2, // Ως λογιστής 
        AccountantOffice = 3, // Ως λογιστής λογιστικού γραφείου
        Representative = 4, // Ως εκπρόσωπος νομικού προσώπου 
    }
}
