namespace AMTools.Core
{
    public class OperationResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public static OperationResult Ok(string msg) =>
            new OperationResult { Success = true, Message = msg };

        public static OperationResult Fail(string msg) =>
            new OperationResult { Success = false, Message = msg };
    }
}
