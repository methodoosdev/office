using App.Core.Domain.Traders;

namespace App.Web.Infra.Queries.Accounting
{
    public class _AggregateAnalysisPaymentsQuery : TraderFactoryQuery
    {
        public _AggregateAnalysisPaymentsQuery(int logistikiProgramTypeId, int categoryBookTypeId) : base(logistikiProgramTypeId, categoryBookTypeId)
        {
        }

        public override string SoftOne_C => @"
            SELECT A.TRNDATE AS Date, 'payments' AS Id, 
                CASE WHEN A.TFPRMS = 102 THEN (ISNULL(A.SUMAMNT,0) * -1)
                ELSE ISNULL(A.SUMAMNT,0) END AS Total
            FROM FINDOC A LEFT OUTER JOIN TRDR B ON A.TRDR=B.TRDR 
            WHERE A.COMPANY=@pCompanyId 
                AND A.SOSOURCE IN (1281, 1412) 
                AND A.SOREDIR=0 
	            AND A.TRNDATE >= @pStartDate
	            AND A.TRNDATE <= @pEndDate
                AND A.SODTYPE IN (12, 14)
            ORDER BY A.TRNDATE DESC,A.FINDOC
        ";

        public override string SoftOne_B => @"
            SELECT A.TRNDATE AS Date, 'payments' AS Id, 
                CASE WHEN A.TFPRMS = 102 THEN (ISNULL(A.SUMAMNT,0) * -1)
                ELSE ISNULL(A.SUMAMNT,0) END AS Total
            FROM FINDOC A LEFT OUTER JOIN TRDR B ON A.TRDR=B.TRDR 
            WHERE A.COMPANY=@pCompanyId 
                AND A.SOSOURCE IN (1281, 1412) 
                AND A.SOREDIR=0 
	            AND A.TRNDATE >= @pStartDate
	            AND A.TRNDATE <= @pEndDate
                AND A.SODTYPE IN (12, 14)
            ORDER BY A.TRNDATE DESC,A.FINDOC
        ";

        public override string Prosvasis => @"
            SELECT A.TRNDATE AS Date, 'payments' AS Id, 
                CASE WHEN A.TFPRMS = 102 THEN (ISNULL(A.SUMAMNT,0) * -1)
                ELSE ISNULL(A.SUMAMNT,0) END AS Total
            FROM FINDOC A LEFT OUTER JOIN TRDR B ON A.TRDR=B.TRDR 
            WHERE A.COMPANY=@pCompanyId 
                AND A.SOSOURCE IN (1281, 1412) 
                AND A.SOREDIR=0 
	            AND A.TRNDATE >= @pStartDate
	            AND A.TRNDATE <= @pEndDate
                AND A.SODTYPE IN (12, 14)
            ORDER BY A.TRNDATE DESC,A.FINDOC
        ";

        public override string Prosvasis_C => @"
            SELECT A.TRNDATE AS Date, 'payments' AS Id, 
                CASE WHEN A.TFPRMS = 102 THEN (ISNULL(A.SUMAMNT,0) * -1)
                ELSE ISNULL(A.SUMAMNT,0) END AS Total
            FROM FINDOC A LEFT OUTER JOIN TRDR B ON A.TRDR=B.TRDR 
            WHERE A.COMPANY=@pCompanyId 
                AND A.SOSOURCE IN (1281, 1412) 
                AND A.SOREDIR=0 
	            AND A.TRNDATE >= @pStartDate
	            AND A.TRNDATE <= @pEndDate
                AND A.SODTYPE IN (12, 14)
            ORDER BY A.TRNDATE DESC,A.FINDOC
        ";
    }
}
