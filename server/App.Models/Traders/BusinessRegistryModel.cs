using System;

namespace App.Models.Traders
{
    public class BusinessRegistryModel
    {
        public string Vat { get; set; } // P.F_AFM AS Vat, -- ΑΦΜ
        public string LastName { get; set; } // P.F_SURNAME_A AS LastName, -- Επώνυμο Α ή Επωνυμία
        public string Doy { get; set; } // DOY.DESCR AS Doy, -- Όνομα εφορίας
        public int ActivatedTypeId { get; set; } // P.F_ACTIVATED AS ActivatedTypeId, -- Ενεργός i
        public int ProfessionTypeId { get; set; }//F_JOB - Άσκηση επαγγέλματος
        public string TradeName { get; set; } // P.F_TITLE AS TradeName, -- Διακριτικός τίτλος
        public string JobAddress { get; set; } // P.F_JOB_ADDRESS AS JobAddress, -- Διεύθυνση
        public string JobStreetNumber { get; set; } // P.F_JOB_NUMBER AS JobStreetNumber, -- Αριθμός
        public string JobCity { get; set; } // P.F_JOB_CITY AS JobCity, -- Πόλη
        public string JobPostcode { get; set; } // P.F_JOB_POSTCODE AS JobPostcode, -- Ταχ.Κώδικας
        public DateTime? StartingDate { get; set; } // P.F_FROMDATE AS StartingDate, -- Ημερ.Έναρξης εργασιών d?
        public DateTime? ExpiryDate { get; set; } // P.F_TODATE AS ExpiryDate, -- Ημερ.Λήξης εργασιών d?
        public string ProfessionalActivity { get; set; } // P.F_JOB AS ProfessionalActivity, -- Επάγγελμα
    }
}
