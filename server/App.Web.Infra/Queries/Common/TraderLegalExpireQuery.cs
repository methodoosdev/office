namespace App.Web.Infra.Queries.Common
{
    public static partial class TraderLegalExpireQuery
    {
        public static string TraderLegalExpireItem => @"

        SELECT  [F_AFM] AS Vat
			   ,[TAXIS_USERNAME] AS TaxisUsername
			   ,[TAXIS_PASSWORD] AS TaxisPassword
			   ,[F_SURNAME_A]    AS LastName
			   ,[F_NAME]         AS FirstName
			   ,[F_TYPE]		 AS CustomerTypeHyper

        FROM [TaxSystem].[dbo].[PARTY]
  
        where  F_ACTIVATED < 1
		and F_AFM IS NOT NULL 
		and (TAXIS_USERNAME IS NOT NULL AND [TAXIS_PASSWORD] IS NOT NULL)
        ";
    }
}
