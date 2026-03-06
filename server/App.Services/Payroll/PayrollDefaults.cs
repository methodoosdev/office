namespace App.Services.Payroll
{
    public static partial class PayrollDefaults
    {
        public static string PersonTermExpiryQuery => @"
            SELECT 
				X.CompanyId, 
				X.Vat,
				'Χωρίς υπεύθυνο' AS Email,
				X.CompanyName,
				X.BranchCode, 
				X.BranchName,
				X.PersonName, 
				X.PersonType, 
				X.ExpirationDate,
				CASE 
					WHEN X.ExpirationDate < CAST(GETDATE() AS DATE) THEN 1
					WHEN X.ExpirationDate >= CAST(GETDATE() AS DATE) AND X.ExpirationDate <= DATEADD(month, 2, CAST(GETDATE() AS DATE)) THEN 2
					ELSE 0
				END AS DateStatusId,
	            CASE 
                    WHEN X.ExpirationDate < CAST(GETDATE() AS DATE) THEN 'Έληξε'
                    WHEN X.ExpirationDate >= CAST(GETDATE() AS DATE) AND X.ExpirationDate <= DATEADD(month, 2, CAST(GETDATE() AS DATE)) THEN 'Προς λήξη'
                    ELSE 'Έγκυρη σύμβαση'
                END AS DateStatus,
	            CASE 
                    WHEN ExpirationDate < CAST(GETDATE() AS DATE) THEN 
                        CONCAT('Έληξε πριν (', DATEDIFF(day, ExpirationDate, GETDATE()), ' ημέρες)')
                    WHEN ExpirationDate >= CAST(GETDATE() AS DATE) AND ExpirationDate <= DATEADD(month, 2, CAST(GETDATE() AS DATE)) THEN 
                        CONCAT('Προς λήξη μετά από (', DATEDIFF(day, GETDATE(), ExpirationDate), ' ημέρες)')
                    ELSE 
                        'Έγκυρη σύμβαση'
                END AS DayStatus
            FROM
	            (SELECT 
		            Y.ID_CMP AS CompanyId, 
					FORMAT(C.VAT, 'D9') AS Vat,
		            C.NAME AS CompanyName,
		            Y.COD_YPOKAT AS BranchCode, 
		            Y.DESCR AS BranchName, 
		            CONCAT(P.SURNAME, ' ', P.NAME) AS PersonName, 
		            CASE
			            WHEN Y.PER_TECH1 = P.ID_PERSON THEN 'Τεχνικός ασφαλείας 1'
			            WHEN Y.PER_TECH2 = P.ID_PERSON THEN 'Τεχνικός ασφαλείας 2'
			            WHEN Y.PER_DCTR1 = P.ID_PERSON THEN 'Ιατρός εργασίας 1'
			            WHEN Y.PER_DCTR2 = P.ID_PERSON THEN 'Ιατρός εργασίας 2'
			            END AS PersonType, 
		            CASE
			            WHEN Y.PER_TECH1 = P.ID_PERSON THEN Y.DATE_EFF_TECH1
			            WHEN Y.PER_TECH2 = P.ID_PERSON THEN Y.DATE_EFF_TECH2
			            WHEN Y.PER_DCTR1 = P.ID_PERSON THEN Y.DATE_EFF_DOC1
			            WHEN Y.PER_DCTR2 = P.ID_PERSON THEN Y.DATE_EFF_DOC2
			            END AS ExpirationDate
	            FROM
		            CMP_YPOKAT AS Y INNER JOIN 
		            PERSON AS P ON( Y.PER_TECH1 = P.ID_PERSON OR Y.PER_TECH2 = P.ID_PERSON OR Y.PER_DCTR1 = P.ID_PERSON OR Y.PER_DCTR2 = P.ID_PERSON) INNER JOIN 
		            CMP AS C ON C.ID_CMP = Y.ID_CMP
	            WHERE C.ISACTIVE = 1 AND Y.IS_ACTIVE = 1) AS X
            WHERE X.ExpirationDate IS NOT NULL
            ORDER BY X.ExpirationDate
        ";
    }
}