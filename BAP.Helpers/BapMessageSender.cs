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
        private IPublisher<PlayAudioMessage> PlayAudioMessageSender { get; set; } = default!;
        private IPublisher<GameEventMessage> GameEventMessageSender { get; set; } = default!;
        private IPublisher<NodeChangeMessage> NodeChangeSender { get; set; } = default!;
        private IPublisher<LayoutChangeMessage> LayoutChangeSender { get; set; } = default!;
        private IControlHandler ControlHandler { get; set; }
        public int ButtonCount
        {
            get
            {
                return ControlHandler.CurrentButtonProvider?.GetConnectedButtons().Count ?? 0;
            }
        }

        public BapMessageSender(IPublisher<ButtonPressedMessage> buttonPressed, IPublisher<StandardButtonImageMessage> standardButtonMessageSender, IPublisher<RestartButtonMessage> restartButtonMessageSender, IPublisher<StatusButtonMessage> statusButtonMessageSender, IPublisher<TurnOffButtonMessage> turnOffButtonMessageSender, IPublisher<GameEventMessage> gameEventMessageSender, IPublisher<PlayAudioMessage> playAudioMessageSender, IControlHandler control, IPublisher<LayoutChangeMessage> layoutChangeSender)
        {

            StandardButtonMessageSender = standardButtonMessageSender;
            RestartButtonMessageSender = restartButtonMessageSender;
            StatusButtonMessageSender = statusButtonMessageSender;
            ButtonPressedSender = buttonPressed;
            ControlHandler = control;
            TurnOffButtonMessageSender = turnOffButtonMessageSender;
            GameEventMessageSender = gameEventMessageSender;
            PlayAudioMessageSender = playAudioMessageSender;
            LayoutChangeSender = layoutChangeSender;
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
            return ControlHandler.CurrentButtonProvider?.GetConnectedButtons() ?? new List<string>();
        }
        public List<string> GetConnectedButtonsInOrder()
        {
            return ControlHandler.CurrentButtonProvider?.GetConnectedButtonsInOrder() ?? new List<string>();
        }

        public List<(string nodeId, ButtonStatus buttonStatus)> GetAllConnectedButtonInfo()
        {
            return ControlHandler.CurrentButtonProvider?.GetAllConnectedButtonInfo() ?? new List<(string nodeId, ButtonStatus buttonStatus)>();
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

        //public void SendCustomImage(CustomImage customImage)
        //{
        //    CustomImageMessageSender.Publish(new CustomImageMessage(customImage));
        //}

        //public void SendCustomImage(string nodeId, CustomImage customImage)
        //{
        //    CustomImageMessageSender.Publish(new CustomImageMessage(customImage, nodeId));
        //}

        //public void SendInternalCustomImage(string nodeId, InternalCustomImage customImage)
        //{
        //    InternalCustomImageMessageSender.Publish(new InternalCustomImageMessage(customImage, nodeId));
        //}
        //public void SendInternalCustomImage(InternalCustomImageMessage internalCustomImageMessage)
        //{
        //    InternalCustomImageMessageSender.Publish(internalCustomImageMessage);
        //}

        //public void SendCustomImageToAllButtons(CustomImage customImage)
        //{
        //    CustomImageMessageSender.Publish(new ButtonImage(customImage));
        //}

        public void ClearButtons()
        {
            SendImageToAllButtons(new ButtonImage());
        }
    }
}
