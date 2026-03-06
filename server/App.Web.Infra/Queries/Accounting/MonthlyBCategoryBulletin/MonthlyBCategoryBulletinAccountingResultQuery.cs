using App.Core.Domain.Traders;

namespace App.Web.Infra.Queries.Accounting
{
    public class MonthlyBCategoryBulletinAccountingResultQuery : TraderFactoryQuery
    {
        public MonthlyBCategoryBulletinAccountingResultQuery(int logistikiProgramTypeId, int categoryBookTypeId) : base(logistikiProgramTypeId, categoryBookTypeId)
        {
        }

        public override string SoftOne_C => "";

        public override string SoftOne_B => @"
			SELECT  C.CODE AS Code,
					B.NAME AS Description,
					A.PERIOD AS Periodos,
					CAST(SUM(A.IMPVAL - A.EXPVAL) AS DECIMAL(18,2)) AS Value,
					CASE
						WHEN SUBSTRING(C.CODE, 1, 1) = '2' THEN '2.Purchases'
						WHEN SUBSTRING(C.CODE, 1, 1) = '7' THEN '1.Sales'
						WHEN SUBSTRING(C.CODE, 1, 1) = '6' THEN '3.Expenses'
						WHEN SUBSTRING(C.CODE, 1, 1) = '8' THEN '3.Expenses'
					END as Type

			FROM (
				(SELECT  X.ID,A.COMPANY,A.SODTYPE,A.MTRL,A.CODE,A.NAME,A.MTRTYPE,A.MTRTYPE1 
					FROM (

							SELECT A.MTRL as ID 
							FROM MTRL A 
							WHERE A.COMPANY= @pCompanyId AND A.SODTYPE=61) X JOIN MTRL A  ON X.ID=A.MTRL)) AS C JOIN MTRBALSHEET A	INNER JOIN MTRL B ON A.MTRL=B.MTRL ON C.ID=A.MTRL 

			WHERE A.COMPANY = @pCompanyId 
				  AND LEN(C.CODE) = 18
			      AND ((SUBSTRING(C.CODE, 1, 1) IN ('7','2','6')) OR C.CODE LIKE '80.03.00.0__0.%')
				  AND A.FISCPRD = @pYear 
				  AND A.PERIOD >= 0
				  AND A.PERIOD <= @pPeriod
				  AND B.MTRTYPE IN (1,2,3,4,5,6) 
				  AND (B.MTRMODEL IS NULL OR B.MTRMODEL = 0)

			GROUP BY C.CODE, A.PERIOD, B.MTRTYPE, B.NAME, A.FISCPRD
        ";

        public override string Prosvasis => @"
			SELECT  C.CODE AS Code,
					B.NAME AS Description,
					A.PERIOD AS Periodos,
					CAST(SUM(A.IMPVAL - A.EXPVAL) AS DECIMAL(18,2)) AS Value,
					CASE
						WHEN SUBSTRING(C.CODE, 1, 1) = '2' THEN '2.Purchases'
						WHEN SUBSTRING(C.CODE, 1, 1) = '7' THEN '1.Sales'
						WHEN SUBSTRING(C.CODE, 1, 1) = '6' THEN '3.Expenses'
						WHEN SUBSTRING(C.CODE, 1, 1) = '8' THEN '3.Expenses'
					END as Type

			FROM (
				(SELECT  X.ID,A.COMPANY,A.SODTYPE,A.MTRL,A.CODE,A.NAME,A.MTRTYPE,A.MTRTYPE1 
					FROM (

							SELECT A.MTRL as ID 
							FROM MTRL A 
							WHERE A.COMPANY= @pCompanyId AND A.SODTYPE=61) X JOIN MTRL A  ON X.ID=A.MTRL)) AS C JOIN MTRBALSHEET A	INNER JOIN MTRL B ON A.MTRL=B.MTRL ON C.ID=A.MTRL 

			WHERE A.COMPANY = @pCompanyId 
				  AND LEN(C.CODE) = 18
			      AND ((SUBSTRING(C.CODE, 1, 1) IN ('7','2','6')) OR C.CODE LIKE '80.03.00.0__0.%')
				  AND A.FISCPRD = @pYear 
				  AND A.PERIOD >= 0
				  AND A.PERIOD <= @pPeriod
				  AND B.MTRTYPE IN (1,2,3,4,5,6) 
				  AND (B.MTRMODEL IS NULL OR B.MTRMODEL = 0)

			GROUP BY C.CODE, A.PERIOD, B.MTRTYPE, B.NAME, A.FISCPRD
        ";

        public override string Prosvasis_C => @"
			SELECT  C.CODE AS Code,
					B.NAME AS Description,
					A.PERIOD AS Periodos,
					CAST(SUM(A.IMPVAL - A.EXPVAL) AS DECIMAL(18,2)) AS Value,
					CASE
						WHEN SUBSTRING(C.CODE, 1, 1) = '2' THEN '2.Purchases'
						WHEN SUBSTRING(C.CODE, 1, 1) = '7' THEN '1.Sales'
						WHEN SUBSTRING(C.CODE, 1, 1) = '6' THEN '3.Expenses'
						WHEN SUBSTRING(C.CODE, 1, 1) = '8' THEN '3.Expenses'
					END as Type

			FROM (
				(SELECT  X.ID,A.COMPANY,A.SODTYPE,A.MTRL,A.CODE,A.NAME,A.MTRTYPE,A.MTRTYPE1 
					FROM (

							SELECT A.MTRL as ID 
							FROM MTRL A 
							WHERE A.COMPANY= @pCompanyId AND A.SODTYPE=61) X JOIN MTRL A  ON X.ID=A.MTRL)) AS C JOIN MTRBALSHEET A	INNER JOIN MTRL B ON A.MTRL=B.MTRL ON C.ID=A.MTRL 

			WHERE A.COMPANY = @pCompanyId 
				  AND LEN(C.CODE) = 18
			      AND ((SUBSTRING(C.CODE, 1, 1) IN ('7','2','6')) OR C.CODE LIKE '80.03.00.0__0.%')
				  AND A.FISCPRD = @pYear 
				  AND A.PERIOD >= 0
				  AND A.PERIOD <= @pPeriod
				  AND B.MTRTYPE IN (1,2,3,4,5,6) 
				  AND (B.MTRMODEL IS NULL OR B.MTRMODEL = 0)

			GROUP BY C.CODE, A.PERIOD, B.MTRTYPE, B.NAME, A.FISCPRD
        ";

    }
}
