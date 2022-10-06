using MessagePack;
using Microsoft.Extensions.Hosting;
using MQTTnet;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BapButton;
using BapShared;
using static BapButton.TinkerButtonGameMethods;

namespace MqttDecoder
{

    public class Decoder : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            string clientId = $"MqttDecoder-{GetRandomInt(800000, 999999)}";
            //This needs to subscribe to all the messages that are passing back and forth or something.
            //await core.InitializeWtihCustomCallback(CallBack, clientId);
        }

        private void CallBack(MqttApplicationMessageReceivedEventArgs e)
        {
            string topic = e.ApplicationMessage.Topic;
            if (e.ApplicationMessage.Topic.StartsWith("$SYS"))
            {
                Console.WriteLine($"Got a Sys Message from topic {topic}");
                Console.WriteLine(e.ApplicationMessage.ConvertPayloadToString());
            }
            else
            {
                if (topic.EndsWith("command"))
                {
                    if (topic.StartsWith("button"))
                    {
                        string clientId = topic.Split("/")[1];
                        Console.WriteLine($"Command for button {clientId}");
                    }
                    else
                    {
                        Console.WriteLine($"General Command for all buttons");
                        ButtonDisplay sbm = MessagePackSerializer.Deserialize<ButtonDisplay>(e.ApplicationMessage.Payload);
                        Console.WriteLine(sbm.ToString());
                    }
                }
                else if (topic.StartsWith("button"))
                {
                    Console.WriteLine($"Got a message on topic {topic}");
                    string effectiveTopic = topic.Split("/")[2];
                    string clientId = topic.Split("/")[1];
                    if (effectiveTopic == "announce")
                    {
                        ButtonPress bp = MessagePackSerializer.Deserialize<ButtonPress>(e.ApplicationMessage.Payload);
                        Console.WriteLine($"Button press on {clientId} with the TimeSinceLightTurnedOffSetTo {bp.TimeSinceLightTurnedOff} at the time it was pressed");
                        Console.WriteLine($"MillisSinceLight is: {bp.MillisSinceLight}. Time is {bp.UnixTimeOfPress} with an Offset of {bp.MillisOffsetOnPress}");
                    }
                    if (effectiveTopic == "status")
                    {
                        if (e.ApplicationMessage.Payload == null)
                        {
                            Console.WriteLine($"Button {clientId} was disconnected");
                        }
                        else
                        {
                            ButtonStatus bp = MessagePackSerializer.Deserialize<ButtonStatus>(e.ApplicationMessage.Payload);
                            Console.WriteLine($"Button {clientId} reported Status");
                            Console.WriteLine($"{clientId} - Wifi is at {bp.WifiStrength}");
                            Console.WriteLine($"{clientId} - Battery Level is at {bp.BatteryLevel}");
                            Console.WriteLine($"{clientId} - IpAddress is at {bp.IPAddress}");
                            Console.WriteLine($"{clientId} - Version is at {bp.VersionId}");
                        }
                    }

                }


            }
        }
    }
}
