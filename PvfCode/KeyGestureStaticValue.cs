using System.Windows.Input;

namespace PvfCode;

public class KeyGestureStaticValue
{
	private static KeyGestureStaticValue Ebql1Tttpo;

	public static KeyGestureStaticValue Instance
	{
		get
		{
			if (Ebql1Tttpo == null)
			{
				Ebql1Tttpo = new KeyGestureStaticValue();
			}
			return Ebql1Tttpo;
		}
	}

	public KeyGesture AltAndLeft => new KeyGesture((Key)23, (ModifierKeys)1, AppSetting.Instance.GetIlogger()?.GetStr("mess_AltLeftArrow"));

	public KeyGesture AltAndRight => new KeyGesture((Key)25, (ModifierKeys)1, AppSetting.Instance.GetIlogger()?.GetStr("mess_AltRightArrow"));

	public KeyGesture AltAndUp => new KeyGesture((Key)24, (ModifierKeys)1, "Alt+↑");

	public KeyGesture AltAndDown => new KeyGesture((Key)26, (ModifierKeys)1, "Alt+↓");

	public KeyGesture CtrlAndBackslash => new KeyGesture((Key)145, (ModifierKeys)2, "Ctrl+/");

	public KeyGesture CtrlAndDown => new KeyGesture((Key)26, (ModifierKeys)2, "Ctrl+↓");

	public KeyGesture CtrlAndUp => new KeyGesture((Key)24, (ModifierKeys)2, "Ctrl+↑");

	public KeyGesture CtrlAndEnter => new KeyGesture((Key)6, (ModifierKeys)2, "Ctrl+回车");

	public KeyGestureStaticValue()
	{
	}
}
