using App.Framework.Models;
using System;
using System.Collections.Generic;

namespace App.Web.Common.Models.Payroll
{
    public partial record ApdContributionSearchModel : BaseNopModel
    {
        public ApdContributionSearchModel()
        {
            EmployerIds = new List<int>();
        }

        public IList<int> EmployerIds { get; set; }
        public DateTime Period { get; set; } // Επιθυμητή ημερομηνία

    }

    public partial record ApdContributionModel : BaseNopEntityModel
    {
        public string CompanyName {  get; set; } //Επωνυμία
        public decimal TotalEfka { get; set; } //Σύνολο ΕΦΚΑ  
        public decimal TotalTeka { get; set; } //Συνολο Τεκα
        public decimal ChristmasPresentEfka {  get; set; } // Δώρο χριστουγέννων ΕΦΚΑ
        public decimal ChristmasPresentTeka { get; set; } // Δώρο χριστουγέννων ΤΕΚΑ
        public decimal EasterPresentEfka { get; set; } // Δώρο Πάσχα ΕΦΚΑ
        public decimal EasterPresentTeka { get; set; } // Δώρο Πάσχα ΤΕΚΑ
        public string Notes { get; set; } //Παρατηρήσεις

        public string EmployeeName { get; set; }

    }
    public partial record ApdContributionTableModel : BaseNopModel
    { 
    }

    public class ApdContributionResult
    {
        public string CompanyName { get; set; } //Επωνυμία
        public decimal EfkaEmployer { get; set; } //Σύνολο ΕΦΚΑ  
        public decimal Teka { get; set; } //Συνολο Τεκα
        public int Periodos { get; set; } //Περίοδος εισφορών ( 13 Δώρο Χριστουγέννων / 14 Δώρο Πάσχα) 
        public int EmployeeId { get; set; }
        public string FullName { get; set; }

    }
    public class EmployeeHelper 
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; }
    }
}
