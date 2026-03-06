using System;

namespace App.Core.Domain.SimpleTask
{
    [Flags]
    public enum SimpleTaskPriorityType // Τύπος Προτεραιότητα εργασίας
    {
        Uninterested = 1, // Αδιάφορη
        Low = 2, // Χαμηλή
        Normal = 4, // Κανονική
        Emergency = 8, // Επείγον
        Immediate = 16 // Άμεση
    }
    [Flags]
    public enum SimpleTaskType // Τύπος Κατάσταση εργασίας
    {
        ToStart = 1, // Προς έναρξη
        InProgress = 2, // Σε εξέλιξη
        Completed = 4, // Ολοκληρώθηκε
        Canceled = 8, // Ακυρώθηκε
        Uninterested = 16, // Αδιάφορο
        Waiting = 32, // Σε αναμονή
        Postponed = 64 // Σε αναβολή
    }
}
