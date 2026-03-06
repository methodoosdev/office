using System;

namespace App.Core.Domain.Messages
{
    [Flags]
    public enum EmailMessageType // Τύπος email
    {
        FinancialObligation = 1, // Οικονομική υποχρέωση
        Newsletter = 2, // Ενημερωτικό
        Announcement = 4, // Ανακοινώσεων
        Confirmation = 8, // Επιβεβαίωσης
        Survey = 16, // Έρευνας
        Transactional = 32, // Συναλλαγής
        Welcome = 64,
        Offer = 128,
        Review = 256,
        NewProduct = 512,
        Other = 1024 // Άλλο
    }
}
