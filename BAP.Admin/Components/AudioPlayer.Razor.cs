using BAP.Types;
using MessagePipe;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;


namespace BAP.Admin.Components
{
    [TopMenu()]
    public partial class AudioPlayer : ComponentBase, IDisposable
    {
        [Inject]
        ILogger<AudioPlayer> _logger { get; set; } = default!;
        [Inject]
        IOptionsSnapshot<BapSettings> _bapSettings { get; set; } = default!;
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
                    string fileNameToPlay = e.FullPathToAudioFileWithFileName;
                    if (e.FullPathToAudioFileWithFileName.StartsWith(_bapSettings.Value.AddonSaveLocation))
                    {
                        fileNameToPlay.Substring(_bapSettings.Value.AddonSaveLocation.Length);
                        List<string> directoryList = new();
                        string remainingFileInfo = e.FullPathToAudioFileWithFileName;
                        while (string.IsNullOrEmpty(remainingFileInfo))
                        {
                            var directoryName = Path.GetDirectoryName(remainingFileInfo);
                            if (directoryName?.Length > 0)
                            {
                                directoryList.Add(remainingFileInfo.Substring(directoryName.Length));
                                remainingFileInfo = directoryName;
                            }
                        }
                        //Replace all Pipes with doubles so that they can be swapped out on rehydration.
                        directoryList.ForEach(t => t = t.Replace("|", "||"));
                        fileNameToPlay = string.Join("|", directoryList);
                    }
                    try
                    {
                        if (!string.IsNullOrEmpty(fileNameToPlay) && !VolumeMuted)
                        {
                            await js.InvokeVoidAsync("PlayAudioFile", fileNameToPlay);
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
