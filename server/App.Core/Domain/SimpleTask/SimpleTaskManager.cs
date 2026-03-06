using System;

namespace App.Core.Domain.SimpleTask
{
    public partial class SimpleTaskManager : BaseEntity // Διαχείρηση εργασιών
    {
        public string Name { get; set; } //* Εργασία

        public int AssignorId { get; set; } //* Εντολέας
        public int EmployeeId { get; set; } // Υπάλληλο
        public DateTime? StartingDate { get; set; } //* Ημερ/νία έναρξης
        public DateTime? EndingDate { get; set; } //* Ημερ/νία λήξης

        public int TraderId { get; set; } // Συναλλασσόμενος
        public string Branch { get; set; } // Υπηρεσία
        public string Contact { get; set; } // Είδος νόμου

        public DateTime? CreatedDate { get; set; } //* Ημερομηνία δημιουργίας
        public int SimpleTaskPriorityTypeId { get; set; } // Προτεραιότητα
        public int SimpleTaskTypeId { get; set; } // Κατάσταση

        public int? SimpleTaskCategoryId { get; set; } // Κατηγορία
        public int? SimpleTaskNatureId { get; set; } // Φύση αντικειμένου
        public int? SimpleTaskDepartmentId { get; set; } // Τμήμα
        public int? SimpleTaskSectorId { get; set; } // Τομέας

        public string RelateTo { get; set; } // Αφορά Υπάλληλο, Συν/μενο, Άλλο
        public string GovService { get; set; } // Υπηρεσία
        public string LawType { get; set; } // Είδος νόμου
        public string DocumentType { get; set; } // Είδος εγγραφου


        public string Notes { get; set; } // Περιγραφή εργασίας
        public string Participants { get; set; } // Συμμετέχοντες
        public int CustomerId { get; set; }
    }
}
