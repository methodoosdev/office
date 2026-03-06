namespace App.Web.Infra.Queries.Accounting
{
    public static partial class CashAvailableQuery
    {
        public static string All => @"
            SELECT A.CODE AS Id
                ,A.NAME AS [Description]
                ,CAST(SUM(B.LDEBITTMP) - SUM(B.LCREDITTMP) AS DECIMAL(18,2)) AS Total,	  
					CASE 
                        WHEN SUBSTRING(A.CODE, 1, 5) = '38.01' THEN '1.Cash'
                        WHEN SUBSTRING(A.CODE, 1, 5) = '38.02' THEN '2.Bank'
                        WHEN SUBSTRING(A.CODE, 1, 5) = '38.03' THEN '3.Term'
						ELSE '4.Else'
					END AS [Type]
            FROM ACNT A, ACNBALSHEET B
            WHERE A.ACNT = B.ACNT
                AND A.CODE LIKE '38%'
                AND A.SODTYPE = 89
                AND A.ACNSCHEMA = @pSchema
                AND B.COMPANY IN (@pCompanyId)
                AND B.FISCPRD = @pYear
                AND B.PERIOD BETWEEN @pFromPeriod AND @pToPeriod
            GROUP BY A.CODE, A.NAME
            ORDER BY A.CODE, A.NAME
        ";
    }
}
