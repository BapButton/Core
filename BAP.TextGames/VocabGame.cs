using BAP.Types;
using BAP.Helpers;
using BAP.TextGames.Components;

namespace BAP.Web.Games
{
    public class SavedVocab
    {
        public bool IsSpanish { get; set; } = true;
        public List<string> SavedWords { get; set; } = new();
        public DateTime DateSaved { get; set; }
        //public AuthorizationCodeTokenResponse SpotifyToken { get; set; } = new AuthorizationCodeTokenResponse();
    }

    public class VocabGame : KeyboardGameBase, IBapGame, IDisposable
    {
        //private SpotifyClient? _spotifyClient { get; set; }
        public SavedVocab SavedVocab { get; set; }
        public Queue<int> WordOrder { get; set; } = new Queue<int>();
        public int CurrentWordNumber { get; set; } = -1;
        public override ILogger _logger { get; set; }
        public override IGameDataSaver DbSaver { get; set; }
        internal bool useTestingButtons { get; set; } = true;
        internal string CurrentSongName { get; set; } = "";
        internal int PlaylistCount { get; set; }
        internal int PlaylistTotalSeconds { get; set; }
        internal string PlaylistLengthDisplay
        {
            get
            {
                return TimeSpan.FromSeconds(PlaylistTotalSeconds).ToString(@"hh\:mm\:ss");
            }

        }

        //public bool IsSpotifyAuthorized()
        //{
        //    return _spotifyClient != null;
        //}

        public override Score GenerateScoreWithCurrentData()
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(SecondsToRun);

            decimal normalizedScore = 0;
            if (SecondsToRun > 0 && correctScore + wrongScore != 0)
            {
                normalizedScore = ((decimal)correctScore - ((decimal)wrongScore * 2.0M)) / ((decimal)SecondsToRun / 30.0M);
            }
            Score score = new()
            {
                DifficultyId = "",
                DifficultyDescription = "",
                ScoreData = CurrentWordNumber.ToString(),
                NormalizedScore = normalizedScore,
                ScoreDescription = $"",
            };
            return score;
        }

        internal void RefreshSavedVocab()
        {
            SavedVocab = AsyncHelpers.RunSync(() => DbSaver.GetGameStorage<SavedVocab>()) ?? new();
        }

        public VocabGame(IGameDataSaver<VocabGame> gameDataSaver,  IKeyboardProvider keyboardProvider, IGameProvider gameHandler, ILayoutProvider layoutProvider, ILogger<VocabGame> logger, ISubscriber<KeyboardKeyPressedMessage> keyPressed, IBapMessageSender messageSender, IGameDataSaver dbSaver) : base(keyboardProvider, gameHandler, layoutProvider, messageSender, keyPressed,gameDataSaver)
        {
            _logger = logger;
            DbSaver = dbSaver;
            RefreshSavedVocab();
            //if (SavedVocab?.SpotifyToken.ExpiresIn > 0)
            //{
            //    AsyncHelpers.RunSync(() => CreateSpotifyClient());
            //}
        }

        public override async Task<bool> WrongButtonPressed(bool setupNextMathProblem)
        {
            //await Skip();
            return await base.WrongButtonPressed(setupNextMathProblem);
        }

        public void ReplayCurrentWord()
        {
            if (SavedVocab.IsSpanish)
            {
                MsgSender.PlayTTS($"y {SavedVocab.SavedWords[CurrentWordNumber]}", true, TTSLanguage.Spanish);
            }
            else
            {
                MsgSender.PlayTTS(SavedVocab.SavedWords[CurrentWordNumber], true);
            }
        }

        //public void Pause()
        //{

        //    _spotifyClient?.Player.PausePlayback();

        //}

        public async Task<SavedVocab> GetSavedVocab()
        {
            return (await DbSaver.GetGameStorage<SavedVocab>()) ?? new SavedVocab();
        }

        //public async Task<bool> UpdateSpotifyCode(string spotifyCode)
        //{
        //    var response = await new OAuthClient().RequestToken(
        //                        new AuthorizationCodeTokenRequest("4a9b9dab320240659583880719c5816d", "e80134e65a46424d9475cf840fad12fd", spotifyCode, new Uri("https://localhost:5001/spotifyCallback/"))
        //                      );
        //    SavedVocab.SpotifyToken = response;
        //    await DbSaver.UpdateGameStorage(SavedVocab);
        //    await CreateSpotifyClient();
        //    MsgSender.SendUpdate("Spotify Activated", pageRefreshRecommended: true);
        //    return true;
        //}

        //public async Task<bool> CreateSpotifyClient()
        //{
        //    try
        //    {
        //        var config = SpotifyClientConfig
        //          .CreateDefault()
        //          .WithAuthenticator(new AuthorizationCodeAuthenticator("4a9b9dab320240659583880719c5816d", "e80134e65a46424d9475cf840fad12fd", SavedVocab.SpotifyToken));

        //        _spotifyClient = new SpotifyClient(config);
        //        await UpdateCurrentSongName();
        //        await UpdatePlaylistInfo();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(new EventId(1, "Spotify Error"), ex, "Could not create spotify client");
        //        return false;
        //    }
        //    return true;
        //}

        public async Task<bool> SaveNewVocabWords(string seperatedWordsAsString, bool isSpanish)
        {
            seperatedWordsAsString = seperatedWordsAsString.Replace("\r\n", "\n");
            char splitCharacter = ',';
            List<char> possibleSpittingCharacters = new() { ',', ';', '\n' };
            foreach (char character in possibleSpittingCharacters)
            {
                if (seperatedWordsAsString.Split(character).Count() > 3)
                {
                    splitCharacter = character;
                    break;
                }
            }
            SavedVocab updatedWords = new()
            {
                IsSpanish = isSpanish,
                DateSaved = DateTime.Now,
                SavedWords = seperatedWordsAsString.Split(splitCharacter).Select(t => t.Trim()).ToList(),
                //SpotifyToken = SavedVocab.SpotifyToken
            };
            for (int i = 0; i < updatedWords.SavedWords.Count; i++)
            {
                updatedWords.SavedWords[i] = new string(updatedWords.SavedWords[i].Where(t => !char.IsPunctuation(t) && !char.IsDigit(t) && !char.IsWhiteSpace(t)).ToArray());
            }
            await DbSaver.UpdateGameStorage(updatedWords);
            return true;
        }

        //public async Task<bool> Skip()
        //{
        //    if (_spotifyClient != null)
        //    {
        //        var currentPlayBack = await _spotifyClient.Player.GetCurrentPlayback();
        //        int currentPosition = currentPlayBack.ProgressMs;
        //        await _spotifyClient.Player.SeekTo(new PlayerSeekToRequest(currentPosition + (10 * 1000)));
        //    }
        //    return true;
        //}

        //public void Play()
        //{
        //    _spotifyClient?.Player.ResumePlayback();
        //}
        //public async Task<bool> UpdateCurrentSongName()
        //{
        //    string tempSongName = "";
        //    if (_spotifyClient != null)
        //    {
        //        var result = await _spotifyClient.Player.GetCurrentlyPlaying(new());
        //        IPlayableItem? item = result?.Item;
        //        if (item != null)
        //        {
        //            if (item is FullTrack track)
        //            {
        //                tempSongName = track.Name;
        //            }
        //            if (item is FullEpisode episode)
        //            {
        //                tempSongName = episode.Name;
        //            }
        //        }
        //    }
        //    if (CurrentSongName != tempSongName)
        //    {
        //        CurrentSongName = tempSongName;
        //        MsgSender.SendUpdate("Song changed", pageRefreshRecommended: true);
        //    }
        //    return true;
        //}

        //public async Task<bool> UpdatePlaylistInfo()
        //{
        //    int playlistLength = 0;
        //    int trackCount = 0;
        //    if (_spotifyClient != null)
        //    {
        //        var result = await _spotifyClient.Player.GetCurrentlyPlaying(new());
        //        if (result.Context.Type.Equals("playlist", StringComparison.OrdinalIgnoreCase))
        //        {
        //            var playList = await _spotifyClient.Playlists.Get(result.Context.Uri.Split(':')[2]);
        //            trackCount = playList.Tracks.Total ?? 0;
        //            int calculatedLengthInSeconds = 0;
        //            foreach (var item in playList.Tracks.Items)
        //            {

        //                if (item.Track is FullTrack track)
        //                {
        //                    calculatedLengthInSeconds += track.DurationMs / 1000;
        //                }
        //                if (item.Track is FullEpisode episode)
        //                {
        //                    calculatedLengthInSeconds += episode.DurationMs / 1000;
        //                }
        //            }
        //            playlistLength = calculatedLengthInSeconds;
        //        }
        //        else
        //        {
        //            IPlayableItem? item = result?.Item;
        //            if (item != null)
        //            {
        //                if (item is FullTrack track)
        //                {
        //                    playlistLength = track.DurationMs / 1000;
        //                }
        //                if (item is FullEpisode episode)
        //                {
        //                    playlistLength = episode.DurationMs / 1000;
        //                }
        //            }
        //            trackCount = 1;
        //        }

        //    }
        //    if (trackCount != PlaylistCount || playlistLength != PlaylistTotalSeconds)
        //    {
        //        PlaylistCount = trackCount;
        //        PlaylistTotalSeconds = playlistLength;
        //        MsgSender.SendUpdate("Playlist changed", pageRefreshRecommended: true);
        //    }
        //    return true;
        //}

        public override async Task<bool> SetupNextMathProblem(bool wasLastPressCorrect)
        {
            CurrentSpotInAnswerString = 0;
            if (!wasLastPressCorrect && CurrentWordNumber >= 0)
            {
                WordOrder.Enqueue(CurrentWordNumber);
            }
            bool moreWords = WordOrder.TryDequeue(out int nextWord);
            if (!moreWords)
            {
                return false;
            }
            CurrentWordNumber = nextWord;
            MsgSender.SendUpdate("Next vocab word selected");
            Answer = SavedVocab.SavedWords[CurrentWordNumber].ToCharArray();
            await Task.Delay(1000);
            if (SavedVocab.IsSpanish)
            {
                MsgSender.PlayTTS(SavedVocab.SavedWords[CurrentWordNumber], true, TTSLanguage.Spanish);
            }
            else
            {
                MsgSender.PlayTTS(SavedVocab.SavedWords[CurrentWordNumber], true);
            }

            return await Task.FromResult(true);
        }





        public override async Task<bool> Start()
        {
            if (IsGameRunning)
            {
                return false;
            }
            WordOrder.Clear();
            List<int> wordNumbers = Enumerable.Range(0, SavedVocab.SavedWords.Count).ToList();
            wordNumbers.Shuffle();
            foreach (var item in wordNumbers)
            {
                WordOrder.Enqueue(item);
            }


            await base.StartGame(1200, false, BasicKeyboardLetters.SpanishLowerCase);
            return true;
        }
    }
}
