namespace BAP.Types
{
	public interface IKeyboardHandler
	{
		IBapKeyboardProvider CurrentKeyboard { get; }

		bool ReloadKeyboard();
		bool ResetKeyboard();
		bool SetSpecificKeyboard<T>() where T : IBapKeyboardProvider;
	}
}