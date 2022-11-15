using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BapShared;

namespace MockButtonCore
{
    public class CheckImage
    {
        private Dictionary<int, ulong> _currentImage { get; set; }
        public CheckImage(ulong[] imageData)
        {
            _currentImage = new Dictionary<int, ulong>();
            for (int i = 0; i < imageData.Length; i++)
            {
                _currentImage.Add(i, imageData[i]);
            }
        }
        public ulong CurrentColor(int lightId)
        {
            return _currentImage[lightId];

        }
    }
}
