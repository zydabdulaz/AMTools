using AMTools.Models;
using AMTools.Services;
using System.IO;
using System.Threading.Tasks;

namespace AMTools.Core
{
    public class ModToolManager
    {
        private readonly FileService _fileService;
        private readonly LoggerService _logger;

        public ModToolManager(FileService fileService, LoggerService logger)
        {
            _fileService = fileService;
            _logger = logger;
        }

        public async Task<OperationResult> InstallModAsync(string modPath, string targetDir)
        {
            if (!_fileService.DirectoryExists(modPath))
                return OperationResult.Fail("Mod path not found.");

            if (!_fileService.DirectoryExists(targetDir))
                return OperationResult.Fail("Target directory not found.");

            string coreJsonPath = Path.Combine(modPath, "core.json");
            if (!_fileService.FileExists(coreJsonPath))
                return OperationResult.Fail("core.json not found in mod folder.");

            try
            {
                string destPath = Path.Combine(targetDir, "core.json");
                await _fileService.CopyFileAsync(coreJsonPath, destPath);

                _logger.Info($"Installed mod from {modPath} to {targetDir}");
                return OperationResult.Ok("Install successful.");
            }
            catch (System.Exception ex)
            {
                _logger.Error(ex.Message);
                return OperationResult.Fail("Error: " + ex.Message);
            }
        }
    }
}
