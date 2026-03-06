using System;

namespace App.Core.Domain.Payroll
{
    [Flags]
    public enum WorkerScheduleType //Τύπος ωραρίου
    {
        Daily = 1, // Ημερήσιο
        Weekly = 2, // Εβδομαδιαίο
        Monhly = 4, // Μηνιαίο
        Free = 8 // Ελεύθερο
    }
    [Flags]
    public enum WorkerScheduleModeType //Κατάσταση ωραρίου
    {
        Waiting = 1, // Σε αναμονή
        Submited = 2, // Υποβληθηκε
        Canceled = 4 // Ακυρώθηκε
    }
}
