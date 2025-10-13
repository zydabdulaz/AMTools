using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ArdysaModsTools.Core.Services
{
    public interface ILogger
    {
        void Log(string message);
        void FlushBufferedLogs();
    }

    public class Logger : ILogger
    {
        private readonly RichTextBox _consoleLog;
        private readonly List<string> _buffer = new();

        public Logger(RichTextBox consoleLog)
        {
            _consoleLog = consoleLog ?? throw new ArgumentNullException(nameof(consoleLog));
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
                // Write timestamp first (in dim gray)
                int timestampStart = _consoleLog.TextLength;
                _consoleLog.AppendText($"[{timestamp}] ");
                _consoleLog.Select(timestampStart, 7); // [HH:mm]
                _consoleLog.SelectionColor = Color.DimGray;

                // Write message next, colored by type
                int msgStart = _consoleLog.TextLength;
                _consoleLog.AppendText($"{message}\r\n");

                Color color = DetermineColor(message);
                if (color != Color.Empty)
                    Colorize(msgStart, message.Length, color);

                // Scroll and restore caret
                _consoleLog.SelectionStart = _consoleLog.TextLength;
                _consoleLog.ScrollToCaret();
            }));
        }

        public void FlushBufferedLogs()
        {
            if (_buffer.Count == 0 || !_consoleLog.IsHandleCreated)
                return;

            foreach (var msg in _buffer)
                Log(msg);

            _buffer.Clear();
        }

        // --- Color Logic ---
        private static Color DetermineColor(string message)
        {
            message = message.ToLowerInvariant();

            // --- Success ---
            if (message.Contains("done") ||
                message.Contains("success") ||
                message.Contains("ready") ||
                message.Contains("installed successfully") ||
                message.Contains("applied successfully"))
                return Color.FromArgb(0, 200, 0);   // green

            // --- Warning / Notice ---
            if (message.Contains("need update") ||
                message.Contains("processing") ||
                message.Contains("skipped") ||
                message.Contains("ensure") ||
                message.Contains("maybe") ||
                message.Contains("missing") ||
                (message.Contains("no ") && !message.Contains("error")))
                return Color.FromArgb(255, 165, 0); // orange

            // --- Error / Fail ---
            if (message.Contains("error") ||
                message.Contains("failed") ||
                message.Contains("critical") ||
                message.Contains("required") ||
                message.Contains("cannot") ||
                message.Contains("exception"))
                return Color.FromArgb(255, 60, 60); // red

            // --- Operation / Progress ---
            if (message.Contains("fetching") ||
                message.Contains("checking") ||
                message.Contains("extracting") ||
                message.Contains("validating") ||
                message.Contains("please wait") ||
                message.Contains("generating") ||
                message.Contains("loading"))
                return Color.FromArgb(100, 149, 237); // blue

            // --- Neutral info ---
            return Color.LightGray;
        }

        private void Colorize(int start, int length, Color color)
        {
            _consoleLog.SelectionStart = start;
            _consoleLog.SelectionLength = length;
            _consoleLog.SelectionColor = color;
        }
    }
}
