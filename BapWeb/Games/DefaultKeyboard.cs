
using BapButton;

namespace BapWeb.Games;
public class DefaultKeyboard : KeyboardBase
{
    public DefaultKeyboard(ILogger<DefaultKeyboard> logger, ISubscriber<ButtonPressedMessage> buttonPressed, MessageSender msgSender, IPublisher<KeyboardKeyPressedMessage> keyboardKeyPressedSender) : base(logger, buttonPressed, msgSender, keyboardKeyPressedSender)
    {

    }

    public void SetupKeyboard(KeyboardType keyboardType, BapColor? color = null, bool showX = true, ButtonDisplay? defaultShowOnPress = null, List<string>? nodesToAvoid = null, bool alwaysUseAllButtons = false, bool playKeyPressSound = false)
    {
        List<char> chars = new(); ;
        color = color ?? new(0, 255, 0);
        switch (keyboardType)
        {
            case KeyboardType.Numbers:
                chars = "0123456789".ToCharArray().ToList();
                break;
            case KeyboardType.CapitalLetters:
                chars = "ABCDEFGHIJKLMNOPRSTUVWXYZ".ToCharArray().ToList();
                break;
            case KeyboardType.CapitalLettersAndNumbers:
                chars = "ABCDEFGHIJKLMNOPRSTUVWXYZ0123456789".ToCharArray().ToList();
                break;
            case KeyboardType.SpanishLowerCaseWithNumbers:
                chars = "aábcdeéfghiíjklmnñoópqrstuúüvwxyz123456789".ToCharArray().ToList();
                break;
            case KeyboardType.SpanishLowerCase:
                chars = "aábcdeéfghiíjklmnñoópqrstuúüvwxyz".ToCharArray().ToList();
                break;
            default:
                break;
        }
        if (showX && defaultShowOnPress == null)
        {
            defaultShowOnPress = new ButtonDisplay(new BapColor(255, 0, 0), Patterns.XOut, 0, 0);
        }

        base.SetupKeyboard(chars, color, defaultShowOnPress, nodesToAvoid, alwaysUseAllButtons, playKeyPressSound);

    }
}

public enum KeyboardType
{
    Numbers = 1,
    CapitalLetters = 2,
    CapitalLettersAndNumbers = 3,
    SpanishLowerCaseWithNumbers = 4,
    SpanishLowerCase = 5

}