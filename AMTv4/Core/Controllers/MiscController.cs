using ArdysaModsTools.Core.Models;
using ArdysaModsTools.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ArdysaModsTools.Core.Controllers
{
    public class MiscController
    {
        private readonly MiscGenerationService _generationService;
        private readonly MiscUtilityService _utilityService;

        public MiscController()
        {
            _generationService = new MiscGenerationService();
            _utilityService = new MiscUtilityService();
        }

        public async Task<OperationResult> GenerateModsAsync(
            string targetPath,
            Dictionary<string, string> selections,
            Action<string> log,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // Step 1 – Basic validation
                if (string.IsNullOrWhiteSpace(targetPath))
                    return new OperationResult { Success = false, Message = "Target path not set." };

                if (selections == null || selections.Count == 0)
                    return new OperationResult { Success = false, Message = "No selections provided." };

                log("Validating environment...");
                await Task.Delay(200, cancellationToken);

                // Step 2 – Run the backend generation
                var result = await _generationService.PerformGenerationAsync(
                    targetPath,
                    selections,
                    log,
                    cancellationToken);

                // Step 3 – Perform silent cleanup, no logging
                if (result.Success)
                {
                    await _utilityService.CleanupTempFoldersAsync(targetPath, log);
                    result.Message = "Done! All mods applied successfully."; // Assign final message
                }

                return result;
            }
            catch (Exception ex)
            {
                log($"Controller Error: {ex.Message}");
                return new OperationResult { Success = false, Message = ex.Message, Exception = ex };
            }
        }
    }
}
