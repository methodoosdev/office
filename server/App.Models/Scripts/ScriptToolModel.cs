using App.Framework.Models;
using System;
using System.Collections.Generic;

namespace App.Models.Scripts
{
    public partial record ScriptToolConfigModel : BaseNopModel
    {
        public bool Active { get; set; }
        public int FiscalYear { get; set; }
        public int PeriodFrom { get; set; }
        public int PeriodTo { get; set; }
        public bool Inventory { get; set; }
    }
    public partial record ScriptToolSearchModel : BaseSearchModel
    {
        public ScriptToolSearchModel() : base("order") { }
    }
    public partial record ScriptToolListModel : BasePagedListModel<ScriptToolModel>
    {
    }
    public partial record ScriptToolModel : BaseNopEntityModel
    {
        public int TraderId { get; set; } // Συναλλασσόμενος
        public string Title { get; set; } // Τίτλος
        public string Subtitle { get; set; } // Υπότιτλος
        public string Description { get; set; } // Σχόλια
        public int Order { get; set; } // Κατάταξη
        public string FileName { get; set; } // Όνομα αρχείου
        public string Extension { get; set; } // Επέκταση
        public string ContentType { get; set; } // Τύπος περιεχομένου
        public long? SizeBytes { get; set; } // Μέγεθος Bytes
        public byte[] Bytes { get; set; } // Bytes
        public DateTime? CreatedOnUtc { get; set; } // Ημερ.Δημιουργίας
        public DateTime CreatedOn { get; set; } // Ημερ.Δημιουργίας
    }
    public partial record ScriptToolFormModel : BaseNopModel
    {
    }
    public sealed record ScriptToolFillRequest
    {
        public Dictionary<string, object> Mapping { get; set; }
        public string DownloadFileName { get; set; }
    }
}