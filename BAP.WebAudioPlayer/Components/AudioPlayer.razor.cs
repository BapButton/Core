using BAP.Types;
using MessagePipe;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;


namespace BAP.WebAudioPlayer.Components
{
	[TopMenu()]
	public partial class AudioPlayer : ComponentBase, IDisposable
	{
		[Inject]
		ILogger<AudioPlayer> _logger { get; set; } = default!;
		[Inject]
		IMemoryCache memoryCache { get; set; } = default!;
		[Inject]
		IJSRuntime js { get; set; } = default!;
		[Inject]
		ISubscriber<PlayAudioMessage> AudioMessagePipe { get; set; } = default!;
		IDisposable Subscriptions { get; set; } = default!;
		public bool VolumeMuted { get; set; }
		protected override void OnInitialized()
		{
			var bag = DisposableBag.CreateBuilder();
			AudioMessagePipe.Subscribe(async (x) => await PlayNextAudio(x)).AddTo(bag);
			Subscriptions = bag.Build();
			base.OnInitialized();
		}
		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			var module = await js.InvokeAsync<IJSObjectReference>(
				"import", "./_content/BAP.WebAudioPlayer/Components/AudioPlayer.razor.js");
		}
		async Task PlayNextAudio(PlayAudioMessage e)
		{
			try
			{
				if (e.StopAllPlayingAudio)
				{
					await js.InvokeVoidAsync("StopAllAudio");
				}
				else if (e.ClearAllCachedAudio)
				{
					await js.InvokeVoidAsync("ClearAudio");
				}
				string fileNameToPlay = e.FullPathToAudioFileWithFileName;
				if (!string.IsNullOrEmpty(e.FullPathToAudioFileWithFileName))
				{

					if (!e.FullPathToAudioFileWithFileName.StartsWith("/api"))
					{
						string justTheFilename = memoryCache.Get<string>(e.FullPathToAudioFileWithFileName) ?? "";
						if (string.IsNullOrEmpty(justTheFilename))
						{
							string extension = Path.GetExtension(e.FullPathToAudioFileWithFileName);
							string guid = Guid.NewGuid().ToString();
							justTheFilename = $"{guid}{extension}";
							memoryCache.Set(e.FullPathToAudioFileWithFileName, justTheFilename, TimeSpan.FromMinutes(60));
						}
						memoryCache.Set(justTheFilename, e.FullPathToAudioFileWithFileName, TimeSpan.FromMinutes(60));
						fileNameToPlay = $"/api/audio/{justTheFilename}";
					}

				}
				if (!string.IsNullOrEmpty(fileNameToPlay))
				{
					try
					{
						if (!string.IsNullOrEmpty(fileNameToPlay) && !VolumeMuted)
						{
							await js.InvokeVoidAsync("PlayAudioFile", fileNameToPlay);
						}
					}
					catch (Exception ex)
					{
						_logger.LogError(ex.Message, $"Problem playing the sound -{ex.Message}");
					}
				}


			}
			catch (Exception ex)
			{

				_logger.LogError(ex.Message, $"Problem playing sound with error -{ex.Message}");
			}




		}

		async void ToggleAudio(MouseEventArgs e)
		{
			VolumeMuted = !VolumeMuted;
			await InvokeAsync(() =>
			{
				StateHasChanged();
			});
		}

		public void Dispose()
		{
			if (Subscriptions != null)
			{
				Subscriptions.Dispose();
			}
		}
	}
}
