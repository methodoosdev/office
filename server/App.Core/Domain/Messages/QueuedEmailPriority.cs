using System;

namespace App.Core.Domain.Messages
{
    /// <summary>
    /// Represents priority of queued email
    /// </summary>
    [Flags]
    public enum QueuedEmailPriority
    {
        /// <summary>
        /// Low
        /// </summary>
        Low = 0,

        /// <summary>
        /// High
        /// </summary>
        High = 5
    }
}
