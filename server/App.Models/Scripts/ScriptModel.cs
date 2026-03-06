using App.Framework.Models;
using System;

namespace App.Models.Scripts
{
    public partial record ScriptSearchModel : BaseSearchModel
    {
        public ScriptSearchModel() : base("order") { }
    }
    public partial record ScriptListModel : BasePagedListModel<ScriptModel>
    {
    }
    public partial record ScriptModel : BaseNopEntityModel
    {
        public int TraderId { get; set; } // Συναλλασσόμενος
        public string ScriptName { get; set; } // Όνoμα σεναρίου
        [Obsolete]
        public string Group { get; set; } // Ομαδοποίηση
        public int ScriptGroupId { get; set; } // Ομαδοποίηση
        public string Description { get; set; } // Περιγραφή
        public int Order { get; set; } // Κατάταξη
        public bool IsPercent { get; set; } // Εμφάνιση ποσοστό
        public bool Printed { get; set; } // Εκτυπώνεται 
        public string Replacement { get; set; } // Αντικατάσταση 

        public int ScriptAlignTypeId { get; set; } // Τοποθέτηση
        public string ScriptCode { get; set; } // Κωδικός
        public bool HasHeader { get; set; } // Έχει επικεφαλίδα
        public string HeaderCode { get; set; } // Κείμενο κωδικού
        public string Header { get; set; } // Κείμενο
        public string HeaderLeft { get; set; } // Κείμενο αριστερά
        public string HeaderRight { get; set; } // Κείμενο δεξιά

        public string ScriptGroupName { get; set; } // Ομαδοποίηση
        public string ScriptAlignTypeName { get; set; } // Τοποθέτηση
    }
    public partial record ScriptFormModel : BaseNopModel
    {
    }
    public partial record ScriptReport : BaseNopEntityModel
    {
        public int TraderId { get; set; } // Συναλλασσόμενος
        public string ScriptName { get; set; } // Όνoμα σεναρίου
        public string Group { get; set; } // Ομαδοποίηση
        public int ScriptGroupId { get; set; } // Ομαδοποίηση
        public string Description { get; set; } // Περιγραφή
        public int Order { get; set; } // Κατάταξη
        public bool IsPercent { get; set; } // Εμφάνιση ποσοστό
        public bool Printed { get; set; } // Εκτυπώνεται 
        public string Replacement { get; set; } // Αντικατάσταση 
        public int ScriptAlignTypeId { get; set; } // Τοποθέτηση
        public string ScriptCode { get; set; } // Κωδικός
        public bool HasHeader { get; set; } // Έχει επικεφαλίδα
        public string HeaderCode { get; set; } // Κείμενο κωδικού
        public string Header { get; set; } // Κείμενο
        public string HeaderLeft { get; set; } // Κείμενο αριστερά
        public string HeaderRight { get; set; } // Κείμενο δεξιά
        public decimal Value { get; set; } // Αξία
        public int ScriptGroupAlignTypeId { get; set; } // Τοποθέτηση

    }
}