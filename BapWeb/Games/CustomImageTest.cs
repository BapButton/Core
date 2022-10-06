using Microsoft.Extensions.Logging;
using NLog.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BapButton;
using BapShared;

namespace BapWeb.Games
{
    public class CustomImageTest : ITinkerGame
    {
        ILogger<CustomImageTest> _logger { get; set; }
        MessageSender MsgSender { get; set; }

        public bool IsGameRunning { get; set; }

        public CustomImageTest(ILogger<CustomImageTest> logger, MessageSender messageSender)
        {
            _logger = logger;
            MsgSender = messageSender;
        }

        public string GameName()
        {
            return "Custom Image Testing";
        }

        public async Task<bool> ForceEndGame()
        {
            End("Game was force Closed");
            return true;
        }

        public async Task<bool> Start()
        {
            _logger.LogInformation($"Starting Custom Images");
            IsGameRunning = true;
            MsgSender.SendUpdate("Starting Custom Image Test");
            string path = Path.Combine(".", "wwwroot", "sprites", "SwordBonusGame.bmp");
            SpriteParser spriteParser = new SpriteParser(path);
            var sprites = spriteParser.GetCustomImagesFromCustomSprite();
            foreach (var sprite in sprites)
            {
                MsgSender.SendCustomImage(new CustomImage() { ImageData = sprite.Value, ImageId = sprite.Key + 1 });
                await Task.Delay(100);
            }
            for (int i = 1; i <= 2; i++)
            {
                if (IsGameRunning)
                {
                    _logger.LogInformation($"Sending Image {i}");
                    ButtonDisplay msg = new ButtonDisplay(0, 255, 0, Patterns.NoPattern, i);
                    MsgSender.SendGeneralCommand(new StandardButtonCommand(msg));
                    await Task.Delay(2500);

                }

            }

            IsGameRunning = false;
            MsgSender.SendUpdate("Ended Letter Test", true);
            return true;
        }
        public void End(string reason)
        {
            MsgSender.SendUpdate(reason, true);
            IsGameRunning = false;
        }

        public void Dispose()
        {

        }
    }

}
