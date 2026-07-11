using System.Collections.Generic;
using System.Linq;
using System.Text;
using PvfCode.Services.PvfParsingNew;

namespace PvfCode.Services.PreviewPvfFileFolder;

public class PvfFilePreviewHelper
{
	public static string? GetEquWhiteAttributes(ScriptFileParserNew scriptFileParserNewNew, List<SectionBase> sections, PvfGroup Pvf)
	{
		if (sections == null)
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder();
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[equipment physical attack]", sections, out string val))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder3 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(7, 1, stringBuilder2);
			handler.AppendLiteral("物理攻击力 +");
			handler.AppendFormatted(val);
			stringBuilder3.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[equipment magical attack]", sections, out string val2))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder4 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(7, 1, stringBuilder2);
			handler.AppendLiteral("魔法攻击力 +");
			handler.AppendFormatted(val2);
			stringBuilder4.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[separate attack]", sections, out string val3))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder5 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(7, 1, stringBuilder2);
			handler.AppendLiteral("独立攻击力 +");
			handler.AppendFormatted(val3);
			stringBuilder5.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValueArray(sections.Where((SectionBase it) => it.GetSectionName() == "[equipment magical defense]").FirstOrDefault(), out List<string> arr) && arr.Count >= 1)
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder6 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(7, 1, stringBuilder2);
			handler.AppendLiteral("魔法防御力 +");
			handler.AppendFormatted(arr[0]);
			stringBuilder6.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValueArray(sections.Where((SectionBase it) => it.GetSectionName() == "[equipment physical defense]").FirstOrDefault(), out List<string> arr2) && arr2.Count >= 1)
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder7 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(7, 1, stringBuilder2);
			handler.AppendLiteral("物理防御力 +");
			handler.AppendFormatted(arr2[0]);
			stringBuilder7.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[magical attack]", sections, out string val4))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder8 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(4, 1, stringBuilder2);
			handler.AppendLiteral("智力 +");
			handler.AppendFormatted(val4);
			stringBuilder8.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[magical defense]", sections, out string val5))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder9 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(4, 1, stringBuilder2);
			handler.AppendLiteral("精神 +");
			handler.AppendFormatted(val5);
			stringBuilder9.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[physical attack]", sections, out string val6))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder10 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(4, 1, stringBuilder2);
			handler.AppendLiteral("力量 +");
			handler.AppendFormatted(val6);
			stringBuilder10.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[physical defense]", sections, out string val7))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder11 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(4, 1, stringBuilder2);
			handler.AppendLiteral("体力 +");
			handler.AppendFormatted(val7);
			stringBuilder11.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[anti evil]", sections, out string val8))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder12 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(5, 1, stringBuilder2);
			handler.AppendLiteral("抗魔值 +");
			handler.AppendFormatted(val8);
			stringBuilder12.AppendLine(ref handler);
		}
		if (stringBuilder.Length <= 0)
		{
			return null;
		}
		return stringBuilder.ToString();
	}

	public static string? EquBlueAttributes(ScriptFileParserNew scriptFileParserNewNew, List<SectionBase> sections, PvfGroup Pvf)
	{
		if (sections == null)
		{
			return null;
		}
		StringBuilder stringBuilder = new StringBuilder();
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[MP regen speed]", sections, out string val))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder3 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(14, 2, stringBuilder2);
			handler.AppendLiteral("每分钟恢复");
			handler.AppendFormatted(XE9jLU3BYw(val, 3.0));
			handler.AppendLiteral("MP (实际效果");
			handler.AppendFormatted(XE9jLU3BYw(val, 11.0));
			handler.AppendLiteral(")");
			stringBuilder3.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[HP regen speed]", sections, out string val2))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder4 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(14, 2, stringBuilder2);
			handler.AppendLiteral("每分钟恢复");
			handler.AppendFormatted(XE9jLU3BYw(val2, 3.0));
			handler.AppendLiteral("HP (实际效果");
			handler.AppendFormatted(XE9jLU3BYw(val2, 11.0));
			handler.AppendLiteral(")");
			stringBuilder4.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[HP MAX]", sections, out string val3))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder5 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(16, 2, stringBuilder2);
			handler.AppendLiteral("HP最大值 +");
			handler.AppendFormatted(val3);
			handler.AppendLiteral(" (实际效果 +");
			handler.AppendFormatted(XE9jLU3BYw(val3, 1.73));
			handler.AppendLiteral(")");
			stringBuilder5.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[MP MAX]", sections, out string val4))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder6 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(15, 2, stringBuilder2);
			handler.AppendLiteral("MP最大值 +");
			handler.AppendFormatted(val4);
			handler.AppendLiteral(" (实际效果 +");
			handler.AppendFormatted(XE9jLU3BYw(val4, 1.73));
			stringBuilder6.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[hit recovery]", sections, out string val5))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder7 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(4, 1, stringBuilder2);
			handler.AppendLiteral("硬直 +");
			handler.AppendFormatted(val5);
			stringBuilder7.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[rigidity]", sections, out string val6))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder8 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(4, 1, stringBuilder2);
			handler.AppendLiteral("硬直 +");
			handler.AppendFormatted(val6);
			stringBuilder8.AppendLine(ref handler);
		}
		if (vWBjp0r1CV(scriptFileParserNewNew, sections, Pvf, out string val7))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder9 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(0, 1, stringBuilder2);
			handler.AppendFormatted(val7);
			stringBuilder9.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[attack speed]", sections, out string val8))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder10 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(7, 1, stringBuilder2);
			handler.AppendLiteral("攻击速度 +");
			handler.AppendFormatted(XE9jLU3BYw(val8, 0.1));
			handler.AppendLiteral("%");
			stringBuilder10.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[cast speed]", sections, out string val9))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder11 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(7, 1, stringBuilder2);
			handler.AppendLiteral("释放速度 +");
			handler.AppendFormatted(XE9jLU3BYw(val9, 0.1));
			handler.AppendLiteral("%");
			stringBuilder11.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[move speed]", sections, out string val10))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder12 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(7, 1, stringBuilder2);
			handler.AppendLiteral("移动速度 +");
			handler.AppendFormatted(XE9jLU3BYw(val10, 0.1));
			handler.AppendLiteral("%");
			stringBuilder12.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[physical critical hit]", sections, out string val11))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder13 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(8, 1, stringBuilder2);
			handler.AppendLiteral("物理暴击率 +");
			handler.AppendFormatted(val11);
			handler.AppendLiteral("%");
			stringBuilder13.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[magical critical hit]", sections, out string val12))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder14 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(8, 1, stringBuilder2);
			handler.AppendLiteral("魔法暴击率 +");
			handler.AppendFormatted(val12);
			handler.AppendLiteral("%");
			stringBuilder14.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[inventory limit]", sections, out string val13))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder15 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(10, 1, stringBuilder2);
			handler.AppendLiteral("增加负重上限 +");
			handler.AppendFormatted(XE9jLU3BYw(val13, 0.001));
			handler.AppendLiteral("kg");
			stringBuilder15.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[fire attack]", sections, out string val14))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder16 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(7, 1, stringBuilder2);
			handler.AppendLiteral("火属性强化 +");
			handler.AppendFormatted(val14);
			stringBuilder16.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[dark attack]", sections, out string val15))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder17 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(7, 1, stringBuilder2);
			handler.AppendLiteral("暗属性强化 +");
			handler.AppendFormatted(val15);
			stringBuilder17.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[water attack]", sections, out string val16))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder18 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(7, 1, stringBuilder2);
			handler.AppendLiteral("冰属性强化 +");
			handler.AppendFormatted(val16);
			stringBuilder18.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[light attack]", sections, out string val17))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder19 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(7, 1, stringBuilder2);
			handler.AppendLiteral("光属性强化 +");
			handler.AppendFormatted(val17);
			stringBuilder19.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[all elemental attack]", sections, out string val18))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder20 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(8, 1, stringBuilder2);
			handler.AppendLiteral("所有属性强化 +");
			handler.AppendFormatted(val18);
			stringBuilder20.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[fire resistance]", sections, out string val19))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder21 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(7, 1, stringBuilder2);
			handler.AppendLiteral("火属性抗性 +");
			handler.AppendFormatted(val19);
			stringBuilder21.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[dark resistance]", sections, out string val20))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder22 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(7, 1, stringBuilder2);
			handler.AppendLiteral("暗属性抗性 +");
			handler.AppendFormatted(val20);
			stringBuilder22.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[light resistance]", sections, out string val21))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder23 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(7, 1, stringBuilder2);
			handler.AppendLiteral("光属性抗性 +");
			handler.AppendFormatted(val21);
			stringBuilder23.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[water resistance]", sections, out string val22))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder24 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(7, 1, stringBuilder2);
			handler.AppendLiteral("冰属性抗性 +");
			handler.AppendFormatted(val22);
			stringBuilder24.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[slow resistance]", sections, out string val23))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder25 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(6, 1, stringBuilder2);
			handler.AppendLiteral("减速抗性 +");
			handler.AppendFormatted(val23);
			stringBuilder25.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[freeze resistance]", sections, out string val24))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder26 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(6, 1, stringBuilder2);
			handler.AppendLiteral("冰冻抗性 +");
			handler.AppendFormatted(val24);
			stringBuilder26.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[poison resistance]", sections, out string val25))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder27 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(6, 1, stringBuilder2);
			handler.AppendLiteral("中毒抗性 +");
			handler.AppendFormatted(val25);
			stringBuilder27.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[stun resistance]", sections, out string val26))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder28 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(6, 1, stringBuilder2);
			handler.AppendLiteral("眩晕抗性 +");
			handler.AppendFormatted(val26);
			stringBuilder28.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[curse resistance]", sections, out string val27))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder29 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(6, 1, stringBuilder2);
			handler.AppendLiteral("诅咒抗性 +");
			handler.AppendFormatted(val27);
			stringBuilder29.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[blind resistance]", sections, out string val28))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder30 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(6, 1, stringBuilder2);
			handler.AppendLiteral("失明抗性 +");
			handler.AppendFormatted(val28);
			stringBuilder30.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[lightning resistance]", sections, out string val29))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder31 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(6, 1, stringBuilder2);
			handler.AppendLiteral("感电抗性 +");
			handler.AppendFormatted(val29);
			stringBuilder31.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[stone resistance]", sections, out string val30))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder32 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(6, 1, stringBuilder2);
			handler.AppendLiteral("石化抗性 +");
			handler.AppendFormatted(val30);
			stringBuilder32.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[sleep resistance]", sections, out string val31))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder33 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(6, 1, stringBuilder2);
			handler.AppendLiteral("睡眠抗性 +");
			handler.AppendFormatted(val31);
			stringBuilder33.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[burn resistance]", sections, out string val32))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder34 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(6, 1, stringBuilder2);
			handler.AppendLiteral("灼伤抗性 +");
			handler.AppendFormatted(val32);
			stringBuilder34.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[bleeding resistance]", sections, out string val33))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder35 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(6, 1, stringBuilder2);
			handler.AppendLiteral("流血抗性 +");
			handler.AppendFormatted(val33);
			stringBuilder35.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[piercing resistance]", sections, out string val34))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder36 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(6, 1, stringBuilder2);
			handler.AppendLiteral("穿刺抗性 +");
			handler.AppendFormatted(val34);
			stringBuilder36.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[confuse resistance]", sections, out string val35))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder37 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(6, 1, stringBuilder2);
			handler.AppendLiteral("混乱抗性 +");
			handler.AppendFormatted(val35);
			stringBuilder37.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[hold resistance]", sections, out string val36))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder38 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(6, 1, stringBuilder2);
			handler.AppendLiteral("束缚抗性 +");
			handler.AppendFormatted(val36);
			stringBuilder38.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[all elemental resistance]", sections, out string val37))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder39 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(8, 1, stringBuilder2);
			handler.AppendLiteral("所有属性抗性 +");
			handler.AppendFormatted(val37);
			stringBuilder39.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[all activestatus resistance]", sections, out string val38))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder40 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(10, 1, stringBuilder2);
			handler.AppendLiteral("所有异常状态抗性 +");
			handler.AppendFormatted(val38);
			stringBuilder40.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[stuck resistance]", sections, out string val39))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder41 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(6, 1, stringBuilder2);
			handler.AppendLiteral("回避率 +");
			handler.AppendFormatted(XE9jLU3BYw(val39, 0.1));
			handler.AppendLiteral("%");
			stringBuilder41.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[stuck]", sections, out string val40) && !string.IsNullOrEmpty(val40))
		{
			float result2;
			if (int.TryParse(val40, out var result))
			{
				if (result < 0)
				{
					StringBuilder stringBuilder2 = stringBuilder;
					StringBuilder stringBuilder42 = stringBuilder2;
					StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(6, 1, stringBuilder2);
					handler.AppendLiteral("命中率 +");
					handler.AppendFormatted(val40.ToString().Remove(0, 1));
					handler.AppendLiteral("%");
					stringBuilder42.AppendLine(ref handler);
				}
				else
				{
					StringBuilder stringBuilder2 = stringBuilder;
					StringBuilder stringBuilder43 = stringBuilder2;
					StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(6, 1, stringBuilder2);
					handler.AppendLiteral("命中率 -");
					handler.AppendFormatted(val40.ToString());
					handler.AppendLiteral("%");
					stringBuilder43.AppendLine(ref handler);
				}
			}
			else if (float.TryParse(val40, out result2))
			{
				if (result2 < 0f)
				{
					StringBuilder stringBuilder2 = stringBuilder;
					StringBuilder stringBuilder44 = stringBuilder2;
					StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(6, 1, stringBuilder2);
					handler.AppendLiteral("命中率 +");
					handler.AppendFormatted(val40.ToString().Remove(0, 1));
					handler.AppendLiteral("%");
					stringBuilder44.AppendLine(ref handler);
				}
				else
				{
					StringBuilder stringBuilder2 = stringBuilder;
					StringBuilder stringBuilder45 = stringBuilder2;
					StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(6, 1, stringBuilder2);
					handler.AppendLiteral("命中率 -");
					handler.AppendFormatted(val40.ToString());
					handler.AppendLiteral("%");
					stringBuilder45.AppendLine(ref handler);
				}
			}
			else
			{
				StringBuilder stringBuilder2 = stringBuilder;
				StringBuilder stringBuilder46 = stringBuilder2;
				StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(6, 1, stringBuilder2);
				handler.AppendLiteral("命中率 +");
				handler.AppendFormatted(val40?.ToString());
				handler.AppendLiteral("%");
				stringBuilder46.AppendLine(ref handler);
			}
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[jump power]", sections, out string val41))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder47 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(5, 1, stringBuilder2);
			handler.AppendLiteral("跳跃力 +");
			handler.AppendFormatted(val41);
			stringBuilder47.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetItemAuraExplainDataString(sections, out StringBuilder bui))
		{
			stringBuilder.AppendLine(bui.ToString());
		}
		if (scriptFileParserNewNew.GetSectionValue(Pvf, "[room list move speed rate]", sections, out string val42))
		{
			StringBuilder stringBuilder2 = stringBuilder;
			StringBuilder stringBuilder48 = stringBuilder2;
			StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(10, 1, stringBuilder2);
			handler.AppendLiteral("城镇内移动速度 +");
			handler.AppendFormatted(XE9jLU3BYw(val42, 100.0));
			handler.AppendLiteral("%");
			stringBuilder48.AppendLine(ref handler);
		}
		if (scriptFileParserNewNew.GetGetSkillLevelUpString(sections, out StringBuilder bui2))
		{
			stringBuilder.AppendLine(bui2.ToString());
		}
		if (stringBuilder.Length <= 0)
		{
			return null;
		}
		return stringBuilder.ToString();
	}

	private static bool vWBjp0r1CV(ScriptFileParserNew P_0, List<SectionBase> P_1, PvfGroup P_2, out string? val)
	{
		val = null;
		SectionBase sectionBase = P_1.Where((SectionBase it) => it.GetSectionName() == "[elemental property]").FirstOrDefault();
		if (sectionBase == null)
		{
			return false;
		}
		if (P_0.GetSectionStringArray(sectionBase, out List<string> arr))
		{
			List<string> list = new List<string>();
			foreach (string item in arr)
			{
				if (!(item == "[fire element]"))
				{
					if (!(item == "[water element]"))
					{
						if (!(item == "[light element]"))
						{
							if (item == "[dark element]")
							{
								list.Add("暗属性攻击");
							}
							else
							{
								list.Add("未能识别的属性攻击");
							}
						}
						else
						{
							list.Add("光属性攻击");
						}
					}
					else
					{
						list.Add("冰属性攻击");
					}
				}
				else
				{
					list.Add("火属性攻击");
				}
			}
			val = string.Join("\r\n", list);
			return true;
		}
		return false;
	}

	private static string XE9jLU3BYw(string? obj, double P_1)
	{
		if (string.IsNullOrEmpty(obj))
		{
			return null;
		}
		if (obj.Contains("."))
		{
			return (float.Parse(obj) * (float)P_1).ToString("0.00");
		}
		string text = ((double)int.Parse(obj) * P_1).ToString();
		if (!text.Contains("."))
		{
			return text;
		}
		return float.Parse(text).ToString("0.00");
	}

	public PvfFilePreviewHelper()
	{
	}
}
