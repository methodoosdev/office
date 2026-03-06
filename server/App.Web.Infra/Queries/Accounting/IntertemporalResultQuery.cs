namespace App.Web.Infra.Queries.Accounting
{
    public static partial class IntertemporalResultQuery
    {
        public static string GetBCategory => @"
            SELECT 
	            X.Id, 
	            X.Year, 
	            CAST( SUM(X.Total)  AS DECIMAL(18,2)) AS Total
            FROM
	            (
            SELECT 
                A.FISCPRD AS Year,
                CAST(SUM(A.IMPVAL - A.EXPVAL) AS DECIMAL(18,2)) AS Total,
                'Sales' AS Id
            FROM 
                MTRBALSHEET A
                INNER JOIN MTRL B ON A.MTRL = B.MTRL
            WHERE 
                A.COMPANY IN (@pCompanyId)
                AND LEN(B.CODE) = 18
                AND B.CODE LIKE '70%' 
                AND A.FISCPRD > 2021 
                AND A.PERIOD >= 0 
                AND A.PERIOD <= 12 
                AND B.MTRTYPE IN (1,2,3,4,5,6)
                AND (B.MTRMODEL IS NULL OR B.MTRMODEL = 0)
            GROUP BY 
                A.FISCPRD

            UNION ALL

            SELECT 
                A.FISCPRD AS Year,
                CAST(SUM(A.IMPVAL - A.EXPVAL) AS DECIMAL(18,2)) AS Total,
                'Income' AS Id
            FROM 
                MTRBALSHEET A
                INNER JOIN MTRL B ON A.MTRL = B.MTRL
            WHERE 
                A.COMPANY IN (@pCompanyId)
                AND LEN(B.CODE) = 18
                AND B.CODE LIKE '70%' 
                AND (B.CODE LIKE '71%' OR B.CODE LIKE '72%' OR B.CODE LIKE '73%' OR B.CODE LIKE '74%' 
                    OR B.CODE LIKE '75%' OR B.CODE LIKE '76%' OR B.CODE LIKE '77%' OR B.CODE LIKE '79%' OR B.CODE LIKE '80%')
                AND A.FISCPRD > 2021 
                AND A.PERIOD >= 0 
                AND A.PERIOD <= 12 
                AND B.MTRTYPE IN (1,2,3,4,5,6)
                AND (B.MTRMODEL IS NULL OR B.MTRMODEL = 0)
            GROUP BY 
                A.FISCPRD

            UNION ALL

            SELECT 
                A.FISCPRD AS Year,
                CAST(SUM(A.IMPVAL - A.EXPVAL) AS DECIMAL(18,2)) AS Total,
                'OpeningInventory' AS Id
            FROM 
                MTRBALSHEET A
                INNER JOIN MTRL B ON A.MTRL = B.MTRL
            WHERE 
                A.COMPANY IN (@pCompanyId)
                AND LEN(B.CODE) = 18
                AND CODE LIKE '2_._1%'
                AND A.FISCPRD > 2021 
                AND A.PERIOD <= 12 
                AND B.MTRTYPE IN (1,2,3,4,5,6)
                AND (B.MTRMODEL IS NULL OR B.MTRMODEL = 0)
            GROUP BY 
                A.FISCPRD

            UNION ALL

            SELECT 
                A.FISCPRD AS Year,
                CAST(SUM(A.IMPVAL - A.EXPVAL) AS DECIMAL(18,2)) AS Total,
                'Purchases' AS Id
            FROM 
                MTRBALSHEET A
                INNER JOIN MTRL B ON A.MTRL = B.MTRL
            WHERE 
                A.COMPANY IN (@pCompanyId)
                AND LEN(B.CODE) = 18
                AND (LEFT(B.CODE,5)  IN ('20.02','20.03','20.04','20.05')
                    OR LEFT(B.CODE,3)  IN ('24','25','26','27')
                    OR LEFT(B.CODE,8)  IN ('22.01.02','22.01.03','22.01.04','22.01.05','22.01.06',
                                            '22.02.02','22.02.03','22.02.04','22.02.05','22.02.06'))
                AND A.FISCPRD > 2021 
                AND A.PERIOD >= 0 
                AND A.PERIOD <= 12 
                AND B.MTRTYPE IN (1,2,3,4,5,6)
                AND (B.MTRMODEL IS NULL OR B.MTRMODEL = 0)
            GROUP BY 
                A.FISCPRD

            UNION ALL

            SELECT 
                A.FISCPRD AS Year,
                CAST(SUM(A.IMPVAL - A.EXPVAL) AS DECIMAL(18,2)) AS Total,
                'ClosingInventory' AS Id
            FROM 
                MTRBALSHEET A
                INNER JOIN MTRL B ON A.MTRL = B.MTRL
            WHERE 
                A.COMPANY IN (@pCompanyId)
                AND LEN(B.CODE) = 18
                AND B.CODE LIKE '2%.06%'
                AND A.FISCPRD > 2021 
                AND A.PERIOD = 1000
                AND B.MTRTYPE IN (1,2,3,4,5,6)
                AND (B.MTRMODEL IS NULL OR B.MTRMODEL = 0)
            GROUP BY 
                A.FISCPRD

            UNION ALL

            SELECT 
                A.FISCPRD AS Year,
                CAST(SUM(A.IMPVAL - A.EXPVAL) AS DECIMAL(18,2)) AS Total,
                'Expenses' AS Id
            FROM 
                MTRBALSHEET A
                INNER JOIN MTRL B ON A.MTRL = B.MTRL
            WHERE 
                A.COMPANY IN (@pCompanyId)
                AND LEN(B.CODE) = 18
                AND (B.CODE LIKE '60%' OR B.CODE LIKE '61%' OR B.CODE LIKE '62%' OR B.CODE LIKE '63%' OR B.CODE LIKE '64%' 
                    OR B.CODE LIKE '65%' OR B.CODE LIKE '66%' OR B.CODE LIKE '67%' OR B.CODE LIKE '68%')
                AND A.FISCPRD > 2021 
                AND A.PERIOD >= 0 
                AND A.PERIOD <= 12 
                AND B.MTRTYPE IN (1,2,3,4,5,6)
                AND (B.MTRMODEL IS NULL OR B.MTRMODEL = 0)
            GROUP BY 
                A.FISCPRD
            ) AS X
            GROUP BY 
	            X.Id, X.Year
        ";

        public static string GetCCategory => @"
            SELECT 
	            X.Id, 
	            X.Year, 
	            CAST( SUM(X.Total)  AS DECIMAL(18,2)) AS Total
            FROM
	            (SELECT 
                B.FISCPRD AS Year,  
                SUM(B.LCREDITTMP - B.LDEBITTMP) AS Total,
                'Sales' AS Id
            FROM 
                ACNT A, ACNBALSHEET B
            WHERE 
                A.ACNT = B.ACNT
                AND LEN(A.CODE) >= 18
				AND A.ACNSCHEMA = @pSchema
				AND B.COMPANY IN (@pCompanyId)
                AND B.FISCPRD > 2021
                AND B.PERIOD <= 12
                AND A.SODTYPE = 89
                AND A.CODE LIKE '70%'
            GROUP BY 
                B.FISCPRD

            UNION ALL

            SELECT 
                B.FISCPRD AS Year,  
                SUM(B.LCREDITTMP - B.LDEBITTMP) AS Total,
                'Income' AS Id
            FROM 
                ACNT A, ACNBALSHEET B
            WHERE 
                A.ACNT = B.ACNT
                AND LEN(A.CODE) >= 18
				AND A.ACNSCHEMA = @pSchema
				AND B.COMPANY IN (@pCompanyId)
                AND B.FISCPRD > 2021
                AND B.PERIOD <= 12
                AND A.SODTYPE = 89
                AND (A.CODE LIKE '71%' OR A.CODE LIKE '72%' OR A.CODE LIKE '73%' OR A.CODE LIKE '74%' 
                    OR A.CODE LIKE '75%' OR A.CODE LIKE '76%' OR A.CODE LIKE '77%' OR A.CODE LIKE '79%' OR A.CODE LIKE '80%')
            GROUP BY 
                B.FISCPRD

            UNION ALL

            SELECT 
                B.FISCPRD AS Year,  
                SUM(B.LDEBITTMP - B.LCREDITTMP) AS Total,
                'OpeningInventory' AS Id
            FROM 
                ACNT A, ACNBALSHEET B
            WHERE 
                A.ACNT = B.ACNT
                AND LEN(A.CODE) >= 18
				AND A.ACNSCHEMA = @pSchema
				AND B.COMPANY IN (@pCompanyId)
                AND B.FISCPRD > 2021
                AND B.PERIOD <= 12
                AND A.SODTYPE = 89
                AND A.CODE LIKE '2_._1%'
            GROUP BY 
                B.FISCPRD

            UNION ALL

            SELECT 
                B.FISCPRD AS Year,  
                SUM(B.LDEBITTMP - B.LCREDITTMP) AS Total,
                'Purchases' AS Id
            FROM 
                ACNT A, ACNBALSHEET B
            WHERE 
                A.ACNT = B.ACNT
                AND LEN(A.CODE) >= 18
				AND A.ACNSCHEMA = @pSchema
				AND B.COMPANY IN (@pCompanyId)
                AND B.FISCPRD > 2021
                AND B.PERIOD <= 12
                AND A.SODTYPE = 89
                AND (LEFT(A.CODE,5)  IN ('20.02','20.03','20.04','20.05')
                    OR LEFT(A.CODE,3)  IN ('24','25','26','27')
                    OR LEFT(A.CODE,8)  IN ('22.01.02','22.01.03','22.01.04','22.01.05','22.01.06',
                                            '22.02.02','22.02.03','22.02.04','22.02.05','22.02.06'))
            GROUP BY 
                B.FISCPRD

            UNION ALL

            SELECT 
                B.FISCPRD AS Year,  
                SUM(B.LDEBITTMP - B.LCREDITTMP) AS Total,
                'ClosingInventory' AS Id
            FROM 
                ACNT A, ACNBALSHEET B
            WHERE 
                A.ACNT = B.ACNT
                AND LEN(A.CODE) >= 18
				AND A.ACNSCHEMA = @pSchema
				AND B.COMPANY IN (@pCompanyId)
                AND B.FISCPRD > 2021
                AND B.PERIOD = 1000
                AND A.SODTYPE = 89
                AND A.CODE LIKE '2%.06%'
            GROUP BY 
                B.FISCPRD

            UNION ALL

            SELECT 
                B.FISCPRD AS Year,  
                SUM(B.LDEBITTMP - B.LCREDITTMP) AS Total,
                'Expenses' AS Id
            FROM 
                ACNT A, ACNBALSHEET B
            WHERE 
                A.ACNT = B.ACNT
                AND LEN(A.CODE) >= 18
				AND A.ACNSCHEMA = @pSchema
				AND B.COMPANY IN (@pCompanyId)
                AND B.FISCPRD > 2021
                AND B.PERIOD <= 12
                AND A.SODTYPE = 89
                AND (A.CODE LIKE '60%' OR A.CODE LIKE '61%' OR A.CODE LIKE '62%' OR A.CODE LIKE '63%' OR A.CODE LIKE '64%' 
                    OR A.CODE LIKE '65%' OR A.CODE LIKE '66%' OR A.CODE LIKE '67%' OR A.CODE LIKE '68%')
            GROUP BY 
                B.FISCPRD
            ) AS X
            GROUP BY 
	            X.Id, X.Year

        ";
    }
}
