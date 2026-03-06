using App.Core.Domain.Traders;

namespace App.Web.Infra.Queries.Accounting
{
    public class VatCalculationRefundQuery : TraderFactoryQuery
    {
        public VatCalculationRefundQuery(int logistikiProgramTypeId, int categoryBookTypeId) : base(logistikiProgramTypeId, categoryBookTypeId)
        {
        }

        public override string SoftOne_C => @"
            SELECT B.CODE AS AccountingCode, 
                   A.PERIOD AS Period, 
                   CAST(A.CREDIT AS DECIMAL(18,2)) AS Debit, 
                   CAST(A.DEBIT AS DECIMAL(18,2)) AS Credit
            FROM ACNHEADER AS X INNER JOIN
                 ACNTRN AS A ON X.ACNEDIT = A.ACNEDIT AND X.ACNEDIT = A.ACNEDIT AND X.ACNHEADER = A.ACNHEADER INNER JOIN
                 ACNT B ON A.ACNT = B.ACNT
            WHERE X.COMPANY = @pCompanyId
                AND X.SODTYPE = 89
                AND (B.CODE LIKE ('54.02.03.____.99%')  )
                AND (X.FPRMS = 5499) 
                AND (X.SOSOURCE = 1089)
                --AND (A.FISCPRD = @pYear)
                AND X.TRNDATE >= @pStartingDate
                AND X.TRNDATE <= @pEndingDate
                AND (B.ACNSCHEMA = @pSchema)
            ORDER BY B.CODE
        ";

        public override string SoftOne_B => @"
        ";

        public override string Prosvasis => @"
        ";

        public override string Prosvasis_C => @"
            SELECT B.CODE AS AccountingCode, 
                   A.PERIOD AS Period, 
                   CAST(A.CREDIT AS DECIMAL(18,2)) AS Debit, 
                   CAST(A.DEBIT AS DECIMAL(18,2)) AS Credit
            FROM ACNHEADER AS X INNER JOIN
                 ACNTRN AS A ON X.ACNEDIT = A.ACNEDIT AND X.ACNEDIT = A.ACNEDIT AND X.ACNHEADER = A.ACNHEADER INNER JOIN
                 ACNT B ON A.ACNT = B.ACNT
            WHERE X.COMPANY = @pCompanyId
                AND X.SODTYPE = 89
                AND (B.CODE LIKE ('54.02.03.____.99%')  )
                AND (X.FPRMS = 5499) 
                AND (X.SOSOURCE = 1089)
                --AND (A.FISCPRD = @pYear)
                AND X.TRNDATE >= @pStartingDate
                AND X.TRNDATE <= @pEndingDate
                AND (B.ACNSCHEMA = @pSchema)
            ORDER BY B.CODE
        ";
    }
}
