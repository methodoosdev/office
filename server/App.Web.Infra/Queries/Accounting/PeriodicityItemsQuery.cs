namespace App.Web.Infra.Queries.Accounting
{
    public static partial class PeriodicityItemsQuery
    {
        public static string RealEstateRent => @"
                SELECT 
                    A.TRNVAL AS Price, 
                    B.PERIOD AS Periodos, 
                    CAST(CASE WHEN B.SOSOURCE = 1253 THEN 1 ELSE 0 END AS BIT) AS Credit
                FROM TRDR AS TR INNER JOIN
                     TRDTRN AS A LEFT OUTER JOIN
                     FINDOC AS B ON A.FINDOC = B.FINDOC ON TR.TRDR = A.TRDR
                WHERE (A.COMPANY = @pCompanyId) 
                    AND (A.FISCPRD = @pYear) 
                    AND (A.SODTYPE = 12) 
                    AND (TR.CODE IN
                        (SELECT DISTINCT X.VendorId
                         FROM MTRLINES AS ML LEFT OUTER JOIN
                              MTRL AS M ON ML.MTRL = M.MTRL LEFT OUTER JOIN
                              (SELECT F.FINDOC, F.FISCPRD AS Year, T.CODE AS VendorId
                               FROM FINDOC AS F LEFT OUTER JOIN
                                    TRDR AS T ON F.TRDR = T.TRDR  AND F.FISCPRD = @pYear
                               WHERE (F.SOSOURCE = 1253) 
                                    AND (F.SODTYPE = 12)
                               ) AS X ON X.FINDOC = ML.FINDOC
                         WHERE (ML.SODTYPE = 53) 
                            AND LEN(M.CODE) = 18 
                            AND (M.CODE LIKE '62.04.01.%'))
                        )
                ORDER BY Periodos
            ";
    }
}
