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
                int originalLength = _consoleLog.TextLength;
                _consoleLog.AppendText(fullMessage);

                // 🌈 Color by type
                Color color = DetermineColor(message);
                if (color != Color.Empty)
                    Colorize(originalLength, fullMessage.Length, color);

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

        // === Utility ===
        private static Color DetermineColor(string message)
        {
            message = message.ToLowerInvariant();

            if (message.Contains("ready") || message.Contains("success"))
                return Color.FromArgb(0, 200, 0); // ✅ Green
            if (message.Contains("need update") || message.Contains("ensure") || message.Contains("skipped"))
                return Color.FromArgb(255, 165, 0); // ⚠️ Orange
            if (message.Contains("error") || message.Contains("failed") || message.Contains("critical") || message.Contains("required"))
                return Color.FromArgb(255, 50, 50); // ❌ Red
            if (message.Contains("checking") || message.Contains("detecting") || message.Contains("starting"))
                return Color.FromArgb(100, 149, 237); // 🟦 Blue
            return Color.Empty;
        }

        private void Colorize(int start, int length, Color color)
        {
            _consoleLog.SelectionStart = start;
            _consoleLog.SelectionLength = length;
            _consoleLog.SelectionColor = color;
        }
    }
}
