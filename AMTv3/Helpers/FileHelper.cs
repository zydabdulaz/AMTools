using System;
using System.IO;
using System.Threading;

namespace ArdysaModsTools.Helpers
{
    public static class FileHelper
    {
        /// <summary>
        /// Recursively copy a directory. Files that can't be copied (locked/permissions) are retried a few times then skipped.
        /// This mirrors the original behavior (silently skipping files that fail).
        /// </summary>
        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            var dir = new DirectoryInfo(sourceDirName);
            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {sourceDirName}");

            Directory.CreateDirectory(destDirName);

            // Copy files with retries
            foreach (var file in dir.GetFiles())
            {
                string destPath = Path.Combine(destDirName, file.Name);
                bool copied = false;

                for (int attempt = 1; attempt <= 3 && !copied; attempt++)
                {
                    try
                    {
                        file.CopyTo(destPath, true);
                        copied = true;
                    }
                    catch (IOException)
                    {
                        // retry small delay
                        if (attempt < 3) Thread.Sleep(200);
                    }
                    catch
                    {
                        // any other error — break and skip this file (same as prior silent behavior)
                        break;
                    }
                }
                // if not copied after retries -> skip silently
            }

            if (copySubDirs)
            {
                foreach (var subdir in dir.GetDirectories())
                {
                    var tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }

        public static void EnsureDirectory(string path)
        {
            if (string.IsNullOrEmpty(path)) return;
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
        }

        public static void SafeDeleteDirectory(string path)
        {
            try
            {
                if (Directory.Exists(path))
                    Directory.Delete(path, true);
            }
            catch
            {
                // swallow - best effort cleanup
            }
        }

        public static void SafeDeleteFile(string path)
        {
            try
            {
                if (File.Exists(path)) File.Delete(path);
            }
            catch
            {
                // swallow
            }
        }
    }
}
