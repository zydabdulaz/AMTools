using System;
using System.Drawing;
using System.Windows.Forms;

namespace ArdysaModsTools.Services
{
    public class LoggerService
    {
        private readonly RichTextBox _console;

        public LoggerService(RichTextBox console)
        {
            _console = console;
        }

        public void Log(string msg) => Write(msg, _console.ForeColor);
        public void Info(string msg) => Write(msg, Color.LightGray);
        public void Warn(string msg) => Write(msg, Color.Orange);
        public void Error(string msg) => Write(msg, Color.Red);

        private void Write(string msg, Color color)
        {
            if (_console.InvokeRequired)
            {
                _console.Invoke(new Action(() => Write(msg, color)));
                return;
            }

            string timestamp = DateTime.Now.ToString("HH:mm");
            string text = $"[{timestamp}] {msg}\r\n";

            int start = _console.TextLength;
            _console.AppendText(text);
            _console.SelectionStart = start;
            _console.SelectionLength = text.Length;
            _console.SelectionColor = color;
            _console.ScrollToCaret();
        }
    }
}
