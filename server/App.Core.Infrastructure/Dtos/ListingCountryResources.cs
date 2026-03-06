using System.Collections.Generic;

namespace App.Core.Infrastructure.Dtos
{
    public static partial class ListingCountryResources
    {
        public static Dictionary<string, string> CountryDict => new()
        {
            ["AT"] = "Αυστρία - AT",
            ["BE"] = "Βέλγιο - BE",
            ["BG"] = "Βουλγαρία - BG",
            ["XI"] = "Β. Ιρλανδία - XI",
            ["FR"] = "Γαλλία - FR",
            ["DE"] = "Γερμανία - DE",
            ["DK"] = "Δανία - DK",
            ["EE"] = "Εσθονία - EE",
            ["IE"] = "Ιρλανδία - IE",
            ["ES"] = "Ισπανία - ES",
            ["IT"] = "Ιταλία - IT",
            ["HR"] = "Κροατία - HR",
            ["CY"] = "Κύπρος - CY",
            ["LV"] = "Λετονία - LV",
            ["LT"] = "Λιθουανία - LT",
            ["LU"] = "Λουξεμβούργο - LU",
            ["MT"] = "Μάλτα - MT",
            ["NL"] = "Ολλανδία - NL",
            ["HU"] = "Ουγγαρία - HU",
            ["PL"] = "Πολωνία - PL",
            ["PT"] = "Πορτογαλία - PT",
            ["RO"] = "Ρουμανία - RO",
            ["SK"] = "Σλοβακία - SK",
            ["SI"] = "Σλοβενία - SI",
            ["SE"] = "Σουηδία - SE",
            ["CZ"] = "Τσεχία - CZ",
            ["FI"] = "Φινλανδία - FI"
        };
    }
}