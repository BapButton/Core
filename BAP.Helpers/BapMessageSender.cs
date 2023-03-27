using MessagePipe;
using BAP.Types;
using System.Collections.Generic;

namespace BAP.Helpers
{
    public class BapMessageSender : IBapMessageSender
    {

        private IPublisher<StandardButtonImageMessage> StandardButtonMessageSender { get; set; }
        private IPublisher<RestartButtonMessage> RestartButtonMessageSender { get; set; }
        private IPublisher<StatusButtonMessage> StatusButtonMessageSender { get; set; }
        //private IPublisher<CustomImageMessage> CustomImageMessageSender { get; set; }
        //private IPublisher<InternalCustomImageMessage> InternalCustomImageMessageSender { get; set; }
        private IPublisher<ButtonPressedMessage> ButtonPressedSender { get; set; }
        private IPublisher<TurnOffButtonMessage> TurnOffButtonMessageSender { get; set; }
        private IPublisher<PlayAudioMessage> PlayAudioMessageSender { get; set; }
        private IPublisher<GameEventMessage> GameEventMessageSender { get; set; }
        private IPublisher<NodeChangeMessage> NodeChangeSender { get; set; }
        private IPublisher<EnableTestingModeMessage> EnableTestingModeSender { get; set; }
        private IPublisher<LayoutChangeMessage> LayoutChangeSender { get; set; }
        private IButtonProvider ButtonProvider { get; set; }
        public int ButtonCount
        {
            get
            {
                return ButtonProvider.GetConnectedButtons().Count;
            }
        }

        public BapMessageSender(IPublisher<ButtonPressedMessage> buttonPressed, IPublisher<StandardButtonImageMessage> standardButtonMessageSender, IPublisher<RestartButtonMessage> restartButtonMessageSender, IPublisher<StatusButtonMessage> statusButtonMessageSender, IPublisher<TurnOffButtonMessage> turnOffButtonMessageSender, IPublisher<GameEventMessage> gameEventMessageSender, IPublisher<PlayAudioMessage> playAudioMessageSender, IPublisher<NodeChangeMessage> nodeChangeSender, IPublisher<EnableTestingModeMessage> testingModeSender, IButtonProvider buttonProvider, IPublisher<LayoutChangeMessage> layoutChangeSender)
        {

            StandardButtonMessageSender = standardButtonMessageSender;
            RestartButtonMessageSender = restartButtonMessageSender;
            StatusButtonMessageSender = statusButtonMessageSender;
            ButtonPressedSender = buttonPressed;
            NodeChangeSender = nodeChangeSender;
            TurnOffButtonMessageSender = turnOffButtonMessageSender;
            GameEventMessageSender = gameEventMessageSender;
            PlayAudioMessageSender = playAudioMessageSender;
            ButtonProvider = buttonProvider;
            LayoutChangeSender = layoutChangeSender;
            EnableTestingModeSender = testingModeSender;
        }

        public void PlayAudio(string fileName, bool stopAllOtherAudio = false)
        {
            PlayAudioMessageSender.Publish(new PlayAudioMessage(fileName, stopAllOtherAudio));
        }
        public void StopAllAudio()
        {
            PlayAudioMessageSender.Publish(new PlayAudioMessage("", true, false));
        }
        public void ClearAllCachedAudio()
        {
            PlayAudioMessageSender.Publish(new PlayAudioMessage("", false, true));
        }

        public void PlayTTS(string messageToAnnounce, bool stopAllOtherAudio = false, TTSLanguage tTSLaunguage = TTSLanguage.English)
        {
            string fileName = $"/api/tts/{messageToAnnounce}";
            if (tTSLaunguage == TTSLanguage.Spanish)
            {
                fileName = $"/api/tts/spanish/{messageToAnnounce}";
            }

            PlayAudioMessageSender.Publish(new PlayAudioMessage(fileName, stopAllOtherAudio));
        }

        public void SendNodeChange(NodeChangeMessage msg)
        {
            NodeChangeSender.Publish(msg);
        }

        public void SendNodeChange(string nodeId, bool isRemoved)
        {
            SendNodeChange(new NodeChangeMessage(nodeId, isRemoved));
        }

        public void SendUpdate(string message, bool gameEnded = false, bool highScoreAchieved = false, bool fatalError = false, bool pageRefreshRecommended = false)
        {
            GameEventMessageSender.Publish(new GameEventMessage(gameEnded, message, highScoreAchieved, fatalError, pageRefreshRecommended));
        }

        //public void SendUpdate(GameEventMessage msg)
        //{
        //    GameEventMessageSender.Publish(msg);
        //}
        //ButttonLayoutId of 0 means deselected
        public void SendLayoutUpdate(int buttonLayoutId)
        {
            LayoutChangeSender.Publish(new LayoutChangeMessage(buttonLayoutId));
        }

        public List<string> GetConnectedButtons()
        {
            return ButtonProvider.GetConnectedButtons() ?? new List<string>();
        }
        public List<string> GetConnectedButtonsInOrder()
        {
            return ButtonProvider.GetConnectedButtonsInOrder() ?? new List<string>();
        }

        public List<(string nodeId, ButtonStatus buttonStatus)> GetAllConnectedButtonInfo()
        {
            return ButtonProvider.GetAllConnectedButtonInfo() ?? new List<(string nodeId, ButtonStatus buttonStatus)>();
        }

        public void MockClickButton(string nodeId, ButtonPress buttonPress)
        {
            ButtonPressedSender.Publish(new ButtonPressedMessage(nodeId, buttonPress));
        }
        public void SendImage(string nodeId, ButtonImage buttonImage)
        {
            StandardButtonMessageSender.Publish(new StandardButtonImageMessage(buttonImage, nodeId));
        }

        public void GetStatusFromAllButttons()
        {
            StatusButtonMessageSender.Publish(new StatusButtonMessage());
        }
        public void GetStatusFromAButtton(string nodeId)
        {
            StatusButtonMessageSender.Publish(new StatusButtonMessage(nodeId));
        }

        public void RestartAllButtons()
        {
            RestartButtonMessageSender.Publish(new RestartButtonMessage());
        }

        public void TurnOffAllButtons()
        {
            TurnOffButtonMessageSender.Publish(new TurnOffButtonMessage());
        }
        public void TurnOffAButton(string nodeId)
        {
            TurnOffButtonMessageSender.Publish(new TurnOffButtonMessage(nodeId));
        }

        public void RestartAButton(string nodeId)
        {
            RestartButtonMessageSender.Publish(new RestartButtonMessage(nodeId));
        }

        public void SendImageToAllButtons(ButtonImage buttonImage)
        {
            StandardButtonMessageSender.Publish(new StandardButtonImageMessage(buttonImage));
        }

        public void ClearButtons()
        {
            SendImageToAllButtons(new ButtonImage());
        }

        public void EnableLocalTestingModeForButton(string nodeId)
        {
            EnableTestingModeSender.Publish(new EnableTestingModeMessage(nodeId));
        }
    }
}
