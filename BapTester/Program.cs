using System;
using System.Threading;
using System.Threading.Tasks;
using BapButton;
using System.Linq;
using BapShared;

namespace BapTester
{
    class Program
    {
        static ManualResetEvent _quitEvent = new ManualResetEvent(false);
        static async Task Main(string[] args)
        {
            Console.CancelKeyPress += (sender, eArgs) =>
            {
                _quitEvent.Set();
                eArgs.Cancel = true;
            };
            string newFileName = "c:\\scripts\\ExplodingBap.bmp";


            var spriteParser = new SpriteParser(".\\\\images\\\\GelotteCastle.png");
            var castle = spriteParser.GetImageFromFile();
            SpriteCreator.CreateNewSprite(newFileName, castle);
            spriteParser = new(".\\images\\Emoji.png");
            var sprite = spriteParser.GetSprite(4, 5, 24, 20, 16, 4, 1);
            SpriteCreator.AddImageToSprite(newFileName, sprite);
            sprite = spriteParser.GetSprite(4, 5, 24, 20, 16, 3, 1);
            SpriteCreator.AddImageToSprite(newFileName, sprite);
            sprite = spriteParser.GetSprite(4, 5, 24, 20, 16, 4, 6);
            SpriteCreator.AddImageToSprite(newFileName, sprite);
            sprite = spriteParser.GetSprite(4, 5, 24, 20, 16, 5, 3);
            SpriteCreator.AddImageToSprite(newFileName, sprite);
            sprite = spriteParser.GetSprite(4, 5, 24, 20, 16, 9, 4);
            SpriteCreator.AddImageToSprite(newFileName, sprite);
            sprite = spriteParser.GetSprite(4, 5, 24, 20, 16, 9, 5);
            SpriteCreator.AddImageToSprite(newFileName, sprite);
            sprite = spriteParser.GetSprite(4, 5, 24, 20, 16, 6, 7);
            SpriteCreator.AddImageToSprite(newFileName, sprite);
            sprite = spriteParser.GetSprite(4, 5, 24, 20, 16, 4, 9);
            SpriteCreator.AddImageToSprite(newFileName, sprite);
            sprite = spriteParser.GetSprite(4, 5, 24, 20, 16, 1, 5);
            SpriteCreator.AddImageToSprite(newFileName, sprite);
            sprite = spriteParser.GetSprite(4, 5, 24, 20, 16, 8, 10);
            SpriteCreator.AddImageToSprite(newFileName, sprite);
            sprite = spriteParser.GetSprite(4, 5, 24, 20, 16, 5, 6);
            SpriteCreator.AddImageToSprite(newFileName, sprite);
            Console.WriteLine("Added");

            //SpriteParser spriteParser = new SpriteParser();
            //
            //for (int i = 0; i < imageDetails.Length; i++)
            //{
            //    Console.WriteLine($"{imageDetails[i]},");
            //}
            //int imageId = 2;
            //SpriteParser spriteParser = new SpriteParser();
            ////var sprite = spriteParser.GetSprite(".\\images\\Emoji.png", 4, 5, 24, 20, 16, 5, 7);
            //SendCustomImage sendCustomImage = new SendCustomImage();
            ////await sendCustomImage.SendCurrentImage(sprite, imageId);
            ////await sendCustomImage.SendCustomImageMessage(imageId);
            //ITinkerController core = new BapButton.TinkerConnectionCore();
            //await core.Initialize();
            //var allTheSprites = spriteParser.GetCustomImagesFromCustomSprite("./images/SwordBonusGame.bmp");
            //foreach (var sprite in allTheSprites)
            //{
            //    await MsgSender.SendCustomImage(new CustomImage() { ImageData = sprite.Value, ImageId = sprite.Key + 1 });
            //    await Task.Delay(100);
            //}

            _quitEvent.WaitOne();
        }
    }
}
