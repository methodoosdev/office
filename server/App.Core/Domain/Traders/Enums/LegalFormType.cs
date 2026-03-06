namespace App.Core.Domain.Traders
{
    public enum LegalFormType // Νομική μορφή
    {
        None = 0, // Κενό
        Ae = 1, // Α.Ε.
        PublicService = 2, //Δημόσια υπηρεσία
        MunicipalCompany =3,//Δημοτική επιχείρηση
        Εe = 4,//Ε.Ε
        Ltd = 5,//Ε.Π.Ε.
        ImeLtd = 6,//Ι.Μ.Ε Ε.Π.Ε
        Consortium = 7,//Κοινοπραξία
        Society = 8,//Κοινωνία
        Other = 9,//Λοιπά
        NpNonprofit = 10,//Ν.Π. Μη Κερδοσκοπικό
        ShippingCompany = 11,//Ναυτική Εταιρεία
        Oe = 12,//Ο.Ε.
        CoOwnership = 13,//Συμπλοιοκτησία
        Cooperative = 14,//Συνεταιρισμός
        Ike = 15//Ι.Κ.Ε.
    }
}
