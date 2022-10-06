using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BapButton;
using BapShared;

namespace BapTester
{
    public class SoundTester
    {


        SoundCore sound = new SoundCore();
        bool good = true;
        public async Task<bool> Start()
        {
            //This method no longer works. 
            await Task.Delay(2000);
            Console.WriteLine("Starting Sound Tester");
            Console.WriteLine("Press CTRL-C to end game.");
            await sound.PlaySound(".\\Audio\\error_004.wav");
            return true;
        }
    }
}
