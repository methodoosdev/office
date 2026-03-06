using System;

namespace App.Core.Domain.Traders
{
    public partial class TraderMembership : BaseEntity // Στοιχεία μελών νομικού προσώπου
    {
        public int ParentId { get; set; }
        public int TraderId { get; set; }
        public string Vat { get; set; } // ΑΦΜ
        public string SurnameFatherName { get; set; } // Επώνυμο Όνομα Πατρώνυμο
        public DateTime StartDateOnUtc { get; set; } // Ημ/νία έναρξης μέλους
        public DateTime? ExpireDateOnUtc { get; set; } // Ημ/νία διακοπής μέλους
        public string ParticipationName { get; set; } // Είδος συμμετοχής
        public int TraderBoardMemberTypeId { get; set; } // Είδος συμμετοχής
        public decimal ParticipationRate { get; set; } // Ποσοστό συμμετοχής
        public string ParticipatingFraction { get; set; } // Συμμετοχή σε κλάσμα
        public string Notes { get; set; } // Σχόλια
    }
}