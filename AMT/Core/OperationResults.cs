namespace ArdysaModsTools.Core
{
    /// <summary>
    /// Represents the result of an operation (success or failure) with a message.
    /// </summary>
    public class OperationResult
    {
        public bool Success { get; }
        public string Message { get; }

        private OperationResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        /// <summary>
        /// Creates a successful operation result.
        /// </summary>
        public static OperationResult Ok(string message = "Success") =>
            new OperationResult(true, message);

        /// <summary>
        /// Creates a failed operation result.
        /// </summary>
        public static OperationResult Fail(string message = "Error") =>
            new OperationResult(false, message);

        public override string ToString() => $"{(Success ? "OK" : "FAIL")}: {Message}";
    }
}
