namespace App.Web.Infra.Queries.Accounting
{
    public static partial class ESendQuery
    {
        public static string Docs => @"
			SELECT A.FPRMS AS DocId, A.NAME AS DocName 
            FROM FPRMS A 
            WHERE A.COMPANY = @pCompanyId AND A.SOSOURCE=1351 AND A.NAME IN (@pDocName)
            ORDER BY A.FPRMS
        ";

        public static string Tameiakes => @"
            SELECT CAST(A.SERIES as varchar(10)) AS Id, A.NAME AS Tameiaki
            FROM SERIES A 
            WHERE A.COMPANY IN (@pCompanyId) AND A.SOSOURCE=@pSoSource
            ORDER BY A.SERIES
        ";
    }
}
