using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BapDb
{
    public class FirmwareInfo
    {
        public int FirmwareInfoId { get; set; }
        public string FirmwareVersion { get; set; }
        public DateTime DateUploaded { get; set; }
        public bool IsLatestVersion { get; set; }
        public string Description { get; set; }
        public string FileName { get; set; }
        public string Md5Hash { get; set; }
        public FirmwareInfo()
        {
            FileName = "";
            FirmwareVersion = "";
            FileName = "";
            Description = "";
        }
        public FirmwareInfo(string version, string description, bool isLatestVersion, string filename)
        {
            FirmwareVersion = version;
            DateUploaded = DateTime.Now;
            IsLatestVersion = isLatestVersion;
            Description = description;
            FileName = filename;
        }

    }


}
