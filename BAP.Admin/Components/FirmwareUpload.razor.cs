using BAP.Db;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blazored.FluentValidation;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Components;

namespace BAP.Admin.Components
{
    [MenuItem("Upload Firmware", "Upload new firmware", false, "a732d108-9101-498a-8951-ef6a441bcf69")]
    public partial class FirmwareUpload : ComponentBase, IDisposable
    {
        private List<FirmwareInfo> allUploads = new List<FirmwareInfo>();
        private CancellationTokenSource cancelation { get; set; } = default!;
        [Inject]
        DbAccessor dba { get; set; } = default!;
        private bool displayProgress { get; set; }
        private EditContext editContext { get; set; } = default!;
        private FileUpload fileUpload { get; set; } = default!;
        private int progressPercent;
        private FluentValidationValidator fluentValidationValidator = default!;

        protected override void OnInitialized()
        {
            cancelation = new CancellationTokenSource();
            fileUpload = new FileUpload();
            editContext = new EditContext(fileUpload);

        }

        protected override async Task OnInitializedAsync()
        {
            allUploads = await dba.GetAllFirmwareInfo();
        }

        private void OnChange(InputFileChangeEventArgs eventArgs)
        {
            fileUpload.Firmware = eventArgs.File;
            editContext.NotifyFieldChanged(FieldIdentifier.Create(() => fileUpload.Firmware));
        }

        private async Task OnSubmit()
        {

            string newFirmwareFileName = Path.Combine("data", "firmware", "firmware" + fileUpload.Version.Replace('.', '_') + ".bin");
            //Check ifthe file currently exists. If not generate an empty model.


            //Need to to handle the overwriting component if it is selected.
            if (fileUpload.OverWriteCurrentFirmware)
            {
                ButtonContext db = dba.GetButtonContext();
                //This wont work in the DB concept.
                var oldItem = db.FirmwareInfos.Where(t => t.FirmwareVersion == fileUpload.Version);
                if (oldItem != null)
                {
                    db.FirmwareInfos.RemoveRange(oldItem);
                    await db.SaveChangesAsync();
                }
            }
            else if (allUploads.Any(t => t.FirmwareVersion == fileUpload.Version))
            {
                fileUpload.ShowOverwriteButton = true;
                return;
            }
            FirmwareInfo latestFirmwareInfo = new FirmwareInfo(fileUpload.Version, fileUpload.Description, true, newFirmwareFileName);

            Directory.CreateDirectory(Path.GetDirectoryName(newFirmwareFileName)!);
            //using var file = File.OpenWrite(newFirmwareFileName);
            using (FileStream fs = new(newFirmwareFileName, FileMode.Create))
            {
                await fileUpload.Firmware.OpenReadStream().CopyToAsync(fs);
            };


            //var buffer = new byte[4 * 1096];
            //int bytesRead;
            //double totalRead = 0;

            //displayProgress = true;

            //while ((bytesRead = await stream.ReadAsync(buffer, cancelation.Token)) != 0)
            //{
            //    totalRead += bytesRead;
            //    await file.WriteAsync(buffer, cancelation.Token);

            //    progressPercent = (int)((totalRead / fileUpload.Firmware.Size) * 100);
            //    StateHasChanged();
            //}
            using (var md5 = MD5.Create())
            {
                await using FileStream uploadedFile = new FileStream(newFirmwareFileName, FileMode.Open);
                var hash = md5.ComputeHash(uploadedFile);
                latestFirmwareInfo.Md5Hash = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
            displayProgress = false;
            await dba.AddLatestFirmware(latestFirmwareInfo);
            allUploads = await dba.GetAllFirmwareInfo();
            fileUpload = new FileUpload();
            StateHasChanged();

        }

        public void Dispose()
        {
            cancelation.Cancel();
        }
    }
}
