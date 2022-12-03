using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Memory;
using BAP.Web.Pages;
using BAP.Db;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BAP.Web.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class FirmwareController : ControllerBase
    {
        private readonly IMemoryCache _memoryCache;
        private ILogger<FirmwareController> _logger;
        DbAccessor _dba;

        public FirmwareController(IMemoryCache memoryCache, ILogger<FirmwareController> logger, DbAccessor dbAccessor)
        {
            _memoryCache = memoryCache;
            _logger = logger;
            _dba = dbAccessor;
        }


        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status304NotModified)]
        public async Task<ActionResult> Get(bool downloadAnyway = false)
        {

            FirmwareInfo currentFile = await _dba.GetLatestFirmwareInfo();
            var allHeaders = HttpContext.Request.Headers.ToList();
            _logger.LogTrace($"Firmware Update Request the headers are {string.Join(',', allHeaders)}");

            if (!downloadAnyway && (!allHeaders.Any(t => t.Key == "x-ESP8266-chip-size") || !allHeaders.Any(t => t.Key == "x-ESP8266-sketch-md5") || !allHeaders.Any(t => t.Key == "x-ESP8266-version")))
            {
                _logger.LogDebug($"A device tried to download firmware but does not appear to be an esp8266 chip. The headers are {string.Join(',', allHeaders)}");
                return this.StatusCode(StatusCodes.Status404NotFound);
            }
            string macAddress = Guid.NewGuid().ToString();
            if (!downloadAnyway)
            {
                macAddress = allHeaders.FirstOrDefault(t => t.Key == "x-ESP8266-STA-MAC").Value;
            }
            string md5Checksum = Guid.NewGuid().ToString();
            if (!downloadAnyway)
            {
                md5Checksum = allHeaders.FirstOrDefault(t => t.Key == "x-ESP8266-sketch-md5").Value;
            }
            if (_memoryCache.TryGetValue(macAddress, out object value))
            {
                _logger.LogTrace($"Cache caused us to skip firmware check");
                //need to have a check for the mac address and return 304 to avoid a reboot loop if the version coded in the uploaded firmware does not match what is typed in on upload;
                return this.StatusCode(StatusCodes.Status304NotModified);

            }
            string moduleFirmwareVersion = Guid.NewGuid().ToString();
            if (!downloadAnyway)
            {
                moduleFirmwareVersion = allHeaders.FirstOrDefault(t => t.Key == "x-ESP8266-version").Value.FirstOrDefault() ?? "";
            }
            if (moduleFirmwareVersion != null && moduleFirmwareVersion.Length > 0)
            {
                if (moduleFirmwareVersion == currentFile.FirmwareVersion)
                {
                    _logger.LogTrace($"Firmware Match - {moduleFirmwareVersion} equals {currentFile.FirmwareVersion} the md5 checks for the module is {md5Checksum} and in the db it is {currentFile.Md5Hash} ");
                    return this.StatusCode(StatusCodes.Status304NotModified);
                }
                _logger.LogTrace($"Updating firmware for device with mac address {macAddress} from version {moduleFirmwareVersion} with md5 of {md5Checksum} to version {currentFile.FirmwareVersion} with md5 {currentFile.Md5Hash}");
                _memoryCache.Set(macAddress, true, TimeSpan.FromMinutes(5));
                var path = currentFile.FileName;
                var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                return File(stream, "application/octet-stream", "firmware.bin");
            }

            return this.StatusCode(StatusCodes.Status404NotFound);
        }
    }
}
