using System.Collections.Generic;

namespace PvfCode.Services.PreviewPvfFileFolder;

public class avatar_select_ability : avatar_select_ability_Base
{
	private static readonly HashSet<string> PercentageCommands = new HashSet<string>
	{
		"[ACTIVESTATUS_TOLERANCE_STUCK]",
		"[ATTACK_SPEED]",
		"[CAST_SPEED]",
		"[MAGICAL ABSOLUTE DEFENSE]",
		"[MOVE_SPEED]",
		"[PHYSICAL ABSOLUTE DEFENSE]",
		"[STUCK ON ATTACK]"
	};

	private static readonly HashSet<string> ValueCommands = new HashSet<string>
	{
		"[ACTIVESTATUS_TOLERANCE_ALL]",
		"[ELEMENT_TOLERANCE_DARK]",
		"[ELEMENT_TOLERANCE_FIRE]",
		"[ELEMENT_TOLERANCE_LIGHT]",
		"[ELEMENT_TOLERANCE_WATER]",
		"[EQUIPMENT_MAGICAL_DEFENSE]",
		"[EQUIPMENT_PHYSICAL_DEFENSE]",
		"[HIT_RECOVERY]",
		"[HP MAX]",
		"[HP_REGENRATE]",
		"[INVENTORY_MAX_WEIGHT]",
		"[JUMP_POWER]",
		"[MAGICAL_ATTACK]",
		"[MAGICAL_DEFENSE]",
		"[MP MAX]",
		"[MP_REGENRATE]",
		"[PHYSICAL_ATTACK]",
		"[PHYSICAL_DEFENSE]"
	};

	public string AddType { get; set; }

	public int Value { get; set; }

	public override string? Text
	{
		get
		{
			PvfFileHelper.avatar_select_abilityCommandTypeDic.TryGetValue(Command, out string value);
			if (string.IsNullOrEmpty(value))
			{
				value = "未识别的效果";
			}
			if (PercentageCommands.Contains(Command))
			{
				return $"{value} {(double)Value * 0.1}% 增加";
			}
			if (ValueCommands.Contains(Command))
			{
				return $"{value} {Value} 增加";
			}
			return null;
		}
	}

	public avatar_select_ability(string command, string addType, int value)
	{
		Command = command;
		AddType = addType;
		Value = value;
	}
}
