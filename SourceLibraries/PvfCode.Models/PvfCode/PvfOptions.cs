using System.Collections.Generic;
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
	private HashSet<string> avatarParts;

	private PvfReleaseLog releaseLog;

	private HashSet<string> lstFileUseScriptFile;

	private HashSet<string> aniSectionNames;

	private bool savePvfPackTrimmableStringBinFile;

	private bool savePvfPackTrimmableStringViewFile;

	private bool notPromptSavePvfPackOptionsDialog;

	private HashSet<string> aniFrameSections;

	private Dictionary<string, PvfFileType> lstExtensions;

	private AutoTheBackupPvfOptions autoBackupConfig;

	public readonly string StringLstFileName;

	private EncodingType? defaultEncoding;

	private bool? fileTextTraditionalConvertSimplified;

	private List<EncodingType> pvfEncodingList;

	private bool? editorConvertTraditionalChinese;

	private Dictionary<string, PvfFileType> pvfFileTypeDic;

	private Dictionary<string, TreelistCommentRes> treelistCommentDic;

	private HashSet<PvfFileType> fileTypeSet;

	private HashSet<string> fileTypes;

	private ExtractConfig extractConfig;

	private ImportConfig importConfig;

	private PvfCommentPriority? pvfCommentPriority;

	private bool savePvfLoadingDisableMainWindow;

	private bool? pvfSaveWhenTheErrorBackUp;

	private bool savePvfPackShowDialog;

	[JsonIgnore]
	public HashSet<string> AvatarParts
	{
		get
		{
			if (avatarParts == null)
			{
				avatarParts = new HashSet<string>
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
			return avatarParts;
		}
	}

	public PvfReleaseLog ReleaseLog
	{
		get
		{
			if (releaseLog == null)
			{
				releaseLog = new PvfReleaseLog();
			}
			return releaseLog;
		}
		set
		{
			releaseLog = value;
		}
	}

	public HashSet<string> LstFileUseScriptFile
	{
		get
		{
			if (lstFileUseScriptFile == null)
			{
				lstFileUseScriptFile = new HashSet<string>
				{
					"n_quest/dailyrandomquest.lst",
					"n_quest/epicquest.lst",
					"n_quest/trainingquest.lst"
				};
			}
			return lstFileUseScriptFile;
		}
	}

	public bool SavePvfPackTrimmableStringBinFile
	{
		get
		{
			return savePvfPackTrimmableStringBinFile;
		}
		set
		{
			savePvfPackTrimmableStringBinFile = value;
			DoNotify(nameof(SavePvfPackTrimmableStringBinFile));
		}
	}

	public bool SavePvfPackTrimmableStringViewFile
	{
		get
		{
			return savePvfPackTrimmableStringViewFile;
		}
		set
		{
			savePvfPackTrimmableStringViewFile = value;
			DoNotify(nameof(SavePvfPackTrimmableStringViewFile));
		}
	}

	public bool NotPromptSavePvfPackOptionsDialog
	{
		get
		{
			return notPromptSavePvfPackOptionsDialog;
		}
		set
		{
			notPromptSavePvfPackOptionsDialog = value;
			DoNotify(nameof(NotPromptSavePvfPackOptionsDialog));
		}
	}

	[JsonIgnore]
	public HashSet<string> AniSectionNames
	{
		get
		{
			if (aniSectionNames == null)
			{
				aniSectionNames = new HashSet<string>();
				aniSectionNames.Add("[SHADOW]");
				aniSectionNames.Add("[FRAME MAX]");
				aniSectionNames.Add("[LOOP]");
				aniSectionNames.Add("[COORD]");
				aniSectionNames.Add("[SPECTRUM]");
				aniSectionNames.Add("[SPECTRUM TERM]");
				aniSectionNames.Add("[SPECTRUM LIFE TIME]");
				aniSectionNames.Add("[SPECTRUM COLOR]");
				aniSectionNames.Add("[SPECTRUM EFFECT]");
				aniSectionNames.Add("[OPERATION]");
				aniSectionNames.Add("[IMAGE]");
				aniSectionNames.Add("[IMAGE POS]");
				aniSectionNames.Add("[INTERPOLATION]");
				aniSectionNames.Add("[GRAPHIC EFFECT]");
				aniSectionNames.Add("[DELAY]");
				aniSectionNames.Add("[IMAGE ROTATE]");
				aniSectionNames.Add("[ATTACK BOX]");
				aniSectionNames.Add("[IMAGE RATE]");
				aniSectionNames.Add("[RGBA]");
				aniSectionNames.Add("[SET FLAG]");
				aniSectionNames.Add("[SHADOW]");
				aniSectionNames.Add("[DAMAGE TYPE]");
				aniSectionNames.Add("[DAMAGE BOX]");
				aniSectionNames.Add("[LOOP START]");
				aniSectionNames.Add("[LOOP END]");
				aniSectionNames.Add("[PLAY SOUND]");
				aniSectionNames.Add("[FLIP TYPE]");
				aniSectionNames.Add("[CLIP]");
				aniSectionNames.Add("[PRELOAD]");
			}
			return aniSectionNames;
		}
	}

	[JsonIgnore]
	public HashSet<string> AniFRAMEHas
	{
		get
		{
			if (aniFrameSections == null)
			{
				aniFrameSections = new HashSet<string>();
				aniFrameSections.Add("[IMAGE]");
				aniFrameSections.Add("[IMAGE POS]");
				aniFrameSections.Add("[INTERPOLATION]");
				aniFrameSections.Add("[GRAPHIC EFFECT]");
				aniFrameSections.Add("[DELAY]");
				aniFrameSections.Add("[IMAGE ROTATE]");
				aniFrameSections.Add("[ATTACK BOX]");
				aniFrameSections.Add("[IMAGE RATE]");
				aniFrameSections.Add("[RGBA]");
				aniFrameSections.Add("[SET FLAG]");
				aniFrameSections.Add("[SHADOW]");
				aniFrameSections.Add("[DAMAGE TYPE]");
				aniFrameSections.Add("[DAMAGE BOX]");
				aniFrameSections.Add("[LOOP START]");
				aniFrameSections.Add("[LOOP END]");
				aniFrameSections.Add("[PLAY SOUND]");
				aniFrameSections.Add("[FLIP TYPE]");
				aniFrameSections.Add("[CLIP]");
				aniFrameSections.Add("[PRELOAD]");
			}
			return aniFrameSections;
		}
	}

	public Dictionary<string, PvfFileType> LstExtensions
	{
		get
		{
			if (lstExtensions == null)
			{
				lstExtensions = new Dictionary<string, PvfFileType>();
				lstExtensions.Add("town/town.lst", PvfFileType.twn);
				lstExtensions.Add("aura/aura.lst", PvfFileType.ora);
				lstExtensions.Add("region/region.lst", PvfFileType.rgn);
				lstExtensions.Add("stagemap/stagemap.lst", PvfFileType.stm);
				lstExtensions.Add("worldmap/worldmap.lst", PvfFileType.wdm);
				lstExtensions.Add("appendage/appendage.lst", PvfFileType.apd);
				lstExtensions.Add("character/character.lst", PvfFileType.chr);
				lstExtensions.Add("equipment/equipment.lst", PvfFileType.equ);
				lstExtensions.Add("pet/pet.lst", PvfFileType.pet);
				lstExtensions.Add("stackable/stackable.lst", PvfFileType.stk);
				lstExtensions.Add("aicharacter/aicharacter.lst", PvfFileType.aic);
				lstExtensions.Add("dungeon/dungeon.lst", PvfFileType.dgn);
				lstExtensions.Add("chatemoticon/chatemoticon.lst", PvfFileType.emo);
				lstExtensions.Add("monster/monster.lst", PvfFileType.mob);
				lstExtensions.Add("creature/creature.lst", PvfFileType.cre);
				lstExtensions.Add("cashshop/cashshop.lst", PvfFileType.shp);
				lstExtensions.Add("map/map.lst", PvfFileType.map);
				lstExtensions.Add("npc/npc.lst", PvfFileType.npc);
				lstExtensions.Add("itemshop/itemshop.lst", PvfFileType.shp);
				lstExtensions.Add("passiveobject/passiveobject.lst", PvfFileType.obj);
				lstExtensions.Add("n_quest/quest.lst", PvfFileType.qst);
				lstExtensions.Add("pvp_mission/mission.lst", PvfFileType.msn);
				lstExtensions.Add("etc/independentdrop.lst", PvfFileType.etc);
				lstExtensions.Add("skill/swordmanskill.lst", PvfFileType.skl);
				lstExtensions.Add("skill/fighterskill.lst", PvfFileType.skl);
				lstExtensions.Add("skill/gunnerskill.lst", PvfFileType.skl);
				lstExtensions.Add("skill/mageskill.lst", PvfFileType.skl);
				lstExtensions.Add("skill/priestskill.lst", PvfFileType.skl);
				lstExtensions.Add("skill/atgunnerskill.lst", PvfFileType.skl);
				lstExtensions.Add("skill/thiefskill.lst", PvfFileType.skl);
				lstExtensions.Add("skill/atfighterskill.lst", PvfFileType.skl);
				lstExtensions.Add("skill/atmageskill.lst", PvfFileType.skl);
				lstExtensions.Add("skill/demonicswordman.lst", PvfFileType.skl);
				lstExtensions.Add("skill/creatormage.lst", PvfFileType.skl);
			}
			return lstExtensions;
		}
	}

	public AutoTheBackupPvfOptions AutoTheBackupPvfConfig
	{
		get
		{
			if (autoBackupConfig == null)
			{
				autoBackupConfig = new AutoTheBackupPvfOptions();
			}
			return autoBackupConfig;
		}
		set
		{
			autoBackupConfig = value;
		}
	}

	public EncodingType DefaultEncoding
	{
		get
		{
			if (!defaultEncoding.HasValue)
			{
				defaultEncoding = EncodingType.TW;
			}
			return defaultEncoding.Value;
		}
		set
		{
			defaultEncoding = value;
		}
	}

	public bool FileTextTraditionalConvertSimplified
	{
		get
		{
			if (!fileTextTraditionalConvertSimplified.HasValue)
			{
				fileTextTraditionalConvertSimplified = true;
			}
			return fileTextTraditionalConvertSimplified.Value;
		}
		set
		{
			fileTextTraditionalConvertSimplified = value;
			DoNotify("FileTextTraditionalConvertSimplified");
		}
	}

	[JsonIgnore]
	public List<EncodingType> PvfEncodingList
	{
		get
		{
			if (pvfEncodingList == null)
			{
				pvfEncodingList = new List<EncodingType>();
				foreach (EnumberEntity item in EnumberHelper.EnumToList<EncodingType>())
				{
					pvfEncodingList.Add((EncodingType)item.EnumValue);
				}
			}
			return pvfEncodingList;
		}
	}

	[JsonIgnore]
	public bool UseCompatibleDecompiler { get; set; }

	public bool AutoConvertStringLink { get; set; }

	public bool EditorConvertTraditionalChinese
	{
		get
		{
			if (!editorConvertTraditionalChinese.HasValue)
			{
				editorConvertTraditionalChinese = true;
			}
			return editorConvertTraditionalChinese.Value;
		}
		set
		{
			editorConvertTraditionalChinese = value;
			DoNotify("EditorConvertTraditionalChinese");
		}
	}

	[JsonIgnore]
	public Dictionary<string, PvfFileType> PvfFileTypeDic
	{
		get
		{
			if (pvfFileTypeDic == null)
			{
				pvfFileTypeDic = new Dictionary<string, PvfFileType>();
				foreach (EnumberEntity item in EnumberHelper.EnumToList<PvfFileType>())
				{
					pvfFileTypeDic.Add("." + item.EnumName.ToLower(), (PvfFileType)item.EnumValue);
				}
			}
			return pvfFileTypeDic;
		}
	}

	public Dictionary<string, TreelistCommentRes> TreelistCommentDic
	{
		get
		{
			if (treelistCommentDic == null)
			{
				treelistCommentDic = new Dictionary<string, TreelistCommentRes>();
			}
			return treelistCommentDic;
		}
		set
		{
			treelistCommentDic = value;
		}
	}

	[JsonIgnore]
	public HashSet<PvfFileType> FileTypeHasSet
	{
		get
		{
			if (fileTypeSet == null)
			{
				fileTypeSet = new HashSet<PvfFileType>();
				foreach (EnumberEntity item in EnumberHelper.EnumToList<PvfFileType>())
				{
					if (item.EnumName != "未知")
					{
						fileTypeSet.Add((PvfFileType)item.EnumValue);
					}
				}
			}
			return fileTypeSet;
		}
	}

	[JsonIgnore]
	public HashSet<string> FileTypes
	{
		get
		{
			if (fileTypes == null)
			{
				fileTypes = new HashSet<string>();
				foreach (EnumberEntity item in EnumberHelper.EnumToList<PvfFileType>())
				{
					fileTypes.Add("." + item.EnumName);
				}
			}
			return fileTypes;
		}
	}

	public ExtractConfig ExtractConfig
	{
		get
		{
			if (extractConfig == null)
			{
				extractConfig = new ExtractConfig();
			}
			return extractConfig;
		}
		set
		{
			extractConfig = value;
		}
	}

	public ImportConfig ImportConfig
	{
		get
		{
			if (importConfig == null)
			{
				importConfig = new ImportConfig();
			}
			return importConfig;
		}
		set
		{
			importConfig = value;
		}
	}

	public PvfCommentPriority PvfCommentPriority
	{
		get
		{
			if (!pvfCommentPriority.HasValue)
			{
				pvfCommentPriority = PvfCommentPriority.云端;
			}
			return pvfCommentPriority.Value;
		}
		set
		{
			pvfCommentPriority = value;
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
			return savePvfLoadingDisableMainWindow;
		}
		set
		{
			savePvfLoadingDisableMainWindow = value;
			DoNotify("SavePvfLoadingDisableMainWindow");
		}
	}

	public bool PvfSaveWhenTheErrorBackUp
	{
		get
		{
			if (!pvfSaveWhenTheErrorBackUp.HasValue)
			{
				pvfSaveWhenTheErrorBackUp = true;
			}
			return pvfSaveWhenTheErrorBackUp.Value;
		}
		set
		{
			pvfSaveWhenTheErrorBackUp = value;
			DoNotify("PvfSaveWhenTheErrorBackUp");
		}
	}

	public bool SavePvfPackShowDialog
	{
		get
		{
			return savePvfPackShowDialog;
		}
		set
		{
			savePvfPackShowDialog = value;
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
