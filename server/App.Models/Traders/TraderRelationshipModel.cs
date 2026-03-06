using App.Framework.Models;
using System;

namespace App.Models.Traders
{
    public partial record TraderRelationshipSearchModel : BaseSearchModel
    {
        public TraderRelationshipSearchModel() : base("surnameFatherName") { }
    }
    public partial record TraderRelationshipListModel : BasePagedListModel<TraderRelationshipModel>
    {
    }
    public partial record TraderRelationshipModel : BaseNopEntityModel // Σχέσεις νομικού προσώπου
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

        public string TraderName { get; set; }
        public DateTime StartDateOn { get; set; } // Ημ/νία έναρξης σχέσης
        public DateTime? ExpireDateOn { get; set; } // Ημ/νία διακοπής σχέσης
        public string TraderBoardMemberTypeName { get; set; } // Είδος σχέσης
    }
    public partial record TraderRelationshipFormModel : BaseNopModel
    {
    }
}