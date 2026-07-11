using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.Models.Pvf;
using PvfCode.Models.Pvf.Enums;
using PvfCode.Services.PvfParsingNew;
using Utools;

namespace PvfCode.Services.PreviewPvfFileFolder;

public class StackableOrEquPreviewBase : FilePreviewDataBase
{
	private ScriptFileParserNew BkSjEEqjUg;

	public ScriptFileParserNew ScriptFileParser
	{
		get
		{
			if (BkSjEEqjUg == null)
			{
				BkSjEEqjUg = new ScriptFileParserNew(base.File, base.Pvf);
				BkSjEEqjUg.PraseStructureMain();
			}
			return BkSjEEqjUg;
		}
	}

	public string ItemName
	{
		get
		{
			InitStackable();
			InitEquGroup();
			string itemName = base.Pvf.GetItemName(base.File);
			if (!string.IsNullOrEmpty(itemName))
			{
				return itemName.Replace("\\n", "\r\n");
			}
			return "未设定Name";
		}
	}

	public string ItemCode => base.Pvf.GetItemCode(base.File)?.ToString();

	public string? Name2
	{
		get
		{
			if (!base.File.GetName(base.Pvf, Name_Type.name2, out string name))
			{
				return string.Empty;
			}
			return name?.Replace("\\n", "\r\n");
		}
	}

	public string? Basic_explain
	{
		get
		{
			if (base.File.GetName(base.Pvf, Name_Type.basic_explain, out string name) && !string.IsNullOrEmpty(name))
			{
				return name?.Replace("\\n", "\r\n");
			}
			return null;
		}
	}

	public string? Detail_Explain
	{
		get
		{
			if (base.File.GetName(base.Pvf, Name_Type.Detail_Explain, out string name) && !string.IsNullOrEmpty(name))
			{
				return name.Replace("\\n", "\r\n");
			}
			return null;
		}
	}

	public string? explain
	{
		get
		{
			if (base.File.GetName(base.Pvf, Name_Type.explain, out string name) && !string.IsNullOrEmpty(name))
			{
				return name?.Replace("\\n", "\r\n");
			}
			return null;
		}
	}

	public string? Flavor_text
	{
		get
		{
			if (!base.File.GetName(base.Pvf, Name_Type.flavor_text, out string name))
			{
				return null;
			}
			return name?.Replace("\\n", "\r\n");
		}
	}

	public string? UsableJobStr
	{
		get
		{
			if (base.File.GetUsableJob(base.Pvf, out List<JobType> jobs) && jobs != null && jobs.Count > 0)
			{
				return "可用职业：" + string.Join(",", jobs);
			}
			return null;
		}
	}

	public RarityType? Rarity
	{
		get
		{
			base.File.GetRarity((PvfPack)base.Pvf, out RarityType? rarityType);
			return rarityType;
		}
	}

	public string Weight
	{
		get
		{
			if (base.File.GetItemWeight(base.Pvf, out string weight))
			{
				if (float.TryParse(weight, out var result))
				{
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(2, 1);
					defaultInterpolatedStringHandler.AppendFormatted(result / 1000f);
					defaultInterpolatedStringHandler.AppendLiteral("kg");
					return defaultInterpolatedStringHandler.ToStringAndClear();
				}
				if (int.TryParse(weight, out var result2))
				{
					return ((float)result2 / 1000f).ToString().ToFloat2() + "kg";
				}
			}
			return null;
		}
	}

	public string SalePrice
	{
		get
		{
			if (base.File.GetValuePrice(base.Pvf, out string val))
			{
				return ValueX(val, 0.2) + "金币";
			}
			if (base.File.GetPrice(base.Pvf, out string val2))
			{
				return ValueX(val2, 0.2) + "金币";
			}
			return string.Empty;
		}
	}

	public string AttachType
	{
		get
		{
			if (base.File.GetAttachType(base.Pvf, out var attachType))
			{
				return base.File.ConvertAttachTypeToString(attachType.Value);
			}
			return null;
		}
	}

	public AttachType? AttachTypeEnum
	{
		get
		{
			if (base.File.GetAttachType(base.Pvf, out var attachType))
			{
				return attachType;
			}
			return null;
		}
	}

	public string? MiniNumLevel
	{
		get
		{
			if (base.File.GetMiniNumLevel(base.Pvf, out string val))
			{
				return "lv " + val + "以上可以使用";
			}
			return null;
		}
	}

	public bool ShowDetail_Explain
	{
		get
		{
			return GetProperty(() => ShowDetail_Explain);
		}
		set
		{
			SetProperty(() => ShowDetail_Explain, value);
		}
	}

	public string NeedMaterial
	{
		get
		{
			if (base.File.GetSectionIntValue2("[need material]", base.Pvf, out KeyValuePair<string, string>? val) && !string.IsNullOrEmpty(val.Value.Key) && int.TryParse(val.Value.Key, out var result))
			{
				string text = base.Pvf.ListFileTable.ItemCodeConvertFilePath(new string[2]
				{
					"equipment",
					"stackable"
				}, result);
				if (!string.IsNullOrEmpty(text))
				{
					string itemName = base.Pvf.GetItemName(text);
					if (!string.IsNullOrEmpty(itemName))
					{
						DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(3, 2);
						defaultInterpolatedStringHandler.AppendLiteral("[");
						defaultInterpolatedStringHandler.AppendFormatted(itemName);
						defaultInterpolatedStringHandler.AppendLiteral("]");
						defaultInterpolatedStringHandler.AppendFormatted(val.Value.Value);
						defaultInterpolatedStringHandler.AppendLiteral("个");
						return defaultInterpolatedStringHandler.ToStringAndClear();
					}
				}
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(3, 2);
				defaultInterpolatedStringHandler2.AppendLiteral("[");
				defaultInterpolatedStringHandler2.AppendFormatted(result);
				defaultInterpolatedStringHandler2.AppendLiteral("]");
				defaultInterpolatedStringHandler2.AppendFormatted(val.Value.Value);
				defaultInterpolatedStringHandler2.AppendLiteral("个");
				return defaultInterpolatedStringHandler2.ToStringAndClear();
			}
			return null;
		}
	}

	public StackableOrEquPreviewBase(PvfGroup pvf, PvfFile file, ImageSource? imageSource = null)
		: base(pvf, file, imageSource)
	{
	}

	public virtual void InitEquGroup()
	{
	}

	public virtual void InitStackable()
	{
	}

	[Command]
	public void OnGoToNeedMaterialFile()
	{
		if (base.File.GetSectionIntValue2("[need material]", base.Pvf, out KeyValuePair<string, string>? val) && !string.IsNullOrEmpty(val.Value.Key) && int.TryParse(val.Value.Key, out var result))
		{
			string text = base.Pvf.ListFileTable.ItemCodeConvertFilePath(new string[2]
			{
				"equipment",
				"stackable"
			}, result);
			if (!string.IsNullOrEmpty(text))
			{
				AppSetting.Instance.GetIlogger()?.OpenPvfFileDocument(text);
			}
		}
	}

	[Command]
	public void OnSwitchExplain()
	{
		if (!ShowDetail_Explain)
		{
			if (!string.IsNullOrEmpty(Basic_explain) && !string.IsNullOrEmpty(Detail_Explain))
			{
				ShowDetail_Explain = true;
			}
		}
		else
		{
			ShowDetail_Explain = false;
		}
	}
}
