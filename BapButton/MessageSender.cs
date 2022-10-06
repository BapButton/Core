using MessagePipe;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BapShared;

namespace BapButton
{
    public class MessageSender
    {

        private IPublisher<StandardButtonCommandMessage> StandardButtonMessageSender { get; set; }
        private IPublisher<RestartButtonMessage> RestartButtonMessageSender { get; set; }
        private IPublisher<StatusButtonMessage> StatusButtonMessageSender { get; set; }
        private IPublisher<CustomImageMessage> CustomImageMessageSender { get; set; }
        private IPublisher<InternalCustomImageMessage> InternalCustomImageMessageSender { get; set; }
        private IPublisher<ButtonPressedMessage> ButtonPressedSender { get; set; }
        private IPublisher<TurnOffButtonMessage> TurnOffButtonMessageSender { get; set; }
        private IPublisher<PlayAudioMessage> PlayAudioMessageSender { get; set; } = default!;
        private IPublisher<GameEventMessage> GameEventMessageSender { get; set; } = default!;
        private IPublisher<NodeChangeMessage> NodeChangeSender { get; set; } = default!;
        private IPublisher<LayoutChangeMessage> LayoutChangeSender { get; set; } = default!;
        private ControlHandler ControlHandler { get; set; }
        public int ButtonCount
        {
            get
            {
                return ControlHandler.CurrentController?.GetConnectedButtons().Count ?? 0;
            }
        }

        public MessageSender(IPublisher<ButtonPressedMessage> buttonPressed, IPublisher<StandardButtonCommandMessage> standardButtonMessageSender, IPublisher<RestartButtonMessage> restartButtonMessageSender, IPublisher<StatusButtonMessage> statusButtonMessageSender, IPublisher<CustomImageMessage> customImageSender, IPublisher<TurnOffButtonMessage> turnOffButtonMessageSender, IPublisher<GameEventMessage> gameEventMessageSender, IPublisher<PlayAudioMessage> playAudioMessageSender, ControlHandler control, IPublisher<LayoutChangeMessage> layoutChangeSender, IPublisher<InternalCustomImageMessage> internalCustomImageMessageSender)
        {

            StandardButtonMessageSender = standardButtonMessageSender;
            RestartButtonMessageSender = restartButtonMessageSender;
            StatusButtonMessageSender = statusButtonMessageSender;
            CustomImageMessageSender = customImageSender;
            ButtonPressedSender = buttonPressed;
            ControlHandler = control;
            TurnOffButtonMessageSender = turnOffButtonMessageSender;
            GameEventMessageSender = gameEventMessageSender;
            PlayAudioMessageSender = playAudioMessageSender;
            LayoutChangeSender = layoutChangeSender;
            InternalCustomImageMessageSender = internalCustomImageMessageSender;
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

        public void PlayTTS(string messageToAnnounce, bool stopAllOtherAudio = false)
        {
            string fileName = $"/api/tts/{messageToAnnounce}";
            PlayAudioMessageSender.Publish(new PlayAudioMessage(fileName, stopAllOtherAudio));
        }

        public void PlayTTSSpanish(string messageToAnnounce, bool stopAllOtherAudio = false)
        {
            string fileName = $"/api/tts/spanish/{messageToAnnounce}";
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
            return ControlHandler.CurrentController?.GetConnectedButtons() ?? new List<string>();
        }
        public List<string> GetConnectedButtonsInOrder()
        {
            return ControlHandler.CurrentController?.GetConnectedButtonsInOrder() ?? new List<string>();
        }

        public List<(string nodeId, ButtonStatus buttonStatus)> GetAllConnectedButtonInfo()
        {
            return ControlHandler.CurrentController?.GetAllConnectedButtonInfo() ?? new List<(string nodeId, ButtonStatus buttonStatus)>();
        }

        public void MockClickButton(string nodeId, ButtonPress buttonPress)
        {
            ButtonPressedSender.Publish(new ButtonPressedMessage(nodeId, buttonPress));
        }
        public void SendCommand(string nodeId, StandardButtonCommand standardButtonMessage)
        {

            StandardButtonMessageSender.Publish(new StandardButtonCommandMessage(standardButtonMessage, nodeId));

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

        public void SendGeneralCommand(StandardButtonCommand standardButtonMessage)
        {
            StandardButtonMessageSender.Publish(new StandardButtonCommandMessage(standardButtonMessage));
        }

        public void SendCustomImage(CustomImage customImage)
        {
            CustomImageMessageSender.Publish(new CustomImageMessage(customImage));
        }

        public void SendCustomImage(string nodeId, CustomImage customImage)
        {
            CustomImageMessageSender.Publish(new CustomImageMessage(customImage, nodeId));
        }

        public void SendInternalCustomImage(string nodeId, InternalCustomImage customImage)
        {
            InternalCustomImageMessageSender.Publish(new InternalCustomImageMessage(customImage, nodeId));
        }
        public void SendInternalCustomImage(InternalCustomImageMessage internalCustomImageMessage)
        {
            InternalCustomImageMessageSender.Publish(internalCustomImageMessage);
        }

        public void SendCustomImageToAllButtons(CustomImage customImage)
        {
            CustomImageMessageSender.Publish(new CustomImageMessage(customImage));
        }

        public void ClearButtons()
        {
            SendGeneralCommand(new StandardButtonCommand());
        }
    }
}
