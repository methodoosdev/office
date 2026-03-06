using App.Framework.Models;
using System;

namespace App.Models.Traders
{
    public partial record TaxSystemTraderSearchModel : BaseSearchModel
    {
        public TaxSystemTraderSearchModel() : base("lastName") { }
    }
    public partial record TaxSystemTraderListModel : BasePagedListModel<TaxSystemTraderModel>
    {
    }
    public partial record TaxSystemTraderModel : BaseNopEntityModel
    {
		// Προσωπικά στοιχεία
		public int TaxSystemId { get; set; } // P.ID AS TaxSystemId, -- Id i
		public string Vat { get; set; } // P.F_AFM AS Vat, -- ΑΦΜ
		public string LastName { get; set; } // P.F_SURNAME_A AS LastName, -- Επώνυμο Α ή Επωνυμία
		public string FirstName { get; set; } // P.F_NAME AS FirstName, -- Όνομα

		// Γενικά στοιχεία
		public string CodeName { get; set; } // P.F_CODE AS CodeName, -- Κωδικός
		public string LastName2 { get; set; } // P.F_SURNAME_B AS LastName2, -- Επώνυμο Β
		public string FatherLastName { get; set; } // P.F_FATHERS_SURNAME AS FatherLastName, -- Επώνυμο πατρός
		public string FatherFirstName { get; set; } // P.F_FATHERS_NAME AS FatherFirstName,  -- Όνομα πατρός
		public string MotherLastName { get; set; } // P.F_MOTHERS_SURNAME AS MotherLastName, -- Επώνυμο μητέρας
		public string MotherFirstname { get; set; } // P.F_MOTHERS_NAME AS MotherFirstname, -- Όνομα μητέρας
		public string IdentityTypeId { get; set; } // P.F_ID_TYPE AS IdentityTypeId, -- Τύπος ταυτότητας string
		public string IdentityTypeName { get; set; } // ID_TYPES.DESCR IdentityTypeName, -- Είδος ταυτότητας
		public string IdentityNumber { get; set; } // P.F_NUMBER_ID AS IdentityNumber, -- Αριθμός ταυτότητας
		public DateTime? IdentityDate { get; set; } // P.F_DATE_ID AS IdentityDate, -- Ημερομηνία έκδοσης d?
		public string IdentityDepartment { get; set; } // P.F_DEPARTMENT_ID AS IdentityDepartment, -- Εκδούσα αρχή
		public string Amka { get; set; } // P.F_AMKA AS Amka,  -- ΑΜΚΑ
		public string Gemh { get; set; } // P.F_GEMH AS Gemh, -- ΓΕΜΗ
		public string AmIka { get; set; } // P.F_AR_ASF AS AmIka, -- ΑΜ ΙΚΑ
		public string AmOaee { get; set; } // P.F_AMOAEE AS AmOaee, -- ΑΜ ΟΑΕΕ
		public string AmOga { get; set; } // P.F_AMOGA AS AmOga, --ΑΜ ΟΓΑ
		public string AmEtaa { get; set; } // P.F_AM_ETAA AS AmEtaa, -- ΑΜ ΕΤΑΑ

		public string AmDiasIka { get; set; } // P.F_AR_DIAS AS AmDiasIka, -- ΑΜ ΔΙΑΣ ΙΚΑ
		public string AmDiasEtea { get; set; } // P.F_AR_ETEA AS AmDiasEtea, -- ΑΜ ΔΙΑΣ ΕΤΕΑ
		public string AmEmployer { get; set; } // P.F_AME AS AmEmployer, -- ΑΜ Εργοδότη
		public string AmRetirement { get; set; } // P.F_SINTAKSIS AS AmRetirement, -- ΑΜ Σύνταξης
		public string AmsOga { get; set; } // P.F_AMS_OGA AS AmsOga, -- ΑΜΣ ΟΓΑ
		public string AmsNat { get; set; } // P.F_AMS_NAT AS AmsNat, -- ΑΜΣ ΝΑΤ
		public string Eam { get; set; } // P.F_EAM AS Eam, -- ΕΑΜ
		public string TradeName { get; set; } // P.F_TITLE AS TradeName, -- Διακριτικός τίτλος
		public string ProfessionalActivity { get; set; } // P.F_JOB AS ProfessionalActivity, -- Επάγγελμα
		public string Doy { get; set; } // DOY.DESCR AS Doy, -- Όνομα εφορίας
		public string LocalOffice { get; set; } // P.F_LOCAL_OFFICE AS LocalOffice, -- Τοπικό γραφείο
		public string EnvelopeNumber { get; set; } // P.F_FORDER_NUMBER AS EnvelopeNumber, -- Αριθμός φακέλλου

		public DateTime? Birthday { get; set; } // P.F_BIRTHDAY AS Birthday, -- Ημερομηνία γέννησης d?
		public string BirthPlace { get; set; } // P.F_BIRTH_PLACE AS BirthPlace, -- Τόπος γέννησης
		public string MunicipalNumber { get; set; } // P.F_POP_REGISTER AS MunicipalNumber, -- Αριθμός δημοτολογίου
		public DateTime? MarriedDate { get; set; } // P.F_DTMARRIAGE AS MarriedDate, -- Ημερομηνία γάμου

		public int ActivatedTypeId { get; set; } // P.F_ACTIVATED AS ActivatedTypeId, -- Ενεργός i
		public DateTime? DeathDate { get; set; } // P.F_DEATH_DATE AS DeathDate, --Ημερομηνία Θανάτου d?

		// Στοιχεία έδρας
		public string RegisterCode { get; set; } // P.F_KEDM AS RegisterCode, -- Κωδικός μητρώου
		public string JobAddress { get; set; } // P.F_JOB_ADDRESS AS JobAddress, -- Διεύθυνση
		public string JobStreetNumber { get; set; } // P.F_JOB_NUMBER AS JobStreetNumber, -- Αριθμός
		public string JobCity { get; set; } // P.F_JOB_CITY AS JobCity, -- Πόλη
		public string JobMunicipality { get; set; } // P.F_JOB_MUNICIPALITY AS JobMunicipality, -- Δήμος
		public string JobPlace { get; set; } // P.F_JOB_AREA AS JobPlace, -- Νομός
		public string JobPostcode { get; set; } // P.F_JOB_POSTCODE AS JobPostcode, -- Ταχ.Κώδικας
		public string JobPhoneNumber1 { get; set; } // P.F_JOB_PHONE1 AS JobPhoneNumber1, -- Τηλέφωνο 1
		public string JobPhoneNumber2 { get; set; } // P.F_JOB_PHONE2 AS JobPhoneNumber2, -- Τηλέφωνο 2
		public string JobFax { get; set; } // P.F_JOB_FAX AS JobFax, -- Φαξ

		// Στοιχεία οικίας
		public string HomeAddress { get; set; } // P.F_HOME_ADDRESS AS HomeAddress, -- Διεύθυνση
		public string HomeStreetNumber { get; set; } // P.F_HOME_NUMBER AS HomeStreetNumber, -- Αριθμός
		public string HomeCity { get; set; } // P.F_HOME_CITY AS HomeCity, -- Πόλη
		public string HomeMunicipality { get; set; } // P.F_HOME_MUNICIPALITY AS HomeMunicipality, -- Δήμος
		public string HomePlace { get; set; } // P.F_HOME_AREA AS HomePlace, -- Νομός
		public string HomePostcode { get; set; } // P.F_HOME_POSTCODE AS HomePostcode, -- Ταχ.Κώδικας
		public string HomePhoneNumber1 { get; set; } // P.F_HOME_PHONE1 AS HomePhoneNumber1, -- Τηλέφωνο 1
		public string HomePhoneNumber2 { get; set; } // P.F_HOME_PHONE2 AS HomePhoneNumber2, -- Τηλέφωνο 2
		public string HomeCellphone { get; set; } // P.F_HOME_CELLPHONE AS HomeCellphone, --Κινητό
		public string HomeFax { get; set; } // P.F_HOME_FAX AS HomeFax, -- Φαξ

	    // Άλλα στοιχεία
		public DateTime? DeadlineSubmition { get; set; } // PARTY_DT.F_DEADLINE AS DeadlineSubmition, -- Προθεσμία Υποβολής

		public int StatusTypeId { get; set; } // PARTY_DT.F_STATUS AS StatusTypeId, -- Κατάσταση i?
		public int CustomerTypeId { get; set; } // P.F_TYPE AS CustomerTypeId, -- Είδος i
        public string CustomerTypeName { get; set; }
        public int FarmerTypeId { get; set; } // ISNULL(P.F_FARMER, 0) AS FarmerTypeId, -- Αγρότης i
		public bool AbroatResident { get; set; } // CAST(CASE WHEN PARTY_DT.F_CHK_FOREIGN_RESIDENT = 1 THEN 1 ELSE 0 END AS BIT) AS AbroatResident, -- Κάτοικος εξωτερικού b
		public string Bank { get; set; } // BANKS.DESCR Bank, -- Τράπεζα

		public int GenderTypeId { get; set; } // ISNULL(P.F_SEX, 0) AS GenderTypeId, -- Φύλο i
		public string Comments { get; set; } // PARTY_DT.F_NOTES AS Comments, -- Σημειώσεις
		public string Email { get; set; } // PARTY_DT.F_EMAIL AS Email, -- Email
		public string Iban { get; set; } // P.F_IBAN AS Iban, -- IBAN Gr

		// Στοιχεία μητρώου
		public int CategoryBookTypeId { get; set; } // ISNULL(PARTY_DT.F_BOOKS, 0) AS CategoryBookId, -- Κατηγορία βιβλίων i
        public string CategoryBookTypeName { get; set; }
        public int LegalFormTypeId { get; set; } // ISNULL(P.F_LEGAL_FORM, 0) AS LegalFormTypeId, -- Νομική μορφή i
		public int VatSystemTypeId { get; set; } // ISNULL(P.F_REGIME_VAT, 0) AS VatSystemTypeId, -- Καθεστώς ΦΠΑ i
		public string CommentRegistry { get; set; } // P.F_REGISTER_NOTES AS CommentRegistry, -- Σημειώσεις μητρώου

		public DateTime? StartingDate { get; set; } // P.F_FROMDATE AS StartingDate, -- Ημερ.Έναρξης εργασιών d?
		public DateTime? ExpiryDate { get; set; } // P.F_TODATE AS ExpiryDate, -- Ημερ.Λήξης εργασιών d?
		public bool IntraTrade { get; set; } // CAST(P.F_INTRA_TRADE AS BIT) AS IntraTrade, -- Ενδοκοινωτικές συναλλαγές b
		public bool SeparateDeclaration { get; set; } // CAST(ISNULL(PARTY_DT.F_SEPARATESTATEMENT, 0) AS BIT) AS SeparateDeclaration, -- Γνωστοποίηση χωριστής δήλωσης b

		// Στοιχεία νομίμου εκπροσώπου - λογιστή
		public string RepresentativeName { get; set; } // THIRDS.SURNAME_A RepresentativeName, -- Νόμιμος Εκπροσώπος
		public string RepresentativeVat { get; set; } // THIRDS.AFM RepresentativeVat, -- ΑΦΜ Νομίμου Εκπροσώπου
		public string AccountantName { get; set; } // ACCOUNTERS.LNAME AccountantName, -- Λογιστής
		public string AccountantVat { get; set; } // ACCOUNTERS.AFM AccountantVat, -- ΑΦΜ Λογιστή
		public int? SpowseId { get; set; } // P.F_REL AS SpowseId, -- Σύζυγος i?
		public string SpowseName { get; set; } // R.F_SURNAME_A SpowseName, -- Σύζυγος

		// Στοιχεία σύνδεσης
		public string TaxisUserName { get; set; } // P.TAXIS_USERNAME AS TaxisUserName, -- Όνομα χρήστη(Τάξις)
		public string TaxisPassword { get; set; } // P.TAXIS_PASSWORD AS TaxisPassword, -- Κωδικός πρόσβασης(Τάξις)
		public string TaxisKeyNumber { get; set; } // P.F_KLEIDARITHMOS AS TaxisKeyNumber, -- Κλειδάριμος Τάξις
		public bool SubmissionAuthorizedForms { get; set; } // CAST(P.F_OLDWAY_INTERNET AS BIT) AS SubmissionAuthorizedForms, -- Υποβολή εντύπων με εξουσιοδότηση λογιστή b

		// Στοιχεία σύνδεσης
		public string IntrastatUserName { get; set; } // P.F_INTRASTAT_USRNAME AS IntrastatUserName, -- Όνομα χρήστη(Intrastat)
		public string IntrastatPassword { get; set; } // P.F_INTRASTAT_PWRD AS IntrastatPassword, -- Κωδικός πρόσβασης(Intrastat)
		public string EstateUserName { get; set; } // P.F_ESTATE_USRNAME AS EstateUserName, -- Όνομα χρήστη(Κτηματολόγιο)
		public string EstatePassword { get; set; } // P.F_ESTATE_PWRD AS EstatePassword, -- Κωδικός πρόσβασης(Κτηματολόγιο)
		public string KeaoUserName { get; set; } // P.F_KEAO_USERNAME AS KeaoUserName, -- Όνομα χρήστη(ΚΕΑΟ)
		public string KeaoPassword { get; set; } // P.F_KEAO_PSWD AS KeaoPassword, -- Κωδικός πρόσβασης(ΚΕΑΟ)
		public string OaeeUserName { get; set; } // P.F_OAEE_USRNAME AS OaeeUserName, -- Όνομα χρήστη(ΟΑΕΕ)
		public string OaeePassword { get; set; } // P.F_OAEE_PWRD AS OaeePassword, -- Κωδικός πρόσβασης(ΟΑΕΕ)
		public string OaeeKeynumber { get; set; } // P.F_OAEE_KEYNUMBER AS OaeeKeynumber, -- Κλειδάριμος ΟΑΕΕ
		public string EmployerIkaUserName { get; set; } // P.F_USERIKAERGODOTI AS EmployerIkaUserName, -- Όνομα χρήστη(ΙΚΑ Εργοδότη)
		public string EmployerIkaPassword { get; set; } // P.F_PASSIKAERGODOTI AS EmployerIkaPassword, -- Κωδικός πρόσβασης(ΙΚΑ Εργοδότη)
		public string EmployeeIkaUserName { get; set; } // P.F_USERIKAASFALISMENOU AS EmployeeIkaUserName, -- Όνομα χρήστη(ΙΚΑ Ασφαλισμένου)
		public string EmployeeIkaPassword { get; set; } // P.F_PASSIKAASFALISMENOU AS EmployeeIkaPassword, -- Κωδικός πρόσβασης(ΙΚΑ Ασφαλισμένου)
		public string IkaKeyNumber { get; set; } // P.F_KLEIDARITHMOS_IKA AS IkaKeyNumber, -- Κλειδάριθμος ΙΚΑ
		public string OaedUserName { get; set; } // P.F_OAED_USERNAME AS OaedUserName, -- Όνομα χρήστη(ΟΑΕΔ)
		public string OaedPassword { get; set; } // P.F_OAED_PSWD AS OaedPassword, -- Κωδικός πρόσβασης(ΟΑΕΔ)
		public string OaedKeyNumber { get; set; } // P.F_KLEIDARITHMOS_OAED AS OaedKeyNumber, -- Κλειδάριθμος ΟΑΕΔ
		public string GemhUserName { get; set; } // P.F_GEMH_USERNAME AS GemhUserName, -- Όνομα χρήστη(ΓΕΜΗ)
		public string GemhPassword { get; set; } // P.F_GEMH_PWRD AS GemhPassword, -- Κωδικός πρόσβασης(ΓΕΜΗ)
		public string ErmisUserName { get; set; } // P.F_ERMIS_USERNAME AS ErmisUserName, -- Όνομα χρήστη(Ερμής)
		public string ErmisPassword { get; set; } // P.F_ERMIS_PASSWORD AS ErmisPassword, -- Κωδικός πρόσβασης(Ερμής)
		public string OpsydUserName { get; set; } // P.F_OPSYD_USERNAME AS OpsydUserName, -- Όνομα χρήστη(ΟΠΣΥΔ)
		public string OpsydPassword { get; set; } // P.F_OPSYD_PASSWORD AS OpsydPassword, -- Κωδικός πρόσβασης(ΟΠΣΥΔ)
		public string SepeUserName { get; set; } // P.F_SEPE_USERNAME AS SepeUserName, -- Όνομα χρήστη(ΣΕΠΕ)
		public string SepePassword { get; set; } // P.F_SEPE_PASSWORD AS SepePassword, -- Κωδικός πρόσβασης(ΣΕΠΕ)

		public string OeeUserName { get; set; } // P.F_OEE_USERNAME AS OeeUserName, -- Όνομα χρήστη(ΟΕΕ)
		public string OeePassword { get; set; } // P.F_OEE_PASSWORD AS OeePassword, -- Κωδικός πρόσβασης(ΟΕΕ)
		public string OpekepeUserName { get; set; } // P.F_OPEKEPE_USERNAME AS OpekepeUserName, -- Όνομα χρήστη(ΟΠΕΚΕΠΕ)
		public string OpekepePassword { get; set; } // P.F_OPEKEPE_PASSWORD AS OpekepePassword, -- Κωδικός πρόσβασης(ΟΠΕΚΕΠΕ)
		public string AgrotiUserName { get; set; } // P.F_AGROTI_USER AS AgrotiUserName, -- Όνομα χρήστη(ΑΓΡΟΤΙ)
		public string AgrotiPassword { get; set; } // P.F_AGROTI_PASS AS AgrotiPassword, -- Κωδικός πρόσβασης(ΑΓΡΟΤΙ)
		public string TepahUserName { get; set; } // P.F_TEPAHNAME AS TepahUserName, -- Όνομα χρήστη(ΤΕΠΑΗ)
		public string TepahPassword { get; set; } // P.F_TEPAHPWRD AS TepahPassword, -- Κωδικός πρόσβασης(ΤΕΠΑΗ)
		public string NatUserName { get; set; } // P.F_ΝΑΤΝΑΜΕ AS NatUserName, -- Όνομα χρήστη(ΝΑΤ)
		public string NatPassword { get; set; } // P.F_ΝΑΤPWRD AS NatPassword, -- Κωδικός πρόσβασης(ΝΑΤ)

		public string DipetheUserName { get; set; } // P.DIPETHE_USERNAME AS DipetheUserName, -- Όνομα χρήστη(ΔΙΠΕΘΕ)
		public string DipethePassword { get; set; } // P.DIPETHE_PASSWORD AS DipethePassword, -- Κωδικός πρόσβασης(ΔΙΠΕΘΕ)
		public string ModUserName { get; set; } // P.MOD_USERNAME AS ModUserName, -- Όνομα χρήστη(Συστ.Κρατικών Ενισχύσεων)
		public string ModPassword { get; set; } // P.MOD_PASSWORD AS ModPassword, -- Κωδικός πρόσβασης(Συστ.Κρατικών ενισχύσεων)

		public string SmokePrdUserName { get; set; } // P.F_SMOKEPRD_USERNAME AS SmokePrdUserName, -- Όνομα χρήστη(Ιχνηλ.Κρατικών προιόντων)
		public string SmokePrdPassword { get; set; } // P.F_SMOKEPRD_PWRD AS SmokePrdPassword, -- Κωδικός πρόσβασης(Ιχνηλ.Κρατικών προιόντων)
		public string Article39UserName { get; set; } // P.F_39A_USERNAME AS Article39UserName, -- Όνομα χρήστη(Εφαρμογή άρθρου 39α)
		public string Article39Password { get; set; } // P.F_39A_PASSWORD AS Article39Password, -- Κωδικός πρόσβασης(Εφαρμογή άρθρου 39α)

		public string MhdasoUserName { get; set; } // P.MHDASO_USERNAME AS MhdasoUserName, -- Όνομα χρήστη(ΜΗΔΑΣΟ)
		public string MhdasoPassword { get; set; } // P.MHDASO_PASSWORD AS MhdasoPassword, -- Κωδικός πρόσβασης(ΜΗΔΑΣΟ)
		public string MydataUserName { get; set; } // P.MYDATA_USERNAME AS MydataUserName, -- Όνομα χρήστη(MyData)
		public string MydataPaswword { get; set; } // P.MYDATA_PASSWORD AS MydataPaswword, -- Κωδικός πρόσβασης(MyData)
		public string MydataApi { get; set; } // P.MYDATA_API AS MydataApi -- Mydata Api

        public bool Active { get; set; }
        public int TaxesFee { get; set; }
        public int AccountingSchema { get; set; }
        public int EmployerBreakLimit { get; set; }
    }

	public class TaxSystemKeaoGredentialItem
    {
        public int TaxSystemId { get; set; }
        public int CarrierId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
