using BAP.Db;
using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Security.Cryptography;
using System.Threading;

namespace BAP.WebCore
{
    [MenuItem("Manage Content", "Add or remove Games or Menu Items", true, "cbf9fb4e-edbb-4474-b1a7-811f7f5e8c18")]
    public partial class ManageContent : ComponentBase, IDisposable
    {
        private CancellationTokenSource cancelation { get; set; } = default!;
        [Inject]
        DbAccessor dba { get; set; } = default!;
        [Inject]
        BapSettings BapSettings { get; set; } = default!;
        private bool displayProgress { get; set; }
        private EditContext editContext { get; set; } = default!;
        private int progressPercent;
        private FluentValidationValidator fluentValidationValidator = default!;
        private PackageUpload? packageUpload { get; set; }

        protected override void OnInitialized()
        {
            cancelation = new CancellationTokenSource();
            packageUpload = new PackageUpload();
            editContext = new EditContext(packageUpload);
        }


        private void OnChange(InputFileChangeEventArgs eventArgs)
        {
            packageUpload.Package = eventArgs.File;
            editContext.NotifyFieldChanged(FieldIdentifier.Create(() => packageUpload.Package));
        }

        private async Task OnSubmit()
        {
            string newFileName = Path.Combine(BapSettings.AddonSaveLocation, Path.GetFileNameWithoutExtension(packageUpload.Package.Name), Path.GetFileName(packageUpload.Package.Name));

            Directory.CreateDirectory(Path.GetDirectoryName(newFileName)!);
            //using var file = File.OpenWrite(newFirmwareFileName);
            using (FileStream fs = new(newFileName, FileMode.Create))
            {
                await packageUpload.Package.OpenReadStream().CopyToAsync(fs);
            };

            displayProgress = false;
            packageUpload = new PackageUpload();
            StateHasChanged();

        }

        public void Dispose()
        {
            cancelation.Cancel();
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
