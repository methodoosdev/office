using App.Core.Domain.Traders;

namespace App.Web.Infra.Queries.Accounting
{
    public class MonthlyBCategoryBulletinPdfQuery : TraderFactoryQuery
    {
        public MonthlyBCategoryBulletinPdfQuery(int logistikiProgramTypeId, int categoryBookTypeId) : base(logistikiProgramTypeId, categoryBookTypeId)
        {
        }

        public override string SoftOne_C => "";

        public override string SoftOne_B => @"
			SELECT  C.CODE AS Code,
					A.PERIOD AS Periodos,
					CAST(SUM(A.IMPVAL - A.EXPVAL) AS DECIMAL(18,2)) AS Value,

					CASE
						WHEN C.CODE LIKE '70.01%' THEN 'Πωλήσεις εμπορευμάτων'
						WHEN C.CODE LIKE '70.03%' THEN 'Πωλήσεις προϊόντων'
						WHEN C.CODE LIKE '70.05%' THEN 'Πωλήσεις λοιπών αποθ/ων'
						WHEN C.CODE LIKE '70.07%' THEN 'Πωλήσεις υπηρεσιών'
						WHEN C.CODE LIKE '71%' THEN 'Λοιπά συνήθη έσοδα'
						WHEN C.CODE LIKE '72%' THEN 'Πιστωτικοί τόκοι & συναφή έσοδα'
						WHEN C.CODE LIKE '73%' THEN 'Πιστωτικές συναλ/τικές διαφορές '
						WHEN C.CODE LIKE '75%' THEN 'Κέρδη από πώληση παγίων'
						WHEN C.CODE LIKE '79%' THEN 'Ασυνήθη έσοδα & κέρδη'
	
						WHEN C.CODE LIKE '20.02%' THEN 'Αγορές εμπορευμάτων'
						WHEN C.CODE LIKE '24.02%' THEN 'Αγορές Α-Β υλών'
						WHEN C.CODE LIKE '25.02%' THEN 'Αγορές αναλωσίμων'
						WHEN C.CODE LIKE '26.02%' THEN 'Αγορές ανταλλακτικών παγίων'
						WHEN C.CODE LIKE '27.02%' THEN 'Αγορές λοιπών αποθ/ων'
	
						WHEN C.CODE LIKE '60%' THEN 'Παροχές σε εργαζόμενους'
						WHEN C.CODE LIKE '62%' THEN 'Χρεωστικές συναλ/τικές διαφορές'
						WHEN C.CODE LIKE '63%' THEN 'Ζημίες από πώληση παγίων'
						WHEN C.CODE LIKE '64.01%' THEN 'Αμοιβές για υπηρεσίες'
						WHEN C.CODE LIKE '64.02%' THEN 'Ενέργεια'
						WHEN C.CODE LIKE '64.03%' THEN 'Υδρευση'
						WHEN C.CODE LIKE '64.04%' THEN 'Τηλεπικοινωνίες'
						WHEN C.CODE LIKE '64.05%' THEN 'Ενοίκια'
						WHEN C.CODE LIKE '64.06%' THEN 'Ασφάλιστρα'
						WHEN C.CODE LIKE '64.07%' THEN 'Μεταφορικά έξοδα'
						WHEN C.CODE LIKE '64.08%' THEN 'Αναλώσιμα υλικά'
						WHEN C.CODE LIKE '64.09%' THEN 'Επισκευές & συντηρήσεις'
						WHEN C.CODE LIKE '64.10%' THEN 'Διαφήμιση & προβολή'
						WHEN C.CODE LIKE '64.11%' THEN 'Φόροι & τέλη'
						WHEN C.CODE LIKE '64.12%' THEN 'Λοιπά έξοδα'
						WHEN C.CODE LIKE '80%' THEN 'Εξοδα σε ιδιοπαραγωγή'

						ELSE B.NAME
					END AS Type

			FROM (
				(SELECT  X.ID,A.COMPANY,A.SODTYPE,A.MTRL,A.CODE,A.NAME,A.MTRTYPE,A.MTRTYPE1 
				FROM (

					SELECT A.MTRL as ID 
					FROM MTRL A 
					WHERE A.COMPANY= @pCompanyId AND A.SODTYPE=61) X JOIN MTRL A  ON X.ID=A.MTRL)) AS C JOIN MTRBALSHEET A	INNER JOIN MTRL B ON A.MTRL=B.MTRL ON C.ID=A.MTRL 

				WHERE A.COMPANY = @pCompanyId 
					AND ((SUBSTRING(C.CODE, 1, 1) IN ('7','2','6')) OR C.CODE LIKE '80.03.00.0__0.%')
					AND LEN(C.CODE) = 18
					AND A.FISCPRD = @pYear 
				    AND A.PERIOD >= 0
				    AND A.PERIOD <= @pPeriod
					AND B.MTRTYPE IN (1,2,3,4,5,6) AND (B.MTRMODEL IS NULL OR B.MTRMODEL = 0)

			GROUP BY C.CODE, A.PERIOD, B.MTRTYPE, B.NAME, A.FISCPRD 
        ";

        public override string Prosvasis => @"
			SELECT  C.CODE AS Code,
					A.PERIOD AS Periodos,
					CAST(SUM(A.IMPVAL - A.EXPVAL) AS DECIMAL(18,2)) AS Value,

					CASE
						WHEN C.CODE LIKE '70.01%' THEN 'Πωλήσεις εμπορευμάτων'
						WHEN C.CODE LIKE '70.03%' THEN 'Πωλήσεις προϊόντων'
						WHEN C.CODE LIKE '70.05%' THEN 'Πωλήσεις λοιπών αποθ/ων'
						WHEN C.CODE LIKE '70.07%' THEN 'Πωλήσεις υπηρεσιών'
						WHEN C.CODE LIKE '71%' THEN 'Λοιπά συνήθη έσοδα'
						WHEN C.CODE LIKE '72%' THEN 'Πιστωτικοί τόκοι & συναφή έσοδα'
						WHEN C.CODE LIKE '73%' THEN 'Πιστωτικές συναλ/τικές διαφορές '
						WHEN C.CODE LIKE '75%' THEN 'Κέρδη από πώληση παγίων'
						WHEN C.CODE LIKE '79%' THEN 'Ασυνήθη έσοδα & κέρδη'
	
						WHEN C.CODE LIKE '20.02%' THEN 'Αγορές εμπορευμάτων'
						WHEN C.CODE LIKE '24.02%' THEN 'Αγορές Α-Β υλών'
						WHEN C.CODE LIKE '25.02%' THEN 'Αγορές αναλωσίμων'
						WHEN C.CODE LIKE '26.02%' THEN 'Αγορές ανταλλακτικών παγίων'
						WHEN C.CODE LIKE '27.02%' THEN 'Αγορές λοιπών αποθ/ων'
	
						WHEN C.CODE LIKE '60%' THEN 'Παροχές σε εργαζόμενους'
						WHEN C.CODE LIKE '62%' THEN 'Χρεωστικές συναλ/τικές διαφορές'
						WHEN C.CODE LIKE '63%' THEN 'Ζημίες από πώληση παγίων'
						WHEN C.CODE LIKE '64.01%' THEN 'Αμοιβές για υπηρεσίες'
						WHEN C.CODE LIKE '64.02%' THEN 'Ενέργεια'
						WHEN C.CODE LIKE '64.03%' THEN 'Υδρευση'
						WHEN C.CODE LIKE '64.04%' THEN 'Τηλεπικοινωνίες'
						WHEN C.CODE LIKE '64.05%' THEN 'Ενοίκια'
						WHEN C.CODE LIKE '64.06%' THEN 'Ασφάλσιτρα'
						WHEN C.CODE LIKE '64.07%' THEN 'Μεταφορικά έξοδα'
						WHEN C.CODE LIKE '64.08%' THEN 'Αναλώσιμα υλικά'
						WHEN C.CODE LIKE '64.09%' THEN 'Επισκευές & συντηρήσεις'
						WHEN C.CODE LIKE '64.10%' THEN 'Διαφήμιση & προβολή'
						WHEN C.CODE LIKE '64.11%' THEN 'Φόροι & τέλη'
						WHEN C.CODE LIKE '64.12%' THEN 'Λοιπά έξοδα'
						WHEN C.CODE LIKE '80%' THEN 'Εξοδα σε ιδιοπαραγωγή'

						ELSE B.NAME
					END AS Type

			FROM (
				(SELECT  X.ID,A.COMPANY,A.SODTYPE,A.MTRL,A.CODE,A.NAME,A.MTRTYPE,A.MTRTYPE1 
				FROM (

					SELECT A.MTRL as ID 
					FROM MTRL A 
					WHERE A.COMPANY= @pCompanyId AND A.SODTYPE=61) X JOIN MTRL A  ON X.ID=A.MTRL)) AS C JOIN MTRBALSHEET A	INNER JOIN MTRL B ON A.MTRL=B.MTRL ON C.ID=A.MTRL 

				WHERE A.COMPANY = @pCompanyId 
					AND ((SUBSTRING(C.CODE, 1, 1) IN ('7','2','6')) OR C.CODE LIKE '80.03.00.0__0.%')
					AND LEN(C.CODE) = 18
					AND A.FISCPRD = @pYear 
				    AND A.PERIOD >= 0
				    AND A.PERIOD <= @pPeriod
					AND B.MTRTYPE IN (1,2,3,4,5,6) AND (B.MTRMODEL IS NULL OR B.MTRMODEL = 0)

			GROUP BY C.CODE, A.PERIOD, B.MTRTYPE, B.NAME, A.FISCPRD 
        ";

        public override string Prosvasis_C => @"
			SELECT  C.CODE AS Code,
					A.PERIOD AS Periodos,
					CAST(SUM(A.IMPVAL - A.EXPVAL) AS DECIMAL(18,2)) AS Value,

					CASE
						WHEN C.CODE LIKE '70.01%' THEN 'Πωλήσεις εμπορευμάτων'
						WHEN C.CODE LIKE '70.03%' THEN 'Πωλήσεις προϊόντων'
						WHEN C.CODE LIKE '70.05%' THEN 'Πωλήσεις λοιπών αποθ/ων'
						WHEN C.CODE LIKE '70.07%' THEN 'Πωλήσεις υπηρεσιών'
						WHEN C.CODE LIKE '71%' THEN 'Λοιπά συνήθη έσοδα'
						WHEN C.CODE LIKE '72%' THEN 'Πιστωτικοί τόκοι & συναφή έσοδα'
						WHEN C.CODE LIKE '73%' THEN 'Πιστωτικές συναλ/τικές διαφορές '
						WHEN C.CODE LIKE '75%' THEN 'Κέρδη από πώληση παγίων'
						WHEN C.CODE LIKE '79%' THEN 'Ασυνήθη έσοδα & κέρδη'
	
						WHEN C.CODE LIKE '20.02%' THEN 'Αγορές εμπορευμάτων'
						WHEN C.CODE LIKE '24.02%' THEN 'Αγορές Α-Β υλών'
						WHEN C.CODE LIKE '25.02%' THEN 'Αγορές αναλωσίμων'
						WHEN C.CODE LIKE '26.02%' THEN 'Αγορές ανταλλακτικών παγίων'
						WHEN C.CODE LIKE '27.02%' THEN 'Αγορές λοιπών αποθ/ων'
	
						WHEN C.CODE LIKE '60%' THEN 'Παροχές σε εργαζόμενους'
						WHEN C.CODE LIKE '62%' THEN 'Χρεωστικές συναλ/τικές διαφορές'
						WHEN C.CODE LIKE '63%' THEN 'Ζημίες από πώληση παγίων'
						WHEN C.CODE LIKE '64.01%' THEN 'Αμοιβές για υπηρεσίες'
						WHEN C.CODE LIKE '64.02%' THEN 'Ενέργεια'
						WHEN C.CODE LIKE '64.03%' THEN 'Υδρευση'
						WHEN C.CODE LIKE '64.04%' THEN 'Τηλεπικοινωνίες'
						WHEN C.CODE LIKE '64.05%' THEN 'Ενοίκια'
						WHEN C.CODE LIKE '64.06%' THEN 'Ασφάλσιτρα'
						WHEN C.CODE LIKE '64.07%' THEN 'Μεταφορικά έξοδα'
						WHEN C.CODE LIKE '64.08%' THEN 'Αναλώσιμα υλικά'
						WHEN C.CODE LIKE '64.09%' THEN 'Επισκευές & συντηρήσεις'
						WHEN C.CODE LIKE '64.10%' THEN 'Διαφήμιση & προβολή'
						WHEN C.CODE LIKE '64.11%' THEN 'Φόροι & τέλη'
						WHEN C.CODE LIKE '64.12%' THEN 'Λοιπά έξοδα'
						WHEN C.CODE LIKE '80%' THEN 'Εξοδα σε ιδιοπαραγωγή'

						ELSE B.NAME
					END AS Type

			FROM (
				(SELECT  X.ID,A.COMPANY,A.SODTYPE,A.MTRL,A.CODE,A.NAME,A.MTRTYPE,A.MTRTYPE1 
				FROM (

					SELECT A.MTRL as ID 
					FROM MTRL A 
					WHERE A.COMPANY= @pCompanyId AND A.SODTYPE=61) X JOIN MTRL A  ON X.ID=A.MTRL)) AS C JOIN MTRBALSHEET A	INNER JOIN MTRL B ON A.MTRL=B.MTRL ON C.ID=A.MTRL 

				WHERE A.COMPANY = @pCompanyId 
					AND ((SUBSTRING(C.CODE, 1, 1) IN ('7','2','6')) OR C.CODE LIKE '80.03.00.0__0.%')
					AND LEN(C.CODE) = 18
					AND A.FISCPRD = @pYear 
				    AND A.PERIOD >= 0
				    AND A.PERIOD <= @pPeriod
					AND B.MTRTYPE IN (1,2,3,4,5,6) AND (B.MTRMODEL IS NULL OR B.MTRMODEL = 0)

			GROUP BY C.CODE, A.PERIOD, B.MTRTYPE, B.NAME, A.FISCPRD 
        ";
    }
}
