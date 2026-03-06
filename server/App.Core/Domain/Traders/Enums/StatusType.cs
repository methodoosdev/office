using System;

namespace App.Core.Domain.Traders
{
    public enum StatusType //Κατάσταση
    {
        Pending = 0, // Εκκρεμεί
        Done = 1, // Ολοκληρώθηκε
        Submitted = 2, // Υποβλήθηκε
        Other = 3, // Άλλο
        GotPaid = 4, // Πληρώθηκε
        None = 5, // Κενό
        ExportedToXML = 6 // Εξήχθει σε XML
    }
}
