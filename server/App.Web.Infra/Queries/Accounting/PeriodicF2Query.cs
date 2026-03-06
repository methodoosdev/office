using App.Core.Domain.Traders;

namespace App.Web.Infra.Queries.Accounting
{
    public class PeriodicF2Query : TraderFactoryQuery
    {
        public PeriodicF2Query(int logistikiProgramTypeId, int categoryBookTypeId) : base(logistikiProgramTypeId, categoryBookTypeId)
        {
        }

        public override string SoftOne_C => @"
            SELECT 
                CAST(SUM(B.LCREDITTMP - B.LDEBITTMP) AS DECIMAL(18,2)) AS Value
            FROM 
                ACNT M, ACNBALSHEET B
            WHERE 
                ({0}) 
                AND M.ACNSCHEMA = @pSchema
                AND B.COMPANY IN (@pCompanyId)
                AND B.FISCPRD = @pYear
                AND M.ACNT = B.ACNT
                AND M.SODTYPE = 89
                AND B.PERIOD IN ({1})
        ";

        public override string SoftOne_B => @"
            SELECT 
                CAST(SUM(A.IMPVAL - A.EXPVAL) AS DECIMAL(18,2)) AS Value
            FROM 
                MTRL AS M RIGHT OUTER JOIN
                MTRBALSHEET AS A ON M.MTRL = A.MTRL
            WHERE
                ({0}) 
                AND (A.COMPANY = @pCompanyId) 
                AND (A.FISCPRD = @pYear)
                AND M.ISACTIVE = 1 
                AND M.SODTYPE = 61
                AND (A.PERIOD IN ({1}))
        ";

        public override string Prosvasis => @"
            SELECT 
                CAST(SUM(A.IMPVAL - A.EXPVAL) AS DECIMAL(18,2)) AS Value
            FROM 
                MTRL AS M RIGHT OUTER JOIN
                MTRBALSHEET AS A ON M.MTRL = A.MTRL
            WHERE
                ({0}) 
                AND (A.COMPANY = @pCompanyId) 
                AND (A.FISCPRD = @pYear)
                AND M.ISACTIVE = 1 
                AND M.SODTYPE = 61
                AND (A.PERIOD IN ({1}))
        ";

        public override string Prosvasis_C => @"
            SELECT 
                CAST(SUM(A.IMPVAL - A.EXPVAL) AS DECIMAL(18,2)) AS Value
            FROM 
                MTRL AS M RIGHT OUTER JOIN
                MTRBALSHEET AS A ON M.MTRL = A.MTRL
            WHERE
                ({0}) 
                AND (A.COMPANY = @pCompanyId) 
                AND (A.FISCPRD = @pYear)
                AND M.ISACTIVE = 1 
                AND M.SODTYPE = 61
                AND (A.PERIOD IN ({1}))
        ";
    }
}
