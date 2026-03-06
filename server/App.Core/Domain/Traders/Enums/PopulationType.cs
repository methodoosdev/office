namespace App.Core.Domain.Traders
{
    public enum PopulationType //Πληθυσμιακή κλίμακα
    {
        AreaUpTwoHundredThousandAndMore = 0, //Περιοχή με πληθυσμό διακόσιες χιλιάδες και άνω
        AreaUnderTwoHundredThousand = 1, //Περιοχή με πληθυσμό κάτω των διακοσίων χιλιάδων,
        IslandsUnderThreeThousandOneHundred = 2, //Νησιά κάτω των τριών χιλιάδων εκατό
        NonTouristAreaUnderFiveHundred = 3 //Μη τουριστική περιοχή κάτω των πεντακόσιων
    }
}
