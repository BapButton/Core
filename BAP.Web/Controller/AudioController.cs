using BAP.Web.TTS;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace BAP.Web.Controller
{
    [ApiController]
    public class AudioController : ControllerBase
    {
        BapSettings _bapSettings;
        public AudioController(BapSettings bapSettings)
        {
            _bapSettings = bapSettings;
        }
        [Route("api/[controller]/{fileName?}")]
        public IActionResult Index(string fileName)
        {
            string fileWithPath = Path.Combine(_bapSettings.AddonSaveLocation, fileName.Replace('|', '/').Replace("//", "|"));
            var fileData = System.IO.File.ReadAllBytes(fileWithPath);
            string filename = Path.GetFileName(fileWithPath);
            string extension = Path.GetExtension(fileWithPath);
            string mimeType = extension switch
            {
                "mp3" => "audio/mpeg",
                "wav" => "audio/x-wav",
                "m4a" => "audio/mp4",
                "ogg" => "audio/ogg",
                _ => "audio/mpeg"

            };
            return File(fileData, mimeType, filename);
        }
    }
}
