using BAP.Db;
using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.IO.Compression;
using System.IO.Enumeration;
using System.Net;
using System.Security.Cryptography;
using System.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace BAP.WebCore.Components
{

    public partial class PackageManager : ComponentBase, IDisposable
    {
        private CancellationTokenSource cancelation { get; set; } = default!;
        [Inject]
        DbAccessor dba { get; set; } = default!;
        //[Inject]
        //IOptions<BapSettings> BapSettings { get; set; } = default!;
        [Inject]
        LoadedAddonHolder LoadedAddonHolder { get; set; } = default!;
        [Inject]
        ILogger<PackageManager> Logger { get; set; } = default!;
        [Inject]
        PhysicalFileMaintainer PhysicalFileMaintainer { get; set; } = default!;
        [Inject]
        ISystemProvider SystemProvider { get; set; } = default!;
        List<PackageInfo> CurrentPackages { get; set; } = new();
        private EditContext editContext { get; set; } = default!;
        public bool NugetPackagesLoaded { get; set; }
        public bool MainPackagesLoaded { get; set; }
        public string NugetStatus { get; set; } = "";
        private int progressPercent;
        private FluentValidationValidator fluentValidationValidator = default!;
        private PackageUpload packageUpload { get; set; } = default!;
        private List<string> UniqueIdsOfProviders { get; set; } = new();
        private List<NugetPackageInfo> NugetPackages { get; set; } = new();



        protected override void OnInitialized()
        {
            cancelation = new CancellationTokenSource();
            packageUpload = new PackageUpload();
            editContext = new EditContext(packageUpload);
            foreach (var providerInterface in LoadedAddonHolder.BapProviders)
            {
                UniqueIdsOfProviders.Add(providerInterface.Providers.First(t => t.IsCurrentlySelected).UniqueId);
            }

        }

        protected override async Task OnInitializedAsync()
        {
            CurrentPackages = await PhysicalFileMaintainer.GetPackages();
            MainPackagesLoaded = true;
            try
            {
                NugetPackages = await NugetHelper.FindPackagesAsync();

            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex, "Unagle to load Nuget Packages");
                NugetStatus = "Unable to fetch new Games and Packages. Probably you are offline. Please reconnect and try again.";
            }
            NugetPackagesLoaded = true;
            NugetPackages = NugetPackages.Except(NugetPackages.Where(t => CurrentPackages.Select(s => s.Id).Contains(t.PackageId))).ToList();
        }

        private async Task InstallPackage(string packageId)
        {
            NugetPackages.First(t => t.PackageId == packageId).Downloading = true;
            await NugetHelper.InstallPackageAsync(PhysicalFileMaintainer, packageId);
            NugetPackages.RemoveAll(t => t.PackageId == packageId);
            CurrentPackages = await PhysicalFileMaintainer.GetPackages();
        }
        private async Task UpdatePackage(string packageId)
        {
            CurrentPackages.First(t => t.Id == packageId).IsUpdating = true;
            await NugetHelper.InstallPackageAsync(PhysicalFileMaintainer, packageId);
            CurrentPackages = await PhysicalFileMaintainer.GetPackages();
        }
        private async Task DeletePackage(string packageId)
        {
            CurrentPackages.First(t => t.Id == packageId).IsDeleting = true;
            await PhysicalFileMaintainer.MarkPackageForDeletion(packageId);
            CurrentPackages = await PhysicalFileMaintainer.GetPackages();
        }

        private void OnChange(InputFileChangeEventArgs eventArgs)
        {
            packageUpload.Package = eventArgs.File;
            editContext.NotifyFieldChanged(FieldIdentifier.Create(() => packageUpload.Package));
        }

        private async Task OnSubmit()
        {
            using MemoryStream ms = new MemoryStream();
            await packageUpload.Package.OpenReadStream().CopyToAsync(ms);
            CurrentPackages = await PhysicalFileMaintainer.AddFilePackage(ms, packageUpload.Package.Name);
            StateHasChanged();

        }


        private async Task Reboot()
        {
            await SystemProvider.RebootWebApp();

        }

        private async Task ProviderOnSubmit()
        {
            bool changedAProvider = false;
            for (int i = 0; i < LoadedAddonHolder.BapProviders.Count; i++)
            {
                var providerInterface = LoadedAddonHolder.BapProviders[i];
                var currentProvider = dba.GetRecentlyActiveProvider(providerInterface.ProviderInterfaceType.FullName).FirstOrDefault();
                if (currentProvider == null || currentProvider != UniqueIdsOfProviders[i])
                {
                    var newProvider = providerInterface.Providers.FirstOrDefault(t => t.UniqueId == UniqueIdsOfProviders[i]);
                    if (newProvider != null)
                    {
                        dba.AddActiveProvider(newProvider.BapProviderType, true);
                        changedAProvider = true;
                    }

                }
            }
            if (changedAProvider)
            {
                await SystemProvider.RebootWebApp();
            }

        }

        public void Dispose()
        {
            cancelation.Cancel();
        }
    }
}
