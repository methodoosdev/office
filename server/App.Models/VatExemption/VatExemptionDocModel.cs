using App.Framework.Models;
using System;

namespace App.Models.VatExemption
{
    public partial record VatExemptionDocSearchModel : BaseSearchModel
    {
        public VatExemptionDocSearchModel() : base("createdDate", "desc") { }
    }
    public partial record VatExemptionDocListModel : BasePagedListModel<VatExemptionDocModel>
    {
    }
    public partial record VatExemptionDocModel : BaseNopEntityModel
    {
        public int VatExemptionSerialId { get; set; }
        public decimal ApprovalLimit { get; set; }// ΧΟΡΗΓΗΘΕΝ ΟΡΙΟ ΑΠΑΛΛΑΓΗΣ
        public string SerialName { get; set; }// ΣΕΙΡΑ
        public int SerialNo { get; set; }// Α/Α
        public decimal SerialLimit { get; set; }// ΟΡΙΟ ΣΕΙΡΑΣ
        public string ApprovalNumber { get; set; }//ΑΡΙΘΜ. ΕΓΚΡΙΣΗΣ
        public DateTime ApprovalExpiryDate { get; set; }// ΗΜΕΡΟΜΗΝΙΑ ΛΗΞΗΣ ΔΙΑΡΚΕΙΑΣ ΙΣΧΥΟΣ
        public string TraderFullName { get; set; }// ΟΝΟΜΑΤΕΠΩΝΥΜΟ ή ΕΠΩΝΥΜΙΑ ΕΠΙΧΕΊΡΗΣΗΣ
        public string TraderProfessionalActivity { get; set; }// ΑΝΤΙΚΕΙΜΕΝΟ ΕΡΓΑΣΙΩΝ
        public string TraderAddress { get; set; }// ΟΔΟΣ
        public string TraderStreetNumber { get; set; }// ΑΡΙΘΜ.
        public string TraderPostcode { get; set; }// ΤΑΧ ΚΩΔ.
        public string TraderCity { get; set; }// ΠΟΛΗ
        public string TraderVat { get; set; }// Α.Φ.Μ.
        public string TraderDoy { get; set; }// Δ.Ο.Υ.
        public string SupplierFullName { get; set; }// ΟΝΟΜΑΤΕΠΩΝΥΜΟ ή ΕΠΩΝΥΜΙΑ ΕΠΙΧΕΊΡΗΣΗΣ
        public string SupplierProfessionalActivity { get; set; }// ΑΝΤΙΚΕΙΜΕΝΟ ΕΡΓΑΣΙΩΝ
        public string SupplierAddress { get; set; }// ΟΔΟΣ
        public string SupplierStreetNumber { get; set; }// ΑΡΙΘΜ.
        public string SupplierPostcode { get; set; }// ΤΑΧ ΚΩΔ.
        public string SupplierCity { get; set; }// ΠΟΛΗ
        public string SupplierVat { get; set; }// Α.Φ.Μ.
        public string SupplierDoy { get; set; }// Δ.Ο.Υ.
        public string Customs { get; set; }// Τελωνείο
        public decimal LimitBalance { get; set; }// ΥΠΟΛΟΙΠΟ ΟΡΙΟΥ
        public decimal ReturnDiscount { get; set; }// (+) ΕΚΠΤΩΣΕΙΣ/ΕΠΙΣΤΡΟΦΕΣ
        public decimal TransferFromSeries { get; set; }// (+) ΜΕΤΑΦΟΡΑ ΑΠΟ ΤΗ ΣΕΙΡΑ
        public decimal TransferToSeries { get; set; }// (-) ΜΕΤΑΦΟΡΑ ΣΤΗ ΣΕΙΡΑ
        public decimal AdjustedLimit { get; set; }// ΑΝΑΠΡΟΣΑΡΜΟΣΜΕΝΟ ΟΡΙΟ ΑΠΑΛΛΑΓΗΣ ΠΡΟΣ ΧΡΗΣΗ
        public decimal CurrentTransaction { get; set; }// (-) ΟΡΙΟ ΤΡΕΧΟΥΣΑΣ ΣΥΝΑΛΛΑΓΗΣ
        public string CurrentTransactionAlphabet { get; set; }// (ποσό ολογράφως)
        public decimal CurrentLimit { get; set; } // ΥΠΟΛΟΙΠΟ ΟΡΙΟΥ ΣΕΙΡΑΣ ΠΡΟΣ ΜΕΤΑΦΟΡΑ
        public string CurrentLimitAlphabet { get; set; }// (ποσό ολογράφως)
        public string DocumentCity { get; set; }// (Πόλη)
        public DateTime? CreatedDate { get; set; }// /(Ημερομηνία)
        public int TraderId { get; set; }

        //
        public string CreatedDateValue { get; set; }
        public string TraderName { get; set; }

    }
    public partial record VatExemptionDocFormModel : BaseNopModel
    {
    }
    public class DecimalsToAlphabetModel
    {
        public decimal CurrentTransaction { get; set; }
        public decimal CurrentLimit { get; set; }
    }
}