namespace App.Web.Infra.Queries.Accounting
{
    public static partial class AccountingQuery
    {
        public static string Get => @"
            SELECT A.CODE AS AccountingCode, B.FISCPRD AS Year, B.PERIOD AS Periodos, SUM(B.LDEBITTMP) AS Debit, SUM(B.LCREDITTMP) AS Credit
            FROM ACNT A, ACNBALSHEET B
            WHERE A.ACNT = B.ACNT
                AND A.SODTYPE = 89
                AND B.COMPANY = @pCompanyId
                AND B.FISCPRD = @pYear
                AND A.CODE LIKE @pCode
            GROUP BY A.CODE, B.FISCPRD, B.PERIOD
            ORDER BY B.PERIOD,A.CODE, B.FISCPRD
            ";
        public static string MonthlyFinancialReport => @"
            SELECT A.CODE AS AccountingCode, B.FISCPRD AS Year, B.PERIOD AS Periodos, SUM(B.LDEBIT) AS Debit, SUM(B.LCREDIT) AS Credit
            FROM ACNT A, ACNBALSHEET B
            WHERE A.ACNT = B.ACNT
                AND A.SODTYPE = 89
                AND B.COMPANY = @pCompanyId
                AND B.FISCPRD = @pYear
                AND A.CODE LIKE @pCode
            GROUP BY A.CODE, B.FISCPRD, B.PERIOD
            ORDER BY B.PERIOD,A.CODE, B.FISCPRD
            ";
        public static string PrintAccounting => @"
            SELECT DISTINCT B.CODE AS AccountingCode, B.NAME AS Description
            FROM ACNTRN A LEFT OUTER JOIN ACNT B ON A.ACNT=B.ACNT
            WHERE B.ACNSCHEMA=500 AND B.CODE LIKE @pCode
            ORDER BY B.CODE
            ";
    }
}
