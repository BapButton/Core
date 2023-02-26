using MessagePack;
using MessagePipe;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MQTTnet;
using MQTTnet.Client.Options;
using MQTTnet.Extensions.ManagedClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BAP.Types;
using static BAP.Helpers.BapBasicGameHelper;

namespace DefaultMqttButtonProvider
{
    [BapProvider("Default MQTT Button Provider", "Default provider. Sends MQTT messages to communicate to any subscribed buttons", "4520ba12-1f34-5872-a735-6ce533f940e7")]
    public class DefaultMqttBapButtonProvider : IButtonProvider
    {
        private readonly ILogger<DefaultMqttBapButtonProvider> _logger;
        public ConcurrentDictionary<string, bool> ConnectedNodes = new ConcurrentDictionary<string, bool>();
        public ConcurrentDictionary<string, ButtonStatus> AllButtonStatus = new();
        public IManagedMqttClient? managedMqttClient = null;
        IDisposable subscriptions = default!;
        private ISubscriber<StandardButtonImageMessage> StandardButtonMessagePipe { get; set; }
        private ISubscriber<RestartButtonMessage> RestartButtonMessagePipe { get; set; }
        private ISubscriber<StatusButtonMessage> StatusButtonMessagePipe { get; set; }
        //private ISubscriber<InternalCustomImageMessage> InternalCustomImagePipe { get; set; }
        private ISubscriber<TurnOffButtonMessage> TurnOffButtonMessagePipe { get; set; }
        private IPublisher<NodeChangeMessage> NodeChangeSender { get; set; } = default!;
        private IPublisher<ButtonPressedMessage> ButtonPressSender { get; set; } = default!;
        private ILayoutProvider LayoutProvider { get; init; }

        public DefaultMqttBapButtonProvider(ILogger<DefaultMqttBapButtonProvider> logger, IPublisher<NodeChangeMessage> nodeChangeSender, ISubscriber<StandardButtonImageMessage> standardButtonMessagePipe, ISubscriber<RestartButtonMessage> restartButtonMessagePipe, ISubscriber<StatusButtonMessage> statusButtonMessagePipe, ISubscriber<TurnOffButtonMessage> turnOffButtonMessagePipe, IPublisher<ButtonPressedMessage> buttonPressSender, ILayoutProvider layoutProvider)//ISubscriber<InternalCustomImageMessage> internalCustomImage, )
        {
            _logger = logger;
            StandardButtonMessagePipe = standardButtonMessagePipe;
            RestartButtonMessagePipe = restartButtonMessagePipe;
            StatusButtonMessagePipe = statusButtonMessagePipe;
            TurnOffButtonMessagePipe = turnOffButtonMessagePipe;
            //InternalCustomImagePipe = internalCustomImage;
            NodeChangeSender = nodeChangeSender;
            ButtonPressSender = buttonPressSender;
            LayoutProvider = layoutProvider;

        }
        //This method is not very well thought out. It should probably be an environmental Variable. 
        private string GetCorrectDefautltIpAddress()
        {
            try
            {
                NetworkInterface? bestInterface = null;
                foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces().Where(t => t.OperationalStatus == OperationalStatus.Up && !t.IsReceiveOnly && t.NetworkInterfaceType != NetworkInterfaceType.Loopback).OrderByDescending(t => t.Speed))
                {
                    bestInterface = bestInterface ?? adapter;
                    if (bestInterface.Name.StartsWith("v"))
                    {
                        bestInterface = adapter;
                    }

                }
                string ipAddress = "";
                var gatewayAddress = bestInterface?.GetIPProperties()?.GatewayAddresses?.FirstOrDefault()?.Address ?? new System.Net.IPAddress(new byte[] { 0, 0, 0, 0 });
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    if (gatewayAddress.ToString().StartsWith("192.168.86"))
                    {
                        //Todo: Need to figure out a way to pull this from config or something.
                        ipAddress = "192.168.86.50";
                    }
                }
                if (string.IsNullOrEmpty(ipAddress))
                {
                    ipAddress = gatewayAddress.ToString();
                }
                return ipAddress;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting the Ip Address of the network Gateway");
                return "192.168.0.1";
            }

        }
        private async Task<bool> Initialize(string mqttServerIp = "", Action<MqttApplicationMessageReceivedEventArgs>? customCallback = null, string hostName = "")
        {
            var bag = DisposableBag.CreateBuilder();
            StandardButtonMessagePipe.Subscribe(async (x) => await SendCommand(x.NodeId, x.ButtonImage)).AddTo(bag);
            StatusButtonMessagePipe.Subscribe(async (x) => await GetStatusFromButttons(x.NodeId)).AddTo(bag);
            TurnOffButtonMessagePipe.Subscribe(async (x) => await TurnOffButton(x.NodeId)).AddTo(bag);
            RestartButtonMessagePipe.Subscribe(async (x) => await RestartAButton(x.NodeId)).AddTo(bag);
            //InternalCustomImagePipe.Subscribe(async (x) => await SendInternalCustomImage(x.NodeId, x.CustomImage)).AddTo(bag);
            subscriptions = bag.Build();

            if (managedMqttClient == null)
            {
                if (string.IsNullOrEmpty(mqttServerIp))
                {
                    mqttServerIp = GetCorrectDefautltIpAddress();
                }
                if (mqttServerIp.StartsWith("0"))
                {
                    return false;
                }
                string clientId = string.IsNullOrEmpty(hostName) ? $"ButtonMaster-{GetRandomInt(0, 1000000000)}" : hostName;
                var factory = new MqttFactory();
                var mqttClient = factory.CreateMqttClient();
                var options = new MqttClientOptionsBuilder()
                                .WithClientId(clientId)
                                .WithTcpServer(mqttServerIp)
                                .WithCleanSession(true)
                                .Build();

                managedMqttClient = factory.CreateManagedMqttClient();
                if (customCallback != null)
                {
                    _logger.LogTrace("Using a custom Callback when initializing the MQTT client");
                    managedMqttClient.UseApplicationMessageReceivedHandler(e => { customCallback(e); });
                }
                else
                {
                    managedMqttClient.UseApplicationMessageReceivedHandler(e => { CallBack(e); });
                }
                managedMqttClient.ConnectingFailedHandler = new HandleNotConnected();
                //Need to do all the subscriptions.
                await managedMqttClient.StartAsync(
                    new ManagedMqttClientOptions
                    {
                        ClientOptions = options
                    });
                _logger.LogInformation($"Attempted a connection to mqtt server {mqttServerIp}");
                await Task.Delay(2000);

                if (!managedMqttClient.IsStarted)
                {
                    _logger.LogInformation($"The MQTT client is not started. Things are not looking good but we will wait and see if we can get connected.");
                    await Task.Delay(3000);
                    if (!managedMqttClient.IsConnected)
                    {
                        _logger.LogCritical($"You are not connected to the MQTT server. Everything will fail. This is bad. But maybe if you are just going to use the mock provider then it's fine :)");
                    }
                }
                else
                {
                    _logger.LogInformation($"Looks like the connection succeeded on the first try");
                }
                await managedMqttClient.SubscribeAsync("buttons/#", MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce);
                await Task.Delay(2000);
            }
            return true;
        }


        public IManagedMqttClient? GetCurrentClient()
        {
            return managedMqttClient;
        }
        public async Task<bool> InitializeAsync()
        {
            string? mosquittoAddress = Environment.GetEnvironmentVariable("MosquittoAddress");
            return await Initialize(mosquittoAddress ?? "", null, "");
        }

        public async Task<bool> Initialize(string mqttServerIp = "")
        {
            return await Initialize(mqttServerIp, null, "");
        }
        public async Task<bool> InitializeWtihCustomCallback(Action<MqttApplicationMessageReceivedEventArgs> customCallback, string clientId, string mqttServerIp = "")
        {
            return await Initialize(mqttServerIp, customCallback, clientId);
        }

        public List<string> GetConnectedButtons()
        {
            return ConnectedNodes.Keys.ToList();
        }

        public List<string> GetConnectedButtonsInOrder()
        {
            ButtonLayout? currentLayout = LayoutProvider.CurrentButtonLayout;
            if (currentLayout != null)
            {
                IEnumerable<string> buttons = currentLayout.ButtonPositions.OrderBy(t => t.RowId).ThenBy(t => t.ColumnId).Select(t => t.ButtonId).ToList();
                return buttons.Where(t => ConnectedNodes.ContainsKey(t)).ToList(); ;
            }
            else
            {
                return ConnectedNodes.Keys.ToList();
            }

        }


        private async Task<bool> SendCommand(string nodeId, ButtonImage buttonImage)
        {
            if (string.IsNullOrEmpty(nodeId))
            {
                return await SendImageToAllButtons(buttonImage);
            }

            return await SendPrivate(nodeId, "command", buttonImage);

        }
        private async Task<bool> GetStatusFromButttons(string nodeId)
        {
            if (string.IsNullOrEmpty(nodeId))
            {
                return await GetStatusFromAllButttons();

            }
            return await SendPrivate(nodeId, "getstatus", new ButtonStatus());

        }

        private async Task<bool> GetStatusFromAllButttons()
        {
            ConnectedNodes = new ConcurrentDictionary<string, bool>();
            return await SendPublic("status", true);
        }

        private async Task<bool> RestartAllButtons()
        {
            ConnectedNodes = new ConcurrentDictionary<string, bool>();
            return await SendPublic("restart", true);
        }

        private async Task<bool> RestartAButton(string nodeId)
        {
            if (string.IsNullOrEmpty(nodeId))
            {
                return await RestartAllButtons();
            }
            else
            {
                return await SendPrivate(nodeId, "restart", true);
            }

        }

        private async Task<bool> SendImageToAllButtons(ButtonImage buttonImage)
        {
            return await SendPublic("command", buttonImage);
        }

        public async Task<bool> SendButtonImage(string nodeId, ButtonImage buttonImage)
        {
            if (string.IsNullOrEmpty(nodeId))
            {
                return await SendImageToAllButtons(buttonImage);
            }
            else
            {
                return await SendPrivate(nodeId, "command", buttonImage);
            }
        }


        private async Task<bool> SendPrivate(string nodeId, string topic, object objectToSend)
        {
            if (ConnectedNodes.ContainsKey(nodeId))
            {
                return await Send($"buttons/{nodeId}/{topic}", objectToSend);
            }
            else
            {
                _logger.LogInformation($"You are trying to send to an Invalid Node. Node {nodeId} is not currently in the ConnectedNodes list. We will just forget that this ever happened.");
                return false;
            }
        }
        private async Task<bool> SendPublic(string subTopic, object objectToSend)
        {
            return await Send($"general/{subTopic}", objectToSend);
        }

        private async Task<bool> Send(string topic, object objectToSend)
        {
            if (managedMqttClient != null)
            {
                var message = new MqttApplicationMessageBuilder()
                   .WithTopic(topic)
                   .WithPayload(MessagePackSerializer.Serialize(objectToSend))
                   .WithAtLeastOnceQoS()
                   .Build();
                var result = await managedMqttClient.PublishAsync(message, CancellationToken.None);
                bool success = result.ReasonCode == MQTTnet.Client.Publishing.MqttClientPublishReasonCode.Success;
                if (success)
                {
                    _logger.LogDebug($"Succesfully sent command '{objectToSend}' to node {topic}");

                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }

        private void CallBack(MqttApplicationMessageReceivedEventArgs e)
        {
            if (e.ApplicationMessage.Topic.StartsWith("$SYS"))
            {
                Console.WriteLine($"Got a Sys Message from topic {e.ApplicationMessage.Topic}");
                Console.WriteLine(e.ApplicationMessage.ConvertPayloadToString());
            }
            else
            {

                string effectiveTopic = e.ApplicationMessage.Topic.Split("/")[2];
                string clientId = e.ApplicationMessage.Topic.Split("/")[1];
                if (effectiveTopic == "announce")
                {
                    ButtonPress bp = MessagePackSerializer.Deserialize<ButtonPress>(e.ApplicationMessage.Payload);
                    ButtonPressSender.Publish(new ButtonPressedMessage(clientId, bp));
                }
                if (effectiveTopic == "status")
                {
                    if (e.ApplicationMessage.Payload == null)
                    {
                        ConnectedNodes.TryRemove(clientId, out bool _);
                        AllButtonStatus.TryRemove(clientId, out ButtonStatus _);
                        NodeChangeSender.Publish(new NodeChangeMessage(clientId, true));
                    }
                    else
                    {
                        ConnectedNodes.AddOrUpdate(clientId, false, (key, oldValue) => false);
                        ButtonStatus buttonStatus = MessagePackSerializer.Deserialize<ButtonStatus>(e.ApplicationMessage.Payload);
                        AllButtonStatus.AddOrUpdate(clientId, buttonStatus, (key, oldValue) => buttonStatus);
                        NodeChangeSender.Publish(new NodeChangeMessage(clientId, false));

                    }
                }

            }
        }

        public void Dispose()
        {
            ConnectedNodes = new ConcurrentDictionary<string, bool>();
            managedMqttClient?.Dispose();
            if (subscriptions != null)
            {
                subscriptions.Dispose();
            }
        }

        public List<(string nodeId, ButtonStatus buttonStatus)> GetAllConnectedButtonInfo()
        {
            return AllButtonStatus.Select(t => new ValueTuple<string, ButtonStatus>(t.Key, t.Value)).ToList();
        }

        public async Task<bool> TurnOffAllButtons()
        {
            return await SendPublic("poweroff", true);
        }

        public async Task<bool> TurnOffButton(string nodeId)
        {
            if (string.IsNullOrWhiteSpace(nodeId))
            {
                return await TurnOffAllButtons();
            }
            else
            {
                return await SendPrivate(nodeId, "poweroff", true);
            }


        }

    }

    public class HandleNotConnected : IConnectingFailedHandler
    {
        public Task HandleConnectingFailedAsync(ManagedProcessFailedEventArgs eventArgs)
        {
            Console.WriteLine(eventArgs.Exception.Message);
            return Task.FromResult(true);
        }
    }
}
