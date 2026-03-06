using App.Framework.Models;
using System;
using System.Collections.Generic;

namespace App.Web.Common.Models.Payroll
{
    public partial record FmyContributionSearchModel : BaseNopModel
    {
        public FmyContributionSearchModel()
        {
            EmployerIds = new List<int>();
        }

        public IList<int> EmployerIds { get; set; }
        public DateTime Period { get; set; } // Επιθυμητή ημερομηνία

    }

    public partial record FmyContributionModel : BaseNopEntityModel
    {
        public string CompanyName {  get; set; } //Επωνυμία
        public decimal TotalFmy { get; set; } //Σύνολο ΦΜΥ 
        public decimal ChristmasPresentFmy { get; set; } //Δώρο χριστουγέννων ΦΜΥ 
        public decimal EasterPresentFmy { get; set; } //Δώρο Πάσχα ΦΜΥ 

        public string EmployeeName { get; set; }

    }
    public partial record FmyContributionTableModel : BaseNopModel
    { 
    }

    public class FmyContributionResult
    { 
        public string CompanyName { get; set; }
        public decimal TotalFmy { get; set; }
        public int Periodos { get; set; } //Περίοδος εισφορών ( 13 Δώρο Χριστουγέννων / 14 Δώρο Πάσχα) 
    }
}
