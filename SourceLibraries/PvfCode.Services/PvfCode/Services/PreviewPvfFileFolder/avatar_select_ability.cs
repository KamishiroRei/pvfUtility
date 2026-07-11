using System.Runtime.CompilerServices;

namespace PvfCode.Services.PreviewPvfFileFolder;

public class avatar_select_ability : avatar_select_ability_Base
{
	[CompilerGenerated]
	private string nhu843MT4J;

	[CompilerGenerated]
	private int VHX8QGghjP;

	public string AddType
	{
		[CompilerGenerated]
		get
		{
			return nhu843MT4J;
		}
		[CompilerGenerated]
		set
		{
			nhu843MT4J = value;
		}
	}

	public int Value
	{
		[CompilerGenerated]
		get
		{
			return VHX8QGghjP;
		}
		[CompilerGenerated]
		set
		{
			VHX8QGghjP = value;
		}
	}

	public override string? Text
	{
		get
		{
			PvfFileHelper.avatar_select_abilityCommandTypeDic.TryGetValue(base.Command, out string value);
			if (string.IsNullOrEmpty(value))
			{
				value = "未识别的效果";
			}
			string command = base.Command;
			if (command != null)
			{
				switch (command.Length)
				{
				case 28:
					switch (command[1])
					{
					case 'A':
					{
						if (!(command == "[ACTIVESTATUS_TOLERANCE_ALL]"))
						{
							break;
						}
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler18 = new DefaultInterpolatedStringHandler(4, 2);
						defaultInterpolatedStringHandler18.AppendFormatted(value);
						defaultInterpolatedStringHandler18.AppendLiteral(" ");
						defaultInterpolatedStringHandler18.AppendFormatted(Value);
						defaultInterpolatedStringHandler18.AppendLiteral(" 增加");
						return defaultInterpolatedStringHandler18.ToStringAndClear();
					}
					case 'E':
					{
						if (!(command == "[EQUIPMENT_PHYSICAL_DEFENSE]"))
						{
							break;
						}
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler17 = new DefaultInterpolatedStringHandler(4, 2);
						defaultInterpolatedStringHandler17.AppendFormatted(value);
						defaultInterpolatedStringHandler17.AppendLiteral(" ");
						defaultInterpolatedStringHandler17.AppendFormatted(Value);
						defaultInterpolatedStringHandler17.AppendLiteral(" 增加");
						return defaultInterpolatedStringHandler17.ToStringAndClear();
					}
					}
					break;
				case 14:
					switch (command[1])
					{
					case 'A':
					{
						if (!(command == "[ATTACK_SPEED]"))
						{
							break;
						}
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler13 = new DefaultInterpolatedStringHandler(5, 2);
						defaultInterpolatedStringHandler13.AppendFormatted(value);
						defaultInterpolatedStringHandler13.AppendLiteral(" ");
						defaultInterpolatedStringHandler13.AppendFormatted((double)Value * 0.1);
						defaultInterpolatedStringHandler13.AppendLiteral("% 增加");
						return defaultInterpolatedStringHandler13.ToStringAndClear();
					}
					case 'H':
					{
						if (!(command == "[HIT_RECOVERY]"))
						{
							if (!(command == "[HP_REGENRATE]"))
							{
								break;
							}
							DefaultInterpolatedStringHandler defaultInterpolatedStringHandler14 = new DefaultInterpolatedStringHandler(4, 2);
							defaultInterpolatedStringHandler14.AppendFormatted(value);
							defaultInterpolatedStringHandler14.AppendLiteral(" ");
							defaultInterpolatedStringHandler14.AppendFormatted(Value);
							defaultInterpolatedStringHandler14.AppendLiteral(" 增加");
							return defaultInterpolatedStringHandler14.ToStringAndClear();
						}
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler15 = new DefaultInterpolatedStringHandler(4, 2);
						defaultInterpolatedStringHandler15.AppendFormatted(value);
						defaultInterpolatedStringHandler15.AppendLiteral(" ");
						defaultInterpolatedStringHandler15.AppendFormatted(Value);
						defaultInterpolatedStringHandler15.AppendLiteral(" 增加");
						return defaultInterpolatedStringHandler15.ToStringAndClear();
					}
					case 'M':
					{
						if (!(command == "[MP_REGENRATE]"))
						{
							break;
						}
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler12 = new DefaultInterpolatedStringHandler(4, 2);
						defaultInterpolatedStringHandler12.AppendFormatted(value);
						defaultInterpolatedStringHandler12.AppendLiteral(" ");
						defaultInterpolatedStringHandler12.AppendFormatted(Value);
						defaultInterpolatedStringHandler12.AppendLiteral(" 增加");
						return defaultInterpolatedStringHandler12.ToStringAndClear();
					}
					}
					break;
				case 12:
					switch (command[1])
					{
					case 'C':
					{
						if (!(command == "[CAST_SPEED]"))
						{
							break;
						}
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler9 = new DefaultInterpolatedStringHandler(5, 2);
						defaultInterpolatedStringHandler9.AppendFormatted(value);
						defaultInterpolatedStringHandler9.AppendLiteral(" ");
						defaultInterpolatedStringHandler9.AppendFormatted((double)Value * 0.1);
						defaultInterpolatedStringHandler9.AppendLiteral("% 增加");
						return defaultInterpolatedStringHandler9.ToStringAndClear();
					}
					case 'J':
					{
						if (!(command == "[JUMP_POWER]"))
						{
							break;
						}
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler10 = new DefaultInterpolatedStringHandler(4, 2);
						defaultInterpolatedStringHandler10.AppendFormatted(value);
						defaultInterpolatedStringHandler10.AppendLiteral(" ");
						defaultInterpolatedStringHandler10.AppendFormatted(Value);
						defaultInterpolatedStringHandler10.AppendLiteral(" 增加");
						return defaultInterpolatedStringHandler10.ToStringAndClear();
					}
					case 'M':
					{
						if (!(command == "[MOVE_SPEED]"))
						{
							break;
						}
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler8 = new DefaultInterpolatedStringHandler(5, 2);
						defaultInterpolatedStringHandler8.AppendFormatted(value);
						defaultInterpolatedStringHandler8.AppendLiteral(" ");
						defaultInterpolatedStringHandler8.AppendFormatted((double)Value * 0.1);
						defaultInterpolatedStringHandler8.AppendLiteral("% 增加");
						return defaultInterpolatedStringHandler8.ToStringAndClear();
					}
					}
					break;
				case 24:
					switch (command[19])
					{
					case 'D':
					{
						if (!(command == "[ELEMENT_TOLERANCE_DARK]"))
						{
							break;
						}
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler6 = new DefaultInterpolatedStringHandler(4, 2);
						defaultInterpolatedStringHandler6.AppendFormatted(value);
						defaultInterpolatedStringHandler6.AppendLiteral(" ");
						defaultInterpolatedStringHandler6.AppendFormatted(Value);
						defaultInterpolatedStringHandler6.AppendLiteral(" 增加");
						return defaultInterpolatedStringHandler6.ToStringAndClear();
					}
					case 'F':
					{
						if (!(command == "[ELEMENT_TOLERANCE_FIRE]"))
						{
							break;
						}
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler5 = new DefaultInterpolatedStringHandler(4, 2);
						defaultInterpolatedStringHandler5.AppendFormatted(value);
						defaultInterpolatedStringHandler5.AppendLiteral(" ");
						defaultInterpolatedStringHandler5.AppendFormatted(Value);
						defaultInterpolatedStringHandler5.AppendLiteral(" 增加");
						return defaultInterpolatedStringHandler5.ToStringAndClear();
					}
					}
					break;
				case 25:
					switch (command[19])
					{
					case 'L':
					{
						if (!(command == "[ELEMENT_TOLERANCE_LIGHT]"))
						{
							break;
						}
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler3 = new DefaultInterpolatedStringHandler(4, 2);
						defaultInterpolatedStringHandler3.AppendFormatted(value);
						defaultInterpolatedStringHandler3.AppendLiteral(" ");
						defaultInterpolatedStringHandler3.AppendFormatted(Value);
						defaultInterpolatedStringHandler3.AppendLiteral(" 增加");
						return defaultInterpolatedStringHandler3.ToStringAndClear();
					}
					case 'W':
					{
						if (!(command == "[ELEMENT_TOLERANCE_WATER]"))
						{
							break;
						}
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(4, 2);
						defaultInterpolatedStringHandler2.AppendFormatted(value);
						defaultInterpolatedStringHandler2.AppendLiteral(" ");
						defaultInterpolatedStringHandler2.AppendFormatted(Value);
						defaultInterpolatedStringHandler2.AppendLiteral(" 增加");
						return defaultInterpolatedStringHandler2.ToStringAndClear();
					}
					}
					break;
				case 27:
					switch (command[1])
					{
					case 'E':
					{
						if (!(command == "[EQUIPMENT_MAGICAL_DEFENSE]"))
						{
							break;
						}
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler25 = new DefaultInterpolatedStringHandler(4, 2);
						defaultInterpolatedStringHandler25.AppendFormatted(value);
						defaultInterpolatedStringHandler25.AppendLiteral(" ");
						defaultInterpolatedStringHandler25.AppendFormatted(Value);
						defaultInterpolatedStringHandler25.AppendLiteral(" 增加");
						return defaultInterpolatedStringHandler25.ToStringAndClear();
					}
					case 'P':
					{
						if (!(command == "[PHYSICAL ABSOLUTE DEFENSE]"))
						{
							break;
						}
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler24 = new DefaultInterpolatedStringHandler(5, 2);
						defaultInterpolatedStringHandler24.AppendFormatted(value);
						defaultInterpolatedStringHandler24.AppendLiteral(" ");
						defaultInterpolatedStringHandler24.AppendFormatted((double)Value * 0.1);
						defaultInterpolatedStringHandler24.AppendLiteral("% 增加");
						return defaultInterpolatedStringHandler24.ToStringAndClear();
					}
					}
					break;
				case 8:
					switch (command[1])
					{
					case 'H':
					{
						if (!(command == "[HP MAX]"))
						{
							break;
						}
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler23 = new DefaultInterpolatedStringHandler(4, 2);
						defaultInterpolatedStringHandler23.AppendFormatted(value);
						defaultInterpolatedStringHandler23.AppendLiteral(" ");
						defaultInterpolatedStringHandler23.AppendFormatted(Value);
						defaultInterpolatedStringHandler23.AppendLiteral(" 增加");
						return defaultInterpolatedStringHandler23.ToStringAndClear();
					}
					case 'M':
					{
						if (!(command == "[MP MAX]"))
						{
							break;
						}
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler22 = new DefaultInterpolatedStringHandler(4, 2);
						defaultInterpolatedStringHandler22.AppendFormatted(value);
						defaultInterpolatedStringHandler22.AppendLiteral(" ");
						defaultInterpolatedStringHandler22.AppendFormatted(Value);
						defaultInterpolatedStringHandler22.AppendLiteral(" 增加");
						return defaultInterpolatedStringHandler22.ToStringAndClear();
					}
					}
					break;
				case 17:
					switch (command[1])
					{
					case 'M':
					{
						if (!(command == "[MAGICAL_DEFENSE]"))
						{
							break;
						}
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler20 = new DefaultInterpolatedStringHandler(4, 2);
						defaultInterpolatedStringHandler20.AppendFormatted(value);
						defaultInterpolatedStringHandler20.AppendLiteral(" ");
						defaultInterpolatedStringHandler20.AppendFormatted(Value);
						defaultInterpolatedStringHandler20.AppendLiteral(" 增加");
						return defaultInterpolatedStringHandler20.ToStringAndClear();
					}
					case 'P':
					{
						if (!(command == "[PHYSICAL_ATTACK]"))
						{
							break;
						}
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler21 = new DefaultInterpolatedStringHandler(4, 2);
						defaultInterpolatedStringHandler21.AppendFormatted(value);
						defaultInterpolatedStringHandler21.AppendLiteral(" ");
						defaultInterpolatedStringHandler21.AppendFormatted(Value);
						defaultInterpolatedStringHandler21.AppendLiteral(" 增加");
						return defaultInterpolatedStringHandler21.ToStringAndClear();
					}
					case 'S':
					{
						if (!(command == "[STUCK ON ATTACK]"))
						{
							break;
						}
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler19 = new DefaultInterpolatedStringHandler(5, 2);
						defaultInterpolatedStringHandler19.AppendFormatted(value);
						defaultInterpolatedStringHandler19.AppendLiteral(" ");
						defaultInterpolatedStringHandler19.AppendFormatted((double)Value * 0.1);
						defaultInterpolatedStringHandler19.AppendLiteral("% 增加");
						return defaultInterpolatedStringHandler19.ToStringAndClear();
					}
					}
					break;
				case 30:
				{
					if (!(command == "[ACTIVESTATUS_TOLERANCE_STUCK]"))
					{
						break;
					}
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler16 = new DefaultInterpolatedStringHandler(5, 2);
					defaultInterpolatedStringHandler16.AppendFormatted(value);
					defaultInterpolatedStringHandler16.AppendLiteral(" ");
					defaultInterpolatedStringHandler16.AppendFormatted((double)Value * 0.1);
					defaultInterpolatedStringHandler16.AppendLiteral("% 增加");
					return defaultInterpolatedStringHandler16.ToStringAndClear();
				}
				case 22:
				{
					if (!(command == "[INVENTORY_MAX_WEIGHT]"))
					{
						break;
					}
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler11 = new DefaultInterpolatedStringHandler(4, 2);
					defaultInterpolatedStringHandler11.AppendFormatted(value);
					defaultInterpolatedStringHandler11.AppendLiteral(" ");
					defaultInterpolatedStringHandler11.AppendFormatted(Value);
					defaultInterpolatedStringHandler11.AppendLiteral(" 增加");
					return defaultInterpolatedStringHandler11.ToStringAndClear();
				}
				case 26:
				{
					if (!(command == "[MAGICAL ABSOLUTE DEFENSE]"))
					{
						break;
					}
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler7 = new DefaultInterpolatedStringHandler(5, 2);
					defaultInterpolatedStringHandler7.AppendFormatted(value);
					defaultInterpolatedStringHandler7.AppendLiteral(" ");
					defaultInterpolatedStringHandler7.AppendFormatted((double)Value * 0.1);
					defaultInterpolatedStringHandler7.AppendLiteral("% 增加");
					return defaultInterpolatedStringHandler7.ToStringAndClear();
				}
				case 16:
				{
					if (!(command == "[MAGICAL_ATTACK]"))
					{
						break;
					}
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler4 = new DefaultInterpolatedStringHandler(4, 2);
					defaultInterpolatedStringHandler4.AppendFormatted(value);
					defaultInterpolatedStringHandler4.AppendLiteral(" ");
					defaultInterpolatedStringHandler4.AppendFormatted(Value);
					defaultInterpolatedStringHandler4.AppendLiteral(" 增加");
					return defaultInterpolatedStringHandler4.ToStringAndClear();
				}
				case 18:
				{
					if (!(command == "[PHYSICAL_DEFENSE]"))
					{
						break;
					}
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(4, 2);
					defaultInterpolatedStringHandler.AppendFormatted(value);
					defaultInterpolatedStringHandler.AppendLiteral(" ");
					defaultInterpolatedStringHandler.AppendFormatted(Value);
					defaultInterpolatedStringHandler.AppendLiteral(" 增加");
					return defaultInterpolatedStringHandler.ToStringAndClear();
				}
				}
			}
			return null;
		}
	}

	public avatar_select_ability(string command, string addType, int value)
	{
		base.Command = command;
		AddType = addType;
		Value = value;
	}
}
