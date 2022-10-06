using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Text;

namespace BapButton
{
    public static class SpriteCreator
    {
        private const int borderSize = 4;
        //private const int itemsPerRow = 10;
        public static bool CreateNewSprite(string newSpriteFileName, ulong[] firstSprite)
        {
            using (var image = new Image<Rgb24>(borderSize + borderSize + 8, borderSize + borderSize + 8, new Rgb24(0, 0, 0)))
            {
                WriteItemToImage(image, firstSprite, borderSize, borderSize);
                image.SaveAsBmp(newSpriteFileName);
            }
            return true;
        }

        private static void WriteItemToImage(Image<Rgb24> image, ulong[] imageData, int startingPositionX, int startingPositionY)
        {

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    ulong currentValue = imageData[y * 8 + x];
                    byte r = GetColor(currentValue, 16);
                    byte g = GetColor(currentValue, 8);
                    byte b = GetColor(currentValue, 0);
                    int fy = startingPositionY + y;
                    int fx = startingPositionX + x;
                    image[fx, fy] = new Rgb24(r, g, b);
                }
            }
        }

        static byte GetColor(ulong value, int startingPositionFromRight = 0)
        {
            return (byte)(((1 << 8) - 1) & (value >> (startingPositionFromRight)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="imageToAdd"></param>
        /// <param name="imageNumber">If this is -1 it means to add it to the end of the sprite.</param>
        /// <param name="borderSize"></param>
        /// <returns></returns>
        public static bool AddImageToSprite(string fileName, ulong[] imageToAdd, int imageNumber = -1)
        {
            int startingX = imageNumber % 10 * (8 + borderSize);
            int startingY = 4;


            //need to expand the image;

            using (var image = Image.Load<Rgb24>(fileName))
            {
                if (imageNumber == -1)
                {
                    startingX = image.Width;
                }
                if (image.Width < startingX + 8 + 4)
                {
                    image.Mutate(x => x.Pad(startingX + ((8 + 4) * 2), image.Height, Color.Black));
                    image.Mutate(x => x.Crop(new Rectangle(12, 0, image.Width - 12, image.Height)));
                }
                WriteItemToImage(image, imageToAdd, startingX, startingY);
                image.SaveAsBmp(fileName);
            }

            return true;
        }
    }
}
