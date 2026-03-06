namespace App.Web.Infra.Queries
{
    public static class SoftOneQuery
    {
        public static string CompanyPeriodUses => @"
			SELECT 
				A.FISCPRD AS [Year]
				,A.PERIOD AS [Period]
				,A.FROMDATE AS FromDate
				,A.FINALDATE AS ToDate
			FROM 
				PERIOD A 
			WHERE 
				A.COMPANY = @pCompanyId 
			ORDER 
				BY A.PERIOD
			";

        public static string CompanyYearUses => @"
			SELECT	
				A.FISCPRD AS [Year]
				,A.NAME AS [Name]
				,A.FROMDATE AS FromDate
				,A.FINALDATE AS ToDate
				,A.ACNSCHEMA AS [Schema]
			FROM 
				FISCPRD A 
			WHERE 
				A.COMPANY = @pCompanyId
            ";

        public static string AccountingCodesPerSchemaPerGrade => @"
			SELECT 
				T.CODE AS AccountingCode,
				T.NAME AS [Description],
				--(
				--	SELECT STRING_AGG(
				--				CASE 
				--					WHEN w.rn <= 6 AND LEN(w.Word) > 5 AND w.rn <> w.max_rn 
				--						THEN LEFT(w.Word, 5) + '.'
				--					ELSE w.Word
				--				END, ' '
				--			) WITHIN GROUP (ORDER BY w.rn)
				--	FROM (
				--		SELECT value AS Word,
				--				ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS rn,
				--				COUNT(*) OVER () AS max_rn
				--		FROM STRING_SPLIT(T.NAME, ' ')
				--	) w
				--) AS ProcessedName,
				T.ACNSCHEMA AS [Schema],
				T.ACNGRADE AS Grande
			FROM   
				ACNT AS T
				JOIN (
					SELECT ACNSCHEMA,
						   MAX(ACNGRADE) AS MAX_GRADE
					FROM   ACNT
					GROUP  BY ACNSCHEMA
				) AS M ON T.ACNSCHEMA = M.ACNSCHEMA AND T.ACNGRADE  = M.MAX_GRADE
			WHERE
				T.CODE LIKE @pCode
			ORDER BY 
				T.CODE, T.ACNSCHEMA;
            ";

		public static string AccountingCodesPerSchema => @"
			SELECT 
				T.CODE AS AccountingCode,
				T.NAME AS [Description],
				--(
				--	SELECT STRING_AGG(
				--				CASE 
				--					WHEN w.rn <= 6 AND LEN(w.Word) > 5 AND w.rn <> w.max_rn 
				--						THEN LEFT(w.Word, 5) + '.'
				--					ELSE w.Word
				--				END, ' '
				--			) WITHIN GROUP (ORDER BY w.rn)
				--	FROM (
				--		SELECT value AS Word,
				--				ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS rn,
				--				COUNT(*) OVER () AS max_rn
				--		FROM STRING_SPLIT(T.NAME, ' ')
				--	) w
				--) AS ProcessedName,
				T.ACNSCHEMA AS [Schema],
				T.ACNGRADE AS Grande
			FROM   
				ACNT AS T
				JOIN (
					SELECT TOP 1 ACNSCHEMA
					FROM   FISCPRD
					ORDER BY FISCPRD DESC
				) AS M ON T.ACNSCHEMA = M.ACNSCHEMA
			ORDER BY 
				T.CODE, T.ACNSCHEMA
			";

        public static string AggregateAnalysis => @"
			WITH BaseCTE AS (
				SELECT 
					A.ACNT AS ID,
					A.CODE
				FROM 
					ACNT A
				WHERE A.ACNSCHEMA = @pSchema
					AND A.CODE LIKE @pCode
					AND A.SODTYPE = 89
					AND A.IASACN IN (0,1)
				),
			NumberedCTE AS (
				SELECT 
					ROW_NUMBER() OVER (ORDER BY CODE) AS NO_, ID, CODE
				FROM BaseCTE
			)
			SELECT 
				A.TRNDATE AS Date,
				N.CODE AS AccountingCode,
				CAST(SUM(A.LDEBIT) AS DECIMAL(18,2)) AS Debit,
				CAST(SUM(A.LCREDIT) AS DECIMAL(18,2)) AS Credit
			FROM 
				NumberedCTE N JOIN (
					(ACNTRN A LEFT JOIN ACNEDIT B ON A.ACNEDIT = B.ACNEDIT) LEFT JOIN 
					ACNHEADER C ON A.ACNEDIT = C.ACNEDIT AND A.ACNHEADER = C.ACNHEADER
				) ON N.ID = A.ACNT
			WHERE 
				A.COMPANY = @pCompanyId
				AND A.TRNDATE >= @pFromDate
				AND A.TRNDATE <= @pToDate
				AND A.SODTYPE = 89
				AND A.ISCANCEL IN (0,2)
				AND A.ISGL IN (0,1)
				AND A.ISIAS = 1 
				AND A.APPRV = 1
				AND A.PERIOD <> 0
			GROUP BY 
				A.TRNDATE,N.CODE
			ORDER 
				BY A.TRNDATE,N.CODE
		";

        public static string AggregateAnalysisOpening => @"
			SELECT 
				N.CODE AS AccountingCode,
				CAST(SUM(A.LDEBITTMP) AS DECIMAL(18,2)) AS Debit,
				CAST(SUM(A.LCREDITTMP) AS DECIMAL(18,2)) AS Credit
			FROM 
				ACNT N INNER JOIN 
				ACNBALSHEET A ON N.ACNT = A.ACNT
			WHERE 
				N.ACNSCHEMA = @pSchema
				AND N.CODE LIKE @pCode
				AND A.COMPANY = @pCompanyId
				AND A.FISCPRD = @pYear
				AND N.SODTYPE = 89
				AND N.IASACN IN (0,1)
				AND A.PERIOD < 1
			GROUP BY 
				N.CODE
			ORDER BY
				N.CODE
		";

        public static string AggregateAnalysisPeriods => @"
			SELECT
				CAST(
					SUM(
						CASE
						WHEN LEFT(A.CODE, 2) = '08' OR LEFT(A.CODE, 1) IN ('4','5','7') THEN B.LCREDITTMP - B.LDEBITTMP
						ELSE B.LDEBITTMP - B.LCREDITTMP
						END
					) 
				AS DECIMAL(18,2)
				)
			FROM 
				ACNT A JOIN ACNBALSHEET B ON A.ACNT = B.ACNT
			WHERE
				A.ACNSCHEMA = @pSchema
				AND A.CODE LIKE @pCode
				AND B.COMPANY = @pCompanyId
				AND B.FISCPRD = @pYear
				AND A.SODTYPE = 89
				AND B.PERIOD BETWEEN @pFromPeriod AND @pToPeriod
		";

        public static string AggregateAnalysisV2 => @"
			WITH BaseCTE AS (
				SELECT 
					A.ACNT AS ID,
					A.CODE
				FROM 
					ACNT A
				WHERE A.ACNSCHEMA = @pSchema
					AND A.CODE LIKE @pCode
					AND A.SODTYPE = 89
					AND A.IASACN IN (0,1)
				),
			NumberedCTE AS (
				SELECT 
					ROW_NUMBER() OVER (ORDER BY CODE) AS NO_, ID, CODE
				FROM BaseCTE
			)
			SELECT 
				A.TRNDATE AS Date,
				N.CODE AS Id,
				CASE
					WHEN LEFT(N.CODE, 2) = '08' OR LEFT(N.CODE, 1) IN ('4', '5', '7') THEN CAST(SUM(A.LCREDIT - A.LDEBIT) AS DECIMAL(18,2))
				ELSE
					CAST(SUM(A.LDEBIT - A.LCREDIT) AS DECIMAL(18,2))
                END AS Total
			FROM 
				NumberedCTE N JOIN (
					(ACNTRN A LEFT JOIN ACNEDIT B ON A.ACNEDIT = B.ACNEDIT) LEFT JOIN 
					ACNHEADER C ON A.ACNEDIT = C.ACNEDIT AND A.ACNHEADER = C.ACNHEADER
				) ON N.ID = A.ACNT
			WHERE 
				A.COMPANY = @pCompanyId
				AND A.TRNDATE >= @pFromDate
				AND A.TRNDATE <= @pToDate
				AND A.SODTYPE = 89
				AND A.ISCANCEL IN (0,2)
				AND A.ISGL IN (0,1)
				AND A.ISIAS = 1 
				AND A.APPRV = 1
				AND A.PERIOD <> 0
			GROUP BY 
				A.TRNDATE,N.CODE
			ORDER 
				BY A.TRNDATE,N.CODE
		";

        public static string Receipts => @"
			SELECT 
				A.TRNDATE AS Date,				
				'Receipts' AS Id,
				CAST(T.LLINEVAL AS DECIMAL(18,2)) AS Total
			FROM 
				FINDOC A LEFT OUTER JOIN 
				TRDR B ON A.TRDR=B.TRDR LEFT OUTER JOIN 
				TRDFLINES T ON T.FINDOC = A.FINDOC
			WHERE 
				A.COMPANY = @pCompanyId 
				AND A.TRNDATE >= @pFromDate
				AND A.TRNDATE <= @pToDate
				AND (A.ISCANCEL IN (0,2)) 
				AND ((A.SOSOURCE = 1381) OR (A.SOSOURCE=1581)) 
				AND ((B.SODTYPE = 13) OR (B.SODTYPE = 15)) 
			ORDER BY 
				A.TRNDATE
		";
        public static string Payments => @"
			SELECT 
				A.TRNDATE AS Date,				
				'Payments' AS Id,
				CAST(T.LLINEVAL AS DECIMAL(18,2)) AS Total
			FROM 
				FINDOC A LEFT OUTER JOIN 
				TRDR B ON A.TRDR=B.TRDR LEFT OUTER JOIN 
				TRDFLINES T ON T.FINDOC = A.FINDOC
			WHERE 
				A.COMPANY = @pCompanyId 
				AND A.TRNDATE >= @pFromDate
				AND A.TRNDATE <= @pToDate
				AND (A.ISCANCEL IN (0,2)) 
				AND ((A.SOSOURCE = 1281) OR (A.SOSOURCE=1681))
				AND ((B.SODTYPE = 12) OR (B.SODTYPE = 16)) 

			ORDER BY 
				A.TRNDATE
		";

		public static string Orders => @"
			SELECT 
				A.TRNDATE AS Date,				
				'Orders' AS Id,
				CAST(A.NETAMNT AS DECIMAL(18,2)) AS Total
			FROM 
				FINDOC A LEFT OUTER JOIN TRDR B ON A.TRDR=B.TRDR 
			WHERE
				A.COMPANY = @pCompanyId 
				AND A.TRNDATE >= @pFromDate
				AND A.TRNDATE <= @pToDate
				AND A.SOSOURCE=1351 
				AND A.SOREDIR=0 
				AND A.TFPRMS=201 
				AND A.SODTYPE=13
			ORDER BY 
				A.TRNDATE
		";
        /*
			WHERE	
				A.COMPANY = @pCompanyId 
				AND A.TRNDATE >= @pFromDate
				AND A.TRNDATE <= @pToDate
				AND A.SOSOURCE = 1351 
				AND A.SODTYPE=13 
				AND (A.ISCANCEL IN (0,2)) 
				AND A.FPRMS IN (SELECT F.FPRMS FROM FPRMS F WHERE F.SOSOURCE=1351 AND F.TFPRMS=201) 
				AND A.FULLYTRANSF IN (0,1,2) 
				AND EXISTS(SELECT 1 FROM MTRLINES ML WHERE ML.FINDOC=A.FINDOC AND ML.PENDING IN (0,1,2,3))
		 */
    }
}
