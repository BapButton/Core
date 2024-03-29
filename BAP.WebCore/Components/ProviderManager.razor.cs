﻿using BAP.Db;
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
    
    public partial class ProviderManager : ComponentBase, IDisposable
    {
        private CancellationTokenSource cancelation { get; set; } = default!;
        [Inject]
        DbAccessor dba { get; set; } = default!;
        [Inject]
        IOptions<BapSettings> BapSettings { get; set; } = default!;
        [Inject]
        LoadedAddonHolder LoadedAddonHolder { get; set; } = default!;
        [Inject]
        PhysicalFileMaintainer PhysicalFileMaintainer { get; set; } = default!;
        [Inject]
        ISystemProvider SystemProvider { get; set; } = default!;
        List<PackageInfo> CurrentPackages { get; set; } = new();
        private bool displayProgress { get; set; }
        private string FileUploadMessage { get; set; } = "";
        private EditContext editContext { get; set; } = default!;
        private int progressPercent;
        private FluentValidationValidator fluentValidationValidator = default!;
        private PackageUpload packageUpload { get; set; } = default!;
        private List<string> UniqueIdsOfProviders { get; set; } = new();



        protected override void OnInitialized()
        {
            foreach (var providerInterface in LoadedAddonHolder.BapProviders)
            {
                UniqueIdsOfProviders.Add(providerInterface.Providers.First(t => t.IsCurrentlySelected).UniqueId);
            }

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
            if (cancelation != null)
            {
                cancelation.Cancel();
            }

        }
    }

    public class PackageUpload
    {

        [Required]
        [FileValidation(new[] { ".zip", ".nupkg" })]
        public IBrowserFile Package { get; set; }
    }


    public class FileValidationAttribute : ValidationAttribute
    {
        public FileValidationAttribute(string[] allowedExtensions)
        {
            AllowedExtensions = allowedExtensions;
        }

        private string[] AllowedExtensions { get; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var file = (IBrowserFile)value;

            var extension = System.IO.Path.GetExtension(file.Name);

            if (!AllowedExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            {
                return new ValidationResult($"File must have one of the following extensions: {string.Join(", ", AllowedExtensions)}.", new[] { validationContext.MemberName });
            }

            return ValidationResult.Success;
        }
    }
}
