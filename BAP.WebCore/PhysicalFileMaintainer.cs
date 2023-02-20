using BAP.WebCore.Components;
using Microsoft.Extensions.Options;
using NuGet.Versioning;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAP.WebCore
{
    public class PackageInfo
    {
        public string Name { get; set; } = "";
        public string Version { get; set; } = "";
        public string FullPath { get; set; } = "";
        public bool IsMarkedForDeletion { get; set; }
        public bool IsMarkedForRename { get; set; }
    }


    public class PhysicalFileMaintainer
    {
        private HashSet<string> allowedNonStaticAssetExtensions = new() { "dll", "pdb" };
        BapSettings _bapSettings { get; set; }
        private string frameworkVersion = "net7.0";
        private const string pendingDeleteFileName = "delete.txt";
        private const string pendingRenameFileName = "rename.txt";
        public PhysicalFileMaintainer(IOptionsSnapshot<BapSettings> bapSettings)
        {
            _bapSettings = bapSettings.Value;
        }
        /// <summary>
        /// This cleans up packages that are pending deletion or rename. 
        /// This should only be run on boot before packages have been loaded. 
        /// </summary>
        /// <returns>List of all pacakges after running cleanup </returns>
        public List<PackageInfo> CleanUpPackages()
        {
            List<PackageInfo> packageInfos = GetPackages();
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

            return GetPackages();
        }

        private PackageInfo GetPackageInfo(string directoryFullPath)
        {
            PackageInfo info = new PackageInfo()
            {
                FullPath = directoryFullPath
            };

            DirectoryInfo directoryInfo = new DirectoryInfo(directoryFullPath);
            info.Name = directoryInfo.Name;
            info.IsMarkedForDeletion = directoryInfo.GetFiles(pendingDeleteFileName, SearchOption.TopDirectoryOnly).Length > 0;
            info.IsMarkedForRename = directoryInfo.GetFiles(pendingRenameFileName, SearchOption.TopDirectoryOnly).Length > 0;
            return info;
        }
        public List<PackageInfo> GetPackages()
        {
            var packageInfos = new List<PackageInfo>();
            var directories = Directory.GetDirectories(_bapSettings.AddonSaveLocation);
            foreach (var directory in directories)
            {
                packageInfos.Add(GetPackageInfo(directory));

            }
            return packageInfos;
        }


        private void MarkDirectoryForRename(string directoryName, string packageName)
        {
            File.WriteAllText(Path.Combine(directoryName, pendingRenameFileName), packageName);

        }

        private void MarkDirectoryForDeletion(string directoryName)
        {
            File.WriteAllText(Path.Combine(directoryName, pendingDeleteFileName), $"{DateTime.Now.ToLongDateString()} {DateTime.Now.ToLongTimeString()}");
        }


        /// <summary>
        /// Adds a file Package. If the same name package is found it will mark that for deletion.
        /// </summary>
        /// <param name="zipOrNuget">Memory stream that is seekable syncronously</param>
        /// <returns>List of all packages after addind the new package</returns>
        public List<PackageInfo> AddFilePackage(MemoryStream zipFile, string packageId)
        {

            using ZipArchive archive = new(zipFile);
            string packageName = packageId;
            string newDirectoryName = CleanupFoldersIfNeeded(packageName);

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

            return GetPackages();
        }

        private string CleanupFoldersIfNeeded(string packageName)
        {
            bool renameNeeded = false;
            string newDirectoryName = Path.Combine(_bapSettings.AddonSaveLocation, packageName);
            var packageList = GetPackages();
            bool packageAlreadyExists = packageList.Any(t => t.Name == packageName);
            bool packageAlreadyWaitingToDelete = packageList.Any(t => t.IsMarkedForDeletion && t.Name == packageName);
            bool packageAlreadyWaitingToRename = packageList.Any(t => t.IsMarkedForRename && t.Name == packageName);


            DirectoryInfo directory = new DirectoryInfo(newDirectoryName);

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
            Directory.CreateDirectory(newDirectoryName);
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
            string newDirectoryName = CleanupFoldersIfNeeded(packageId);
            using ZipArchive archive = new(nugetStream);
            ExtractNuget(archive, newDirectoryName, true);
            await NugetHelper.DownloadAllDependencies(packageId, nuGetVersion, this, newDirectoryName);

            return GetPackages();
        }

        public async Task<bool> AddNugetDependency(string packageId, string parentPackageFolder)
        {
            var (packageStream, _) = await NugetHelper.DownloadPackage(packageId);
            if (packageStream != null)
            {
                using ZipArchive archive = new(packageStream);
                ExtractNuget(archive, parentPackageFolder, false);
                return true;
            }

            //Todo - this shoudl really be logged or something. It will probably make things crash later on.
            return false;
        }

        private void ExtractNuget(ZipArchive archive, string directoryName, bool includeNuspec)
        {
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                string extension = Path.GetExtension(entry.FullName).TrimStart('.').ToLowerInvariant();
                if (entry.FullName.StartsWith("staticwebassets"))
                {
                    string? directory = Path.GetDirectoryName(Path.Combine(directoryName, entry.FullName));
                    if (directory != null)
                    {
                        Directory.CreateDirectory(directory);
                    }
                    entry.ExtractToFile(Path.Combine(directoryName, entry.FullName));
                }
                else if (entry.FullName.StartsWith($"lib/{frameworkVersion}") && allowedNonStaticAssetExtensions.Contains(extension) || includeNuspec && extension.Equals("nuspec", StringComparison.OrdinalIgnoreCase))
                {
                    entry.ExtractToFile(Path.Combine(directoryName, entry.Name));
                }
            }
        }
    }
}
