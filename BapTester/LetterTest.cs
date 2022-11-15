using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BapButton;
using BAP.Types;
using BAP.Helpers;

namespace BapTester
{
    public class LetterTest
    {
        public BapMessageSender MsgSender { get; internal set; }
        public LetterTest(BapMessageSender msgSender)
        {
            MsgSender = msgSender;
        }
        public async Task<bool> Start()
        {
            Console.WriteLine("Starting Pattern Test");
            //ButtonDisplay msg = new ButtonDisplay(128, 64, 32, Patterns.NoPattern, 0, turnOffAfterMillis: 3000);
            ////
            //MsgSender.SendCommand(MsgSender.GetConnectedButtons().First(), new StandardButtonCommand(msg));
            Console.WriteLine($"Writing custom image");
            string path = System.IO.Path.Combine(".", "wwwroot", "sprites", "Emoji.png");
            SpriteParser spriteParser = new SpriteParser(path);
            var sword = spriteParser.GetSprite(4, 5, 24, 20, 16, 6, 7);
            MsgSender.SendCustomImage(new CustomImage() { ImageData = sword, ImageId = 16 });
            ButtonDisplay newMsg = new ButtonDisplay(128, 64, 32, Patterns.Border, 16, turnOffAfterMillis: 3000);
            StandardButtonCommand sbc = new StandardButtonCommand(newMsg);
            MsgSender.SendCommand(MsgSender.GetConnectedButtons().First(), sbc);

            await Task.Delay(10000);

            for (int i = 0; i <= (int)Patterns.CheckMark; i++)
            {
                Console.WriteLine($"Writing {(Patterns)i}");
                StandardButtonCommand cmd = new StandardButtonCommand();
                ButtonDisplay bd = new ButtonDisplay(0, 255, 0, (Patterns)i, turnOffAfterMillis: 2000);
                MsgSender.SendGeneralCommand(cmd);

                await Task.Delay(2500);
            }


            return true;
        }
    }
}
