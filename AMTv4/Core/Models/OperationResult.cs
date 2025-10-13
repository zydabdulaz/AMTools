using System;

namespace ArdysaModsTools.Core.Models
{
    /// <summary>
    /// Represents a simple success/failure result returned by services and controllers.
    /// </summary>
    public class OperationResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public Exception? Exception { get; set; }
    }
}
