using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.IO;

namespace BAP.WebCore.Controller
{
    [ApiController]
    public class AudioController : ControllerBase
    {
        IMemoryCache _memoryCache;
        ILogger<AudioController> _logger;
        public AudioController(IMemoryCache memoryCache, ILogger<AudioController> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
        }
        [Route("api/[controller]/{fileName?}")]
        public IActionResult Index(string fileName)
        {
            string fileWithPath = _memoryCache.Get<string>(fileName) ?? "";
            if (!string.IsNullOrEmpty(fileWithPath))
            {
                try
                {
                    var fileData = System.IO.File.ReadAllBytes(fileWithPath);
                    string filename = Path.GetFileName(fileWithPath);
                    string extension = Path.GetExtension(fileWithPath);
                    string mimeType = extension switch
                    {
                        ".mp3" => "audio/mpeg",
                        ".wav" => "audio/x-wav",
                        ".m4a" => "audio/mp4",
                        ".ogg" => "audio/ogg",
                        _ => "audio/mpeg"

                    };
                    return File(fileData, mimeType, filename);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error Retrieving AudioFile");

                }

            }
            return NotFound();
        }
    }
}
