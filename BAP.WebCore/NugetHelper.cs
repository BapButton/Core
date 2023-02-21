using BAP.WebCore.Components;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Frameworks;
using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Resolver;
using NuGet.Versioning;
using System;
using System.Threading;

namespace BAP.WebCore
{
    internal static class NugetHelper
    {
        private static NuGet.Common.ILogger Logger = NullLogger.Instance;
        private static SourceCacheContext Cache = new SourceCacheContext();
        private static SourceRepository Repo = Repository.Factory.GetCoreV3("https://api.nuget.org/v3/index.json");
        private static string targetFramework = "net7.0";
        private static List<string> DependenciesPrefixesToIgnore = new() { "System", "Microsoft","MessagePipe","MudBlazor" };
        private static HashSet<string> DependenciesToIgnore = new();
        private static HashSet<string> BapPackagesThatMustBeShared = new() { "BAP.Types", "BAP.Db" };
        public static async Task<List<string>> FindPackagesAsync()
        {

            var resource = await Repo.GetResourceAsync<PackageSearchResource>();
            var searchFilter = new SearchFilter(includePrerelease: true)
            {
                IncludeDelisted = false,
                SupportedFrameworks = new[] { FrameworkConstants.CommonFrameworks.Net70.Framework },
            };

            var results = await resource.SearchAsync(
                "tags:BapGame",
                searchFilter,
                skip: 0,
                take: 10,
                Logger,
                CancellationToken.None);
            List<string> packages = new();
            foreach (IPackageSearchMetadata result in results)
            {
                packages.Add(result.Identity.Id);
            }
            return packages;
        }

        public static async Task<List<NuGetVersion>> GetPackageVersionsAsync(string packageId)
        {
            var resource = await Repo.GetResourceAsync<FindPackageByIdResource>();
            var versions = await resource.GetAllVersionsAsync(
                packageId,
                Cache,
                NullLogger.Instance,
                CancellationToken.None);
            return versions.ToList();
        }

        public static async Task<(MemoryStream? packageStream, NuGetVersion? nugetVersion)> DownloadPackage(string packageId)
        {
            var resource = await Repo.GetResourceAsync<FindPackageByIdResource>();
            var versions = await GetPackageVersionsAsync(packageId);
            NuGetVersion? latestVersion = versions.LastOrDefault();
            if (latestVersion != null)
            {
                MemoryStream packageStream = new();
                await resource.CopyNupkgToStreamAsync(
                packageId,
                    latestVersion,
                    packageStream,
                    Cache,
                    NullLogger.Instance,
                    CancellationToken.None);
                packageStream.Flush();
                packageStream.Seek(0, SeekOrigin.Begin);
                return (packageStream, latestVersion);
            }
            return (null, null);
        }

        public static async Task<List<PackageInfo>> InstallPackageAsync(PhysicalFileMaintainer physicalFileMaintainer, string packageId)
        {
            var (packageStream, nugetVersion) = await DownloadPackage(packageId);
            if (packageStream != null && nugetVersion != null)
            {
                await physicalFileMaintainer.AddNugetPackage(packageStream, nugetVersion, packageId);
            }

            return physicalFileMaintainer.GetPackages();
        }

        public static async Task DownloadAllDependencies(string packageId, NuGetVersion nuGetVersion, PhysicalFileMaintainer physicalFileMaintainer, string parentDirectoryName)
        {
            var packages = new HashSet<SourcePackageDependencyInfo>(PackageIdentityComparer.Default);
            if (DependenciesToIgnore.Count == 0)
            {
                var dependenciesToIgnore = new HashSet<SourcePackageDependencyInfo>(PackageIdentityComparer.Default);
                foreach (var sharedPackage in BapPackagesThatMustBeShared)
                {
                    var versions = await GetPackageVersionsAsync(sharedPackage);
                    NuGetVersion? latestVersion = versions.LastOrDefault();
                    await ListAllPackageDependencies(
                        new PackageIdentity(sharedPackage, latestVersion),
                        NuGetFramework.ParseFolder(targetFramework),
                        dependenciesToIgnore,
                        CancellationToken.None);
                }
                DependenciesToIgnore = dependenciesToIgnore.Select(t => t.Id).ToHashSet();
            }


            await ListAllPackageDependencies(
                new PackageIdentity(packageId, nuGetVersion),
                NuGetFramework.ParseFolder(targetFramework),
                packages,
                CancellationToken.None);

            // Find the best version for each package
            var resolverContext = new PackageResolverContext(
                dependencyBehavior: DependencyBehavior.Lowest,
                targetIds: new[] { packageId },
                requiredPackageIds: Enumerable.Empty<string>(),
                packagesConfig: Enumerable.Empty<PackageReference>(),
                preferredVersions: Enumerable.Empty<PackageIdentity>(),
                availablePackages: packages,
                new List<PackageSource>() { Repo.PackageSource },
                NullLogger.Instance);

            var resolver = new PackageResolver();
            var resolvedPackages = resolver.Resolve(resolverContext, CancellationToken.None);

            foreach (var resolvedPackage in resolvedPackages)
            {
                if (ShouldPackageEverBeDownloaded(resolvedPackage.Id))
                {
                    await physicalFileMaintainer.AddNugetDependency(resolvedPackage.Id, resolvedPackage.Version, parentDirectoryName);
                }

            }
        }

        private static bool ShouldPackageEverBeDownloaded(string packagId)
        {
            if (DependenciesPrefixesToIgnore.Any(t => packagId.StartsWith(t)))
            {
                return false;
            }
            if (DependenciesToIgnore.Contains(packagId))
            {
                return false;
            }
            if (BapPackagesThatMustBeShared.Contains(packagId))
            {
                return false;
            }

            return true;
        }

        public static async Task ListAllPackageDependencies(
            PackageIdentity package,
            NuGetFramework framework,
            HashSet<SourcePackageDependencyInfo> dependencies,
            CancellationToken cancellationToken)
        {
            if (dependencies.Contains(package))
            {
                return;
            }


            foreach (var repository in new List<SourceRepository>() { Repo })
            {
                var dependencyInfoResource = await repository.GetResourceAsync<DependencyInfoResource>();
                var dependencyInfo = await dependencyInfoResource.ResolvePackage(package, framework, Cache, Logger, cancellationToken);
                if (dependencyInfo == null)
                {
                    continue;
                }
                if (dependencies.Add(dependencyInfo))
                {
                    foreach (var dependency in dependencyInfo.Dependencies)
                    {
                        await ListAllPackageDependencies(
                           new PackageIdentity(dependency.Id, dependency.VersionRange.MinVersion),
                           framework,
                           dependencies,
                           cancellationToken);
                    }
                }
            }
        }
    }
}
