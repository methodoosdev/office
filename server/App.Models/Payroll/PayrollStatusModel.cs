using App.Framework.Models;
using System;
using System.Collections.Generic;

namespace App.Web.Common.Models.Payroll
{
    public partial record PayrollStatusSearchModel : BaseNopModel
    {
        public int TraderId { get; set; } // Συναλλασσόμενος 
        public int PayrollStatusTypeId { get; set; } // Επιθυμητη περίοδος μέρα,μήνας,χρόνος

    }

    public partial record PayrollStatusModel : BaseNopEntityModel
    {
        public string Employee { get; set; } // Υπάλληλος
        public DateTime HireDate { get; set; } // Ημερομηνία πρόσληψης
        public string Specialty { get; set; } // Ειδικότητα  
        public string EmployeeType { get; set; } // Είδος εργαζομένου
        public string ContractType { get; set; } // Είδος σύμβασης
        public DateTime? FixedContractStartDate { get; set; } // Ημερομηνία έναρξης σύμβασης ορισμένου χρόνου
        public DateTime? FixedContractEndDate { get; set; } // Ημερομηνία λήξης σύμβασης ορισμένου χρόνου
        public int Children {  get; set; } // Αριθμός παιδιών

        // Συμφωνηθέντα με δώρα και επιδόματα - Agreed with gifts
        public decimal W_HourlyWages { get; set; } // Ωρομίσθιο
        public decimal W_Salary { get; set; } // Μισθό-ημερομίσθιο
        public decimal W_EmployeeContribution { get; set; } // Εισφορές εργαζόμενου  
        public decimal W_EmployerContribution { get; set; } // Εισφορές εργοδότη  
        public decimal W_Fmy { get; set; } // ΦΜΥ
        public decimal W_NetSalary { get; set; } // Καθαρός μισθός
        public decimal W_NetSalaryPreTax { get; set; } // Καθαρός μισθός προ φόρων
        public decimal W_Cost { get; set; } // Κόστος

        // Νόμιμα με δώρα και επιδόματα - Legal with gifts
        public decimal G_HourlyWages { get; set; } // Ωρομίσθιο
        public decimal G_Salary { get; set; } // Μισθό-ημερομίσθιο
        public decimal G_EmployeeContribution { get; set; } // Εισφορές εργαζόμενου  
        public decimal G_EmployerContribution { get; set; } // Εισφορές εργοδότη  
        public decimal G_Fmy { get; set; } // ΦΜΥ
        public decimal G_NetSalary { get; set; } // Καθαρός μισθός
        public decimal G_NetSalaryPreTax { get; set; } // Καθαρός μισθός προ φόρων
        public decimal G_Cost { get; set; } // Κόστος


    }
    public partial record PayrollStatusTableModel : BaseNopModel
    {
    }
    public enum PayrollStatusType
    {
        Day = 1,
        Month = 2,
        Year = 3
    }
}
