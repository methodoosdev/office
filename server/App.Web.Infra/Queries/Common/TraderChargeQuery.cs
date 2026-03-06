using App.Core.Domain.Traders;

namespace App.Web.Infra.Queries.Common
{
    public class TraderChargeQuery : TraderFactoryQuery
    {
        public TraderChargeQuery(int logistikiProgramTypeId, int categoryBookTypeId) : base(logistikiProgramTypeId, categoryBookTypeId)
        {
        }

        public override string SoftOne_C => @"
			DECLARE @MaxGrade INT;

			SELECT @MaxGrade = MAX(ACNGRADE)
			FROM ACNT
			WHERE ACNSCHEMA = @pSchema;

            SELECT 
				B.FISCPRD AS [Year],
                LEFT(A.CODE, 5) AS Code,
				LEFT(A.CODE, 1) AS CodePrefix,
				SUM(
					CASE 
                        --WHEN LEFT(A.CODE, 1) IN ('2', '6', '7') OR A.CODE LIKE '80.03.00.0__0.%' THEN B.LCREDITTMP - B.LDEBITTMP
						WHEN A.CODE LIKE '7%' THEN B.LCREDITTMP - B.LDEBITTMP
						ELSE B.LDEBITTMP - B.LCREDITTMP
					END
				) AS Total
            FROM ACNT A, ACNBALSHEET B
            WHERE 
				A.ACNT = B.ACNT
	            AND A.ACNGRADE = @MaxGrade
	            AND ((LEFT(A.CODE, 1) IN('1','7','2','6')) OR A.CODE LIKE '80.03.00.0__0.%')
                AND A.ACNSCHEMA = @pSchema
                AND B.COMPANY IN (@pCompanyId)
                AND A.SODTYPE = 89
                AND B.PERIOD >= 0
                AND B.PERIOD <= 12
            GROUP BY 
				B.FISCPRD, LEFT(A.CODE, 5), LEFT(A.CODE, 1)
            ORDER BY 
				B.FISCPRD
        ";

        public override string SoftOne_B => @"
            SELECT
                A.FISCPRD AS [Year],
                LEFT(C.CODE, 5) AS Code,
                CAST(SUM(A.IMPVAL - A.EXPVAL) AS DECIMAL(18, 2)) AS Total,
                CASE LEFT(C.CODE, 1)
                    WHEN '2' THEN 2
                    WHEN '6' THEN 6
                    WHEN '7' THEN 7
                    WHEN '8' THEN 6
                END AS CodePrefix
            FROM MTRBALSHEET A
            JOIN MTRL B ON A.MTRL = B.MTRL
            JOIN (
                SELECT 
                    MTRL AS ID,
                    CODE
                FROM MTRL
                WHERE COMPANY = @pCompanyId
                  AND SODTYPE = 61
            ) C ON C.ID = A.MTRL
            WHERE A.COMPANY = @pCompanyId
              AND LEN(C.CODE) = 18
              AND (
                  LEFT(C.CODE, 1) IN ('2', '6', '7')
                  OR C.CODE LIKE '80.03.00.0__0.%'
              )
              AND A.PERIOD BETWEEN 0 AND 12
              AND B.MTRTYPE IN (1, 2, 3, 4, 5, 6)
              AND (B.MTRMODEL IS NULL OR B.MTRMODEL = 0)
            GROUP BY 
                CASE LEFT(C.CODE, 1)
                    WHEN '2' THEN 2
                    WHEN '6' THEN 6
                    WHEN '7' THEN 7
                    WHEN '8' THEN 6
                END,
                A.FISCPRD, LEFT(C.CODE, 5)
            ORDER BY [Year], CodePrefix;
        ";

        public override string Prosvasis => @"
            SELECT
				A.FISCPRD AS [Year],
                LEFT(C.CODE, 5) AS Code,
				CAST(SUM(A.IMPVAL - A.EXPVAL) AS DECIMAL(18, 2)) AS Total,
				CASE LEFT(C.CODE, 1)
					WHEN '1' THEN 1
					WHEN '2' THEN 2
					WHEN '6' THEN 6
					WHEN '7' THEN 7
					WHEN '8' THEN 8
				END AS CodePrefix
			FROM MTRBALSHEET A
			JOIN MTRL B ON A.MTRL = B.MTRL
			JOIN (
				SELECT 
					MTRL AS ID,
					CODE
				FROM MTRL
				WHERE COMPANY = @pCompanyId
				  AND SODTYPE = 61
			) C ON C.ID = A.MTRL
			WHERE A.COMPANY = @pCompanyId
			  AND LEN(C.CODE) = 18
			  AND (
				  LEFT(C.CODE, 1) IN ('1', '2', '6', '7')
				  OR C.CODE LIKE '80.03.00.0__0.%'
			  )
			  AND A.PERIOD BETWEEN 0 AND 12
			  AND B.MTRTYPE IN (1, 2, 3, 4, 5, 6)
			  AND (B.MTRMODEL IS NULL OR B.MTRMODEL = 0)
			GROUP BY 
				CASE LEFT(C.CODE, 1)
					WHEN '1' THEN 1
					WHEN '2' THEN 2
					WHEN '6' THEN 6
					WHEN '7' THEN 7
					WHEN '8' THEN 8
				END,
				A.FISCPRD, LEFT(C.CODE, 5)
			ORDER BY [Year], CodePrefix;
        ";

        public override string Prosvasis_C => @"
            SELECT
				A.FISCPRD AS [Year],
                LEFT(C.CODE, 5) AS Code,
				CAST(SUM(A.IMPVAL - A.EXPVAL) AS DECIMAL(18, 2)) AS Total,
				CASE LEFT(C.CODE, 1)
					WHEN '1' THEN 1
					WHEN '2' THEN 2
					WHEN '6' THEN 6
					WHEN '7' THEN 7
					WHEN '8' THEN 8
				END AS CodePrefix
			FROM MTRBALSHEET A
			JOIN MTRL B ON A.MTRL = B.MTRL
			JOIN (
				SELECT 
					MTRL AS ID,
					CODE
				FROM MTRL
				WHERE COMPANY = @pCompanyId
				  AND SODTYPE = 61
			) C ON C.ID = A.MTRL
			WHERE A.COMPANY = @pCompanyId
			  AND LEN(C.CODE) = 18
			  AND (
				  LEFT(C.CODE, 1) IN ('1', '2', '6', '7')
				  OR C.CODE LIKE '80.03.00.0__0.%'
			  )
			  AND A.PERIOD BETWEEN 0 AND 12
			  AND B.MTRTYPE IN (1, 2, 3, 4, 5, 6)
			  AND (B.MTRMODEL IS NULL OR B.MTRMODEL = 0)
			GROUP BY 
				CASE LEFT(C.CODE, 1)
					WHEN '1' THEN 1
					WHEN '2' THEN 2
					WHEN '6' THEN 6
					WHEN '7' THEN 7
					WHEN '8' THEN 8
				END,
				A.FISCPRD, LEFT(C.CODE, 5)
			ORDER BY [Year], CodePrefix;
        ";
    }
}
