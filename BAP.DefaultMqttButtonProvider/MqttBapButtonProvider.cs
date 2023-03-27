using MessagePack;
using MessagePipe;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MQTTnet;
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
using SixLabors.ImageSharp;
using Microsoft.Extensions.Configuration;
using MQTTnet.Client;

namespace BAP.DefaultMqttButtonProvider
{
    [BapProvider("Default MQTT Button Provider", "Default provider. Sends MQTT messages to communicate to any subscribed buttons", "4520ba12-1f34-5872-a735-6ce533f940e7")]
    public class DefaultMqttBapButtonProvider : IButtonProvider
    {
        private readonly ILogger<DefaultMqttBapButtonProvider> _logger;
        public ConcurrentDictionary<string, bool> ConnectedNodes = new ConcurrentDictionary<string, bool>();
        public ConcurrentDictionary<string, ButtonStatus> AllButtonStatus = new();
        private string mqttIpAddress;
        public IManagedMqttClient? managedMqttClient = null;
        IDisposable subscriptions = default!;
        private ISubscriber<StandardButtonImageMessage> StandardButtonMessagePipe { get; set; }
        private ISubscriber<RestartButtonMessage> RestartButtonMessagePipe { get; set; }
        private ISubscriber<StatusButtonMessage> StatusButtonMessagePipe { get; set; }
        private ISubscriber<EnableTestingModeMessage> EnableTestingModePipe { get; set; }
        //private ISubscriber<InternalCustomImageMessage> InternalCustomImagePipe { get; set; }
        private ISubscriber<TurnOffButtonMessage> TurnOffButtonMessagePipe { get; set; }
        private IPublisher<NodeChangeMessage> NodeChangeSender { get; set; } = default!;
        private IPublisher<ButtonPressedMessage> ButtonPressSender { get; set; } = default!;
        private ILayoutProvider LayoutProvider { get; init; }

        public DefaultMqttBapButtonProvider(IConfiguration configuration, ILogger<DefaultMqttBapButtonProvider> logger, IPublisher<NodeChangeMessage> nodeChangeSender, ISubscriber<StandardButtonImageMessage> standardButtonMessagePipe, ISubscriber<RestartButtonMessage> restartButtonMessagePipe, ISubscriber<StatusButtonMessage> statusButtonMessagePipe, ISubscriber<TurnOffButtonMessage> turnOffButtonMessagePipe, ISubscriber<EnableTestingModeMessage> enableTestingModePipe, IPublisher<ButtonPressedMessage> buttonPressSender, ILayoutProvider layoutProvider)
        {
            _logger = logger;
            StandardButtonMessagePipe = standardButtonMessagePipe;
            RestartButtonMessagePipe = restartButtonMessagePipe;
            StatusButtonMessagePipe = statusButtonMessagePipe;
            TurnOffButtonMessagePipe = turnOffButtonMessagePipe;
            EnableTestingModePipe = enableTestingModePipe;
            mqttIpAddress = configuration.GetValue<string>("MosquittoAddress") ?? "";
            NodeChangeSender = nodeChangeSender;
            ButtonPressSender = buttonPressSender;
            LayoutProvider = layoutProvider;

        }

        private async Task<bool> Initialize(Action<MqttApplicationMessageReceivedEventArgs>? customCallback = null, string hostName = "")
        {
            var bag = DisposableBag.CreateBuilder();
            StandardButtonMessagePipe.Subscribe(async (x) => await SendCommand(x.NodeId, x.ButtonImage)).AddTo(bag);
            StatusButtonMessagePipe.Subscribe(async (x) => await GetStatusFromButttons(x.NodeId)).AddTo(bag);
            TurnOffButtonMessagePipe.Subscribe(async (x) => await TurnOffButton(x.NodeId)).AddTo(bag);
            RestartButtonMessagePipe.Subscribe(async (x) => await RestartAButton(x.NodeId)).AddTo(bag);
            EnableTestingModePipe.Subscribe(async (x) => await EnableTestingMode(x.NodeId)).AddTo(bag);
            //InternalCustomImagePipe.Subscribe(async (x) => await SendInternalCustomImage(x.NodeId, x.CustomImage)).AddTo(bag);
            subscriptions = bag.Build();

            if (managedMqttClient == null)
            {
                string clientId = string.IsNullOrEmpty(hostName) ? $"ButtonMaster-{GetRandomInt(0, 1000000000)}" : hostName;
                var factory = new MqttFactory();
                var mqttClient = factory.CreateMqttClient();
                var options = new MqttClientOptionsBuilder()
                                .WithClientId(clientId)
                                .WithTcpServer(mqttIpAddress)
                                .WithCleanSession(true)
                                .Build();

                managedMqttClient = factory.CreateManagedMqttClient();
                if (customCallback != null)
                {
                    _logger.LogTrace("Using a custom Callback when initializing the MQTT client");
                    managedMqttClient.ApplicationMessageReceivedAsync += e =>
                    {
                        Console.WriteLine("Received application message.");
                        customCallback(e);
                        return Task.CompletedTask;
                    };
                }
                else
                {
                    managedMqttClient.ApplicationMessageReceivedAsync += e =>
                    {
                        Console.WriteLine("Received application message.");
                        {
                            CallBack(e);
                            return Task.CompletedTask;
                        };
                    };
                    managedMqttClient.ConnectingFailedAsync += e =>
                    {
                        HandleConnectingFailedAsync(e);
                        return Task.CompletedTask;
                    };
                    //Need to do all the subscriptions.
                    await managedMqttClient.StartAsync(
                        new ManagedMqttClientOptions
                        {
                            ClientOptions = options
                        });
                    _logger.LogInformation($"Attempted a connection to mqtt server {mqttIpAddress}");
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

            }
            return true;
        }

        private Task HandleConnectingFailedAsync(ConnectingFailedEventArgs eventArgs)
        {
            _logger.LogError($"Connecting to MQTT failed with message {eventArgs.Exception.Message}");
            Console.WriteLine(eventArgs.Exception.Message);
            return Task.FromResult(true);
        }

        public IManagedMqttClient? GetCurrentClient()
        {
            return managedMqttClient;
        }
        public async Task<bool> InitializeAsync()
        {
            return await Initialize(null, "");
        }

        public async Task<bool> InitializeWtihCustomCallback(Action<MqttApplicationMessageReceivedEventArgs> customCallback, string clientId)
        {
            return await Initialize(customCallback, clientId);
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

        private async Task<bool> EnableTestingMode(string nodeId)
        {
            return await SendPrivate(nodeId, "testing", true);

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
                   .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                   .Build();
                await managedMqttClient.EnqueueAsync(message);
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



}
