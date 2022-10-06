using MessagePack;
using System;
using System.Collections.Generic;
using System.Text;

namespace BapShared
{
    [MessagePackObject]
    public class CustomImage
    {
        [Key(0)]
        public int ImageId { get; set; }
        [Key(1)]
        public ulong[] ImageData { get; set; }

        public CustomImage()
        {
            ImageData = new ulong[64];
        }
        public override string ToString()
        {
            return $"Custom Image with Id {ImageId}";
        }
    }

    public class InternalCustomImage
    {
        public ulong[,] Image { get; set; }
        public InternalCustomImage()
        {
            Image = new ulong[8, 8];
        }
        public InternalCustomImage(ulong[,] image)
        {
            Image = image;
        }

    }
}
