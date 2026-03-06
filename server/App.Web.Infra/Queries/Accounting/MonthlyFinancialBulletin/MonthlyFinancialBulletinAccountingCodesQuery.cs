using App.Core.Domain.Traders;

namespace App.Web.Infra.Queries.Accounting
{
    public class MonthlyFinancialBulletinAccountingCodesQuery : TraderFactoryQuery
    {
        public MonthlyFinancialBulletinAccountingCodesQuery(int logistikiProgramTypeId, int categoryBookTypeId) : base(logistikiProgramTypeId, categoryBookTypeId)
        {
        }

        public override string SoftOne_C => @"
            SELECT A.CODE AS Id, A.NAME AS Description, A.ACNGRADE AS Level
            FROM ACNT A 
            WHERE A.ACNSCHEMA = @pSchema
                AND A.ACNTYPE IN (10,11,20,30,33,38,40,50,55,60,64,70,80) 
                AND A.IASACN IN (0,2) 
                AND A.SODTYPE=89 
                AND A.ACNGRADE<=6 
            ORDER BY A.CODE
        ";

        public override string SoftOne_B => @"
            SELECT A.CODE AS Id, A.NAME AS Description, 5 AS Level
            FROM MTRL A
            WHERE A.COMPANY IN (@pCompanyId) 
	            AND A.SODTYPE=61 
	            AND LEN(A.CODE) = 18
            ORDER BY A.CODE
        ";

        public override string Prosvasis => @"
            SELECT A.CODE AS Id, A.NAME AS Description, 5 AS Level
            FROM MTRL A
            WHERE A.COMPANY IN (@pCompanyId) 
	            AND A.SODTYPE=61 
	            AND LEN(A.CODE) = 18
            ORDER BY A.CODE
        ";

        public override string Prosvasis_C => @"
            SELECT A.CODE AS Id, A.NAME AS Description, A.ACNGRADE AS Level
            FROM ACNT A 
            WHERE A.ACNSCHEMA = @pSchema
                AND A.ACNTYPE IN (10,11,20,30,33,38,40,50,55,60,64,70,80) 
                AND A.IASACN IN (0,2) 
                AND A.SODTYPE=89 
                AND A.ACNGRADE<=6 
            ORDER BY A.CODE
        ";
    }
}
