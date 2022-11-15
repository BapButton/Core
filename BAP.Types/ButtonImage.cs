using MessagePack;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace BAP.Types
{
    [MessagePackObject]
    public class ButtonImage
    {
        [Key(0)]
        public ulong[] ImageData { get; set; } = new ulong[64];
        public ButtonImage(ulong[] imageData)
        {
            ImageData = imageData;
        }
        public ButtonImage()
        {
            ImageData = new ulong[64]; ;
        }
        public ButtonImage(ulong[,] imageDataMatrix)
        {
            List<ulong> data = new List<ulong>();
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    data.Add(imageDataMatrix[r, c]);
                }
            }
            ImageData = data.ToArray();
        }

        public ButtonImage(List<Byte> bytes, BapColor bapColor)
        {
            ImageData = new ulong[64];
            BitArray bits = new BitArray(bytes.ToArray());
            int length = bits.Length > 63 ? 63 : bits.Length;
            for (int i = 0; i < length; i++)
            {
                if (bits[i])
                {
                    ImageData[i] = bapColor.LongColor;
                }
                else
                {
                    ImageData[i] = 0;
                }
            }
        }
    }

    public static class ButtonImageExtensions
    {
        public static ulong[,] GetImageMatrix(this ButtonImage buttonImage)
        {
            ulong[,] matrix = new ulong[8, 8];
            int currentLocation = 0;
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    matrix[r, c] = buttonImage.ImageData[currentLocation];
                    currentLocation++;
                }
            }
            return matrix;
        }

        //easy way to set a pattern
    }
}
