using App.Core.Domain.Traders;

namespace App.Web.Infra.Queries.Accounting
{
    public class MonthlyFinancialBulletinExpirationQuery : TraderFactoryQuery
    {
        public MonthlyFinancialBulletinExpirationQuery(int logistikiProgramTypeId, int categoryBookTypeId) : base(logistikiProgramTypeId, categoryBookTypeId)
        {
        }

        public override string SoftOne_C => @"
            SELECT A.CODE AS Id, A.NAME AS Description, B.FISCPRD AS Year, B.PERIOD AS Periodos,
                A.ACNGRADE AS Level, 
                SUM(B.LDEBITTMP) - SUM(B.LCREDITTMP) AS Total
            FROM ACNT A, ACNBALSHEET B
            WHERE A.ACNT = B.ACNT
	            AND LEN(A.CODE) >= 18
	            AND A.CODE LIKE @pCode
                AND A.ACNSCHEMA = @pSchema
                AND B.COMPANY IN (@pCompanyId)
                AND B.FISCPRD IN (@pCurrentYear)
                AND A.SODTYPE = 89
                AND B.PERIOD >= 0
                AND B.PERIOD <= 12
            GROUP BY A.CODE, A.NAME, B.FISCPRD, B.PERIOD, A.ACNGRADE
            ORDER BY A.CODE, A.NAME, B.FISCPRD, B.PERIOD, A.ACNGRADE
        ";

        public override string SoftOne_B => @"
            SELECT A.CODE AS Id, A.NAME AS Description, B.FISCPRD AS Year, B.PERIOD AS Periodos,
                A.ACNGRADE AS Level, 
                0 AS Total
            FROM ACNT A, ACNBALSHEET B
            WHERE A.ACNT = B.ACNT
	            AND B.PERIOD = 99
            GROUP BY A.CODE, A.NAME, B.FISCPRD, B.PERIOD, A.ACNGRADE
            ORDER BY A.CODE, A.NAME, B.FISCPRD, B.PERIOD, A.ACNGRADE
        ";

        public override string Prosvasis => @"
            SELECT A.CODE AS Id, A.NAME AS Description, B.FISCPRD AS Year, B.PERIOD AS Periodos,
                A.ACNGRADE AS Level, 
                0 AS Total
            FROM ACNT A, ACNBALSHEET B
            WHERE A.ACNT = B.ACNT
	            AND B.PERIOD = 99
            GROUP BY A.CODE, A.NAME, B.FISCPRD, B.PERIOD, A.ACNGRADE
            ORDER BY A.CODE, A.NAME, B.FISCPRD, B.PERIOD, A.ACNGRADE
        ";

        public override string Prosvasis_C => @"
            SELECT A.CODE AS Id, A.NAME AS Description, B.FISCPRD AS Year, B.PERIOD AS Periodos,
                A.ACNGRADE AS Level, 
                SUM(B.LDEBITTMP) - SUM(B.LCREDITTMP) AS Total
            FROM ACNT A, ACNBALSHEET B
            WHERE A.ACNT = B.ACNT
	            AND LEN(A.CODE) >= 18
	            AND A.CODE LIKE @pCode
                AND A.ACNSCHEMA = @pSchema
                AND B.COMPANY IN (@pCompanyId)
                AND B.FISCPRD IN (@pCurrentYear)
                AND A.SODTYPE = 89
                AND B.PERIOD >= 0
                AND B.PERIOD <= 12
            GROUP BY A.CODE, A.NAME, B.FISCPRD, B.PERIOD, A.ACNGRADE
            ORDER BY A.CODE, A.NAME, B.FISCPRD, B.PERIOD, A.ACNGRADE
        ";
    }
}
