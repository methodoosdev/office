using System;

namespace App.Core.Domain.Scripts
{
    public partial class ScriptTableName : BaseEntity // Ονομασίες πινάκων
    {
        public string Name { get; set; } // Όνομα πίνακα
        public int Order { get; set; } // Κατάταξη
    }
    public partial class ScriptTable : BaseEntity // Πίνακες λογαριασμών
    {
        public int TraderId { get; set; } // Συναλλασσόμενος
        public string TableName { get; set; } // Όνομα πίνακα
        [Obsolete]
        public string Group { get; set; } // Ομαδοποίηση
        public int ScriptGroupId { get; set; } // Ομαδοποίηση
        public int Order { get; set; } // Κατάταξη
    }
    public partial class ScriptGroup : BaseEntity // Ομαδοποίηση
    {
        public int TraderId { get; set; } // Συναλλασσόμενος
        public string GroupName { get; set; } // Όνομα ομαδοποίησης
        public int ScriptAlignTypeId { get; set; } // Τοποθέτηση
        public int Order { get; set; } // Κατάταξη
    }
    public enum ScriptBehaviorType // Συμπεριφορά
    {
        Included = 0, // Περιλαμβάνεται
        Excluded = 1 // Εξαιρείται
    }
    public partial class ScriptTableItem : BaseEntity // Στοιχείο πίνακα λογαριασμών
    {
        public int ScriptTableId { get; set; } // Πίνακας λογαριασού
        public string AccountingCode { get; set; } // Λογαριασμός
        public int ScriptBehaviorTypeId { get; set; } // Συμπεριφορά
    }
    public enum ScriptFiscalYearType // Οικονομική περίοδος χρήσης
    {
        Current = 0, // Τρέχουσα
        Previews = 1, // Προηγούμενη
        //Initial = 2 // Αρχική
    }
    public enum ScriptAggregateType // Τύπος υπολογισμού
    {
        Sum = 0, // Άθροισμα
        Average = 1, // Μέσος όρος
        Count = 2, //Πλήθος
        Min = 3, // Μικρότερη
        Max = 4, // Μεγαλύτερη
    }
    public enum ScriptFieldType // Τύπος πεδίου
    {
        Table = 0, // Πίνακας
        Query = 1, // Ερώτημα στη βάση
        Function = 2, // Διαδικασία
        Fixed = 3 // Σταθερή τιμή
    }
    public enum ScriptQueryType // Τύπος διαδικασίας
    {
        Payments = 1, // Πληρωμές
        Receipts = 2, // Εισπράξεις
        Orders = 3, // Παραγγελίες
    }
    public enum ScriptFunctionType // Τύπος διαδικασίας
    {
        EmployeesCount = 1, // Πλήθος εργαζομένων
        TaxesFee = 2, // Τέλος επιτηδεύματος
    }
    public partial class ScriptField : BaseEntity // Υπολογιζόμενα πεδία
    {
        // Table
        public int ScriptTableId { get; set; } // Πίνακας λογαριασμού
        public int StartingFiscalYear { get; set; } // Αρχική οικον.χρήση
        public int FiscalYear { get; set; } // Οικονομική χρήση
        public int PeriodFrom { get; set; } // Περίοδος από
        public int PeriodTo { get; set; } // Περίοδος εώς
        public bool Inventory { get; set; } // Απογραφή
        public bool BalanceSheet { get; set; } // Ισολογισμός
        public bool Locked { get; set; } // Κλειδωμένο
        // Query
        public int ScriptQueryTypeId { get; set; } // Ερώτημα στη βάση
        // Function
        public int ScriptFunctionTypeId { get; set; } // Σταθερές διαδικασίες
        [Obsolete]
        public int ScriptFunctionId { get; set; } // Σταθερές διαδικασίες
        // Value
        public decimal FixedValue { get; set; } // Σταθερή τιμή

        public int TraderId { get; set; } // Συναλλασσόμενος
        public string FieldName { get; set; } // Όνομα πεδίου
        [Obsolete]
        public string Group { get; set; } // Ομαδοποίηση
        public int ScriptGroupId { get; set; } // Ομαδοποίηση
        public string Description { get; set; } // Περιγραφή
        public int ScriptAggregateTypeId { get; set; } // Τύπος υπολογισμού
        public int ScriptFieldTypeId { get; set; } // Τύπος πεδίου
        public int Order { get; set; } // Κατάταξη
    }
    public enum ScriptAlignType
    {
        Left = 0, // Ανά μήνα
        Right = 1 // Εκ μεταφοράς
    }
    public partial class Script : BaseEntity // Σενάρια υπολογισμού
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
    }
    public enum ScriptType // Τύπος σεναρίου
    {
        Field = 0, // Πεδίου
        Script = 1 // Σεναρίου
    }
    public enum ScriptOperationType // Αριθμητική πράξη 
    {
        Addition = 0, // Πρόσθεση
        Subtraction = 1, // Αφαίρεση
        Multiplication = 2, // Πολλαπλασιασμός
        Division = 3, // Διαίρεση
        Percent = 4 // Ποσοστό
    }
    public partial class ScriptItem : BaseEntity // Σενάρια υπολογισμού λεπτομέρειες
    {
        public int ScriptId { get; set; } // Σενάριο υπολογισμού
        public int ScriptTypeId { get; set; } // Τύπος σεναρίου
        public int ScriptFieldId { get; set; } // Υπολογιζόμενο πεδίο
        public int ParentId { get; set; } // Σενάριο υπολογισμού
        public int ScriptOperationTypeId { get; set; } // Αριθμητική πράξη 
        public int Order { get; set; } // Κατάταξη
    }
    public enum ScriptPivotShowType
    {
        Period = 0, // Ανά μήνα
        Transfer = 1 // Εκ μεταφοράς
    }
    public partial class ScriptPivot : BaseEntity // Σενάρια υπολογισμού
    {
        public int TraderId { get; set; } // Συναλλασσόμενος
        public string ScriptPivotName { get; set; } // Όνoμα σεναρίου
        [Obsolete]
        public string Group { get; set; } // Ομαδοποίηση
        public int ScriptGroupId { get; set; } // Ομαδοποίηση
        public string Description { get; set; } // Περιγραφή
        public int Order { get; set; } // Κατάταξη
    }
    public partial class ScriptPivotItem : BaseEntity // Σενάρια υπολογισμού λεπτομέρειες
    {
        public int ScriptPivotId { get; set; } // Σενάριο υπολογισμού
        public int ScriptFieldId { get; set; } // Υπολογιζόμενο πεδίο
        public int ScriptOperationTypeId { get; set; } // Αριθμητική πράξη 
        public bool Printed { get; set; } // Εκτυπώνεται 
        public int Order { get; set; } // Κατάταξη
    }

    public partial class ScriptTool : BaseEntity // Εργαλεία
    {
        public int TraderId { get; set; } // Συναλλασσόμενος
        public string Title { get; set; } // Τίτλος
        public string Subtitle { get; set; } // Υπότιτλος
        public string Description { get; set; } // Σχόλια
        public int Order { get; set; } // Κατάταξη
        public string FileName { get; set; }
        public string Extension { get; set; }
        public string ContentType { get; set; }
        public long? SizeBytes { get; set; }
        public byte[] Bytes { get; set; }
        public DateTime? CreatedOnUtc { get; set; }
    }

    public partial class ScriptToolItem : BaseEntity // Εργαλεία λεπτομέρειες
    {
        public int ScriptToolId { get; set; } // Εργαλεία
        public int ScriptId { get; set; } // Σενάρια υπολογισμού λεπτομέρειες
    }
}
