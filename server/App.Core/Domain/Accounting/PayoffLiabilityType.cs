namespace App.Core.Domain.Accounting
{
    public enum PayoffLiabilityType // Αριθμοδείκτες
    {
        // Ρευστότητας
        GeneralLiquidity = 1, // Γενική ρευστότητα
        ImmediateLiquidity = 2, // Άμεση ρευστότητα
        CashFlow = 3, // Ταμειακή ρευστότητα 
        AverageReceivablesCollectionTime = 4, // Μέσος χρόνος είσπραξης απαιτήσεων
        StockRecycling = 5, // Ανακύκλωση αποθεμάτων 
        PaymentTimeOfShortTermLiabilities = 6, // Χρόνος εξόφλησης βραχυπρόθεσμων υποχρεώσεων
        ReceivableCollectionTime = 7, // Χρόνος είσπραξης απαιτήσεων
        DefenseTimePeriod = 8, // Αμυντικού χρονικού διαστήματος

        // Κεφαλαιακής δομής και βιωσιμότητας
        Autonomy = 9, // Αυτονομίας
        Overdraft = 10, // Υπερχρέωσης
        CurrentAssetsToLiabilities = 11, // Κυκλοφορούν ενεργητικό προς υποχρεώσεις
        DebtBurden = 12, // Δανειακής επιβάρυνσης
        AssetsToLiabilities = 13, // Πάγια προς παθητικό
        CapitalIntensive = 14, // Εντάσεως κεφαλαίου
        AgingOfFixedAssets = 15, // Παλαιότητας παγίων
        InterestCoverage = 16, // Κάλυψης τόκων

        // Αποδοτικότητας
        GrossProfitMargin = 17, // Μεικτού περιθωρίου κέρδους %
        OperatingProfitMargin = 18, // Περιθωρίου λειτουργικών  κερδών %
        NetProfit = 19, // Καθαρού κέρδους
        OperatingPerformance = 20, // Απόδοση λειτουργίας

        // Εξόδων
        AssetsMaintenance = 21, // Συντήρησης παγίων
        OperatingExpenses = 22, // Εξόδων λειτουργίας
        StaffPerformance = 23, // Απόδοσης προσωπικού
        WagesToEmployeesNumber = 24, // Αμοιβές απασχολούμενων προς τον αριθμό τους
        ProfitsToWages = 25, // Κέρδη προς αμοιβές απασχολούμενων
        //CapitalIntensity = 26, // Εντάσεως κεφαλαίου
    }

    public enum PayoffLiabilityFactorType // Συντελεστές
    {
        CurrentAssets = 1, // Κυκλοφορούν ενεργητικό
        ShortTermLiabilities = 2, // Βραχυπρόθεσμες υποχρεώσεις
        Inventories = 3, // Αποθέματα 
        Available = 4, // Διαθέσιμα 
        Requirements = 5, // Απαιτήσεις 
        AnnualSales = 6, // Ετήσιες πωλήσεις 
        CostOfGoodsSold = 7, // Κόστος πωληθέντων 
        Depreciation = 8, // Αποσβέσεις 
        OperatingExpenses = 9,// Λειτουργικά έξοδα 
        Equity = 10, // 'Ιδια κεφάλαια 
        TotalFunds = 11, // Συνολικά κεφάλαια 
        ForeignCapitals = 12, // Ξένα κεφάλαια 
        TotalLiabilities = 13, // Σύνολο υποχρεώσεων 
        TotalAssets = 14, // Σύνολο ενεργητικού 
        LoanObligations = 15, // Δανειακές υποχρεώσεις
        AnnualLaborCost = 16, // Ετήσιο εργατικό κόστος 
        TotalLiability = 17, // Σύνολο παθητικού 
        AcquisitionValueOfFixedAssets = 18, // Αξία κτησης παγίων 
        AccumulatedDepreciation = 19, // Σωρευμένες αποσβέσεις 
        AnnualInterest = 20, // Ετήσιοι τόκοι
        EarningsBeforeInterestAndTaxes = 21, // Κέρδη προ τόκων ή φόρων (ΕΒΙΤ ή λειτουργικά) 
        GrossProfitMargin = 22, // Μικτό περιθώριο κέρδους
        NetProfit = 23, // Καθαρό κέρδος 
        StaffExpensesAndFees = 24, // Έξοδα και αμοιβές προσωπικού
        NumberOfEmployees = 25, // Αριθμός απασχολούμενων 
    }
    public enum PayoffLiabilityCategoryType // Κατηγορίες αριθμοδεικτών
    {
        Liquidity = 1, // Ρευστότητα
        CapitalStructureSustainability = 2, // Κεφαλαιακής δομής και βιωσιμότητας
        Efficiency = 3, // Αποδοτικότητας
        Expenses = 4 // Εξόδων
    }
}
