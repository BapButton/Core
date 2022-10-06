//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;
//using System.Timers;
//using BapButton;
//using BapShared;

//namespace BapTester
//{
//    public class WakeUpTest
//    {
//        private static Timer wakeUpTimer;
//        TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
//        Random rand = new Random();
//        public Wake
//        public async Task<bool> Start()
//        {
//            await core.Initialize();
//            Console.WriteLine("Starting Reaction Game");
//            Console.WriteLine("Press CTRL-C to end game.");
//            wakeUpTimer = new System.Timers.Timer(5 * 1000);
//            wakeUpTimer.Elapsed += EndGameEvent;
//            wakeUpTimer.AutoReset = false;
//            wakeUpTimer.Enabled = true;

//            core.ButtonPressed += OnButtonPressed;
//            int buttonCount = core.GetConnectedButtons().Count;
//            Console.WriteLine($"You have {buttonCount} buttons connected.");
//            await core.SendGeneralCommand(new StandardButtonCommand(new ButtonDisplay(0, 255, 0, Patterns.AllOneColor, 0, 3000)));
//            await tcs.Task;

//            return true;
//        }


//        private async void EndGameEvent(Object source, ElapsedEventArgs e)
//        {
//            await EndGame("Time Expired - Game Ended");
//        }
//        private async Task<bool> EndGame(string message)
//        {
//            wakeUpTimer.Dispose();
//            core.ButtonPressed -= OnButtonPressed;
//            await Task.Delay(3000);
//            MsgSender.SendGeneralCommand(new StandardButtonCommand(new ButtonDisplay()));
//            await Task.Delay(1000);
//            tcs.SetResult(true);
//            return true;
//        }


//        async void OnButtonPressed(object? sender, ButtonPress e)
//        {
//            Console.WriteLine($"Button {e.NodeId} was pressed");
//            await Task.Delay(500);
//            ButtonDisplay standardButtonMessage = new ButtonDisplay((ushort)rand.Next(0, 255), (ushort)rand.Next(0, 255), (ushort)rand.Next(0, 255));
//            await core.SendCommand(e.NodeId, new StandardButtonCommand(standardButtonMessage));


//        }
//    }
//}
