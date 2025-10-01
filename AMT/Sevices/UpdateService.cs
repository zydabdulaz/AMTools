using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ArdysaModsTools.Services
{
    public class UpdateService
    {
        private static readonly HttpClient httpClient = new HttpClient();

        private const string GitHubApiUrl = "https://api.github.com/repos/Anneardysa/ArdysaModsTools/releases/latest";

        public async Task<OperationResult> CheckForUpdatesAsync(string currentVersion)
        {
            try
            {
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("ArdysaModsTools");

                var response = await httpClient.GetAsync(GitHubApiUrl);
                if (!response.IsSuccessStatusCode)
                    return OperationResult.Fail($"GitHub returned {response.StatusCode}");

                string json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                string latestVersion = doc.RootElement.GetProperty("tag_name").GetString() ?? "0.0.0";

                if (IsNewer(latestVersion, currentVersion))
                    return OperationResult.Ok($"New version {latestVersion} available!");
                else
                    return OperationResult.Ok($"Already up to date ({currentVersion}).");
            }
            catch (Exception ex)
            {
                return OperationResult.Fail($"Update check failed: {ex.Message}");
            }
        }

        private bool IsNewer(string latest, string current)
        {
            var l = latest.TrimStart('v').Split('.');
            var c = current.Split('.');
            for (int i = 0; i < Math.Max(l.Length, c.Length); i++)
            {
                int li = i < l.Length ? int.Parse(l[i]) : 0;
                int ci = i < c.Length ? int.Parse(c[i]) : 0;
                if (li > ci) return true;
                if (li < ci) return false;
            }
            return false;
        }
    }
}
