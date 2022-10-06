
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BapWeb.TTS;


namespace BapWeb.Controller
{
   
    [ApiController]
    public class TTSController : ControllerBase
    {
        private ITtsService _ttsService;

        public TTSController(ITtsService tts)
        {
            _ttsService = tts;
        }
        [Route("api/[controller]/{text?}")]
        public async Task<IActionResult> Index(string text)
        {
            if (text.EndsWith(".wav"))
            {
                text = text.Remove(text.Length - text.Length);
            }
            var stream = await _ttsService.GetAudio(text);
            var filename = $"{text.Replace(' ', '_')}.wav";
            return File(stream, "audio/wav", filename);
        }
        [Route("api/[controller]/spanish/{text?}")]
        public async Task<IActionResult> Spanish(string text)
        {
            if (text.EndsWith(".wav"))
            {
                text = text.Remove(text.Length - text.Length);
            }
            var stream = await _ttsService.GetAudio(text, true);
            var filename = $"{text.Replace(' ', '_')}.wav";
            return File(stream, "audio/wav", filename);
        }
    }
}
