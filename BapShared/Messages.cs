using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BapShared
{

    public class GameEventMessage
    {

        public bool GameEnded { get; set; }
        public string Message { get; set; }
        public bool HighScoreAchieved { get; set; }
        public bool PageRefreshRecommended { get; set; }
        public bool FatalError { get; set; }
        public GameEventMessage(bool gameEnded, string message = "", bool highScoreAchieved = false, bool fatalError = false, bool pageRefreshRecommended = false)
        {
            GameEnded = gameEnded;
            Message = message;
            HighScoreAchieved = highScoreAchieved;
            FatalError = fatalError;
            PageRefreshRecommended = pageRefreshRecommended;
        }
    }

    public class PlayAudioMessage
    {
        public string FileName { get; set; }
        public bool StopAllPlayingAudio { get; set; }
        public bool ClearAllCachedAudio { get; set; }
        public PlayAudioMessage(string fileName, bool stopAllPlayingAudio = false, bool clearAllCachedAudio = false)
        {
            StopAllPlayingAudio = stopAllPlayingAudio;
            ClearAllCachedAudio = clearAllCachedAudio;
            FileName = fileName;
        }
    }
    public class ButtonPressedMessage
    {
        public string NodeId { get; set; }
        public ButtonPress ButtonPress { get; set; }
        public ButtonPressedMessage(string nodeId, ButtonPress buttonPress)
        {
            NodeId = nodeId;
            ButtonPress = buttonPress;
        }
    }

    public class KeyboardKeyPressedMessage
    {
        public char KeyValue { get; set; }
        public ButtonPress ButtonPress { get; set; }
        public KeyboardKeyPressedMessage(char keyValue, ButtonPress buttonPress)
        {
            KeyValue = keyValue;
            ButtonPress = buttonPress;
        }
    }

    public class InternalSimpleGameUpdates
    {
        public int CorrectScore { get; set; }
        public int WrongScore { get; set; }
        public bool GameEnded { get; set; }
        public InternalSimpleGameUpdates(int correctScore, int wrongScore, bool gameEnded)
        {
            CorrectScore = correctScore;
            WrongScore = wrongScore;
            GameEnded = gameEnded;
        }
    }

    public class LayoutChangeMessage
    {
        public int NewButtonLayoutId { get; set; }
        public LayoutChangeMessage(int newButtonLayoutId)
        {
            NewButtonLayoutId = newButtonLayoutId;
        }
    }

    public class NodeChangeMessage
    {
        public string NodeId { get; set; }
        public bool IsRemoved { get; set; }
        public NodeChangeMessage(string nodeId, bool isRemoved)
        {
            NodeId = nodeId;
            IsRemoved = isRemoved;
        }
    }
    public class MessageFailedMessage
    {
        public string NodeId { get; set; }
        public MessageFailedMessage(string nodeId)
        {
            NodeId = nodeId;
        }
    }
    public class GameStateChangedMessage
    {
        public string Message { get; set; }
        public GameStateChangedMessage(string message)
        {
            Message = message;
        }
    }
    public class CustomImageMessage
    {
        public string NodeId { get; set; }
        public CustomImage CustomImage { get; set; }
        public bool ShowNow { get; set; }
        public CustomImageMessage(CustomImage customImage, string nodeId = "")
        {
            NodeId = nodeId;
            CustomImage = customImage;
        }
    }
    public class AnimationCompleteMessage
    {
        public List<string> NodeIds { get; set; }
        public AnimationCompleteMessage(List<string> nodeIds)
        {
            NodeIds = nodeIds;
        }
    }

    public class InternalCustomImageMessage
    {
        public string NodeId { get; set; }
        public InternalCustomImage CustomImage { get; set; }
        public InternalCustomImageMessage(InternalCustomImage customImage, string nodeId = "")
        {
            NodeId = nodeId;
            CustomImage = customImage;
        }
        public InternalCustomImageMessage(ulong[,] image, string nodeId = "")
        {
            NodeId = nodeId;
            CustomImage = new(image);
        }
    }

    public class RestartButtonMessage
    {
        public string NodeId { get; set; }
        public RestartButtonMessage(string nodeId = "")
        {
            NodeId = nodeId;
        }
    }

    public class TurnOffButtonMessage
    {
        public string NodeId { get; set; }
        public TurnOffButtonMessage(string nodeId = "")
        {
            NodeId = nodeId;
        }
    }
    public class StatusButtonMessage
    {
        public string NodeId { get; set; }
        public StatusButtonMessage(string nodeId = "")
        {
            NodeId = nodeId;
        }
    }

    public class StandardButtonCommandMessage
    {
        public string NodeId { get; set; }
        public StandardButtonCommand StandardButtonMessage { get; set; }
        public StandardButtonCommandMessage(StandardButtonCommand standardButtonMessage, string nodeId = "")
        {
            StandardButtonMessage = standardButtonMessage;
            NodeId = nodeId;
        }

    }

}
