namespace App.Web.Infra.Queries.Accounting
{
    public static class VatCalculationQuery
    {
        public static string SoftOne_C => @"
            SELECT B.CODE AS AccountingCode, 
                   A.PERIOD AS Period, 
                   CAST(ISNULL(A.CREDIT,0) AS DECIMAL(18,2)) AS Debit, 
                   CAST(ISNULL(A.DEBIT,0) AS DECIMAL(18,2)) AS Credit
            FROM ACNHEADER AS X INNER JOIN
                 ACNTRN AS A ON X.ACNEDIT = A.ACNEDIT AND X.ACNEDIT = A.ACNEDIT AND X.ACNHEADER = A.ACNHEADER INNER JOIN
                 ACNT B ON A.ACNT = B.ACNT
            WHERE X.COMPANY = @pCompanyId
                AND X.SODTYPE = 89
                AND B.CODE LIKE '54.02%'
                AND B.CODE NOT IN ('54.02.03.0000.0000')
                AND X.TRNDATE >= @pStartingDate
                AND X.TRNDATE <= @pEndingDate
                AND (X.FPRMS <> 5499) 
                AND (X.SOSOURCE = 1089)
                AND (B.ACNSCHEMA = @pSchema)
            ORDER BY B.CODE
        ";

        public static string SoftOne_B => @"
            SELECT C.CODE AS AccountingCode, A.PERIOD AS Period, SUM(A.IMPVAL - A.EXPVAL) AS Debit, 0 AS Credit
            FROM (
	            (SELECT X.ID, A.COMPANY, A.SODTYPE, A.MTRL, A.CODE, A.NAME, A.MTRTYPE, A.MTRTYPE1 
		            FROM (
			            SELECT A.MTRL AS ID 
			            FROM MTRL A 
			            WHERE A.COMPANY = @pCompanyId AND A.SODTYPE = 61 AND A.CODE LIKE '54.02%') AS X JOIN
				            MTRL A ON X.ID = A.MTRL)) AS C JOIN 
				            MTRBALSHEET AS A INNER JOIN 
				            MTRL B ON A.MTRL=B.MTRL ON C.ID=A.MTRL 
            WHERE A.COMPANY = @pCompanyId 
	            AND LEN(C.CODE) = 18
	            AND A.FISCPRD = @pYear 
	            AND B.MTRTYPE IN (1,2,3,4,5,6) 
	            AND (B.MTRMODEL IS NULL OR B.MTRMODEL = 0) 
            GROUP BY C.CODE, A.PERIOD
        ";

        public static string Prosvasis => @"
            SELECT C.CODE AS AccountingCode, A.PERIOD AS Period, SUM(A.IMPVAL - A.EXPVAL) AS Debit, 0 AS Credit
            FROM (
	            (SELECT X.ID, A.COMPANY, A.SODTYPE, A.MTRL, A.CODE, A.NAME, A.MTRTYPE, A.MTRTYPE1 
		            FROM (
			            SELECT A.MTRL AS ID 
			            FROM MTRL A 
			            WHERE A.COMPANY = @pCompanyId AND A.SODTYPE = 61 AND A.CODE LIKE '54.02%') AS X JOIN
				            MTRL A ON X.ID = A.MTRL)) AS C JOIN 
				            MTRBALSHEET AS A INNER JOIN 
				            MTRL B ON A.MTRL=B.MTRL ON C.ID=A.MTRL 
            WHERE A.COMPANY = @pCompanyId 
	            AND LEN(C.CODE) = 18
	            AND A.FISCPRD = @pYear 
	            AND B.MTRTYPE IN (1,2,3,4,5,6) 
	            AND (B.MTRMODEL IS NULL OR B.MTRMODEL = 0) 
            GROUP BY C.CODE, A.PERIOD
        ";

        public static string FiscalPeriod => @"
			SELECT	
				A.ACNSCHEMA
			FROM 
				FISCPRD A 
			WHERE 
				A.COMPANY = @pCompanyId
                AND A.FISCPRD = @pYear
            ";

        public static string Periodoi => @"
            SELECT 
	            A.FISCPRD AS [Year]
	            ,A.PERIOD AS [Period]
	            ,A.FROMDATE AS FromDate
	            ,A.FINALDATE AS ToDate
            FROM PERIOD A 
            WHERE 
	            A.COMPANY = @pCompanyId 
                AND A.FISCPRD = @pYear
            ORDER 
	            BY A.PERIOD        
        ";

        public static string Refund_SoftOne_C => @"
            SELECT B.CODE AS AccountingCode, 
                   A.PERIOD AS Period, 
                   CAST(A.CREDIT AS DECIMAL(18,2)) AS Debit, 
                   CAST(A.DEBIT AS DECIMAL(18,2)) AS Credit
            FROM ACNHEADER AS X INNER JOIN
                 ACNTRN AS A ON X.ACNEDIT = A.ACNEDIT AND X.ACNEDIT = A.ACNEDIT AND X.ACNHEADER = A.ACNHEADER INNER JOIN
                 ACNT B ON A.ACNT = B.ACNT
            WHERE X.COMPANY = @pCompanyId
                AND X.SODTYPE = 89
                AND (B.CODE LIKE ('54.02.03.____.99%')  )
                AND (X.FPRMS = 5499) 
                AND (X.SOSOURCE = 1089)
                --AND (A.FISCPRD = @pYear)
                AND X.TRNDATE >= @pStartingDate
                AND X.TRNDATE <= @pEndingDate
                AND (B.ACNSCHEMA = @pSchema)
            ORDER BY B.CODE
        ";

    }
}
