namespace App.Web.Infra.Queries.Accounting
{
    public static partial class AggregateAnalysisQuery
    {
        public static string Get => @"
            SELECT 
                A.TRNDATE AS Date,
                D.CODE AS Id,
                CASE 
                    WHEN D.CODE LIKE '7%' OR D.CODE LIKE '08%' OR D.CODE LIKE '4%' OR D.CODE LIKE '5%' THEN SUM(ISNULL(A.CREDIT,0) - ISNULL(A.DEBIT,0))
                ELSE 
                    SUM(ISNULL(A.DEBIT,0) - ISNULL(A.CREDIT,0)) 
                END AS Total
            FROM ((ACNTRN A LEFT OUTER JOIN ACNEDIT B ON A.ACNEDIT=B.ACNEDIT) 
                LEFT OUTER JOIN ACNHEADER C ON A.ACNEDIT=C.ACNEDIT AND A.ACNHEADER=C.ACNHEADER) 
                LEFT OUTER JOIN ACNT D ON A.ACNT=D.ACNT 
            WHERE 
                A.COMPANY=@pCompanyId 
                AND A.SODTYPE=89 
                AND A.TRNDATE >= @pStartDate
                AND A.TRNDATE <= @pEndDate
                AND (A.ISCANCEL IN (0,2)) 
            GROUP BY A.TRNDATE, D.CODE
            ORDER BY A.TRNDATE
            ";
        public static string Rest => @"
            SELECT A.TRNDATE AS Date, D.CODE AS Id, 
                CASE 
                    WHEN LEFT(D.CODE,2) = '50' THEN SUM(A.CREDIT) - SUM(A.DEBIT)
                ELSE 
                    SUM(A.DEBIT) - SUM(A.CREDIT) 
                END AS Total
            FROM ((ACNTRN A LEFT OUTER JOIN ACNEDIT B ON A.ACNEDIT=B.ACNEDIT) 
                LEFT OUTER JOIN ACNHEADER C ON A.ACNEDIT=C.ACNEDIT AND A.ACNHEADER=C.ACNHEADER) 
                LEFT OUTER JOIN ACNT D ON A.ACNT=D.ACNT 
            WHERE A.COMPANY = @pCompanyId  
				AND LEFT(D.CODE,2) IN ('50','30')
                AND A.SODTYPE = 89 
                AND A.TRNDATE >= '20220101'
                AND A.TRNDATE < @pEndDate
                AND A.ISCANCEL IN (0,2) 
            GROUP BY A.TRNDATE, D.CODE
            ORDER BY A.TRNDATE
            ";
        public static string Payments => @"
			SELECT 
				A.TRNDATE AS Date
				, 'payments' AS Id
				, SUM(CASE 
					WHEN A.TFPRMS = 101 
					THEN  ABS(ISNULL(A.SUMAMNT,0))    -- force positive
					ELSE -ABS(ISNULL(A.SUMAMNT,0))    -- force negative
				END) AS Total
			FROM 
				FINDOC A LEFT OUTER JOIN TRDR B ON A.TRDR=B.TRDR 
			WHERE 
				A.COMPANY = @pCompanyId 
				AND A.TRNDATE >= @pStartDate
	            AND A.TRNDATE <= @pEndDate
				AND  (A.TFPRMS IN (101,102)) 
				AND (
						(A.SOSOURCE = 1412 AND A.SODTYPE = 14)
					OR (A.SOSOURCE = 1281 AND A.SODTYPE = 12 AND A.SOREDIR = 0)
				)
			GROUP BY A.TRNDATE
            ";
        public static string Receipts => @"
			SELECT 
				A.TRNDATE AS Date,
				'receipts' AS Id,
				SUM(
				  CASE 
					WHEN A.TFPRMS = 101 THEN  ABS(ISNULL(A.SUMAMNT,0))
					ELSE                 -ABS(ISNULL(A.SUMAMNT,0))
				  END
				) AS Total
			FROM 
				FINDOC A LEFT JOIN TRDR B ON A.TRDR = B.TRDR
			WHERE 
				A.TRNDATE >= @pStartDate
	            AND A.TRNDATE <= @pEndDate
				AND A.TFPRMS IN (101,102)
				AND (
					 (A.COMPANY = @pCompanyId AND A.SOSOURCE = 1413 AND A.SODTYPE = 14)
				  OR (A.SOSOURCE = 1381 AND A.SOREDIR = 0 AND A.SODTYPE = 13)
				)
			GROUP BY 
				A.TRNDATE
			ORDER BY
				A.TRNDATE DESC;
            ";
        public static string Orders => @"
            SELECT 
				A.TRNDATE AS Date, 'orders' AS Id, SUM(ISNULL(A.NETAMNT,0)) AS Total
            FROM 
				FINDOC A LEFT OUTER JOIN TRDR B ON A.TRDR=B.TRDR 
            WHERE A.COMPANY = @pCompanyId  
	            AND A.SOSOURCE = 1351 
	            AND A.SODTYPE = 13 
	            AND A.TRNDATE >= @pStartDate
	            AND A.TRNDATE <= @pEndDate
	            AND A.ISCANCEL IN (0,2) 
	            AND  A.TFPRMS = 201
            GROUP BY A.TRNDATE
            ";
    }
}
