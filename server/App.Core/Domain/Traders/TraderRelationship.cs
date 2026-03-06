using System;

namespace App.Core.Domain.Traders
{
    public partial class TraderRelationship : BaseEntity // Σχέσεις νομικού προσώπου
    {
        public int ParentId { get; set; }
        public int TraderId { get; set; }
        public string Vat { get; set; } // ΑΦΜ
        public string SurnameFatherName { get; set; } // Επώνυμο Όνομα Πατρώνυμο
        public DateTime StartDateOnUtc { get; set; } // Ημ/νία έναρξης σχέσης
        public DateTime? ExpireDateOnUtc { get; set; } // Ημ/νία διακοπής σχέσης
        public string RelationshipName { get; set; } // Είδος σχέσης
        public int TraderBoardMemberTypeId { get; set; } // Είδος σχέσης
        public decimal RelationshipRate { get; set; } // Ποσοστό συμμετοχής
        public string Notes { get; set; } // Σχόλια
    }
}