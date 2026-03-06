using App.Framework.Models;
using System;

namespace App.Models.Traders
{
    public partial record TraderMembershipSearchModel : BaseSearchModel
    {
        public TraderMembershipSearchModel() : base("surnameFatherName") { }
    }
    public partial record TraderMembershipListModel : BasePagedListModel<TraderMembershipModel>
    {
    }
    public partial record TraderMembershipModel : BaseNopEntityModel
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

        public string TraderName { get; set; }
        public DateTime StartDateOn { get; set; } // Ημ/νία έναρξης μέλους
        public DateTime? ExpireDateOn { get; set; } // Ημ/νία διακοπής μέλους
        public string TraderBoardMemberTypeName { get; set; } // Είδος σχέσης
    }
    public partial record TraderMembershipFormModel : BaseNopModel
    {
    }
}