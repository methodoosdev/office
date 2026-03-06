using System;

namespace App.Core.Domain.Traders
{
    [Flags]
    public enum LogistikiProgramType
    {
        SoftOne = 1,
        Pylon = 2,
        Galaxy = 4,
        HyperL = 8,
        Atlantis = 16,
        Prosvasis = 32
    }
    [Flags]
    public enum PayrollProgramType
    {
        Prosvasis = 1,
        HyperM = 2
    }
}
