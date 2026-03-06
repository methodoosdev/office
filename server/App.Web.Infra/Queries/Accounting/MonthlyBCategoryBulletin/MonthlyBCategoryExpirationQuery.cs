using App.Core.Domain.Traders;

namespace App.Web.Infra.Queries.Accounting
{
    public class MonthlyBCategoryExpirationQuery : TraderFactoryQuery
    {
        public MonthlyBCategoryExpirationQuery(int logistikiProgramTypeId, int categoryBookTypeId) : base(logistikiProgramTypeId, categoryBookTypeId)
        {
        }

        public override string SoftOne_C => "";

        public override string SoftOne_B => @"
            SELECT C.CODE, B.NAME AS Description, A.PERIOD AS Periodos, SUM(A.IMPVAL - A.EXPVAL) AS Value,
                '4.Expenses' as Type, 
                A.FISCPRD AS Year
            FROM (
	            (SELECT  X.ID,A.COMPANY,A.SODTYPE,A.MTRL,A.CODE,A.NAME,A.MTRTYPE,A.MTRTYPE1 
	            FROM (

		            SELECT A.MTRL as ID 
		            FROM MTRL A 
		            WHERE A.COMPANY=@pCompanyId AND A.SODTYPE=61 AND A.CODE LIKE @pCode) X JOIN MTRL A  ON X.ID=A.MTRL)) AS C JOIN MTRBALSHEET A	INNER JOIN MTRL B ON A.MTRL=B.MTRL ON C.ID=A.MTRL 

	            WHERE A.COMPANY = @pCompanyId 
                    AND LEN(C.CODE) = 18
		            AND A.FISCPRD = @pYear 
				    AND A.PERIOD >= 0
				    AND A.PERIOD <= @pPeriod
		            AND B.MTRTYPE IN (2,4) AND (B.MTRMODEL IS NULL OR B.MTRMODEL = 0)

            GROUP BY C.CODE, B.NAME, A.FISCPRD, A.PERIOD, B.MTRTYPE
        ";

        public override string Prosvasis => @"
            SELECT C.CODE, B.NAME AS Description, A.PERIOD AS Periodos, SUM(A.IMPVAL - A.EXPVAL) AS Value,
                '4.Expenses' as Type, 
                A.FISCPRD AS Year
            FROM (
	            (SELECT  X.ID,A.COMPANY,A.SODTYPE,A.MTRL,A.CODE,A.NAME,A.MTRTYPE,A.MTRTYPE1 
	            FROM (

		            SELECT A.MTRL as ID 
		            FROM MTRL A 
		            WHERE A.COMPANY=@pCompanyId AND A.SODTYPE=61 AND A.CODE LIKE @pCode) X JOIN MTRL A  ON X.ID=A.MTRL)) AS C JOIN MTRBALSHEET A	INNER JOIN MTRL B ON A.MTRL=B.MTRL ON C.ID=A.MTRL 

	            WHERE A.COMPANY = @pCompanyId 
                    AND LEN(C.CODE) = 18
		            AND A.FISCPRD = @pYear 
				    AND A.PERIOD >= 0
				    AND A.PERIOD <= @pPeriod
		            AND B.MTRTYPE IN (2,4) AND (B.MTRMODEL IS NULL OR B.MTRMODEL = 0)

            GROUP BY C.CODE, B.NAME, A.FISCPRD, A.PERIOD, B.MTRTYPE
        ";

        public override string Prosvasis_C => @"
            SELECT C.CODE, B.NAME AS Description, A.PERIOD AS Periodos, SUM(A.IMPVAL - A.EXPVAL) AS Value,
                '4.Expenses' as Type, 
                A.FISCPRD AS Year
            FROM (
	            (SELECT  X.ID,A.COMPANY,A.SODTYPE,A.MTRL,A.CODE,A.NAME,A.MTRTYPE,A.MTRTYPE1 
	            FROM (

		            SELECT A.MTRL as ID 
		            FROM MTRL A 
		            WHERE A.COMPANY=@pCompanyId AND A.SODTYPE=61 AND A.CODE LIKE @pCode) X JOIN MTRL A  ON X.ID=A.MTRL)) AS C JOIN MTRBALSHEET A	INNER JOIN MTRL B ON A.MTRL=B.MTRL ON C.ID=A.MTRL 

	            WHERE A.COMPANY = @pCompanyId 
                    AND LEN(C.CODE) = 18
		            AND A.FISCPRD = @pYear 
				    AND A.PERIOD >= 0
				    AND A.PERIOD <= @pPeriod
		            AND B.MTRTYPE IN (2,4) AND (B.MTRMODEL IS NULL OR B.MTRMODEL = 0)

            GROUP BY C.CODE, B.NAME, A.FISCPRD, A.PERIOD, B.MTRTYPE
        ";

    }
}
