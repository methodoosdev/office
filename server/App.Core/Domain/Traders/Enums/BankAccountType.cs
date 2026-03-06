namespace App.Core.Domain.Traders
{
    public enum BankAccountType // Φύση λογαρισμού
    {
        Cash = 1,// Ταμείο
        CreditCard = 2,// Πιστωτική κάρτα
        CheckingAccount = 3,// Λογαριασμός όψεως
        TimeDepositAccount = 4,// Λογαριασμός καταθέσεων προθεσμίας
        MediumTermLoansAccount = 5,// Λογαριασμός μεσοπροθέσμων δανείων
        LongTermLoansAccount = 6,// Λογαριασμός μακροπρόθεσμων δανείων
        ShortTermLoansAccount = 7,// Λογαριασμός βραχυπρόθεσμων δανείων
        CheckGuaranteeAccount = 8,// Λογαριασμός εγγυήσεων επιταγών
        Indifferent = 9,// Αδιάφορο
        SecuritiesReceivable = 10,// Αξιόγραφα εισπρακτέα
        NotesPayable = 11 // Αξιόγραφα πληρωτέα
    }
}