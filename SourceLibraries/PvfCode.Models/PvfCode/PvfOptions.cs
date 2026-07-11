using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using PvfCode.Dot;
using PvfCode.Models.Enums;
using PvfCode.Models.Options;
using PvfCode.Models.Pvf.Enums;
using PvfCode.Models.Pvf.ImportModels;
using PvfCode.Services.ExtractFilesModel;
using Utools;

namespace PvfCode;

[JsonObject(MemberSerialization.OptOut)]
public class PvfOptions : ModelBase
{
	private HashSet<string> WZqnEKchuV;

	private PvfReleaseLog WXUnZTyD84;

	private HashSet<string> d3mn8rLEW9;

	private HashSet<string> ypUnuwGq1c;

	private bool KV7n5XZ84w;

	private bool rXunpfpmjA;

	private bool hYanDBwvon;

	private HashSet<string> ML0n3ec5ie;

	private Dictionary<string, PvfFileType> i0VnHk3b7E;

	private AutoTheBackupPvfOptions RBcn7WPrcH;

	public readonly string StringLstFileName;

	private EncodingType? gFQncEUXri;

	private bool? pGAngjIAsJ;

	private List<EncodingType> BtOnK50TxJ;

	[CompilerGenerated]
	private bool WLGnYpIL8V;

	[CompilerGenerated]
	private bool KVSnJ7WoJg;

	private bool? PevndeOBwg;

	private Dictionary<string, PvfFileType> x8vn1AQ8FU;

	private Dictionary<string, TreelistCommentRes> nXYnGTgbyK;

	private HashSet<PvfFileType> IWSnOKuYmC;

	private HashSet<string> vdKnrW2u4x;

	private ExtractConfig NwAnecBfTM;

	private ImportConfig Sm2njbnj24;

	private PvfCommentPriority? tpenxpJM61;

	private bool kelnmn3VZ9;

	private bool? FGEnSyvUHm;

	private bool BFWnB2giU5;

	[JsonIgnore]
	public HashSet<string> AvatarParts
	{
		get
		{
			if (WZqnEKchuV == null)
			{
				WZqnEKchuV = new HashSet<string>
				{
					"[aurora avatar]",
					"[waist avatar]",
					"[hat avatar]",
					"[coat avatar]",
					"[face avatar]",
					"[hair avatar]",
					"[breast avatar]",
					"[pants avatar]",
					"[shoes avatar]",
					"[skin avatar]"
				};
			}
			return WZqnEKchuV;
		}
	}

	public PvfReleaseLog ReleaseLog
	{
		get
		{
			if (WXUnZTyD84 == null)
			{
				WXUnZTyD84 = new PvfReleaseLog();
			}
			return WXUnZTyD84;
		}
		set
		{
			WXUnZTyD84 = value;
		}
	}

	public HashSet<string> LstFileUseScriptFile
	{
		get
		{
			if (d3mn8rLEW9 == null)
			{
				d3mn8rLEW9 = new HashSet<string>
				{
					"n_quest/dailyrandomquest.lst",
					"n_quest/epicquest.lst",
					"n_quest/trainingquest.lst"
				};
			}
			return d3mn8rLEW9;
		}
	}

	public bool SavePvfPackTrimmableStringBinFile
	{
		get
		{
			return KV7n5XZ84w;
		}
		set
		{
			KV7n5XZ84w = value;
			DoNotify("SavePvfPackTrimmableStringBinFile");
		}
	}

	public bool SavePvfPackTrimmableStringViewFile
	{
		get
		{
			return rXunpfpmjA;
		}
		set
		{
			rXunpfpmjA = value;
			DoNotify("SavePvfPackTrimmableStringViewFile");
		}
	}

	public bool NotPromptSavePvfPackOptionsDialog
	{
		get
		{
			return hYanDBwvon;
		}
		set
		{
			hYanDBwvon = value;
			DoNotify("NotPromptSavePvfPackOptionsDialog");
		}
	}

	[JsonIgnore]
	public HashSet<string> AniSectionNames
	{
		get
		{
			if (ypUnuwGq1c == null)
			{
				ypUnuwGq1c = new HashSet<string>();
				ypUnuwGq1c.Add("[SHADOW]");
				ypUnuwGq1c.Add("[FRAME MAX]");
				ypUnuwGq1c.Add("[LOOP]");
				ypUnuwGq1c.Add("[COORD]");
				ypUnuwGq1c.Add("[SPECTRUM]");
				ypUnuwGq1c.Add("[SPECTRUM TERM]");
				ypUnuwGq1c.Add("[SPECTRUM LIFE TIME]");
				ypUnuwGq1c.Add("[SPECTRUM COLOR]");
				ypUnuwGq1c.Add("[SPECTRUM EFFECT]");
				ypUnuwGq1c.Add("[OPERATION]");
				ypUnuwGq1c.Add("[IMAGE]");
				ypUnuwGq1c.Add("[IMAGE POS]");
				ypUnuwGq1c.Add("[INTERPOLATION]");
				ypUnuwGq1c.Add("[GRAPHIC EFFECT]");
				ypUnuwGq1c.Add("[DELAY]");
				ypUnuwGq1c.Add("[IMAGE ROTATE]");
				ypUnuwGq1c.Add("[ATTACK BOX]");
				ypUnuwGq1c.Add("[IMAGE RATE]");
				ypUnuwGq1c.Add("[RGBA]");
				ypUnuwGq1c.Add("[SET FLAG]");
				ypUnuwGq1c.Add("[SHADOW]");
				ypUnuwGq1c.Add("[DAMAGE TYPE]");
				ypUnuwGq1c.Add("[DAMAGE BOX]");
				ypUnuwGq1c.Add("[LOOP START]");
				ypUnuwGq1c.Add("[LOOP END]");
				ypUnuwGq1c.Add("[PLAY SOUND]");
				ypUnuwGq1c.Add("[FLIP TYPE]");
				ypUnuwGq1c.Add("[CLIP]");
				ypUnuwGq1c.Add("[PRELOAD]");
			}
			return ypUnuwGq1c;
		}
	}

	[JsonIgnore]
	public HashSet<string> AniFRAMEHas
	{
		get
		{
			if (ML0n3ec5ie == null)
			{
				ypUnuwGq1c = new HashSet<string>();
				ypUnuwGq1c.Add("[IMAGE]");
				ypUnuwGq1c.Add("[IMAGE POS]");
				ypUnuwGq1c.Add("[INTERPOLATION]");
				ypUnuwGq1c.Add("[GRAPHIC EFFECT]");
				ypUnuwGq1c.Add("[DELAY]");
				ypUnuwGq1c.Add("[IMAGE ROTATE]");
				ypUnuwGq1c.Add("[ATTACK BOX]");
				ypUnuwGq1c.Add("[IMAGE RATE]");
				ypUnuwGq1c.Add("[RGBA]");
				ypUnuwGq1c.Add("[SET FLAG]");
				ypUnuwGq1c.Add("[SHADOW]");
				ypUnuwGq1c.Add("[DAMAGE TYPE]");
				ypUnuwGq1c.Add("[DAMAGE BOX]");
				ypUnuwGq1c.Add("[LOOP START]");
				ypUnuwGq1c.Add("[LOOP END]");
				ypUnuwGq1c.Add("[PLAY SOUND]");
				ypUnuwGq1c.Add("[FLIP TYPE]");
				ypUnuwGq1c.Add("[CLIP]");
				ypUnuwGq1c.Add("[PRELOAD]");
			}
			return ML0n3ec5ie;
		}
	}

	public Dictionary<string, PvfFileType> LstExtensions
	{
		get
		{
			if (i0VnHk3b7E == null)
			{
				i0VnHk3b7E = new Dictionary<string, PvfFileType>();
				i0VnHk3b7E.Add("town/town.lst", PvfFileType.twn);
				i0VnHk3b7E.Add("aura/aura.lst", PvfFileType.ora);
				i0VnHk3b7E.Add("region/region.lst", PvfFileType.rgn);
				i0VnHk3b7E.Add("stagemap/stagemap.lst", PvfFileType.stm);
				i0VnHk3b7E.Add("worldmap/worldmap.lst", PvfFileType.wdm);
				i0VnHk3b7E.Add("appendage/appendage.lst", PvfFileType.apd);
				i0VnHk3b7E.Add("character/character.lst", PvfFileType.chr);
				i0VnHk3b7E.Add("equipment/equipment.lst", PvfFileType.equ);
				i0VnHk3b7E.Add("pet/pet.lst", PvfFileType.pet);
				i0VnHk3b7E.Add("stackable/stackable.lst", PvfFileType.stk);
				i0VnHk3b7E.Add("aicharacter/aicharacter.lst", PvfFileType.aic);
				i0VnHk3b7E.Add("dungeon/dungeon.lst", PvfFileType.dgn);
				i0VnHk3b7E.Add("chatemoticon/chatemoticon.lst", PvfFileType.emo);
				i0VnHk3b7E.Add("monster/monster.lst", PvfFileType.mob);
				i0VnHk3b7E.Add("creature/creature.lst", PvfFileType.cre);
				i0VnHk3b7E.Add("cashshop/cashshop.lst", PvfFileType.shp);
				i0VnHk3b7E.Add("map/map.lst", PvfFileType.map);
				i0VnHk3b7E.Add("npc/npc.lst", PvfFileType.npc);
				i0VnHk3b7E.Add("itemshop/itemshop.lst", PvfFileType.shp);
				i0VnHk3b7E.Add("passiveobject/passiveobject.lst", PvfFileType.obj);
				i0VnHk3b7E.Add("n_quest/quest.lst", PvfFileType.qst);
				i0VnHk3b7E.Add("pvp_mission/mission.lst", PvfFileType.msn);
				i0VnHk3b7E.Add("etc/independentdrop.lst", PvfFileType.etc);
				i0VnHk3b7E.Add("skill/swordmanskill.lst", PvfFileType.skl);
				i0VnHk3b7E.Add("skill/fighterskill.lst", PvfFileType.skl);
				i0VnHk3b7E.Add("skill/gunnerskill.lst", PvfFileType.skl);
				i0VnHk3b7E.Add("skill/mageskill.lst", PvfFileType.skl);
				i0VnHk3b7E.Add("skill/priestskill.lst", PvfFileType.skl);
				i0VnHk3b7E.Add("skill/atgunnerskill.lst", PvfFileType.skl);
				i0VnHk3b7E.Add("skill/thiefskill.lst", PvfFileType.skl);
				i0VnHk3b7E.Add("skill/atfighterskill.lst", PvfFileType.skl);
				i0VnHk3b7E.Add("skill/atmageskill.lst", PvfFileType.skl);
				i0VnHk3b7E.Add("skill/demonicswordman.lst", PvfFileType.skl);
				i0VnHk3b7E.Add("skill/creatormage.lst", PvfFileType.skl);
			}
			return i0VnHk3b7E;
		}
	}

	public AutoTheBackupPvfOptions AutoTheBackupPvfConfig
	{
		get
		{
			if (RBcn7WPrcH == null)
			{
				RBcn7WPrcH = new AutoTheBackupPvfOptions();
			}
			return RBcn7WPrcH;
		}
		set
		{
			RBcn7WPrcH = value;
		}
	}

	public EncodingType DefaultEncoding
	{
		get
		{
			if (!gFQncEUXri.HasValue)
			{
				gFQncEUXri = EncodingType.TW;
			}
			return gFQncEUXri.Value;
		}
		set
		{
			gFQncEUXri = value;
		}
	}

	public bool FileTextTraditionalConvertSimplified
	{
		get
		{
			if (!pGAngjIAsJ.HasValue)
			{
				pGAngjIAsJ = true;
			}
			return pGAngjIAsJ.Value;
		}
		set
		{
			pGAngjIAsJ = value;
			DoNotify("FileTextTraditionalConvertSimplified");
		}
	}

	[JsonIgnore]
	public List<EncodingType> PvfEncodingList
	{
		get
		{
			if (BtOnK50TxJ == null)
			{
				BtOnK50TxJ = new List<EncodingType>();
				foreach (EnumberEntity item in EnumberHelper.EnumToList<EncodingType>())
				{
					BtOnK50TxJ.Add((EncodingType)item.EnumValue);
				}
			}
			return BtOnK50TxJ;
		}
	}

	[JsonIgnore]
	public bool UseCompatibleDecompiler
	{
		[CompilerGenerated]
		get
		{
			return WLGnYpIL8V;
		}
		[CompilerGenerated]
		set
		{
			WLGnYpIL8V = value;
		}
	}

	public bool AutoConvertStringLink
	{
		[CompilerGenerated]
		get
		{
			return KVSnJ7WoJg;
		}
		[CompilerGenerated]
		set
		{
			KVSnJ7WoJg = value;
		}
	}

	public bool EditorConvertTraditionalChinese
	{
		get
		{
			if (!PevndeOBwg.HasValue)
			{
				PevndeOBwg = true;
			}
			return PevndeOBwg.Value;
		}
		set
		{
			PevndeOBwg = value;
			DoNotify("EditorConvertTraditionalChinese");
		}
	}

	[JsonIgnore]
	public Dictionary<string, PvfFileType> PvfFileTypeDic
	{
		get
		{
			if (x8vn1AQ8FU == null)
			{
				x8vn1AQ8FU = new Dictionary<string, PvfFileType>();
				foreach (EnumberEntity item in EnumberHelper.EnumToList<PvfFileType>())
				{
					x8vn1AQ8FU.Add("." + item.EnumName.ToLower(), (PvfFileType)item.EnumValue);
				}
			}
			return x8vn1AQ8FU;
		}
	}

	public Dictionary<string, TreelistCommentRes> TreelistCommentDic
	{
		get
		{
			if (nXYnGTgbyK == null)
			{
				nXYnGTgbyK = new Dictionary<string, TreelistCommentRes>();
			}
			return nXYnGTgbyK;
		}
		set
		{
			nXYnGTgbyK = value;
		}
	}

	[JsonIgnore]
	public HashSet<PvfFileType> FileTypeHasSet
	{
		get
		{
			if (IWSnOKuYmC == null)
			{
				IWSnOKuYmC = new HashSet<PvfFileType>();
				foreach (EnumberEntity item in EnumberHelper.EnumToList<PvfFileType>())
				{
					if (item.EnumName != "未知")
					{
						IWSnOKuYmC.Add((PvfFileType)item.EnumValue);
					}
				}
			}
			return IWSnOKuYmC;
		}
	}

	[JsonIgnore]
	public HashSet<string> FileTypes
	{
		get
		{
			if (vdKnrW2u4x == null)
			{
				vdKnrW2u4x = new HashSet<string>();
				foreach (EnumberEntity item in EnumberHelper.EnumToList<PvfFileType>())
				{
					vdKnrW2u4x.Add("." + item.EnumName);
				}
			}
			return vdKnrW2u4x;
		}
	}

	public ExtractConfig ExtractConfig
	{
		get
		{
			if (NwAnecBfTM == null)
			{
				NwAnecBfTM = new ExtractConfig();
			}
			return NwAnecBfTM;
		}
		set
		{
			NwAnecBfTM = value;
		}
	}

	public ImportConfig ImportConfig
	{
		get
		{
			if (Sm2njbnj24 == null)
			{
				Sm2njbnj24 = new ImportConfig();
			}
			return Sm2njbnj24;
		}
		set
		{
			Sm2njbnj24 = value;
		}
	}

	public PvfCommentPriority PvfCommentPriority
	{
		get
		{
			if (!tpenxpJM61.HasValue)
			{
				tpenxpJM61 = PvfCommentPriority.云端;
			}
			return tpenxpJM61.Value;
		}
		set
		{
			tpenxpJM61 = value;
			DoNotify("PvfCommentPriority");
		}
	}

	[JsonIgnore]
	public List<PvfCommentPriority> PvfCommentPriorityList => new List<PvfCommentPriority>
	{
		PvfCommentPriority.云端,
		PvfCommentPriority.本地
	};

	public bool SavePvfLoadingDisableMainWindow
	{
		get
		{
			return kelnmn3VZ9;
		}
		set
		{
			kelnmn3VZ9 = value;
			DoNotify("SavePvfLoadingDisableMainWindow");
		}
	}

	public bool PvfSaveWhenTheErrorBackUp
	{
		get
		{
			if (!FGEnSyvUHm.HasValue)
			{
				FGEnSyvUHm = true;
			}
			return FGEnSyvUHm.Value;
		}
		set
		{
			FGEnSyvUHm = value;
			DoNotify("PvfSaveWhenTheErrorBackUp");
		}
	}

	public bool SavePvfPackShowDialog
	{
		get
		{
			return BFWnB2giU5;
		}
		set
		{
			BFWnB2giU5 = value;
			DoNotify("SavePvfPackShowDialog");
		}
	}

	public PvfOptions()
	{
		StringLstFileName = "n_string.lst";
		AutoConvertStringLink = true;
	}

	public string StrTableAndStrViewConvertStrContent(string str)
	{
		switch (DefaultEncoding)
		{
		case EncodingType.TW:
			return ChineseHelper.ToTraditional(str);
		case EncodingType.CN:
			if (AppSetting.Instance.PvfConfig.FileTextTraditionalConvertSimplified)
			{
				return ChineseHelper.ToSimplified(str);
			}
			return str;
		default:
			return str;
		}
	}

	public string FileTextTraditionalConvertSimplifiedAutoMethods(string str)
	{
		if (DefaultEncoding != EncodingType.TW || !FileTextTraditionalConvertSimplified)
		{
			return str;
		}
		return ChineseHelper.ToSimplified(str);
	}

	public PvfFileType GetPvfFileType(string ex)
	{
		if (AppSetting.Instance.PvfConfig.PvfFileTypeDic.TryGetValue(ex, out var value))
		{
			return value;
		}
		return PvfFileType.未知;
	}
}
