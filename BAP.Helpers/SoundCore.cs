using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using BapShared;
using BAP.Types;

namespace BapButton
{
	public class SoundCore : IBapAudioProvider
	{
		//This really works poorly.
		//NetCoreAudio.Player player = new NetCoreAudio.Player();
		private readonly ILogger<SoundCore> _logger;

		public string Name => "NetCoreAudio Local Audio Player";

		public SoundCore(ILogger<SoundCore> logger = null)
		{
			_logger = logger ?? NullLogger<SoundCore>.Instance;
			//todo: this should probably play a blank sound to initialize the player.
		}


		public Task<bool> Initialize()
		{
			return Task.FromResult(true);
		}

		public async Task<(bool success, string message)> PlaySound(string pathToAudioFile)
		{
			string message = "";
			//if (player.Playing || player.Paused)
			//{
			//    try
			//    {
			//        await player.Stop();

			//    }
			//    catch (Exception ex)
			//    {
			//        message = $"On stopping the player errored with {ex.Message}";
			//    }
			//}
			//try
			//{
			//    await player.Play(pathToAudioFile);
			//}
			//catch (Exception ex)
			//{
			//    return (false, $"On playing the player errored with { ex.Message}");
			//}
			return (true, message);
		}

		public void Dispose()
		{

		}
	}
}
