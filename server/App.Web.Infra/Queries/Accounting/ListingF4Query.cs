using App.Core.Domain.Traders;

namespace App.Web.Infra.Queries.Accounting
{
    public class ListingF4Query : TraderFactoryQuery
    {
        public ListingF4Query(int logistikiProgramTypeId, int categoryBookTypeId) : base(logistikiProgramTypeId, categoryBookTypeId)
        {
        }

        public override string SoftOne_C => @"
			SELECT F.FINDOC AS InvId, F.TRNDATE AS InvDate,F.PERIOD AS Periodos, F.FINCODE AS InvSeries, 
				SUBSTRING(B.CODE,1,5) AS Part, B.CODE AS Code, A.CREDIT-A.DEBIT AS [Value],
				SUBSTRING(D.AFM,1,2) AS CountryCode,
				--SUBSTRING(D.AFM,3,LEN(D.AFM)-1) AS Vat,
				D.AFM AS VatNumber,
				CASE 
					WHEN LEN(D.AFM) > 2 THEN RIGHT(D.AFM, LEN(D.AFM) - 2)
					ELSE D.AFM
				END AS Vat,
				F.SHIPKIND AS ShipKind
			FROM ACNTRN A 
				LEFT OUTER JOIN ACNT B ON A.ACNT=B.ACNT
				LEFT OUTER JOIN ACNEDIT AS E ON A.ACNEDIT = E.ACNEDIT
				LEFT OUTER JOIN FINDOC AS F ON E.FINDOC = F.FINDOC
				LEFT OUTER JOIN TRDR AS D ON F.TRDR = D.TRDR
				LEFT OUTER JOIN COUNTRY AS C ON D.COUNTRY = C.COUNTRY
			WHERE 
				A.ACNSCHEMA = @pSchema
				AND A.COMPANY = @pCompanyId
				AND A.FISCPRD = @pYear
				AND F.PERIOD = @pPeriod
				AND (SUBSTRING(B.CODE,1,5) IN ('70.01', '70.03', '70.05', '70.07', '71.04'))
				AND (C.COUNTRYTYPE = 1)
				AND (C.INTERCODE <> 'GR')
			ORDER BY F.FINCODE
        ";

        public override string SoftOne_B => @"
			SELECT F.FINDOC AS InvId, F.TRNDATE AS InvDate,F.PERIOD AS Periodos, 
				F.FINCODE AS InvSeries, SUBSTRING(B.CODE,1,5) AS Part, B.CODE AS Code,
				CASE WHEN FPRMS IN (30160) THEN A.NETLINEVAL * -1 ELSE A.NETLINEVAL END AS [Value],
				SUBSTRING(D.AFM,1,2) AS CountryCode,
				--SUBSTRING(D.AFM,3,LEN(D.AFM)-1) AS Vat,
				D.AFM AS VatNumber,
				CASE 
					WHEN LEN(D.AFM) > 2 THEN RIGHT(D.AFM, LEN(D.AFM) - 2)
					ELSE D.AFM
				END AS Vat,
				F.SHIPKIND AS ShipKind
			FROM MTRLINES A 
				LEFT OUTER JOIN MTRL B ON A.MTRL=B.MTRL
				LEFT OUTER JOIN FINDOC F ON F.FINDOC=A.FINDOC
				LEFT OUTER JOIN TRDR AS D ON F.TRDR = D.TRDR 
				LEFT OUTER JOIN COUNTRY AS C ON D.COUNTRY = C.COUNTRY
			WHERE A.COMPANY = @pCompanyId
				AND F.FISCPRD = @pYear
				AND F.PERIOD = @pPeriod
				AND (SUBSTRING(B.CODE,1,5) IN ('70.01', '70.03', '70.05', '70.07', '71.04'))
				AND (C.COUNTRYTYPE = 1)
				AND (C.INTERCODE <> 'GR')
			ORDER BY F.FINCODE
        ";

        public override string Prosvasis => @"
			SELECT F.FINDOC AS InvId, F.TRNDATE AS InvDate,F.PERIOD AS Periodos, 
				F.FINCODE AS InvSeries, SUBSTRING(B.CODE,1,5) AS Part, B.CODE AS Code,
				CASE WHEN FPRMS IN (30160) THEN A.NETLINEVAL * -1 ELSE A.NETLINEVAL END AS [Value],
				SUBSTRING(D.AFM,1,2) AS CountryCode,
				--SUBSTRING(D.AFM,3,LEN(D.AFM)-1) AS Vat,
				D.AFM AS VatNumber,
				CASE 
					WHEN LEN(D.AFM) > 2 THEN RIGHT(D.AFM, LEN(D.AFM) - 2)
					ELSE D.AFM
				END AS Vat,
				F.SHIPKIND AS ShipKind
			FROM MTRLINES A 
				LEFT OUTER JOIN MTRL B ON A.MTRL=B.MTRL
				LEFT OUTER JOIN FINDOC F ON F.FINDOC=A.FINDOC
				LEFT OUTER JOIN TRDR AS D ON F.TRDR = D.TRDR 
				LEFT OUTER JOIN COUNTRY AS C ON D.COUNTRY = C.COUNTRY
			WHERE A.COMPANY = @pCompanyId
				AND F.FISCPRD = @pYear
				AND F.PERIOD = @pPeriod
				AND (SUBSTRING(B.CODE,1,5) IN ('70.01', '70.03', '70.05', '70.07', '71.04'))
				AND (C.COUNTRYTYPE = 1)
				AND (C.INTERCODE <> 'GR')
			ORDER BY F.FINCODE
        ";

        public override string Prosvasis_C => @"
			SELECT F.FINDOC AS InvId, F.TRNDATE AS InvDate,F.PERIOD AS Periodos, 
				F.FINCODE AS InvSeries, SUBSTRING(B.CODE,1,5) AS Part, B.CODE AS Code,
				CASE WHEN FPRMS IN (30160) THEN A.NETLINEVAL * -1 ELSE A.NETLINEVAL END AS [Value],
				SUBSTRING(D.AFM,1,2) AS CountryCode,
				--SUBSTRING(D.AFM,3,LEN(D.AFM)-1) AS Vat,
				D.AFM AS VatNumber,
				CASE 
					WHEN LEN(D.AFM) > 2 THEN RIGHT(D.AFM, LEN(D.AFM) - 2)
					ELSE D.AFM
				END AS Vat,
				F.SHIPKIND AS ShipKind
			FROM MTRLINES A 
				LEFT OUTER JOIN MTRL B ON A.MTRL=B.MTRL
				LEFT OUTER JOIN FINDOC F ON F.FINDOC=A.FINDOC
				LEFT OUTER JOIN TRDR AS D ON F.TRDR = D.TRDR 
				LEFT OUTER JOIN COUNTRY AS C ON D.COUNTRY = C.COUNTRY
			WHERE A.COMPANY = @pCompanyId
				AND F.FISCPRD = @pYear
				AND F.PERIOD = @pPeriod
				AND (SUBSTRING(B.CODE,1,5) IN ('70.01', '70.03', '70.05', '70.07', '71.04'))
				AND (C.COUNTRYTYPE = 1)
				AND (C.INTERCODE <> 'GR')
			ORDER BY F.FINCODE
        ";
    }
}
