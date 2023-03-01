using MessagePipe;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BAP.Types;

namespace BAP.TestUtilities
{
    [BapProvider("Mock Button Provider", "Sends to on screen buttons via messages", "5720ba12-1f98-5872-a735-6ce233f940e7")]
    public class MockButtonProvider : IButtonProvider
    {
        private const string buttonAddressBase = "AAA";
        private const string NodeListGamdDataSaverKey = "NodeList";
        private readonly ILogger<MockButtonProvider> _logger;
        //public delegate void MyClickHandler(object sender, string myValue);
        public ConcurrentDictionary<string, bool> ConnectedNodes = new();
        public ConcurrentDictionary<string, ButtonStatus> AllButtonStatus = new();
        private IPublisher<NodeChangeMessage> NodeChangeSender { get; set; } = default!;
        public ISubscriber<TurnOffButtonMessage> TurnOffButtonMessagePipe { get; set; }
        IDisposable subscriptions = default!;
        IGameDataSaver<MockButtonProvider> GameDataSaver { get; set; }

        public string Name => "Mock Buttons";

        public MockButtonProvider(ILogger<MockButtonProvider> logger, ISubscriber<TurnOffButtonMessage> turnOffButtonMessagePipe, IPublisher<NodeChangeMessage> nodeChangeSender, IGameDataSaver<MockButtonProvider> gameDataSaver)
        {
            _logger = logger;
            TurnOffButtonMessagePipe = turnOffButtonMessagePipe;
            var bag = DisposableBag.CreateBuilder();
            TurnOffButtonMessagePipe.Subscribe(async (x) => await TurnOffButton(x)).AddTo(bag);
            subscriptions = bag.Build();
            NodeChangeSender = nodeChangeSender;
            GameDataSaver = gameDataSaver;
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task TurnOffButton(TurnOffButtonMessage turnOffButtonMessage)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            if (string.IsNullOrEmpty(turnOffButtonMessage.NodeId))
            {
                foreach (var node in ConnectedNodes.ToList())
                {
                    await RemoveNode(node.Key);
                }
            }
            else
            {
               await  RemoveNode(turnOffButtonMessage.NodeId);
            }

        }
        public async Task<(string, ButtonStatus)> AddNode(string clientId = "")
        {
            if (string.IsNullOrEmpty(clientId))
            {
                string highestNodeId = ConnectedNodes.Count > 0 ? ConnectedNodes.OrderByDescending(t => t.Key).FirstOrDefault().Key : "";
                if (string.IsNullOrEmpty(highestNodeId))
                {
                    clientId = buttonAddressBase + "001";
                }
                else
                {
                    int number = int.Parse(highestNodeId[3..].TrimStart('0'));
                    number++;
                    clientId = buttonAddressBase + number.ToString("D3");
                }
            }

            ButtonStatus buttonStatus = new ButtonStatus()
            {
                WifiStrength = 100,
                BatteryLevel = 0,
                IPAddress = "0.0.0.0",
                VersionId = "Mock"

            };
            ConnectedNodes.AddOrUpdate(clientId, false, (key, oldValue) => false);
            AllButtonStatus.AddOrUpdate(clientId, buttonStatus, (key, oldValue) => buttonStatus);
            NodeChangeSender.Publish(new NodeChangeMessage(clientId, false));
            await GameDataSaver.UpdateGameStorage(ConnectedNodes.Select(t => t.Key).ToList(), NodeListGamdDataSaverKey);
            return (clientId, buttonStatus);
        }

        public async Task RemoveNode(string clientId)
        {

            ConnectedNodes.TryRemove(clientId, out bool _);
            AllButtonStatus.TryRemove(clientId, out ButtonStatus _);
            NodeChangeSender.Publish(new NodeChangeMessage(clientId, true));
            await GameDataSaver.UpdateGameStorage(ConnectedNodes.Select(t => t.Key).ToList(), NodeListGamdDataSaverKey);
        }

        public async Task<bool> Initialize(string ignoredString)
        {
            if (ConnectedNodes == null || ConnectedNodes.Count == 0)
            {
                ConnectedNodes = new ConcurrentDictionary<string, bool>();
                var oldNodes = await GameDataSaver.GetGameStorage<List<string>?>(NodeListGamdDataSaverKey);
                if (oldNodes != null)
                {
                    foreach (var node in oldNodes)
                    {
                        await AddNode(node);
                    }
                }
                else
                {
                    for (int i = 0; i < 5; i++)
                    {
                        await AddNode();
                    }
                }

            }

            return true;
        }

        public List<string> GetConnectedButtons()
        {
            return ConnectedNodes.Keys.ToList();
        }


        public void Dispose()
        {
            ConnectedNodes = new ConcurrentDictionary<string, bool>();
            if (subscriptions != null)
            {
                subscriptions.Dispose();
            }


        }

        public List<(string nodeId, ButtonStatus buttonStatus)> GetAllConnectedButtonInfo()
        {
            return AllButtonStatus.Select(t => new ValueTuple<string, ButtonStatus>(t.Key, t.Value)).ToList();
        }

        public List<string> GetConnectedButtonsInOrder()
        {
            return ConnectedNodes.Keys.OrderBy(t => t).ToList();
        }

        public async Task<bool> InitializeAsync()
        {
            return await Initialize("");
        }
    }


}
