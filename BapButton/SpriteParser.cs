using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Text;

namespace BapButton
{
    public class SpriteParser
    {
        private string FileName { get; set; }
        public SpriteParser(string fileName)
        {
            FileName = fileName;
        }
        public ulong[] GetImageFromFile()
        {
            using (var image = Image.Load<Rgb24>(FileName))
            {
                ulong[] fullImageData = GetSpriteFromImage(0, 0, image, 1);
                return fullImageData;
            }
        }
        public List<ulong[,]> GetCustomMatricesFromCustomSprite()
        {
            List<ulong[,]> sprites = new List<ulong[,]>();
            try
            {
                using (var image = Image.Load<Rgb24>(FileName))
                {
                    int requiredHeight = 16;
                    if (image.Height != requiredHeight)
                    {
                        throw new Exception($"Invalid Height for a custom sprint. It must be {requiredHeight} but instead it is {image.Height}");
                    }
                    int remainder = (image.Width - 4) % 12;
                    if (remainder != 0)
                    {
                        throw new Exception($"Invalid width for a custom sprint. When subtractring 4 it must be directly divisable by 8. but the Width is {image.Width}");
                    }
                    int imageCount = (image.Width - 4) / 12;
                    for (int i = 0; i < imageCount; i++)
                    {
                        var sprite = GetSpriteFromImageAsMatrix((i * 12) + 4, 4, image);
                        sprites.Add(sprite);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return sprites;
        }
        public Dictionary<int, ulong[]> GetCustomImagesFromCustomSprite()
        {
            Dictionary<int, ulong[]> sprites = new Dictionary<int, ulong[]>();
            try
            {
                using (var image = Image.Load<Rgb24>(FileName))
                {
                    int requiredHeight = 16;
                    if (image.Height != requiredHeight)
                    {
                        throw new Exception($"Invalid Height for a custom sprint. It must be {requiredHeight} but instead it is {image.Height}");
                    }
                    int remainder = (image.Width - 4) % 12;
                    if (remainder != 0)
                    {
                        throw new Exception($"Invalid width for a custom sprint. When subtractring 4 it must be directly divisable by 8. but the Width is {image.Width}");
                    }
                    int imageCount = (image.Width - 4) / 12;
                    for (int i = 0; i < imageCount; i++)
                    {
                        var sprite = GetSpriteFromImage((i * 12) + 4, 4, image);
                        sprites.Add(i, sprite);
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return sprites;
        }
        public ulong[] GetSprite(int pixelScaling, int leftOffset, int topOffset, int betweenImageOffset, int vertImageSpacing, int rowId, int numberInRow)
        {

            rowId--; //Make it zero based.
            numberInRow--; //Make it zero mased.
            int startingxLocation = (numberInRow) * 8 * pixelScaling + leftOffset + ((numberInRow) * betweenImageOffset);
            int startingyLocation = rowId * 8 * pixelScaling + topOffset + rowId * vertImageSpacing;
            using (var image = Image.Load<Rgb24>(FileName))
            {
                ulong[] fullImageData = GetSpriteFromImage(startingxLocation, startingyLocation, image, pixelScaling);
                return fullImageData;
            }

        }
        public ulong[,] GetSpriteAsMatrix(int pixelScaling, int leftOffset, int topOffset, int betweenImageOffset, int vertImageSpacing, int rowId, int numberInRow)
        {

            rowId--; //Make it zero based.
            numberInRow--; //Make it zero mased.
            int startingxLocation = (numberInRow) * 8 * pixelScaling + leftOffset + ((numberInRow) * betweenImageOffset);
            int startingyLocation = rowId * 8 * pixelScaling + topOffset + rowId * vertImageSpacing;
            using (var image = Image.Load<Rgb24>(FileName))
            {
                ulong[,] fullImageData = GetSpriteFromImageAsMatrix(startingxLocation, startingyLocation, image, pixelScaling);
                return fullImageData;
            }

        }

        public ulong[] GetSpriteFromImage(int startingxLocation, int startingyLocation, Image<Rgb24> image, int pixelScaling = 1)
        {
            ulong[] fullImageData = new ulong[64];
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    int fy = startingyLocation + y * pixelScaling;
                    int fx = startingxLocation + x * pixelScaling;
                    var rgb = image[fx, fy];
                    fullImageData[y * 8 + x] = (ulong)(((rgb.R & 0x0ff) << 16) | ((rgb.G & 0x0ff) << 8) | (rgb.B & 0x0ff));
                }
            }
            return fullImageData;
        }
        public ulong[,] GetSpriteFromImageAsMatrix(int startingxLocation, int startingyLocation, Image<Rgb24> image, int pixelScaling = 1)
        {
            ulong[,] fullImageData = new ulong[8, 8];
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    int fy = startingyLocation + y * pixelScaling;
                    int fx = startingxLocation + x * pixelScaling;
                    var rgb = image[fx, fy];
                    fullImageData[y, x] = (ulong)(((rgb.R & 0x0ff) << 16) | ((rgb.G & 0x0ff) << 8) | (rgb.B & 0x0ff));
                }
            }
            return fullImageData;
        }
    }
}
