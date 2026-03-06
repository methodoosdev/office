namespace App.Web.Infra.Queries.Accounting
{
    public static partial class CountingDocumentQuery
    {
            public static string Get => @"
                SELECT 
                    COUNT(*) AS RecCount, FPRMS.NAME AS DocName, 'Sales' AS DocType
                FROM 
                    FINDOC AS A INNER JOIN
                    FPRMS ON A.COMPANY = FPRMS.COMPANY AND A.SOSOURCE = FPRMS.SOSOURCE AND A.FPRMS = FPRMS.FPRMS LEFT OUTER JOIN
                    MTRDOC AS B ON A.FINDOC = B.FINDOC LEFT OUTER JOIN
                    EXPANAL AS C ON C.FINDOC = A.FINDOC AND C.LINENUM = 1 LEFT OUTER JOIN
                    TRDR AS D ON A.TRDR = D.TRDR LEFT OUTER JOIN
                    TRDEXTRA AS E ON A.TRDR = E.TRDR
                WHERE
                    (A.COMPANY = @pCompanyId) AND (A.SOSOURCE = 1351) AND (A.SOREDIR = 0) AND (A.TRNDATE >= @pStartingDate) AND (A.TRNDATE <= @pEndingDate) AND (A.SODTYPE = 13)
                GROUP BY FPRMS.NAME
                UNION
                SELECT
                    COUNT(*) AS RecCount, FPRMS.NAME AS DocName, 'Purchase' AS DocType
                FROM
                    FINDOC AS A INNER JOIN
                    FPRMS ON A.COMPANY = FPRMS.COMPANY AND A.SOSOURCE = FPRMS.SOSOURCE AND A.FPRMS = FPRMS.FPRMS LEFT OUTER JOIN
                    MTRDOC AS B ON A.FINDOC = B.FINDOC LEFT OUTER JOIN
                    EXPANAL AS C ON C.FINDOC = A.FINDOC AND C.LINENUM = 1 LEFT OUTER JOIN
                    TRDR AS D ON A.TRDR = D.TRDR LEFT OUTER JOIN
                    TRDEXTRA AS E ON A.TRDR = E.TRDR
                WHERE
                    (A.COMPANY = @pCompanyId) AND (A.SOSOURCE = 1251) AND (A.TRNDATE >= @pStartingDate) AND (A.TRNDATE <= @pEndingDate) AND (A.SODTYPE = 12)
                GROUP BY FPRMS.NAME
                UNION
                SELECT
                    COUNT(*) AS RecCount, FPRMS.NAME AS DocName, 'CustomerCollections' AS DocType
                FROM
                    FINDOC AS A INNER JOIN
                    FPRMS ON A.COMPANY = FPRMS.COMPANY AND A.SOSOURCE = FPRMS.SOSOURCE AND A.FPRMS = FPRMS.FPRMS LEFT OUTER JOIN
                    TRDR AS B ON A.TRDR = B.TRDR
                WHERE
                    (A.COMPANY = @pCompanyId) AND (A.SOSOURCE = 1381) AND (A.SOREDIR = 0) AND (A.TRNDATE >= @pStartingDate) AND (A.TRNDATE <= @pEndingDate) AND (A.SODTYPE = 13)
                GROUP BY FPRMS.NAME
                UNION
                SELECT
                    COUNT(*) AS RecCount, FPRMS.NAME AS DocName, 'SupplierPayments' AS DocType
                FROM
                    FINDOC AS A INNER JOIN
                    FPRMS ON A.COMPANY = FPRMS.COMPANY AND A.SOSOURCE = FPRMS.SOSOURCE AND A.FPRMS = FPRMS.FPRMS LEFT OUTER JOIN
                    TRDR AS B ON A.TRDR = B.TRDR
                WHERE
                    (A.COMPANY = @pCompanyId) AND (A.SOSOURCE = 1281) AND (A.SOREDIR = 0) AND (A.TRNDATE >= @pStartingDate) AND (A.TRNDATE <= @pEndingDate) AND (A.SODTYPE = 12)
                GROUP BY FPRMS.NAME
                UNION
                SELECT
                    COUNT(*) AS RecCount, FPRMS.NAME AS DocName, 'CustomerRemittances' AS DocType
                FROM
                    FINDOC AS A INNER JOIN
                    FPRMS ON A.COMPANY = FPRMS.COMPANY AND A.SOSOURCE = FPRMS.SOSOURCE AND A.FPRMS = FPRMS.FPRMS LEFT OUTER JOIN
                    TRDR AS B ON A.TRDR = B.TRDR
                WHERE
                    (A.COMPANY = @pCompanyId) AND (A.SOSOURCE = 1413) AND (A.TRNDATE >= @pStartingDate) AND (A.TRNDATE <= @pEndingDate) AND (A.SODTYPE = 14)
                GROUP BY FPRMS.NAME
                UNION
                SELECT
                    COUNT(*) AS RecCount, FPRMS.NAME AS DocName, 'SupplierRemittances' AS DocType
                FROM
                    FINDOC AS A INNER JOIN
                    FPRMS ON A.COMPANY = FPRMS.COMPANY AND A.SOSOURCE = FPRMS.SOSOURCE AND A.FPRMS = FPRMS.FPRMS LEFT OUTER JOIN
                    TRDR AS B ON A.TRDR = B.TRDR
                WHERE
                    (A.COMPANY = @pCompanyId) AND (A.SOSOURCE = 1412) AND (A.TRNDATE >= @pStartingDate) AND (A.TRNDATE <= @pEndingDate) AND (A.SODTYPE = 14)
                GROUP BY FPRMS.NAME
                UNION
                SELECT
                    COUNT(*) AS RecCount, FPRMS.NAME AS DocName, 'SpecialSuppliers' AS DocType
                FROM
                    FINDOC AS A INNER JOIN
                    FPRMS ON A.COMPANY = FPRMS.COMPANY AND A.SOSOURCE = FPRMS.SOSOURCE AND A.FPRMS = FPRMS.FPRMS LEFT OUTER JOIN
                    MTRLINES AS B ON B.FINDOC = A.FINDOC 
                WHERE
                    (A.COMPANY = @pCompanyId) AND (A.SOSOURCE = 1253) AND (A.TRNDATE >= @pStartingDate) AND (A.TRNDATE <= @pEndingDate) AND (A.SODTYPE = 12)
                GROUP BY FPRMS.NAME
            ";
        }
    }
