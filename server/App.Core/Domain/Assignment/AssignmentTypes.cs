using System;

namespace App.Core.Domain.Assignment
{
    [Flags]
    public enum AssignmentActionStatusType // Κατάσταση ενέργειας
    {
        InProgress = 1, // Σε εξέλιξη
        Completed = 2, // Ολοκληρώθηκε
        Canceled = 4 // Ακυρώθηκε
    }
    [Flags]
    public enum AssignmentActionPriorityType // Τύπος Προτεραιότητα εργασίας
    {
        Low = 1, // Χαμηλή
        Normal = 2, // Κανονική
        Emergency = 4, // Επείγον
        Immediate = 8 // Άμεση
    }
}
