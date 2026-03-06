using App.Framework.Models;
using System;
using System.Collections.Generic;

namespace App.Models.Payroll
{
    public partial record FmySubmissionSearchModel : BaseNopModel
    {
        public FmySubmissionSearchModel() 
        {
            SelectedKeys = new List<int>();
        }

        public IList<int> SelectedKeys { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string Progress { get; set; }
    }

    public partial record FmySubmissionModel : BaseNopModel // Αποδεικτικό υποβολής ΦΜΥ
	{
		public bool HyperMItem { get; set; } 
		public bool? Check { get; set; } // Έλεγχος
        public string Surname { get; set; } // Επωνυμία
        public string SubmissionDate { get; set; } // Ημερομηνία υποβολής
        public string SubmissionType { get; set; } // Τύπος υποβολής
        public string Period { get; set; } // Περίοδος
        public decimal GrossEarnings { get; set; } // Ακαθάριστες αποδοχές
        public decimal TaxAmount { get; set; } // Ποσό φόρου
        public decimal Contribution { get; set; } // Εισφορά αλληλεγγύης
        public decimal Stamp { get; set; } // Χαρτόσημο
        public decimal Stamp1 { get; set; } // Χαρτόσημο 1%
        public decimal StampOga { get; set; } // Χαρτόσημο ΟΓΑ
        public decimal? StampTotal { get; set; } // Χαρτ.Σύνολο
        public decimal? Compensation { get; set; } // Αποζημίωση
    }
    public partial record FmySubmissionTableModel : BaseNopModel
    {
    }
}