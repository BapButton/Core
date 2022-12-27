
using BAP.Helpers;
using BAP.Types;

namespace BAP.WebCore;
public class DefaultKeyboard : KeyboardBase
{
    public DefaultKeyboard(ILogger<DefaultKeyboard> logger, ISubscriber<ButtonPressedMessage> buttonPressed, IBapMessageSender msgSender, IPublisher<KeyboardKeyPressedMessage> keyboardKeyPressedSender) : base(logger, buttonPressed, msgSender, keyboardKeyPressedSender)
    {

    }

    public override string Name => "Default Keyboard";

    //public void SetupKeyboard(string chars, BapColor? color = null, bool showX = true, ButtonImage? defaultShowOnPress = null, List<string>? nodesToAvoid = null, bool alwaysUseAllButtons = false, bool playKeyPressSound = false)
    //{
    //	color = color ?? new(0, 255, 0);
    //	if (showX && defaultShowOnPress == null)
    //	{
    //		defaultShowOnPress = new ButtonImage(PatternHelper.GetBytesForPattern(Patterns.XOut), new BapColor(255, 0, 0));
    //	}

    //	base.SetupKeyboard(chars, color, defaultShowOnPress, nodesToAvoid, alwaysUseAllButtons, playKeyPressSound);

    //}
}

