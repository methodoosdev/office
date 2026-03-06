using App.Core.Domain.Traders;

namespace App.Web.Infra.Queries.Accounting
{
    public class MonthlyBCategoryBeginningPdfQuery : TraderFactoryQuery
    {
        public MonthlyBCategoryBeginningPdfQuery(int logistikiProgramTypeId, int categoryBookTypeId) : base(logistikiProgramTypeId, categoryBookTypeId)
        {
        }

        public override string SoftOne_C => "";

        public override string SoftOne_B => @"
			SELECT  

					CASE
						WHEN C.CODE LIKE '2_.01%' THEN 'Αποθέματα έναρξης περιόδου'
					END AS Type,

					CASE 
						WHEN C.CODE LIKE '20.01%' THEN CAST(SUM(A.IMPVAL - A.EXPVAL) AS DECIMAL(18,2))
							 ELSE 0 
					END AS 'Goods',
					
					CASE 
						WHEN C.CODE LIKE '24.01%' THEN CAST(SUM(A.IMPVAL - A.EXPVAL) AS DECIMAL(18,2))
							 ELSE 0 
					END AS 'Materials',

					CASE 
						WHEN C.CODE LIKE '25.01%' THEN CAST(SUM(A.IMPVAL - A.EXPVAL) AS DECIMAL(18,2))
							 ELSE 0 
					END AS 'Consumables',

					CASE 
						WHEN C.CODE LIKE '26.01%' THEN CAST(SUM(A.IMPVAL - A.EXPVAL) AS DECIMAL(18,2))
							 ELSE 0 
					END AS 'SpareParts',
					
					CASE 
						WHEN C.CODE LIKE '27.01%' THEN CAST(SUM(A.IMPVAL - A.EXPVAL) AS DECIMAL(18,2))
							 ELSE 0 
					END AS 'WarehouseOther'

			FROM (
				(SELECT  X.ID,A.COMPANY,A.SODTYPE,A.MTRL,A.CODE,A.NAME,A.MTRTYPE,A.MTRTYPE1 
				FROM (

					SELECT A.MTRL as ID 
					FROM MTRL A 
					WHERE 
					A.COMPANY= @pCompanyId AND 
					A.SODTYPE=61) X JOIN MTRL A  ON X.ID=A.MTRL)) AS C JOIN MTRBALSHEET A	INNER JOIN MTRL B ON A.MTRL=B.MTRL ON C.ID=A.MTRL 

				WHERE 
					A.COMPANY = @pCompanyId 					
					AND LEN(C.CODE) = 18
					AND A.FISCPRD = @pYear 
				    AND A.PERIOD >= 0
				    AND A.PERIOD <= @pPeriod
					AND B.MTRTYPE IN (1,2,3,4,5,6) AND (B.MTRMODEL IS NULL OR B.MTRMODEL = 0)
					--AND C.CODE IN ('20.01%', '24.01%', '25.01%', '26.01%', '27.01%')
					AND C.CODE LIKE '20.01%'
						OR C.CODE LIKE '24.01%'
						OR C.CODE LIKE '25.01%'
						OR C.CODE LIKE '26.01%'
						OR C.CODE LIKE '27.01%'

			GROUP BY C.CODE , A.FISCPRD
        ";

        public override string Prosvasis => @"
			SELECT  

					CASE
						WHEN C.CODE LIKE '2_.01%' THEN 'Αποθέματα έναρξης περιόδου'
					END AS Type,

					CASE 
						WHEN C.CODE LIKE '20.01%' THEN CAST(SUM(A.IMPVAL - A.EXPVAL) AS DECIMAL(18,2))
							 ELSE 0 
					END AS 'Goods',
					
					CASE 
						WHEN C.CODE LIKE '24.01%' THEN CAST(SUM(A.IMPVAL - A.EXPVAL) AS DECIMAL(18,2))
							 ELSE 0 
					END AS 'Materials',

					CASE 
						WHEN C.CODE LIKE '25.01%' THEN CAST(SUM(A.IMPVAL - A.EXPVAL) AS DECIMAL(18,2))
							 ELSE 0 
					END AS 'Consumables',

					CASE 
						WHEN C.CODE LIKE '26.01%' THEN CAST(SUM(A.IMPVAL - A.EXPVAL) AS DECIMAL(18,2))
							 ELSE 0 
					END AS 'SpareParts',
					
					CASE 
						WHEN C.CODE LIKE '27.01%' THEN CAST(SUM(A.IMPVAL - A.EXPVAL) AS DECIMAL(18,2))
							 ELSE 0 
					END AS 'WarehouseOther'

			FROM (
				(SELECT  X.ID,A.COMPANY,A.SODTYPE,A.MTRL,A.CODE,A.NAME,A.MTRTYPE,A.MTRTYPE1 
				FROM (

					SELECT A.MTRL as ID 
					FROM MTRL A 
					WHERE 
					A.COMPANY= @pCompanyId AND 
					A.SODTYPE=61) X JOIN MTRL A  ON X.ID=A.MTRL)) AS C JOIN MTRBALSHEET A	INNER JOIN MTRL B ON A.MTRL=B.MTRL ON C.ID=A.MTRL 

				WHERE 
					A.COMPANY = @pCompanyId 					
					AND LEN(C.CODE) = 18
					AND A.FISCPRD = @pYear 
				    AND A.PERIOD >= 0
				    AND A.PERIOD <= @pPeriod
					AND B.MTRTYPE IN (1,2,3,4,5,6) AND (B.MTRMODEL IS NULL OR B.MTRMODEL = 0)
					--AND C.CODE IN ('20.01%', '24.01%', '25.01%', '26.01%', '27.01%')
					AND C.CODE LIKE '20.01%'
						OR C.CODE LIKE '24.01%'
						OR C.CODE LIKE '25.01%'
						OR C.CODE LIKE '26.01%'
						OR C.CODE LIKE '27.01%'

			GROUP BY C.CODE , A.FISCPRD
        ";

        public override string Prosvasis_C => @"
			SELECT  

					CASE
						WHEN C.CODE LIKE '2_.01%' THEN 'Αποθέματα έναρξης περιόδου'
					END AS Type,

					CASE 
						WHEN C.CODE LIKE '20.01%' THEN CAST(SUM(A.IMPVAL - A.EXPVAL) AS DECIMAL(18,2))
							 ELSE 0 
					END AS 'Goods',
					
					CASE 
						WHEN C.CODE LIKE '24.01%' THEN CAST(SUM(A.IMPVAL - A.EXPVAL) AS DECIMAL(18,2))
							 ELSE 0 
					END AS 'Materials',

					CASE 
						WHEN C.CODE LIKE '25.01%' THEN CAST(SUM(A.IMPVAL - A.EXPVAL) AS DECIMAL(18,2))
							 ELSE 0 
					END AS 'Consumables',

					CASE 
						WHEN C.CODE LIKE '26.01%' THEN CAST(SUM(A.IMPVAL - A.EXPVAL) AS DECIMAL(18,2))
							 ELSE 0 
					END AS 'SpareParts',
					
					CASE 
						WHEN C.CODE LIKE '27.01%' THEN CAST(SUM(A.IMPVAL - A.EXPVAL) AS DECIMAL(18,2))
							 ELSE 0 
					END AS 'WarehouseOther'

			FROM (
				(SELECT  X.ID,A.COMPANY,A.SODTYPE,A.MTRL,A.CODE,A.NAME,A.MTRTYPE,A.MTRTYPE1 
				FROM (

					SELECT A.MTRL as ID 
					FROM MTRL A 
					WHERE 
					A.COMPANY= @pCompanyId AND 
					A.SODTYPE=61) X JOIN MTRL A  ON X.ID=A.MTRL)) AS C JOIN MTRBALSHEET A	INNER JOIN MTRL B ON A.MTRL=B.MTRL ON C.ID=A.MTRL 

				WHERE 
					A.COMPANY = @pCompanyId 					
					AND LEN(C.CODE) = 18
					AND A.FISCPRD = @pYear 
				    AND A.PERIOD >= 0
				    AND A.PERIOD <= @pPeriod
					AND B.MTRTYPE IN (1,2,3,4,5,6) AND (B.MTRMODEL IS NULL OR B.MTRMODEL = 0)
					--AND C.CODE IN ('20.01%', '24.01%', '25.01%', '26.01%', '27.01%')
					AND C.CODE LIKE '20.01%'
						OR C.CODE LIKE '24.01%'
						OR C.CODE LIKE '25.01%'
						OR C.CODE LIKE '26.01%'
						OR C.CODE LIKE '27.01%'

			GROUP BY C.CODE , A.FISCPRD
        ";
    }
}
