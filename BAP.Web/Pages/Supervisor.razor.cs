using System.Collections;
using System.Text;
using System.Text.Json;
using BAP.Web.Supervisor;

namespace BAP.Web.Pages
{
    public partial class Supervisor
    {
        string returnedMessage = "Nothing yet";
        string address = Environment.GetEnvironmentVariable("BALENA_SUPERVISOR_ADDRESS") ?? "";
        string apiKey = Environment.GetEnvironmentVariable("BALENA_SUPERVISOR_API_KEY") ?? "";
        protected async override Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

        }
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

        public async Task<bool> RestartTheWifiCtrl()
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

        public async Task<bool> RestartEntireDevice()
        {
            using var client = new HttpClient();
            string addressToPostTo = $"{address}/v1/reboot??apikey={apiKey}";
            HttpContent c = new StringContent("", Encoding.UTF8, "application/json");
            var result = await client.PostAsync(addressToPostTo, c);
            returnedMessage = await result.Content.ReadAsStringAsync(); ;
            //string dataToPost = @"{"serviceName": "my - service"}"
            return true;
        }

        public async Task<bool> ShutdownEntireDevice()
        {
            using var client = new HttpClient();
            string addressToPostTo = $"{address}/v1/shutdown?apikey={apiKey}";
            HttpContent c = new StringContent("", Encoding.UTF8, "application/json");
            var result = await client.PostAsync(addressToPostTo, c);
            returnedMessage = await result.Content.ReadAsStringAsync(); ;
            //string dataToPost = @"{"serviceName": "my - service"}"
            return true;
        }

        public async Task<bool> RestartWebApp()
        {
            await RestartContainer("bapwebapp");
            return true;
        }

    }
}
