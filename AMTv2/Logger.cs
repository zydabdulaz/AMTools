namespace ArdysaModsTools.Services
{
    public interface ILogger
    {
        void Log(string message);
    }

    public class Logger : ILogger
    {
        private readonly TextBox _consoleLog;
        private readonly List<string> _buffer = new();

        public Logger(TextBox consoleLog)
        {
            _consoleLog = consoleLog;
        }

        public void Log(string message)
        {
            if (!_consoleLog.IsHandleCreated)
            {
                _buffer.Add(message);
                return;
            }

            string timestamp = DateTime.Now.ToString("HH:mm");
            string fullMessage = $"[{timestamp}] {message}\r\n";
            _consoleLog.BeginInvoke((Action)(() =>
            {
                _consoleLog.AppendText(fullMessage);
                _consoleLog.SelectionStart = _consoleLog.TextLength;
                _consoleLog.ScrollToCaret();
            }));
        }
    }
}
