using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BAP.Types;

namespace BAP.Admin
{
    [BapProvider("Balena System Provider", "Provider for System management when running with Balena", uniqueId: "a5436a9b-7771-48a7-bd2f-647d9b54eff0")]
    internal class BalenaSystemProvider : ISystemProvider
    {
        string returnedMessage = "Nothing yet";
        string address = Environment.GetEnvironmentVariable("BALENA_SUPERVISOR_ADDRESS") ?? "";
        string apiKey = Environment.GetEnvironmentVariable("BALENA_SUPERVISOR_API_KEY") ?? "";

        public async Task<bool> GetBasicStatusInfo()
        {
            using var client = new HttpClient();
            var result = await client.GetAsync($"{address}/v1/device?apikey={apiKey}");
            returnedMessage = await result.Content.ReadAsStringAsync();
            return true;
        }

        public async Task<bool> GetVpnInfo()
        {
            using var client = new HttpClient();
            var result = await client.GetAsync($"{address}/v2/device/vpn?apikey={apiKey}");
            returnedMessage = await result.Content.ReadAsStringAsync();
            return true;
        }

        public async Task<bool> Getv2State()
        {

            using var client = new HttpClient();
            var result = await client.GetAsync($"{address}/v2/state/status?apikey={apiKey}");
            returnedMessage = await result.Content.ReadAsStringAsync();
            return true;
        }

        private async Task<int> GetAppId(string containerName)
        {
            using var client = new HttpClient();
            var result = await client.GetAsync($"{address}/v2/state/status?apikey={apiKey}");
            string jsonContent = await result.Content.ReadAsStringAsync();
            BalenaStatus status = JsonSerializer.Deserialize<BalenaStatus>(jsonContent) ?? new BalenaStatus();
            return status?.containers?.Where(t => t.serviceName.Equals(containerName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault()?.appId ?? 0;
        }

        public async Task<bool> RebootWifi()
        {
            await RestartContainer("wifictrl");
            return true;
        }


        private async Task<bool> RestartContainer(string containerName)
        {
            using var client = new HttpClient();
            var statusResult = await client.GetAsync($"{address}/v2/state/status?apikey={apiKey}");
            string jsonContent = await statusResult.Content.ReadAsStringAsync();
            BalenaStatus status = JsonSerializer.Deserialize<BalenaStatus>(jsonContent) ?? new BalenaStatus();
            int appId = await GetAppId(containerName);
            string addressToPostTo = $"{address}/v2/applications/{appId}/restart-service?apikey={apiKey}";
            var payload = $"{{\"serviceName\": \"{containerName}\"}}";

            HttpContent c = new StringContent(payload, Encoding.UTF8, "application/json");
            var result = await client.PostAsync(addressToPostTo, c);
            returnedMessage = await result.Content.ReadAsStringAsync();
            return true;
        }

        public async Task<bool> RebootSystem()
        {
            using var client = new HttpClient();
            string addressToPostTo = $"{address}/v1/reboot??apikey={apiKey}";
            HttpContent c = new StringContent("", Encoding.UTF8, "application/json");
            var result = await client.PostAsync(addressToPostTo, c);
            returnedMessage = await result.Content.ReadAsStringAsync(); ;
            //string dataToPost = @"{"serviceName": "my - service"}"
            return true;
        }

        public async Task<bool> ShutdownSystem()
        {
            using var client = new HttpClient();
            string addressToPostTo = $"{address}/v1/shutdown?apikey={apiKey}";
            HttpContent c = new StringContent("", Encoding.UTF8, "application/json");
            var result = await client.PostAsync(addressToPostTo, c);
            returnedMessage = await result.Content.ReadAsStringAsync(); ;
            //string dataToPost = @"{"serviceName": "my - service"}"
            return true;
        }

        public async Task<bool> RebootWebApp()
        {
            await RestartContainer("bapwebapp");
            return true;
        }

        public void Dispose()
        {

        }

        public async Task<bool> InitializeAsync()
        {
            return await Task.FromResult(true);

        }
    }
}
