using App.Core.Domain.Traders;

namespace App.Web.Infra.Queries.Accounting
{
    public class _AggregateAnalysisOrdersQuery : TraderFactoryQuery
    {
        public _AggregateAnalysisOrdersQuery(int logistikiProgramTypeId, int categoryBookTypeId) : base(logistikiProgramTypeId, categoryBookTypeId)
        {
        }

        public override string SoftOne_C => @"
            SELECT A.TRNDATE AS Date, 'orders' AS Id, ISNULL(A.NETAMNT,0) AS Total
            FROM FINDOC A LEFT OUTER JOIN TRDR B ON A.TRDR=B.TRDR 
            WHERE A.COMPANY = @pCompanyId 
	            AND A.SOSOURCE = 1351 
	            AND A.TRNDATE >= @pStartDate
	            AND A.TRNDATE <= @pEndDate
	            AND A.SODTYPE = 13 
	            AND A.ISCANCEL IN (0,2) 
	            --AND A.FULLYTRANSF IN (0,2) 
	            AND  A.FPRMS IN (
		            SELECT F.FPRMS 
		            FROM FPRMS F 
		            WHERE F.SOSOURCE = 1351 AND F.TFPRMS = 201) --AND  A.FULLYTRANSF IN (0,2) 
			            --AND  EXISTS(
			            --SELECT 1 
			            --FROM MTRLINES ML 
			            --WHERE ML.FINDOC = A.FINDOC AND ML.PENDING = 1) 
            ORDER BY A.TRNDATE,A.FINCODE
        ";

        public override string SoftOne_B => @"
            SELECT A.TRNDATE AS Date, 'orders' AS Id, ISNULL(A.NETAMNT,0) AS Total
            FROM FINDOC A LEFT OUTER JOIN TRDR B ON A.TRDR=B.TRDR 
            WHERE A.COMPANY = @pCompanyId 
	            AND A.SOSOURCE = 1351 
	            AND A.TRNDATE >= @pStartDate
	            AND A.TRNDATE <= @pEndDate
	            AND A.SODTYPE = 13 
	            AND A.ISCANCEL IN (0,2) 
	            AND A.FULLYTRANSF IN (0,2) 
	            AND  A.FPRMS IN (
		            SELECT F.FPRMS 
		            FROM FPRMS F 
		            WHERE F.SOSOURCE = 1351 AND F.TFPRMS = 201) AND  A.FULLYTRANSF IN (0,2) 
			            AND  EXISTS(
			            SELECT 1 
			            FROM MTRLINES ML 
			            WHERE ML.FINDOC = A.FINDOC AND ML.PENDING = 1) 
            ORDER BY A.TRNDATE,A.FINCODE
        ";

        public override string Prosvasis => @"
            SELECT A.TRNDATE AS Date, 'orders' AS Id, ISNULL(A.NETAMNT,0) AS Total
            FROM FINDOC A LEFT OUTER JOIN TRDR B ON A.TRDR=B.TRDR 
            WHERE A.COMPANY = @pCompanyId 
	            AND A.SOSOURCE = 1351 
	            AND A.TRNDATE >= @pStartDate
	            AND A.TRNDATE <= @pEndDate
	            AND A.SODTYPE = 13 
	            AND A.ISCANCEL IN (0,2) 
	            AND A.FULLYTRANSF IN (0,2) 
	            AND  A.FPRMS IN (
		            SELECT F.FPRMS 
		            FROM FPRMS F 
		            WHERE F.SOSOURCE = 1351 AND F.TFPRMS = 201) AND  A.FULLYTRANSF IN (0,2) 
			            AND  EXISTS(
			            SELECT 1 
			            FROM MTRLINES ML 
			            WHERE ML.FINDOC = A.FINDOC AND ML.PENDING = 1) 
            ORDER BY A.TRNDATE,A.FINCODE
        ";

        public override string Prosvasis_C => @"
            SELECT A.TRNDATE AS Date, 'orders' AS Id, ISNULL(A.NETAMNT,0) AS Total
            FROM FINDOC A LEFT OUTER JOIN TRDR B ON A.TRDR=B.TRDR 
            WHERE A.COMPANY = @pCompanyId 
	            AND A.SOSOURCE = 1351 
	            AND A.TRNDATE >= @pStartDate
	            AND A.TRNDATE <= @pEndDate
	            AND A.SODTYPE = 13 
	            AND A.ISCANCEL IN (0,2) 
	            AND A.FULLYTRANSF IN (0,2) 
	            AND  A.FPRMS IN (
		            SELECT F.FPRMS 
		            FROM FPRMS F 
		            WHERE F.SOSOURCE = 1351 AND F.TFPRMS = 201) AND  A.FULLYTRANSF IN (0,2) 
			            AND  EXISTS(
			            SELECT 1 
			            FROM MTRLINES ML 
			            WHERE ML.FINDOC = A.FINDOC AND ML.PENDING = 1) 
            ORDER BY A.TRNDATE,A.FINCODE
        ";
    }
}
