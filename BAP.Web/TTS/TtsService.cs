
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace BAP.Web.TTS
{
    public class TtsService : ITtsService
    {
        private readonly HttpClient _httpClient;

        public TtsService(HttpClient httpClient)
        {
            _httpClient = httpClient;

        }

        public async Task<Byte[]> GetAudio(string textToConvert, bool useSpanish = false)
        {
            string hostName = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("MosquittoAddress")) ? "localhost" : "opentts";
            string voice = useSpanish ? "nanotts:es-ES" : "nanotts:en-US";
            string url = $"http://{hostName}:5500/api/tts?voice={voice}&text={System.Web.HttpUtility.UrlEncode(textToConvert)}";
            var wav = await _httpClient.GetStreamAsync(url);
            using MemoryStream temp = new MemoryStream();
            wav.CopyTo(temp);
            return temp.ToArray();
        }

    }

    public interface ITtsService
    {
        public Task<byte[]> GetAudio(string textToConvert, bool useSpanish = false);


    }

}
