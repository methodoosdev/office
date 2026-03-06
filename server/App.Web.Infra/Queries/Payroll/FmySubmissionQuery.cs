namespace App.Web.Infra.Queries.Payroll
{
    public static partial class FmySubmissionQuery
    {
        public static string Get => @"            
			WITH fmy AS (SELECT ID_CMP AS CompanyId,
				SUM(M_APODOXES) AS GrossEarnings,
				CAST(SUM(FMY) AS DECIMAL(18,2)) AS TaxAmount,
				CAST(SUM(FMY_EKT_EISF) AS DECIMAL(18,2)) AS Contribution,
				CAST(SUM(XARTOSHMO_VALUE) AS DECIMAL(18,2)) AS Stamp
			FROM PERIODOI_DATA AS PD
			WHERE XRISI = @pYear AND ID_CMP = @pCompanyId AND MONTH(PERIODOS_DATE) IN(@pMonth)
				AND EMP_TYPE <> 9
			GROUP BY ID_CMP) SELECT * FROM fmy
	   
			LEFT JOIN
			(SELECT ID_CMP AS CompanyId_Stamp,
				CAST(SUM(PD.M_APODOXES) AS DECIMAL(18,2)) AS StampTotal
			FROM PERIODOI_DATA AS PD
			WHERE PD.XRISI = @pYear AND ID_CMP = @pCompanyId
				AND ID_PERIODOS IN(@pMonth)
				AND PD.EMP_TYPE = 4 
				AND PD.EMP_SPECIAL_TYPE IN(12,20,23)
				AND pd.EMP_TYPE <> 7
				AND ISNULL(PD.XARTOSHMO, 0) > 0
				AND ISNULL(PD.ARTIST_TYPE, 0) <> 4
				AND PD.EMP_TYPE <> 9
			GROUP BY ID_CMP) AS N ON fmy.CompanyId = N.CompanyId_Stamp
	   
			LEFT JOIN
			(SELECT ID_CMP AS CompanyId_Comp, 
				CAST(SUM(M_APODOXES) AS DECIMAL(18,2)) AS Compensation
			FROM PERIODOI_DATA 
			WHERE XRISI = @pYear AND ID_CMP = @pCompanyId 
				AND ID_PERIODOS = 17 
				AND MONTH(PERIODOS_DATE) IN(@pMonth)
			GROUP BY ID_CMP) AS A ON fmy.CompanyId = A.CompanyId_Comp
            ";
        public static string Get1 => @"
            WITH fmy AS (SELECT ID_CMP AS CompanyId,
				SUM(M_APODOXES) AS GrossEarnings,
				--ID_PERIODOS AS DD,
				CAST(SUM(FMY) AS DECIMAL(18,2)) AS TaxAmount,
				CAST(SUM(FMY_EKT_EISF) AS DECIMAL(18,2)) AS Contribution,
				CAST(SUM(XARTOSHMO_VALUE) AS DECIMAL(18,2)) AS Stamp,
				FROM PERIODOI_DATA AS PD
				WHERE XRISI = @pYear AND ID_CMP = @pCompanyId AND MONTH(PERIODOS_DATE) IN(@pMonth)
				AND EMP_TYPE <> 9
				GROUP BY ID_CMP) SELECT * FROM fmy
	   
				LEFT JOIN
				(SELECT ID_CMP AS CompanyId_Stamp, 
				CAST(SUM(PD.M_APODOXES) AS DECIMAL(18,2)) AS StampTotal,
				FROM PERIODOI_DATA AS PD
				WHERE PD.XRISI = @pYear AND ID_CMP = @pCompanyId
				AND ID_PERIODOS IN(@pMonth)
				AND PD.EMP_TYPE = 4 
				AND PD.EMP_SPECIAL_TYPE IN(12,20,23)
				AND pd.EMP_TYPE <> 7
				AND ISNULL(PD.XARTOSHMO, 0) > 0
				AND ISNULL(PD.ARTIST_TYPE, 0) <> 4
				AND PD.EMP_TYPE <> 9
				GROUP BY ID_CMP) AS N ON fmy.CompanyId = N.CompanyId_Stamp
	   
				LEFT JOIN
				(SELECT ID_CMP AS CompanyId_Comp, 
				CAST(SUM(M_APODOXES) AS DECIMAL(18,2)) AS Compensation,
				FROM PERIODOI_DATA 
				WHERE XRISI = @pYear AND ID_CMP = @pCompanyId 
				AND ID_PERIODOS = 17 
				AND MONTH(PERIODOS_DATE) IN(@pMonth)
				GROUP BY ID_CMP) AS A ON fmy.CompanyId = A.CompanyId_Comp
            ";
    }
}
