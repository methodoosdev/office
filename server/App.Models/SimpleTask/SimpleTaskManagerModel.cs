using App.Framework.Models;
using System;
using System.Collections.Generic;

namespace App.Models.SimpleTask
{
    public partial record SimpleTaskManagerFilterModel : BaseNopModel
    {
        public SimpleTaskManagerFilterModel()
        {
            SimpleTaskPriorityTypeId = new List<int>();
            SimpleTaskTypeId = new List<int>();
        }
        public string Name { get; set; } = null; //* Εργασία

        public int AssignorId { get; set; } = 0; //* Εντολέας
        public int EmployeeId { get; set; } = 0; // Παραλήπτης
        public int TraderId { get; set; } = 0; // Συναλλασσόμενος
        public List<int> SimpleTaskPriorityTypeId { get; set; } // Προτεραιότητα
        public List<int> SimpleTaskTypeId { get; set; } // Κατάσταση

        public int SimpleTaskCategoryId { get; set; } = 0; // Κατηγορία
        public int SimpleTaskNatureId { get; set; } = 0; // Φύση αντικειμένου
        public int SimpleTaskDepartmentId { get; set; } = 0; // Τμήμα
        public int SimpleTaskSectorId { get; set; } = 0; // Τομέας
    }
    public partial record SimpleTaskManagerFilterFormModel : BaseNopModel
    {
    }
    public partial record SimpleTaskManagerSearchModel : BaseSearchModel
    {
        public SimpleTaskManagerSearchModel() : base("endingDate", "desc") { }
    }
    public partial record SimpleTaskManagerListModel : BasePagedListModel<SimpleTaskManagerModel>
    {
    }
    public partial record SimpleTaskManagerModel : BaseNopEntityModel
    {
        public string AssignorName { get; set; } //* Εντολέας
        public string EmployeeName { get; set; } // Παραλήπτης
        public string TraderName { get; set; } // Συναλλασσόμενος
        public string SimpleTaskPriorityTypeName { get; set; } // Προτεραιότητα
        public string SimpleTaskTypeName { get; set; } // Κατάσταση
        public string SimpleTaskCategoryName { get; set; } // Κατηγορία
        public string SimpleTaskNatureName { get; set; } // Φύση αντικειμένου
        public string SimpleTaskDepartmentName { get; set; } // Τμήμα
        public string SimpleTaskSectorName { get; set; } // Τομέας
        public string SimpleTaskPriorityTypeBackground { get; set; }
        public string SimpleTaskTypeBackground { get; set; }
        //
        public string Name { get; set; } //* Εργασία

        public int AssignorId { get; set; } //* Εντολέας
        public int EmployeeId { get; set; } // Παραλήπτης
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
    public partial record SimpleTaskManagerFormModel : BaseNopModel
    {
    }
}