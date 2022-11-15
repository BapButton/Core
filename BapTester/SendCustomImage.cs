using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BapButton;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.ColorSpaces;
using SixLabors.ImageSharp.PixelFormats;
using System.Linq;
using BAP.Types;
using BAP.Helpers;

namespace BapTester
{
    public class SendCustomImage
    {
        BapMessageSender MsgSender;
        SendCustomImage(BapMessageSender sender)
        {
            MsgSender = sender;
        }

        public void SendCurrentImage(ulong[] fullImageData, int imageId)
        {
            CustomImage customImage = new CustomImage()
            {
                ImageId = imageId,
                ImageData = fullImageData
            };
            MsgSender.SendCustomImage(customImage);

        }
        public void SendImage(int imageId)
        {

            CustomImage customImage = new CustomImage()
            {
                ImageId = imageId//,
                //FirstHalf = true
            };
            ulong[] fullImageData = new ulong[64];
            using (var image = Image.Load<Rgb24>(".\\\\images\\\\GelotteCastle.png"))
            {
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        var rgb = image[x, y];
                        fullImageData[y * 8 + x] = (ulong)(((rgb.R & 0x0ff) << 16) | ((rgb.G & 0x0ff) << 8) | (rgb.B & 0x0ff));
                    }
                }
            }
            customImage.ImageData = fullImageData;
            MsgSender.SendCustomImage(customImage);
        }
        public async Task<bool> SendCustomImageMessage(int imageId)
        {
            ButtonDisplay standardButtonMessage = new ButtonDisplay(pattern: Patterns.NoPattern, customImage: imageId);
            string nodeId = MsgSender.GetConnectedButtons().First();
            MsgSender.SendCommand(nodeId, new StandardButtonCommand(standardButtonMessage));
            return true;
        }
    }
}
