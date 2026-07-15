namespace PvfCode.Services.PreviewPvfFileFolder;

public abstract class avatar_select_ability_Base
{
	public string Command { get; set; }

	public abstract string? Text { get; }

	protected avatar_select_ability_Base()
	{
	}
}
