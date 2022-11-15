using System.Collections.Generic;

namespace BAP.Types
{
    public interface IBapMessageSender
    {
        int ButtonCount { get; }

        void ClearAllCachedAudio();
        void ClearButtons();
        List<(string nodeId, ButtonStatus buttonStatus)> GetAllConnectedButtonInfo();
        List<string> GetConnectedButtons();
        List<string> GetConnectedButtonsInOrder();
        void GetStatusFromAButtton(string nodeId);
        void GetStatusFromAllButttons();
        void MockClickButton(string nodeId, ButtonPress buttonPress);
        void PlayAudio(string fileName, bool stopAllOtherAudio = false);
        void PlayTTS(string messageToAnnounce, bool stopAllOtherAudio = false, TTSLanguage tTSLanguage = TTSLanguage.English);
        void RestartAButton(string nodeId);
        void RestartAllButtons();
        void SendImage(string nodeId, ButtonImage buttonImage);
        void SendImageToAllButtons(ButtonImage buttonImage);
        void SendLayoutUpdate(int buttonLayoutId);
        void SendNodeChange(NodeChangeMessage msg);
        void SendNodeChange(string nodeId, bool isRemoved);
        void SendUpdate(string message, bool gameEnded = false, bool highScoreAchieved = false, bool fatalError = false, bool pageRefreshRecommended = false);
        void StopAllAudio();
        void TurnOffAButton(string nodeId);
        void TurnOffAllButtons();
    }

    public enum TTSLanguage
    {
        English = 1,
        Spanish = 2
    }
}