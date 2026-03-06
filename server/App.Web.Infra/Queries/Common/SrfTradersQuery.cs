namespace App.Web.Infra.Queries.Common
{
    public static partial class SrfTradersQuery
    {
        public static string Get => @"
                SELECT
	                C.ID AS SrfId, 
				    CASE
		                WHEN C.FirstName IS NULL THEN ''
		                WHEN TRIM(LOWER(C.FirstName)) = TRIM(LOWER(C.LastName)) THEN ''
						ELSE C.FirstName
	                END AS FirstName,ISNULL(C.LastName, '') AS LastName, ISNULL(C.TaxCode, '') AS Vat, ISNULL(C.Email, '') AS Email,
                    C.TaxSystemID AS TaxSystemId, C.MisthExtraID AS HyperPayrollId, CA.IkaUserName AS EmployerIkaUserName, 
	                CA.IkaPassword AS EmployerIkaPassword, CA.TaxisUserName, CA.TaxisPassword, CA.OaeeUserName, CA.OaeePassword, CA.SepeUserName, CA.SepePassword, 
	                CA.TaxisSpecialName AS SpecialTaxisUserName, CA.TaxisSpecialPassword AS SpecialTaxisUserPassword, CA.EfkaUserName, CA.EfkaPassword, 
	                ISNULL(CO.CompanyErpID, 0) AS CompanyId, CO.DbName AS LogistikiDataBaseName, CO.Password AS LogistikiPassword, CO.UserName AS LogistikiUsername, 
	                CO.IPAddress AS LogistikiIpAddress, CO.Port AS LogistikiPort, ISNULL(CO.ERPType, 0) AS LogistikiProgramTypeId, C.IsActive as Active,
	                CASE
		                WHEN C.CategoryCompany = 0 THEN 2 
		                WHEN C.CategoryCompany = 1 THEN 3
		                ELSE 0
	                END AS CategoryBookTypeId,
					650 AS TaxesFee, 500 AS AccountingSchema, 60 AS EmployerBreakLimit
                FROM
	                Customers AS C LEFT OUTER JOIN
	                CustomerAccounts AS CA ON C.ID = CA.customer_ID LEFT OUTER JOIN
	                Connections AS CO ON C.ID = CO.CustomerID
				WHERE C.IsActive = 1
                ORDER BY C.LastName
            ";
        public static string With => @"
                SELECT
	                C.ID AS SrfId, 
				    CASE
		                WHEN C.FirstName IS NULL THEN ''
		                WHEN TRIM(LOWER(C.FirstName)) = TRIM(LOWER(C.LastName)) THEN ''
						ELSE C.FirstName
	                END AS FirstName,ISNULL(C.LastName, '') AS LastName, ISNULL(C.TaxCode, '') AS Vat, ISNULL(C.Email, '') AS Email,
                    C.TaxSystemID AS TaxSystemId, C.MisthExtraID AS HyperPayrollId, CA.IkaUserName AS EmployerIkaUserName, 
	                CA.IkaPassword AS EmployerIkaPassword, CA.TaxisUserName, CA.TaxisPassword, CA.OaeeUserName, CA.OaeePassword, CA.SepeUserName, CA.SepePassword, 
	                CA.TaxisSpecialName AS SpecialTaxisUserName, CA.TaxisSpecialPassword AS SpecialTaxisUserPassword, CA.EfkaUserName, CA.EfkaPassword, 
	                ISNULL(CO.CompanyErpID, 0) AS CompanyId, CO.DbName AS LogistikiDataBaseName, CO.Password AS LogistikiPassword, CO.UserName AS LogistikiUsername, 
	                CO.IPAddress AS LogistikiIpAddress, CO.Port AS LogistikiPort, ISNULL(CO.ERPType, 0) AS LogistikiProgramTypeId, C.IsActive as Active,
	                CASE
						WHEN C.CategoryCompany = 0 THEN 2 
						WHEN C.CategoryCompany = 1 THEN 3
						ELSE 0
					END AS CategoryBookTypeId,
					650 AS TaxesFee, 500 AS AccountingSchema, 60 AS EmployerBreakLimit
                FROM
	                Customers AS C LEFT OUTER JOIN
	                CustomerAccounts AS CA ON C.ID = CA.customer_ID LEFT OUTER JOIN
	                Connections AS CO ON C.ID = CO.CustomerID
				WHERE C.IsActive = 1
					AND C.ID IN @@pList
                ORDER BY C.LastName
            ";
    }
}
