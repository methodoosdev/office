using System;

namespace App.Core.Domain.Financial
{
    [Flags]
    public enum FinancialObligationType
    {
        KeaoIka = 1,
        Efka = 2,
        EfkaNonSalaried = 4,
        Keao = 8,
        Oaee = 16,
        Aade = 32
    }
    public enum FinancialObligationChoiceType
    {
        IndividualNatural = 1,
        IndividualLeagal = 2
    }
}
