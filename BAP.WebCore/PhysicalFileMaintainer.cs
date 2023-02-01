using BAP.WebCore.Components;
using Microsoft.Extensions.Options;
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
        BapSettings _bapSettings { get; set; }
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
        public List<PackageInfo> AddFilePackage(MemoryStream zipOrNuget, string packageNameFallback)
        {
            bool renameNeeded = false;
            using ZipArchive archive = new(zipOrNuget);
            string packageName = packageNameFallback;
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                if (entry.Name.EndsWith(".deps.json"))
                {
                    packageName = entry.Name.Substring(0, entry.Name.Length - 10);
                    break;
                }
            }
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
            if (renameNeeded)
            {
                MarkDirectoryForRename(newDirectoryName, packageName);
            }
            return GetPackages();
        }

    }
}
