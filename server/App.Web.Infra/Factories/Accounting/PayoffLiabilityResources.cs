using System.Collections.Generic;

namespace App.Web.Infra.Factories.Accounting
{
    public static class PayoffLiabilityResources
    {
        private static string CreateLikeExpressionValues(string[] values)
        {
            string[] list = new string[values.Length];
            for (int i = 0; i < values.Length; i++)
                list[i] = $"A.CODE LIKE '{values[i]}'";

            var text = string.Join(" OR ", list);

            return text;
        }

        public static Dictionary<string, string[]> Numerals => new()
        {
            //Κυκλοφορούν ενεργητικό,
            ["CurrentAssets"] = new string[] { "2%", "30%", "31%", "33.01%", "33.02%", "33.03%", "37%", "38%" },
            //Βραχυπρόθεσμες υποχρεώσεις ,
            ["ShortTermLiabilities"] = new string[] { "50%", "51%", "52.01%", "53%", "54%", "55%", "56%" },
            //Αποθέματα ,
            ["Inventories"] = new string[] { "2_._1%" },
            //Διαθέσιμα ,
            ["Available"] = new string[] { "38%" },
            //Απαιτήσεις ,
            ["Requirements"] = new string[] { "30%", "31%" },
            //Ετήσιες πωλήσεις,
            ["AnnualSales"] = new string[] { "70%" },
            //Κόστος πωληθέντων ,
            ["CostOfGoodsSold"] = new string[] { }, // "2%" - "2_._1"%
            //Αποσβέσεις ,
            ["Depreciation"] = new string[] { "66%" },
            //Λειτουργικά έξοδα ,
            ["OperatingExpenses"] = new string[] { "60%", "64%", "65%" },
            // 'Ιδια κεφάλαια 
            ["Equity"] = new string[] { "4%" },
            // Συνολικά κεφάλαια 
            ["TotalFunds"] = new string[] { "4%", "5%" },
            // Ξένα κεφάλαια 
            ["ForeignCapitals"] = new string[] { "5%" },
            // Σύνολο υποχρεώσεων 
            ["TotalLiabilities"] = new string[] { "5%" },
            // Σύνολο ενεργητικού 
            ["TotalAssets"] = new string[] { "1%", "2%", "3%" },
            // Δανειακές υποχρεώσεις
            ["LoanObligations"] = new string[] { "52%", "53.01%", "53.02%" },
            // Ετήσιο εργατικό κόστος 
            ["AnnualLaborCost"] = new string[] { "60%" },
            // Σύνολο παθητικού 
            ["TotalLiability"] = new string[] { "4%", "5%" },
            // Αξία κτησης παγίων 
            ["AcquisitionValueOfFixedAssets"] = new string[] { "11.01%", "12.01%", "13.01%", "14.01%", "15.01%", "16.01%", "17.01%", "18.01%" },
            // Σωρευμένες αποσβέσεις 
            ["AccumulatedDepreciation"] = new string[] { "11.02%", "12.02%", "13.02%", "14.02%", "15.02%", "16.02%", "17.02%", "18.02%" },
            // Ετήσιοι τόκοι
            ["AnnualInterest"] = new string[] { "65%" },
            // Κέρδη προ τόκων ή φόρων (ΕΒΙΤ ή λειτουργικά) 
            ["EarningsBeforeInterestAndTaxes"] = new string[] { },
            // ("70%", "71%", "72%", "73%", "74%", "75%", "76%", "77%", "79%") - (Κόστος πωληθέντων - "60%", "61%", "62%", "63%", "64%", "66%", "68%")
            // Μικτό περιθώριο κέρδους
            ["GrossProfitMargin"] = new string[] { },// Ετήσιες πωλήσεις -  Κόστος πωληθέντων
            // Καθαρό κέρδος 
            ["NetProfit"] = new string[] { },
            // ("70%", "71%", "72%", "73%", "74%", "75%", "76%", "77%", "79%") - (Κόστος πωληθέντων - "60%", "61%", "62%", "63%", "64%", "65%", "66%", "68%")
            // Έξοδα και αμοιβές προσωπικού
            ["StaffExpensesAndFees"] = new string[] { "60%" },
            // Αριθμός απασχολούμενων 
            ["NumberOfEmployees"] = new string[] { },
        };

        public static Dictionary<string, string> Expressions()
        {
            var list = new Dictionary<string, string>();

            foreach (var nums in Numerals)
            {
                var expression = CreateLikeExpressionValues(nums.Value);
                list.Add(nums.Key, expression);
            }

            return list;
        }
        //public static Dictionary<string, string> Expressions => _Expressions();
    }
}