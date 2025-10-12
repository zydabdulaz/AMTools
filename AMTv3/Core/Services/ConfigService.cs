using System;
using System.IO;
using System.Text.Json;

namespace ArdysaModsTools.Core.Services
{
    public class ConfigService
    {
        private readonly string _configPath;
        private ConfigData _data = new();

        public ConfigService()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = Path.Combine(appData, "ArdysaModsTools");
            Directory.CreateDirectory(folder);
            _configPath = Path.Combine(folder, "config.json");

            Load();
        }

        private void Load()
        {
            if (File.Exists(_configPath))
            {
                try
                {
                    string json = File.ReadAllText(_configPath);
                    _data = JsonSerializer.Deserialize<ConfigData>(json) ?? new ConfigData();
                }
                catch
                {
                    _data = new ConfigData();
                }
            }
        }

        private void Save()
        {
            try
            {
                string json = JsonSerializer.Serialize(_data, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_configPath, json);
            }
            catch { }
        }

        public string? GetLastTargetPath() => _data.LastTargetPath;

        public void SetLastTargetPath(string? path)
        {
            _data.LastTargetPath = path;
            Save();
        }

        private class ConfigData
        {
            public string? LastTargetPath { get; set; }
        }
    }
}
