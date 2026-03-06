namespace App.Web.Infra.Queries.Accounting
{
    public static partial class MyDataQuery
    {
        public static string Products => @"
            SELECT 
	            A.CODE AS Code, V.VAT AS VatId, V.NAME AS VatName, S.SOCURRENCY AS CurrencyId, S.NAME AS CurrencyName, 
	            '(' + A.CODE + ') ' + A.NAME + ', ' + '(' + CAST(V.VAT AS varchar) + ') ' + V.NAME + ', ' + '(' + CAST(S.SOCURRENCY AS varchar) + ') ' + S.NAME AS [Description]
            FROM MTRL AS A INNER JOIN
	            VAT AS V ON A.VAT = V.VAT INNER JOIN
	            SOCURRENCY AS S ON A.SOCURRENCY = S.SOCURRENCY
            WHERE A.COMPANY=@pCompanyId AND A.SODTYPE=51 AND A.ISACTIVE = 1
            ORDER BY A.CODE
        ";

        public static string SpecialSuppliers => @"
            SELECT 
	            A.CODE AS Code, V.VAT AS VatId, V.NAME AS VatName, 
	            '(' + A.CODE + ') ' + A.NAME + ', ' + '(' + CAST(V.VAT AS varchar) + ') ' + V.NAME AS [Description]
            FROM MTRL AS A INNER JOIN
	            VAT AS V ON A.VAT = V.VAT
            WHERE A.COMPANY=@pCompanyId AND A.SODTYPE=53 AND A.ISACTIVE = 1
            ORDER BY A.CODE
        ";

        public static string Customers => @"
            SELECT A.CODE AS Code, '(' + A.CODE + ') ' + A.NAME AS [Description], A.AFM AS Vat
            FROM TRDR A 
            WHERE A.COMPANY=@pCompanyId AND A.SODTYPE=13 AND NOT(ISNULL(A.AFM, '') = '')
            ORDER BY A.CODE
        ";

        public static string Suppliers => @"
            SELECT A.CODE AS Code, '(' + A.CODE + ') ' + A.NAME AS [Description], A.AFM AS Vat
            FROM TRDR A 
            WHERE A.COMPANY=@pCompanyId AND A.SODTYPE=12 AND NOT(ISNULL(A.AFM, '') = '')
            ORDER BY A.CODE
        ";

        public static string VatProvisions => @"
            SELECT A.CODE AS Code, '(' + CAST(A.CODE AS varchar) + ') ' + A.NAME AS [Description] FROM VATPROVISIONS A ORDER BY A.CODE
        ";

        public static string SalesSeries => @"
            SELECT A.SERIES AS Code,('(' + CAST(A.SERIES AS varchar) + ') ' + A.NAME) AS [Description]
            FROM SERIES A 
            WHERE A.COMPANY=@pCompanyId AND A.SOSOURCE=1351 AND A.ISACTIVE=1
            ORDER BY A.SERIES
        ";

        public static string PurchasesSeries => @"
            SELECT A.SERIES AS Code,('(' + CAST(A.SERIES AS varchar) + ') ' + A.NAME) AS [Description]
            FROM SERIES A 
            WHERE A.COMPANY=@pCompanyId AND A.SOSOURCE=1251 AND A.ISACTIVE=1
            ORDER BY A.SERIES
        ";

        public static string ExpensesSeries => @"
            SELECT A.SERIES AS Code,('(' + CAST(A.SERIES AS varchar) + ') ' + A.NAME) AS [Description]
            FROM SERIES A 
            WHERE A.COMPANY=@pCompanyId AND A.SOSOURCE=1253 AND A.ISACTIVE=1
            ORDER BY A.SERIES
        ";

        public static string Sales => @"
            SELECT 
                A.TRNDATE AS InvoiceDate, 
                A.FINCODE AS Invoice,
                ISNULL(A.NETAMNT,0) AS NetAmount,
                C.AFM AS Vat,
                A.VARCHAR01 AS Mark
            FROM FINDOC A LEFT OUTER JOIN MTRDOC B ON A.FINDOC=B.FINDOC LEFT OUTER JOIN TRDR C ON A.TRDR=C.TRDR
            WHERE 
                A.COMPANY=@pCompanyId AND 
                A.TRNDATE>=@pStartDate AND 
                A.TRNDATE<=@pEndDate AND 
                A.SOSOURCE=1351 AND A.SOREDIR=0 AND A.SODTYPE=13 
            ORDER BY A.TRNDATE
        ";

        public static string Purchases => @"
            SELECT 
                A.TRNDATE AS InvoiceDate,
                A.FINCODE AS Invoice,
                ISNULL(A.NETAMNT,0) AS NetAmount,
                D.AFM AS Vat,
                A.VARCHAR01 AS Mark
            FROM FINDOC A LEFT OUTER JOIN MTRDOC B ON A.FINDOC=B.FINDOC LEFT OUTER JOIN TRDR D ON A.TRDR=D.TRDR
            WHERE 
                A.COMPANY=@pCompanyId AND 
                A.TRNDATE>=@pStartDate AND 
                A.TRNDATE<=@pEndDate AND 
                A.SOSOURCE=1251 AND A.SODTYPE=12 
            ORDER BY A.TRNDATE
        ";

        public static string Expenses => @"
            SELECT 
                A.TRNDATE AS InvoiceDate,
                A.FINCODE AS Invoice,
                ISNULL(A.NETAMNT,0) AS NetAmount, 
                D.AFM AS Vat,
                A.VARCHAR01 AS Mark
            FROM FINDOC A LEFT OUTER JOIN MTRDOC B ON A.FINDOC=B.FINDOC LEFT OUTER JOIN TRDR D ON A.TRDR=D.TRDR
            WHERE 
                A.COMPANY=@pCompanyId AND 
                A.TRNDATE>=@pStartDate AND 
                A.TRNDATE<=@pEndDate AND 
                A.SOSOURCE=1253 AND A.SODTYPE=12 
            ORDER BY A.TRNDATE
        ";

        public static string __Products => @"
            SELECT 
	            A.CODE AS Code, 
	            A.CODE + ' : ' + A.NAME AS [Description] ,
	            V.VAT AS VatId, 
	            CAST(V.VAT AS varchar) + ' : ' + V.NAME AS VatName, 
	            S.SOCURRENCY AS CurrencyId, 
	            CAST(S.SOCURRENCY AS varchar) + ' : ' + S.NAME AS CurrencyName
            FROM MTRL AS A INNER JOIN
	            VAT AS V ON A.VAT = V.VAT INNER JOIN
	            SOCURRENCY AS S ON A.SOCURRENCY = S.SOCURRENCY
            WHERE A.COMPANY=@pCompanyId AND A.SODTYPE=51 
            ORDER BY A.CODE
        ";

    }
}
