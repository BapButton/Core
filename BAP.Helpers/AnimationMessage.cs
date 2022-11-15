using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAP.Helpers
{

    public class BapAnimation
    {
        public string NodeId { get; set; } = "";
        public List<Frame> Frames { get; set; } = new();
        /// <summary>
        /// If creating a loop it will loop back to this frame. 0 means to show all frames again.
        /// </summary>
        public int LoopToFrameId { get; set; }
        /// <summary>
        /// Number of times to Loop. LoopCount of 0 means leave the last frame up. Loopcount of 1 means loop once (display all images twice)
        /// </summary>
        public int LoopCount { get; set; }
        public BapAnimation(List<Frame> frames, string nodeId = "", int loopToFrameId = 0, int loopCount = 0)
        {
            Frames = frames;
            NodeId = nodeId;
            LoopToFrameId = loopToFrameId;
            LoopCount = loopCount;
        }
    }

    public class Frame
    {
        public Frame(ulong[,] image, int frameNumberToShow)
        {
            Image = image;
            FrameNumberToShow = frameNumberToShow;
        }
        public ulong[,] Image { get; set; } = new ulong[8, 8];
        /// <summary>
        /// The animator loop is run on a configurable Framerate. This is the frame number when this image should show up. 
        /// </summary>
        public int FrameNumberToShow { get; set; }
    }
}
