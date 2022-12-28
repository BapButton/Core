using BAP.Types;
using MessagePipe;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;


namespace BAP.Admin.Components
{
    public partial class AudioPlayer : ComponentBase, IDisposable
    {
        [Inject]
        ILogger<AudioPlayer> _logger { get; set; } = default!;
        [Inject]
        IJSRuntime js { get; set; } = default!;
        [Inject]
        ISubscriber<PlayAudioMessage> GameEventPipe { get; set; } = default!;
        IDisposable Subscriptions { get; set; } = default!;
        public bool VolumeMuted { get; set; }
        protected override void OnInitialized()
        {
            var bag = DisposableBag.CreateBuilder();
            GameEventPipe.Subscribe(async (x) => await PlayNextAudio(x)).AddTo(bag);
            Subscriptions = bag.Build();
            base.OnInitialized();
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

                if (!string.IsNullOrEmpty(e.FullPathToAudioFileWithFileName))
                {
                    string fullFileName = e.FullPathToAudioFileWithFileName.StartsWith("/api") ? e.FullPathToAudioFileWithFileName : $"/audio/{e.FullPathToAudioFileWithFileName}";
                    try
                    {
                        if (!string.IsNullOrEmpty(e.FullPathToAudioFileWithFileName) && !VolumeMuted)
                        {
                            await js.InvokeVoidAsync("PlayAudioFile", fullFileName);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message, $"Problem stoppping and playing the sound -{ex.Message}");
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
