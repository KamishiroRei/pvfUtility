using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using PvfCode.Models.Pvf;
using PvfCode.Models.Pvf.Enums;
using PvfCode.Models.Pvf.Enums.Stackable;

namespace PvfCode;

public static class PvfFileHelper
{
	private static Dictionary<string, string> vjAkAZs21N;

	public static Dictionary<string, string> avatar_select_abilityCommandTypeDic
	{
		get
		{
			if (vjAkAZs21N == null)
			{
				vjAkAZs21N = new Dictionary<string, string>();
				vjAkAZs21N.Add("[ACTIVESTATUS_TOLERANCE_ALL]", " 所有异常状态抗性");
				vjAkAZs21N.Add("[ACTIVESTATUS_TOLERANCE_STUCK]", " 回避率");
				vjAkAZs21N.Add("[ATTACK_SPEED]", " 攻击速度");
				vjAkAZs21N.Add("[CAST_SPEED]", " 施放速度");
				vjAkAZs21N.Add("[ELEMENT_TOLERANCE_DARK]", " 暗属性抗性");
				vjAkAZs21N.Add("[ELEMENT_TOLERANCE_FIRE]", " 火属性抗性");
				vjAkAZs21N.Add("[ELEMENT_TOLERANCE_LIGHT]", " 光属性抗性");
				vjAkAZs21N.Add("[ELEMENT_TOLERANCE_WATER]", " 冰属性抗性");
				vjAkAZs21N.Add("[EQUIPMENT_MAGICAL_DEFENSE]", " 魔法防御力");
				vjAkAZs21N.Add("[EQUIPMENT_PHYSICAL_DEFENSE]", " 物理防御力");
				vjAkAZs21N.Add("[HIT_RECOVERY]", " 硬直");
				vjAkAZs21N.Add("[HP MAX]", " HP最大值");
				vjAkAZs21N.Add("[HP_REGENRATE]", " 每分钟恢复HP");
				vjAkAZs21N.Add("[INVENTORY_MAX_WEIGHT]", " 最大负重");
				vjAkAZs21N.Add("[JUMP_POWER]", " 跳跃力");
				vjAkAZs21N.Add("[MAGICAL ABSOLUTE DEFENSE]", " 魔法伤害追加减少");
				vjAkAZs21N.Add("[MAGICAL_ATTACK]", " 智力");
				vjAkAZs21N.Add("[MAGICAL_DEFENSE]", " 精神");
				vjAkAZs21N.Add("[MOVE_SPEED]", " 移动速度");
				vjAkAZs21N.Add("[MP MAX]", " MP最大值");
				vjAkAZs21N.Add("[MP_REGENRATE]", " 每分钟恢复MP");
				vjAkAZs21N.Add("[PHYSICAL ABSOLUTE DEFENSE]", " 物理伤害追加减少");
				vjAkAZs21N.Add("[PHYSICAL_ATTACK]", " 力量");
				vjAkAZs21N.Add("[PHYSICAL_DEFENSE]", " 体力");
				vjAkAZs21N.Add("[STUCK ON ATTACK]", " 命中率");
				vjAkAZs21N.Add("[SKILL_LEVEL]", " 技能等级提升");
			}
			return vjAkAZs21N;
		}
	}

	public static bool GetLstFullPath(this PvfPack pvf, PvfFile lstFile, string rightPath, out string filePath)
	{
		string fileName = lstFile.FileName;
		rightPath = rightPath.ToLower();
		filePath = rightPath;
		if (!string.IsNullOrEmpty(rightPath) && rightPath[0] == '/')
		{
			rightPath = rightPath.Remove(0, 1);
		}
		if (fileName == "event/bluemarble/bluemarblemap.lst" || fileName == "event/bluemarble/bluemarbletile.lst")
		{
			filePath = "event/bluemarble/" + rightPath;
		}
		else if (fileName == "n_string.lst")
		{
			filePath = rightPath;
		}
		else if (fileName == "creature/script/creature.lst")
		{
			filePath = "creature/script/" + rightPath;
		}
		else if (fileName == "etc/randomoption/randomoption.lst")
		{
			filePath = "etc/randomoption/" + rightPath;
		}
		else if (fileName == "etc/randomoption/randomoptionskill.lst")
		{
			filePath = "etc/randomoption/" + rightPath;
		}
		else if (fileName == "event/eventcharacter/eventcharacter.lst")
		{
			filePath = "event/eventcharacter/" + rightPath;
		}
		else if (filePath.Contains("../"))
		{
			try
			{
				while (filePath.Length > 3 && filePath.Substring(0, 3) == "../")
				{
					filePath = filePath.Substring(3, filePath.Length - 3);
				}
			}
			catch (Exception)
			{
				return false;
			}
		}
		else
		{
			filePath = lstFile.LstItemPathToFilePath(rightPath).ToLower();
			if (!pvf.FileAny(filePath) && fileName == "etc/globaltutorialinfo/characterlist.lst")
			{
				filePath = Path.Combine("etc/globaltutorialinfo", rightPath).Replace("\\", "/");
			}
		}
		return pvf.FileAny(filePath);
	}

	public static bool JobDefaultStringConvertEnum(string job, out JobType? jobType)
	{
		jobType = null;
		if (job == null)
		{
			return false;
		}
		if (job != null)
		{
			switch (job.Length)
			{
			case 9:
				switch (job[1])
				{
				case 'f':
					if (job == "[fighter]")
					{
						jobType = JobType.女格斗家;
					}
					break;
				case 'a':
					if (job == "[at mage]")
					{
						jobType = JobType.男魔法师;
					}
					break;
				}
				break;
			case 8:
				switch (job[1])
				{
				case 'g':
					if (job == "[gunner]")
					{
						jobType = JobType.男神枪手;
					}
					break;
				case 'p':
					if (job == "[priest]")
					{
						jobType = JobType.男圣职者;
					}
					break;
				case 'k':
					if (job == "[knight]")
					{
						jobType = JobType.守护者;
					}
					break;
				}
				break;
			case 11:
				switch (job[4])
				{
				case 'g':
					if (job == "[at gunner]")
					{
						jobType = JobType.女神枪手;
					}
					break;
				case 'p':
					if (job == "[at priest]")
					{
						jobType = JobType.女圣职者;
					}
					break;
				}
				break;
			case 12:
				switch (job[1])
				{
				case 'a':
					if (job == "[at fighter]")
					{
						jobType = JobType.男格斗家;
					}
					break;
				case 'g':
					if (job == "[gun blader]")
					{
						jobType = JobType.枪剑士;
					}
					break;
				}
				break;
			case 10:
				if (job == "[swordman]")
				{
					jobType = JobType.鬼剑士;
				}
				break;
			case 6:
				if (job == "[mage]")
				{
					jobType = JobType.女魔法师;
				}
				break;
			case 7:
				if (job == "[thief]")
				{
					jobType = JobType.暗夜使者;
				}
				break;
			case 18:
				if (job == "[demonic swordman]")
				{
					jobType = JobType.黑暗武士;
				}
				break;
			case 14:
				if (job == "[creator mage]")
				{
					jobType = JobType.缔造者;
				}
				break;
			case 13:
				if (job == "[at swordman]")
				{
					jobType = JobType.女鬼剑士;
				}
				break;
			case 16:
				if (job == "[demonic lancer]")
				{
					jobType = JobType.魔枪士;
				}
				break;
			case 5:
				if (job == "[all]")
				{
					jobType = JobType.通用;
				}
				break;
			}
		}
		return jobType.HasValue;
	}

	public static bool ScriptContentToStackableType(string cmd, int num, out StackableType? type, out string? err)
	{
		err = null;
		type = null;
		if (cmd != null)
		{
			switch (cmd.Length)
			{
			case 21:
				switch (cmd[1])
				{
				case 'm':
					if (cmd == "[material expert job]")
					{
						switch (num)
						{
						case 0:
							type = StackableType.材料_炼金控偶_0;
							return true;
						case 1:
							type = StackableType.材料_附魔卡片_1;
							return true;
						}
					}
					break;
				case 'u':
					if (!(cmd == "[usable cera package]") || num != 0)
					{
						break;
					}
					type = StackableType.消耗品_可选属性的时装礼包_0;
					return true;
				}
				break;
			case 10:
				switch (cmd[2])
				{
				case 'a':
					if (cmd == "[material]")
					{
						switch (num)
						{
						case 0:
							type = StackableType.材料_无特殊_0;
							return true;
						case 1:
							type = StackableType.材料_无特殊_1;
							return true;
						case 2:
							type = StackableType.材料_无特殊_2;
							return true;
						case 3:
							type = StackableType.材料_无特殊_3;
							return true;
						case 4:
							type = StackableType.材料_无特殊_4;
							return true;
						case 5:
							type = StackableType.材料_无特殊_5;
							return true;
						case 7:
							type = StackableType.材料_无特殊_7;
							return true;
						case 8:
							type = StackableType.材料_无特殊_8;
							return true;
						case 10:
							type = StackableType.材料_无特殊_10;
							return true;
						case 11:
							type = StackableType.材料_无特殊_11;
							return true;
						}
					}
					break;
				case 'o':
					if (!(cmd == "[contract]") || num != 0)
					{
						break;
					}
					type = StackableType.消耗品_契约包_0;
					return true;
				case 'r':
					if (!(cmd == "[creature]") || num != 1)
					{
						break;
					}
					type = StackableType.消耗品_宠物盒子_1;
					return true;
				case 'i':
					if (!(cmd == "[disguise]") || num != 0)
					{
						break;
					}
					type = StackableType.消耗品_cosplay道具_0;
					return true;
				}
				break;
			case 18:
				switch (cmd[1])
				{
				case 's':
					if (cmd == "[stackable legacy]")
					{
						switch (num)
						{
						case 0:
							type = StackableType.材料_自动售卖机使用券_0;
							return true;
						case 1:
							type = StackableType.材料_自动售卖机使用券_1;
							return true;
						}
					}
					break;
				case 'c':
					if (!(cmd == "[creature expitem]") || num != 0)
					{
						break;
					}
					type = StackableType.消耗品_宠物经验道具_0;
					return true;
				case 't':
					if (!(cmd == "[town and dungeon]") || num != 0)
					{
						break;
					}
					type = StackableType.消耗品_分解机_强化器等_0;
					return true;
				}
				break;
			case 15:
				switch (cmd[1])
				{
				case 'a':
					if (!(cmd == "[avatar emblem]") || num != 0)
					{
						break;
					}
					type = StackableType.徽章_镶嵌使用_0;
					return true;
				case 'e':
					if (!(cmd == "[enchant waste]") || num != 0)
					{
						break;
					}
					type = StackableType.消耗品_附魔宝珠_0;
					return true;
				case 'g':
					if (!(cmd == "[global effect]") || num != 0)
					{
						break;
					}
					type = StackableType.消耗品_全局效果_0;
					return true;
				case 'q':
					if (!(cmd == "[quest receive]") || num != 0)
					{
						break;
					}
					type = StackableType.消耗品_使用后可接受任务_0;
					return true;
				case 'u':
					if (!(cmd == "[unlimited etc]") || num != 0)
					{
						break;
					}
					type = StackableType.消耗品_洗点水_0;
					return true;
				}
				break;
			case 7:
				switch (cmd[1])
				{
				case 'q':
					if (cmd == "[quest]")
					{
						switch (num)
						{
						case 0:
							type = StackableType.任务品_都是任务品无特殊作用_0;
							return true;
						case 2:
							type = StackableType.任务品_都是任务品无特殊作用_2;
							return true;
						case 3:
							type = StackableType.任务品_都是任务品无特殊作用_3;
							return true;
						case 5:
							type = StackableType.任务品_都是任务品无特殊作用_5;
							return true;
						}
					}
					break;
				case 't':
					if (cmd == "[throw]")
					{
						switch (num)
						{
						case 0:
							type = StackableType.消耗品_投掷品_0;
							return true;
						case 1:
							type = StackableType.消耗品_投掷品_1;
							return true;
						case 2:
							type = StackableType.消耗品_投掷品_2;
							return true;
						case 3:
							type = StackableType.消耗品_投掷品_3;
							return true;
						}
					}
					break;
				case 'w':
					if (cmd == "[waste]")
					{
						switch (num)
						{
						case 0:
							type = StackableType.消耗品_药剂等_0;
							return true;
						case 1:
							type = StackableType.消耗品_药剂等_1;
							return true;
						case 3:
							type = StackableType.消耗品_药剂等_3;
							return true;
						case 4:
							type = StackableType.消耗品_药剂等_4;
							return true;
						case 5:
							type = StackableType.消耗品_药剂等_5;
							return true;
						case 6:
							type = StackableType.消耗品_药剂等_6;
							return true;
						case 10:
							type = StackableType.消耗品_药剂等_10;
							return true;
						}
					}
					break;
				}
				break;
			case 8:
				switch (cmd[1])
				{
				case 'r':
					if (cmd == "[recipe]")
					{
						switch (num)
						{
						case 0:
							type = StackableType.设计图_杂七杂八_0;
							return true;
						case 1:
							type = StackableType.设计图_装备类_1;
							return true;
						case 2:
							type = StackableType.设计图_药剂类_2;
							return true;
						case 5:
							type = StackableType.设计图_装备类_5;
							return true;
						}
					}
					break;
				case 'l':
					if (cmd == "[legacy]")
					{
						switch (num)
						{
						case 1:
							type = StackableType.消耗品_罐子_1;
							return true;
						case 5:
							type = StackableType.消耗品_罐子_5;
							return true;
						case 7:
							type = StackableType.消耗品_罐子_7;
							return true;
						case 10:
							type = StackableType.消耗品_罐子_10;
							return true;
						case 11:
							type = StackableType.消耗品_罐子_11;
							return true;
						case 12:
							type = StackableType.消耗品_罐子_12;
							return true;
						case 13:
							type = StackableType.消耗品_罐子_13;
							return true;
						case 14:
							type = StackableType.消耗品_罐子_14;
							return true;
						case 15:
							type = StackableType.消耗品_罐子_15;
							return true;
						case 16:
							type = StackableType.消耗品_罐子_16;
							return true;
						case 17:
							type = StackableType.消耗品_罐子_17;
							return true;
						case 18:
							type = StackableType.消耗品_罐子_18;
							return true;
						case 19:
							type = StackableType.消耗品_罐子_19;
							return true;
						case 20:
							type = StackableType.消耗品_罐子_20;
							return true;
						case 21:
							type = StackableType.消耗品_罐子_21;
							return true;
						case 24:
							type = StackableType.消耗品_罐子_24;
							return true;
						}
					}
					break;
				}
				break;
			case 19:
				switch (cmd[1])
				{
				case 'b':
					if (!(cmd == "[booster selection]") || num != 0)
					{
						break;
					}
					type = StackableType.消耗品_可选盒子_0;
					return true;
				case 'u':
					if (cmd == "[upgradable legacy]")
					{
						switch (num)
						{
						case 0:
							type = StackableType.消耗品_罐子_0;
							return true;
						case 1:
							type = StackableType.消耗品_NPC罐子_1;
							return true;
						}
					}
					break;
				}
				break;
			case 14:
				switch (cmd[6])
				{
				case 'b':
					if (cmd == "[cera booster]")
					{
						switch (num)
						{
						case 0:
							type = StackableType.消耗品_点券盒子_0;
							return true;
						case 1:
							type = StackableType.消耗品_点券盒子_1;
							return true;
						}
					}
					break;
				case 'p':
					if (!(cmd == "[cera package]") || num != 0)
					{
						break;
					}
					type = StackableType.消耗品_点卷礼包_0;
					return true;
				}
				break;
			case 5:
				switch (cmd[1])
				{
				case 'd':
					if (!(cmd == "[dye]") || num != 0)
					{
						break;
					}
					type = StackableType.消耗品_时装染色剂_0;
					return true;
				case 'e':
					if (cmd == "[etc]")
					{
						switch (num)
						{
						case 0:
							type = StackableType.消耗品_其他_0;
							return true;
						case 1:
							type = StackableType.消耗品_其他_1;
							return true;
						case 8:
							type = StackableType.消耗品_其他_8;
							return true;
						}
					}
					break;
				case 's':
					if (cmd == "[set]")
					{
						switch (num)
						{
						case 1:
							type = StackableType.消耗品_布置地雷_1;
							return true;
						case 3:
							type = StackableType.消耗品_布置投掷品_3;
							return true;
						}
					}
					break;
				}
				break;
			case 20:
				switch (cmd[1])
				{
				case 'e':
					if (!(cmd == "[expert town potion]") || num != 0)
					{
						break;
					}
					type = StackableType.消耗品_疲劳药_0;
					return true;
				case 'u':
					if (!(cmd == "[upgrade limit cube]") || num != 0)
					{
						break;
					}
					type = StackableType.消耗品_升级装备用的_0;
					return true;
				}
				break;
			case 17:
				switch (cmd[1])
				{
				case 't':
					if (!(cmd == "[teleport potion]") || num != 0)
					{
						break;
					}
					type = StackableType.消耗品_瞬间移动药剂_0;
					return true;
				case 'u':
					if (cmd == "[unlimited waste]")
					{
						switch (num)
						{
						case 0:
							type = StackableType.消耗品_不会消耗的道具_0;
							return true;
						case 1:
							type = StackableType.消耗品_不会消耗的道具_1;
							return true;
						}
					}
					break;
				}
				break;
			case 16:
				if (!(cmd == "[booster random]") || num != 0)
				{
					break;
				}
				type = StackableType.消耗品_随机盒子_0;
				return true;
			case 9:
				if (cmd == "[booster]")
				{
					switch (num)
					{
					case 0:
						type = StackableType.消耗品_礼包_0;
						return true;
					case 1:
						type = StackableType.消耗品_礼包_1;
						return true;
					}
				}
				break;
			case 6:
				if (!(cmd == "[feed]") || num != 1)
				{
					break;
				}
				type = StackableType.消耗品_宠物饲料_1;
				return true;
			case 36:
				if (!(cmd == "[multi upgradable legacy bonus cera]") || num != 0)
				{
					break;
				}
				type = StackableType.消耗品_点券魔锤魔盒_0;
				return true;
			case 25:
				if (!(cmd == "[multi upgradable legacy]") || num != 0)
				{
					break;
				}
				type = StackableType.消耗品_魔锤魔盒_0;
				return true;
			case 13:
				if (!(cmd == "[only effect]") || num != 0)
				{
					break;
				}
				type = StackableType.消耗品_城镇烟花_0;
				return true;
			case 28:
				if (!(cmd == "[unlimited town and dungeon]") || num != 0)
				{
					break;
				}
				type = StackableType.消耗品_副职业提取器_0;
				return true;
			}
		}
		DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(6, 2);
		defaultInterpolatedStringHandler.AppendFormatted(cmd);
		defaultInterpolatedStringHandler.AppendLiteral("\t");
		defaultInterpolatedStringHandler.AppendFormatted(num);
		defaultInterpolatedStringHandler.AppendLiteral(" 未能识别");
		err = defaultInterpolatedStringHandler.ToStringAndClear();
		return false;
	}

	public static bool StrConvertEquipmentType(string str, out EquipmentType? re)
	{
		re = null;
		if (str != null)
		{
			switch (str.Length)
			{
			case 12:
				switch (str[1])
				{
				case 't':
					if (str == "[title name]")
					{
						re = EquipmentType.称号;
					}
					break;
				case 'h':
					if (str == "[hat avatar]")
					{
						re = EquipmentType.帽子装扮;
					}
					break;
				}
				break;
			case 8:
				switch (str[1])
				{
				case 'w':
					if (str == "[weapon]")
					{
						re = EquipmentType.武器;
					}
					break;
				case 'a':
					if (str == "[amulet]")
					{
						re = EquipmentType.项链;
					}
					break;
				}
				break;
			case 6:
				switch (str[1])
				{
				case 'c':
					if (str == "[coat]")
					{
						re = EquipmentType.上衣;
					}
					break;
				case 'r':
					if (str == "[ring]")
					{
						re = EquipmentType.戒指;
					}
					break;
				}
				break;
			case 7:
				switch (str[1])
				{
				case 'p':
					if (str == "[pants]")
					{
						re = EquipmentType.下衣;
					}
					break;
				case 'w':
					if (!(str == "[waist]"))
					{
						if (str == "[wrist]")
						{
							re = EquipmentType.手镯;
						}
					}
					else
					{
						re = EquipmentType.腰带;
					}
					break;
				case 's':
					if (str == "[shoes]")
					{
						re = EquipmentType.鞋;
					}
					break;
				}
				break;
			case 10:
				switch (str[1])
				{
				case 's':
					if (str == "[shoulder]")
					{
						re = EquipmentType.护肩;
					}
					break;
				case 'c':
					if (str == "[creature]")
					{
						re = EquipmentType.宠物;
					}
					break;
				}
				break;
			case 13:
				switch (str[1])
				{
				case 'm':
					if (str == "[magic stone]")
					{
						re = EquipmentType.魔法石;
					}
					break;
				case 's':
					if (str == "[skin avatar]")
					{
						re = EquipmentType.皮肤装扮;
					}
					break;
				case 'c':
					if (str == "[coat avatar]")
					{
						re = EquipmentType.上衣装扮;
					}
					break;
				case 'f':
					if (str == "[face avatar]")
					{
						re = EquipmentType.脸部装扮;
					}
					break;
				case 'h':
					if (str == "[hair avatar]")
					{
						re = EquipmentType.头部装扮;
					}
					break;
				}
				break;
			case 14:
				switch (str[1])
				{
				case 'a':
					if (str == "[artifact red]")
					{
						re = EquipmentType.宠物装备_红;
					}
					break;
				case 'w':
					if (str == "[waist avatar]")
					{
						re = EquipmentType.腰部装扮;
					}
					break;
				case 'p':
					if (str == "[pants avatar]")
					{
						re = EquipmentType.下装装扮;
					}
					break;
				case 's':
					if (str == "[shoes avatar]")
					{
						re = EquipmentType.鞋装扮;
					}
					break;
				}
				break;
			case 15:
				switch (str[3])
				{
				case 't':
					if (str == "[artifact blue]")
					{
						re = EquipmentType.宠物装备_蓝;
					}
					break;
				case 'r':
					if (str == "[aurora avatar]")
					{
						re = EquipmentType.光环装扮;
					}
					break;
				case 'e':
					if (str == "[breast avatar]")
					{
						re = EquipmentType.胸部装扮;
					}
					break;
				}
				break;
			case 9:
				if (str == "[support]")
				{
					re = EquipmentType.辅助装备;
				}
				break;
			case 16:
				if (str == "[artifact green]")
				{
					re = EquipmentType.宠物装备_绿;
				}
				break;
			}
		}
		return re.HasValue;
	}

	public static MonsterCategoryType? StrConvertMonsterCategoryEnum(string str)
	{
		if (str == null)
		{
			return null;
		}
		if (str != null)
		{
			switch (str.Length)
			{
			case 7:
				switch (str[1])
				{
				case 'h':
					if (!(str == "[human]"))
					{
						break;
					}
					return MonsterCategoryType.人型;
				case 'b':
					if (!(str == "[beast]"))
					{
						break;
					}
					return MonsterCategoryType.野兽;
				case 'p':
					if (!(str == "[plant]"))
					{
						break;
					}
					return MonsterCategoryType.植物;
				case 'd':
					if (!(str == "[devil]"))
					{
						break;
					}
					return MonsterCategoryType.恶魔;
				case 'a':
					if (!(str == "[angel]"))
					{
						break;
					}
					return MonsterCategoryType.天使;
				}
				break;
			case 8:
				switch (str[1])
				{
				case 'i':
					if (!(str == "[insect]"))
					{
						break;
					}
					return MonsterCategoryType.昆虫;
				case 'h':
					if (!(str == "[hybrid]"))
					{
						break;
					}
					return MonsterCategoryType.组合体;
				case 'u':
					if (!(str == "[undead]"))
					{
						break;
					}
					return MonsterCategoryType.不死族;
				case 's':
					if (!(str == "[spirit]"))
					{
						break;
					}
					return MonsterCategoryType.精灵;
				case 'd':
					if (!(str == "[dragon]"))
					{
						break;
					}
					return MonsterCategoryType.龙族;
				}
				break;
			case 9:
				if (!(str == "[machine]"))
				{
					break;
				}
				return MonsterCategoryType.机械;
			}
		}
		return null;
	}
}
