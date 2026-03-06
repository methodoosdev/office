using System;

namespace App.Core.Domain.Common
{
    [Flags]
    public enum TristateType // Τύπος
    {
        Null = 0, // 
        True = 1, // Ναι
        False = 2 // Όχι
    }
}
