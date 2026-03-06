        # Δημιουργια λογιστικου σχεδιου απο ολους τους πελατες
        
        public async Task<bool> DatabaseExistsAsync(string connectionString)
        {
            try
            {
                var builder = new SqlConnectionStringBuilder(connectionString)
                {
                    ConnectTimeout = 5
                };

                await using var connection = GetDbConnection(builder.ConnectionString);

                await connection.OpenAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }


        
            var query = @"
                SELECT 
	                T.CODE AS AccountingCode,
	                T.NAME AS [Description],
	                T.ACNTYPE AS [Type],
	                T.ACNSCHEMA AS [Schema],
	                T.ACNGRADE AS Grande,
	                T.BALTRANSFER AS Tranfer,
	                T.ACNMOVING AS Moving
                FROM   
	                ACNT AS T
	                JOIN (
		                SELECT TOP 1 ACNSCHEMA
		                FROM   FISCPRD
		                ORDER BY FISCPRD DESC
	                ) AS M ON T.ACNSCHEMA = M.ACNSCHEMA
                ORDER BY 
	                T.CODE, T.ACNSCHEMA
                ";

            var traders = (await _traderService.GetAllTradersAsync())
                .Where(x => x.CategoryBookType == CategoryBookType.C
                    && x.LogistikiProgramType == LogistikiProgramType.SoftOne
                    && x.Id != 684).ToList();

            var list1 = new List<_AccountingCode>();

            try
            {
                foreach (var trader in traders)
                {
                    var connectionResult = await _traderConnectionService.GetTraderConnectionAsync(trader.Id);
                    if (!connectionResult.Success)
                        continue;

                    var list2 = await _dataProvider.QueryAsync<_AccountingCode>(connectionResult.Connection, query);
                    if (!list2.Any(x => x.Grande == 5) || list2.Any(x => x.Schema == 1) )
                        continue;

                    list1 = list1.Concat(list2)
                      .DistinctBy(x => x.AccountingCode)
                      .ToList();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }