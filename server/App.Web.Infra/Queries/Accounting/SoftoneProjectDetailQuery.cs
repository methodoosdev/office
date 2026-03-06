namespace App.Web.Infra.Queries.Accounting
{
    public static partial class SoftoneProjectDetailQuery
    {
        public static string Project => @"
			SELECT 
				A.CODE AS [Code], 
				A.NAME AS [Description]
			FROM PRJC AS A
			WHERE A.COMPANY IN (@pCompanyId) AND A.PRJC = @pProjectId  AND A.SODTYPE=40
        ";

        public static string All => @"           
			SELECT 
				A.SOSOURCE AS SoSource
				,A.FINCODE AS Invoice
				,TRIM(ISNULL(TP.NAME,'')) AS InvoiceType
				,A.COMMENTS AS Comments
				,A.SODTYPE AS SodType
				,C.CODE AS TraderId
				,C.NAME AS TraderName
				,A.TRNDATE AS CreatedOnUtc
				,A.TPRMS AS InvoiceTypeId
				,CASE WHEN TP.FLG01 <> 0 THEN A.LTURNOVR * TP.FLG01 ELSE A.LTURNOVR END AS Amount
				,CASE WHEN TP.FLG01 <> 0 THEN A.LTRNVAL * TP.FLG01 ELSE A.LTRNVAL END AS FpaAmount
				,B.VATAMNT AS VatAmount
			FROM (TRDTRN A LEFT OUTER JOIN 
				FINDOC B ON A.FINDOC=B.FINDOC) LEFT OUTER JOIN 
				TRDR C ON A.TRDR=C.TRDR  LEFT OUTER JOIN 
				TPRMS TP ON A.TPRMS = TP.TPRMS  AND TP.SODTYPE=A.SODTYPE
			WHERE A.COMPANY IN (@pCompanyId)
				AND TP.COMPANY = A.COMPANY
				AND C.ISPROSP=0 
				AND A.PRJC IN (@pProjectId)
				AND ((A.SODTYPE <> 14 AND  A.SOSOURCE IN (1212, 1313, 1312, 1361, 1351, 1353, 1553, 1251, 1261, 1253, 1653, 1381, 1413, 1415, 1581, 1281, 1412, 1416, 1681, 1717, 1140) ) OR (A.SODTYPE=14 AND ((A.SOSOURCE IN (1453,1481) OR (A.SOSOURCE=1414 AND A.TRDTRN=0)))) OR (B.TFPRMS=451 AND B.SOSOURCE=1151)) 
			ORDER BY A.TRNDATE
        ";

    }
}
