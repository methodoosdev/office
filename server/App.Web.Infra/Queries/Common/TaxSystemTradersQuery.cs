namespace App.Web.Infra.Queries.Common
{
    public static partial class TaxSystemTradersQuery
    {
        public static string Party => @"
                    SELECT        
                        P.ID AS TaxSystemId, P.F_AFM AS Vat, P.F_SURNAME_A AS LastName, P.F_NAME AS FirstName, P.F_CODE AS CodeName, P.F_SURNAME_B AS LastName2, P.F_FATHERS_SURNAME AS FatherLastName, 
                        P.F_FATHERS_NAME AS FatherFirstName, P.F_MOTHERS_SURNAME AS MotherLastName, P.F_MOTHERS_NAME AS MotherFirstname, P.F_ID_TYPE AS IdentityTypeId, ID_TYPES.DESCR AS IdentityTypeName, 
                        P.F_NUMBER_ID AS IdentityNumber, P.F_DATE_ID AS IdentityDate, P.F_DEPARTMENT_ID AS IdentityDepartment, P.F_AMKA AS Amka, P.F_GEMH AS Gemh, P.F_AR_ASF AS AmIka, P.F_AMOAEE AS AmOaee, 
                        P.F_AMOGA AS AmOga, P.F_AM_ETAA AS AmEtaa, P.F_AR_DIAS AS AmDiasIka, P.F_AR_ETEA AS AmDiasEtea, P.F_AME AS AmEmployer, P.F_SINTAKSIS AS AmRetirement, P.F_AMS_OGA AS AmsOga, P.F_AMS_NAT AS AmsNat, 
                        P.F_EAM AS Eam, P.F_TITLE AS TradeName, P.F_JOB AS ProfessionalActivity, LTRIM(STR(P.F_DOY, 10)) + ' - ' + DOY.DESCR AS Doy, P.F_LOCAL_OFFICE AS LocalOffice, P.F_FORDER_NUMBER AS EnvelopeNumber, 
                        P.F_BIRTHDAY AS Birthday, P.F_BIRTH_PLACE AS BirthPlace, P.F_POP_REGISTER AS MunicipalNumber, P.F_DTMARRIAGE AS MarriedDate, P.F_ACTIVATED AS ActivatedTypeId, P.F_DEATH_DATE AS DeathDate, 
                        P.F_KEDM AS RegisterCode, P.F_JOB_ADDRESS AS JobAddress, P.F_JOB_NUMBER AS JobStreatNumber, P.F_JOB_CITY AS JobCity, P.F_JOB_MUNICIPALITY AS JobMunicipality, P.F_JOB_AREA AS JobPlace, 
                        P.F_JOB_POSTCODE AS JobPostcode, P.F_JOB_PHONE1 AS JobPhoneNumber1, P.F_JOB_PHONE2 AS JobPhoneNumber2, P.F_JOB_FAX AS JobFax, P.F_HOME_ADDRESS AS HomeAddress, 
                        P.F_HOME_NUMBER AS HomeStreatNumber, P.F_HOME_CITY AS HomeCity, P.F_HOME_MUNICIPALITY AS HomeMunicipality, P.F_HOME_AREA AS HomePlace, P.F_HOME_POSTCODE AS HomePostcode, 
                        P.F_HOME_PHONE1 AS HomePhoneNumber1, P.F_HOME_PHONE2 AS HomePhoneNumber2, P.F_HOME_CELLPHONE AS HomeCellphone, P.F_HOME_FAX AS HomeFax, PARTY_DT.F_DEADLINE AS DeadlineSubmition, 
                        PARTY_DT.F_STATUS AS StatusTypeId, P.F_TYPE AS CustomerTypeId, ISNULL(P.F_FARMER, 0) AS FarmerTypeId, CAST(CASE WHEN PARTY_DT.F_CHK_FOREIGN_RESIDENT = 1 THEN 1 ELSE 0 END AS BIT) 
                        AS AbroatResident, P.F_BANK + ' - ' + BANKS.DESCR AS BankName, ISNULL(P.F_SEX, 0) AS GenderTypeId, PARTY_DT.F_NOTES AS Comments, PARTY_DT.F_EMAIL AS Email, P.F_IBAN AS Iban, ISNULL(PARTY_DT.F_BOOKS, 
                        0) AS CategoryBookTypeId, ISNULL(P.F_LEGAL_FORM, 0) AS LegalFormTypeId, ISNULL(P.F_REGIME_VAT, 0) AS VatSystemTypeId, P.F_REGISTER_NOTES AS CommentRegistry, P.F_FROMDATE AS StartingDate, 
                        P.F_TODATE AS ExpiryDate, CAST(P.F_INTRA_TRADE AS BIT) AS IntraTrade, CAST(ISNULL(PARTY_DT.F_SEPARATESTATEMENT, 0) AS BIT) AS SeparateDeclaration, THIRDS.SURNAME_A AS RepresentativeName, 
                        THIRDS.AFM AS RepresentativeVat, ACCOUNTERS.LNAME AS AccountantName, ACCOUNTERS.AFM AS AccountantVat, P.F_REL AS SpowseId, R.F_SURNAME_A AS SpowseName, P.TAXIS_USERNAME AS TaxisUserName, 
                        P.TAXIS_PASSWORD AS TaxisPassword, P.F_KLEIDARITHMOS AS TaxisKeyNumber, CAST(P.F_OLDWAY_INTERNET AS BIT) AS SubmissionAuthorizedForms, P.F_INTRASTAT_USRNAME AS IntrastatUserName, 
                        P.F_INTRASTAT_PWRD AS IntrastatPassword, P.F_ESTATE_USRNAME AS EstateUserName, P.F_ESTATE_PWRD AS EstatePassword, P.F_KEAO_USERNAME AS KeaoUserName, P.F_KEAO_PSWD AS KeaoPassword, 
                        P.F_OAEE_USRNAME AS OaeeUserName, P.F_OAEE_PWRD AS OaeePassword, P.F_OAEE_KEYNUMBER AS OaeeKeynumber, P.F_USERIKAERGODOTI AS EmployerIkaUserName, 
                        P.F_PASSIKAERGODOTI AS EmployerIkaPassword, P.F_USERIKAASFALISMENOU AS EmployeeIkaUserName, P.F_PASSIKAASFALISMENOU AS EmployeeIkaPassword, P.F_KLEIDARITHMOS_IKA AS IkaKeyNumber, 
                        P.F_OAED_USERNAME AS OaedUserName, P.F_OAED_PSWD AS OaedPassword, P.F_KLEIDARITHMOS_OAED AS OaedKeyNumber, P.F_GEMH_USERNAME AS GemhUserName, P.F_GEMH_PWRD AS GemhPassword, 
                        P.F_ERMIS_USERNAME AS ErmisUserName, P.F_ERMIS_PASSWORD AS ErmisPassword, P.F_OPSYD_USERNAME AS OpsydUserName, P.F_OPSYD_PASSWORD AS OpsydPassword, 
                        P.F_SEPE_USERNAME AS SepeUserName, P.F_SEPE_PASSWORD AS SepePassword, P.F_OEE_USERNAME AS OeeUserName, P.F_OEE_PASSWORD AS OeePassword, P.F_OPEKEPE_USERNAME AS OpekepeUserName, 
                        P.F_OPEKEPE_PASSWORD AS OpekepePassword, P.F_AGROTI_USER AS AgrotiUserName, P.F_AGROTI_PASS AS AgrotiPassword, P.F_TEPAHNAME AS TepahUserName, P.F_TEPAHPWRD AS TepahPassword, 
                        P.F_ΝΑΤΝΑΜΕ AS NatUserName, P.F_ΝΑΤPWRD AS NatPassword, P.DIPETHE_USERNAME AS DipetheUserName, P.DIPETHE_PASSWORD AS DipethePassword, P.MOD_USERNAME AS ModUserName, 
                        P.MOD_PASSWORD AS ModPassword, P.F_SMOKEPRD_USERNAME AS SmokePrdUserName, P.F_SMOKEPRD_PWRD AS SmokePrdPassword, P.F_39A_USERNAME AS Article39UserName, 
                        P.F_39A_PASSWORD AS Article39Password, P.MHDASO_USERNAME AS MhdasoUserName, P.MHDASO_PASSWORD AS MhdasoPassword, P.MYDATA_USERNAME AS MydataUserName, 
                        P.MYDATA_PASSWORD AS MydataPaswword, P.MYDATA_API AS MydataApi,
                        650 AS TaxesFee, 500 AS AccountingSchema, 60 AS EmployerBreakLimit
                    FROM
                        PARTY AS P LEFT OUTER JOIN
                        PARTY AS R ON P.F_REL = R.ID INNER JOIN
                        PARTY_DT ON P.ID = PARTY_DT.PARTY_ID AND PARTY_DT.F_YEAR = @pYear LEFT OUTER JOIN
                        DOY ON P.F_DOY = DOY.ID LEFT OUTER JOIN
                        ID_TYPES ON P.F_ID_TYPE = ID_TYPES.ID LEFT OUTER JOIN
                        BANKS ON P.F_BANK = BANKS.ID LEFT OUTER JOIN
                        ACCOUNTERS ON P.F_ID_ACCOUNTER = ACCOUNTERS.ID LEFT OUTER JOIN
                        THIRDS ON P.F_ID_REPRESENTATIVE = THIRDS.ID
                    WHERE
                        P.F_ACTIVATED = 0
                    --    P.F_AFM = @pVat
                ";

        public static string KeaoGredentials => @"
                    SELECT PARTY_ID AS TaxSystemId
                          ,CARRIER_ID AS CarrierId
                          ,USERNAME AS UserName
                          ,[PASSWORD] AS [Password]
                    FROM PARTY_KEAO_CREDENTIALS
                    WHERE CARRIER_ID IN (2 ,3, 39)
                ";
    }
}
