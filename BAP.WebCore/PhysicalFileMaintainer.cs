using BAP.WebCore.Components;
using Microsoft.Extensions.Options;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml;

namespace BAP.WebCore
{
    public class PackageInfo
    {
        public string Id { get; set; } = "";
        public string Version { get; set; } = "";
        public string FullPath { get; set; } = "";
        public string Description { get; set; } = "";
        public bool IsNewPackageAvailable { get; set; }
        public bool IsMarkedForDeletion { get; set; }
        public bool NotYetLoaded { get; set; }
        public bool IsMarkedForRename { get; set; }
        public bool IsUpdating { get; set; }
        public bool IsDeleting { get; set; }
    }

    public class NugetPackageInfo
    {
        public string PackageId { get; set; } = "";
        public string Name { get; set; } = "";
        public string Version { get; set; } = "";
        public string Description { get; set; } = "";
        public bool Downloading { get; set; }
    }

    public class PhysicalFileMaintainer
    {
        private HashSet<string> allowedNonStaticAssetExtensions = new() { "dll", "pdb" };
        ILogger<PhysicalFileMaintainer> _logger;
        BapSettings _bapSettings { get; set; }
        private string frameworkVersion = "net7.0";
        private const string pendingDeleteFileName = "delete.txt";
        private const string pendingRenameFileName = "rename.txt";
        private const string notYetLoadedFileName = "newNeverLoaded.txt";
        public PhysicalFileMaintainer(IOptionsSnapshot<BapSettings> bapSettings, ILogger<PhysicalFileMaintainer> logger)
        {
            _bapSettings = bapSettings.Value;
            _logger = logger;
        }
        /// <summary>
        /// This cleans up packages that are pending deletion or rename. 
        /// This should only be run on boot before packages have been loaded. 
        /// </summary>
        /// <returns>List of all pacakges after running cleanup </returns>
        public async Task<List<PackageInfo>> CleanUpPackages()
        {
            List<PackageInfo> packageInfos = await GetPackages(skipVersionCheck: true);
            foreach (var package in packageInfos.Where(t => t.IsMarkedForDeletion))
            {
                Directory.Delete(package.FullPath, true);
            }
            foreach (var package in packageInfos.Where(t => t.IsMarkedForRename))
            {
                string newDirectoryFullPath = Path.Combine(_bapSettings.AddonSaveLocation, File.ReadLines(Path.Combine(package.FullPath, pendingRenameFileName)).First());

                Directory.Move(package.FullPath, newDirectoryFullPath);
                File.Delete(Path.Combine(newDirectoryFullPath, pendingRenameFileName));
            }
            foreach (var package in packageInfos.Where(t => t.IsNewPackageAvailable))
            {
                File.Delete(Path.Combine(package.FullPath, pendingRenameFileName));
            }
            return await GetPackages();
        }

        private PackageInfo GetVersionNumberFromNuspec(string fileName)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            string version = "";
            string packageId = "";
            string description = "";
            using (var fileStream = File.OpenText(fileName))
            using (XmlReader reader = XmlReader.Create(fileStream, settings))
            {
                bool isNextNodeVersion = false;
                bool isNextNodePackageId = false;
                bool isNextNodeDescription = false;
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name.Equals("Version", StringComparison.OrdinalIgnoreCase))
                            {
                                isNextNodeVersion = true;
                            }
                            if (reader.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
                            {
                                isNextNodePackageId = true;
                            }
                            if (reader.Name.Equals("Description", StringComparison.OrdinalIgnoreCase))
                            {
                                isNextNodeDescription = true;
                            }
                            break;
                        case XmlNodeType.Text:
                            if (isNextNodeVersion)
                            {
                                version = reader.Value;
                                isNextNodeVersion = false;
                            }
                            if (isNextNodePackageId)
                            {
                                packageId = reader.Value;
                                isNextNodePackageId = false;
                            }
                            if (isNextNodeDescription)
                            {
                                description = reader.Value;
                                isNextNodeDescription = false;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            return new() { Version = version, Id = packageId, Description = description };
        }

        private async Task<PackageInfo> GetPackageInfo(string directoryFullPath, bool skipVersionCheck)
        {
            PackageInfo info = new PackageInfo()
            {
                FullPath = directoryFullPath
            };

            DirectoryInfo directoryInfo = new DirectoryInfo(directoryFullPath);
            info.Id = directoryInfo.Name;
            info.IsMarkedForDeletion = directoryInfo.GetFiles(pendingDeleteFileName, SearchOption.TopDirectoryOnly).Length > 0;
            info.IsMarkedForRename = directoryInfo.GetFiles(pendingRenameFileName, SearchOption.TopDirectoryOnly).Length > 0;
            info.NotYetLoaded = directoryInfo.GetFiles(notYetLoadedFileName, SearchOption.TopDirectoryOnly).Length > 0;
            var nuspecFile = directoryInfo.GetFiles("*.nuspec").FirstOrDefault();
            if (nuspecFile != null)
            {
                var nuspecInfo = GetVersionNumberFromNuspec(nuspecFile.FullName);
                info.Id = nuspecInfo.Id;
                info.Version = nuspecInfo.Version;
                info.Description = nuspecInfo.Description;
            }
            if (!string.IsNullOrEmpty(info.Version) && skipVersionCheck == false)
            {
                try
                {
                    var versions = await NugetHelper.GetPackageVersionsAsync(info.Id);
                    var latestVersion = versions.LastOrDefault();
                    if (latestVersion != null)
                    {
                        if (latestVersion.OriginalVersion != info.Version)
                        {
                            info.IsNewPackageAvailable = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, $"Unable to get version info with error {ex.Message}");
                }

            }
            return info;
        }
        public async Task<List<PackageInfo>> GetPackages(bool skipVersionCheck = false)
        {
            var packageInfos = new List<PackageInfo>();
            var directories = Directory.GetDirectories(_bapSettings.AddonSaveLocation);
            foreach (var directory in directories)
            {
                packageInfos.Add(await GetPackageInfo(directory, skipVersionCheck));

            }
            return packageInfos;
        }


        private void MarkDirectoryForRename(string directoryName, string packageName)
        {
            File.WriteAllText(Path.Combine(directoryName, pendingRenameFileName), packageName);

        }

        public async Task<bool> MarkPackageForDeletion(string packageId)
        {
            var packages = await GetPackages(true);
            var package = packages.FirstOrDefault(t => t.Id == packageId);
            if (package != null)
            {
                MarkDirectoryForDeletion(package.FullPath);
            }
            return false;
        }

        private void MarkDirectoryForDeletion(string directoryName)
        {
            File.WriteAllText(Path.Combine(directoryName, pendingDeleteFileName), $"{DateTime.Now.ToLongDateString()} {DateTime.Now.ToLongTimeString()}");
        }

        private void MarkDirectoryAsNew(string directoryName)
        {
            File.WriteAllText(Path.Combine(directoryName, notYetLoadedFileName), $"{DateTime.Now.ToLongDateString()} {DateTime.Now.ToLongTimeString()}");
        }


        /// <summary>
        /// Adds a file Package. If the same name package is found it will mark that for deletion.
        /// </summary>
        /// <param name="zipOrNuget">Memory stream that is seekable syncronously</param>
        /// <returns>List of all packages after addind the new package</returns>
        public async Task<List<PackageInfo>> AddFilePackage(MemoryStream zipFile, string packageId)
        {

            using ZipArchive archive = new(zipFile);
            string newDirectoryName = await CleanupFoldersIfNeeded(packageId);

            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                if (Path.EndsInDirectorySeparator(entry.FullName))
                {
                    Directory.CreateDirectory(Path.Combine(newDirectoryName, entry.FullName));
                }
                else
                {
                    entry.ExtractToFile(Path.Combine(newDirectoryName, entry.FullName));
                }

            }

            return await GetPackages();
        }

        private async Task<string> CleanupFoldersIfNeeded(string packageName)
        {
            bool renameNeeded = false;
            string newDirectoryName = Path.Combine(_bapSettings.AddonSaveLocation, packageName);
            var packageList = await GetPackages(skipVersionCheck: true);
            bool packageAlreadyExists = packageList.Any(t => t.Id == packageName);
            bool packageAlreadyWaitingToDelete = packageList.Any(t => t.IsMarkedForDeletion && t.Id == packageName);
            bool packageAlreadyWaitingToRename = packageList.Any(t => t.IsMarkedForRename && t.Id == packageName);
            PackageInfo? packageNotYetLoaded = packageList.FirstOrDefault(t => t.NotYetLoaded && t.IsMarkedForRename == false && t.Id == packageName);


            DirectoryInfo directory = new DirectoryInfo(newDirectoryName);
            if (packageNotYetLoaded != null)
            {
                Directory.Delete(newDirectoryName, true);
                return packageNotYetLoaded.FullPath;
            }
            else
            {
                if (packageAlreadyExists && !packageAlreadyWaitingToDelete)
                {
                    renameNeeded = true;
                    MarkDirectoryForDeletion(newDirectoryName);
                }
                if (packageAlreadyExists)
                {
                    newDirectoryName = $"{newDirectoryName}_Temp";
                }
                if (packageAlreadyWaitingToRename)
                {
                    Directory.Delete(newDirectoryName, true);
                }
            }

            Directory.CreateDirectory(newDirectoryName);
            MarkDirectoryAsNew(newDirectoryName);
            if (renameNeeded)
            {
                MarkDirectoryForRename(newDirectoryName, packageName);
            }
            return newDirectoryName;
        }


        /// <summary>
        /// Adds a file Package. If the same name package is found it will mark that for deletion.
        /// </summary>
        /// <param name="zipOrNuget">Memory stream that is seekable syncronously</param>
        /// <returns>List of all packages after addind the new package</returns>
        public async Task<List<PackageInfo>> AddNugetPackage(MemoryStream nugetStream, NuGetVersion nuGetVersion, string packageId)
        {
            string newDirectoryName = await CleanupFoldersIfNeeded(packageId);
            using ZipArchive archive = new(nugetStream);
            ExtractNuget(archive, newDirectoryName, true);
            await NugetHelper.DownloadAllDependencies(packageId, nuGetVersion, this, newDirectoryName);

            return await GetPackages();
        }

        public async Task<bool> AddNugetDependency(string packageId, NuGetVersion nuGetVersion, string parentPackageFolder)
        {
            var (packageStream, _) = await NugetHelper.DownloadPackage(packageId);
            if (packageStream != null)
            {
                using ZipArchive archive = new(packageStream);
                ExtractNuget(archive, parentPackageFolder, false);
                return true;
            }

            //Todo - this should really be logged or something. It will probably make things crash later on.
            return false;
        }

        private void ExtractNuget(ZipArchive archive, string directoryName, bool includeNuspec)
        {
            List<ZipArchiveEntry> entriesToExtract = new List<ZipArchiveEntry>();
            List<ZipArchiveEntry> staticAssetsToExtract = new List<ZipArchiveEntry>();
            if (includeNuspec)
            {
                ZipArchiveEntry? entry = archive.Entries.FirstOrDefault(t => t.Name.EndsWith("nuspec"));
                if (entry != null)
                {
                    entriesToExtract.Add(entry);
                }
            }
            staticAssetsToExtract.AddRange(archive.Entries.Where(t => t.FullName.StartsWith("staticwebassets")));

            string folderOfDllName = "";
            HashSet<string> libFolders = new();
            foreach (var entry in archive.Entries.Where(t => t.FullName.StartsWith("lib")))
            {
                if (entry.FullName.TrimStart('/').Split('/').Length > 2)
                {
                    string folderName = entry.FullName.TrimStart('/').Split('/')[1];
                    if (!libFolders.Contains(folderName))
                    {
                        if (folderName.Contains('.'))
                        {
                            libFolders.Add(folderName);
                        }

                    }
                }
            }
            if (libFolders.Contains(frameworkVersion))
            {
                folderOfDllName = frameworkVersion;
            }
            if (string.IsNullOrEmpty(folderOfDllName))
            {
                var netFolder = libFolders.Where(t => t.StartsWith("net") && !t.StartsWith("nets")).OrderByDescending(t => t).FirstOrDefault();
                if (netFolder != null)
                {
                    folderOfDllName = netFolder;
                }
            }
            if (string.IsNullOrEmpty(folderOfDllName))
            {
                var netStdFolder = libFolders.Where(t => t.StartsWith("nets")).OrderByDescending(t => t).FirstOrDefault();
                if (netStdFolder != null)
                {
                    folderOfDllName = netStdFolder;
                }
            }
            if (!string.IsNullOrEmpty(folderOfDllName))
            {
                foreach (var entry in archive.Entries.Where(t => t.FullName.TrimStart('/').StartsWith($"lib/{folderOfDllName}")))
                {
                    string extension = Path.GetExtension(entry.FullName).TrimStart('.').ToLowerInvariant();
                    if (allowedNonStaticAssetExtensions.Contains(extension))
                    {
                        entriesToExtract.Add(entry);
                    }

                }
            }
            foreach (var entry in entriesToExtract)
            {
                entry.ExtractToFile(Path.Combine(directoryName, entry.Name));
            }
            foreach (var entry in staticAssetsToExtract)
            {
                string? directory = Path.GetDirectoryName(Path.Combine(directoryName, entry.FullName));
                if (directory != null)
                {
                    Directory.CreateDirectory(directory);
                }
                entry.ExtractToFile(Path.Combine(directoryName, entry.FullName));
            }
        }
    }
}
