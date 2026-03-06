namespace App.Web.Infra.Queries.Accounting
{
    public static partial class NumeratorsQuery
    {
        //Ενα γενικό ερώτημα όπου επιστρέφει όλες τις εγγραφές ενός λογαριασμού
        //σε ένα συγκεκριμένο διάστημα
        public static string PayoffShortTermLiabilities => @"
            SELECT A.TRNDATE AS GroupDate, SUM(A.LDEBIT) AS Debit, SUM(A.LCREDIT) AS Credit, 0 AS ProgressiveValue
            FROM ACNT AS M LEFT OUTER JOIN
                ACNTRN AS A ON A.ACNT = M.ACNT LEFT OUTER JOIN
                PRJC AS P ON A.COMPANY = P.COMPANY AND A.PRJC = P.PRJC
            WHERE (A.SODTYPE = 89) AND (A.ISCANCEL <> 1) AND (A.ISGL = 1) AND (M.SODTYPE = 89) AND 
                (A.APPRV = 1) AND (A.PERIOD >= 1) AND (A.PERIOD <= 12) AND (M.CODE LIKE @pAccountingCode) AND (A.COMPANY = @pCompanyId)
            GROUP BY A.TRNDATE
            ORDER BY GroupDate
            ";
    }
}
