using System;

namespace App.Core.Infrastructure.Dtos.Accounting
{
    public class FhmItemModel
    {
        public int CashRegisterId { get; set; }         // Α/Α Ταμειακής
        public DateTime StartingDate { get; set; }      // Ημερομ. Έναρξης Λειτουργίας 
        public string Tameiaki { get; set; }            //  Ταμειακή μηχανή
        public string InstallationNumber { get; set; }    // Αριθμός εγκατάστασης
        public string TameiakiType { get; set; }        // Τύπος ταμειακής
    }
    public class PosItemModel
    {
        public int PosId { get; set; }                       // Α/Α POS 
        public string Provider { get; set; }                // Επωνυμία Παρόχου Μέσων Πληρωμών 
        public string DestinationTerminalID { get; set; }      // Αριθμός αναγνώρισης μέσου πληρωμών
        public string DestinationMerchantID { get; set; }    // Kωδικός Χρήστη Υπηρεσιών Πληρωμών
        public string PosType { get; set; }       // Είδος POS
        public string Status { get; set; }       // Κατάσταση
        public DateTime ActivationDate { get; set; }     // Ημερομηνία ενεργοποίησης
        public DateTime? DectivationDate { get; set; }     // Ημερομηνία απενεργοποίησης
    }
    public class ContractsItemModel
    {
        public int PosId { get; set; }                       // Α/Α POS 
        public string Αcquirer { get; set; }                // Επωνυμία Παρόχου Υπηρεσιών Πληρωμών 
        public string DestinationTerminalID { get; set; }      // Αριθμός αναγνώρισης μέσου πληρωμών
        public string DestinationMerchantID { get; set; }    // Kωδικός Χρήστη Υπηρεσιών Πληρωμών
        public string AccountNumber { get; set; }       // Αριθμός Λογαριασμού
        public DateTime StartingDate { get; set; }     // Ημερομηνία έναρξης ισχύος σύμβασης
        public string Provider { get; set; }               // Επωνυμία Παρόχου Μέσων Πληρωμών 
    }
}