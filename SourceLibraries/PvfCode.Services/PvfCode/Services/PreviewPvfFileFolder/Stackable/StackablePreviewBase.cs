using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Media;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.LoggerBase;
using PvfCode.Models.Pvf.Enums.Stackable;
using PvfCode.Services.PreviewPvfFileFolder.Stackable.Models;

namespace PvfCode.Services.PreviewPvfFileFolder.Stackable;

public class StackablePreviewBase : StackableOrEquPreviewBase
{
	private ServiceStackable HNbjNjnb1h;

	[CompilerGenerated]
	private RecipeViewModel? tg8jJsJBMV;

	public StackableType? StackableType
	{
		get
		{
			if (!base.File.GetStackableType(base.Pvf, out var type))
			{
				return null;
			}
			return type;
		}
	}

	public string? StackableTypeToGameText
	{
		get
		{
			if (!StackableType.HasValue)
			{
				return null;
			}
			switch (StackableType.Value)
			{
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.材料_炼金控偶_0:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.材料_附魔卡片_1:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.材料_无特殊_0:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.材料_无特殊_1:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.材料_无特殊_2:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.材料_无特殊_3:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.材料_无特殊_4:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.材料_无特殊_5:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.材料_无特殊_7:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.材料_无特殊_8:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.材料_无特殊_10:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.材料_无特殊_11:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.材料_自动售卖机使用券_0:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.材料_自动售卖机使用券_1:
				return "材料";
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.徽章_镶嵌使用_0:
				return "装扮徽章";
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.任务品_都是任务品无特殊作用_0:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.任务品_都是任务品无特殊作用_2:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.任务品_都是任务品无特殊作用_3:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.任务品_都是任务品无特殊作用_5:
				return "任务";
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.设计图_杂七杂八_0:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.设计图_装备类_1:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.设计图_药剂类_2:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.设计图_装备类_5:
				return "设计图";
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_随机盒子_0:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_可选盒子_0:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_礼包_0:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_礼包_1:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_点券盒子_0:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_点券盒子_1:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_点卷礼包_0:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_契约包_0:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_宠物经验道具_0:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_宠物盒子_1:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_cosplay道具_0:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_时装染色剂_0:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_附魔宝珠_0:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_其他_0:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_其他_1:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_其他_8:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_疲劳药_0:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_宠物饲料_1:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_全局效果_0:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_罐子_1:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_罐子_5:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_罐子_7:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_罐子_10:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_罐子_11:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_罐子_12:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_罐子_13:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_罐子_14:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_罐子_15:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_罐子_16:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_罐子_17:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_罐子_18:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_罐子_19:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_罐子_20:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_罐子_21:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_罐子_24:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_点券魔锤魔盒_0:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_魔锤魔盒_0:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_城镇烟花_0:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_使用后可接受任务_0:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_布置地雷_1:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_布置投掷品_3:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_瞬间移动药剂_0:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_投掷品_0:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_投掷品_1:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_投掷品_2:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_投掷品_3:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_分解机_强化器等_0:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_洗点水_0:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_副职业提取器_0:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_不会消耗的道具_0:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_不会消耗的道具_1:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_罐子_0:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_NPC罐子_1:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_升级装备用的_0:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_可选属性的时装礼包_0:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_药剂等_0:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_药剂等_1:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_药剂等_3:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_药剂等_4:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_药剂等_5:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_药剂等_6:
			case PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_药剂等_10:
				return "消耗品";
			default:
				return null;
			}
		}
	}

	public string? StackLimit
	{
		get
		{
			if (HNbjNjnb1h.ScriptParserNew.GetSectionValue(base.Pvf, "[stack limit]", HNbjNjnb1h.ScriptParserNew.Sections, out string val))
			{
				return "携带上限：" + val + "个";
			}
			return null;
		}
	}

	public BoosterInfo? BoosterInfo
	{
		get
		{
			return GetProperty(() => BoosterInfo);
		}
		set
		{
			SetProperty<BoosterInfo>(() => BoosterInfo, value);
		}
	}

	public RecipeViewModel? RecipeViewModel
	{
		[CompilerGenerated]
		get
		{
			return tg8jJsJBMV;
		}
		[CompilerGenerated]
		set
		{
			tg8jJsJBMV = value;
		}
	}

	public EnchantCardInfo? EnchantCardInfo
	{
		get
		{
			return GetProperty(() => EnchantCardInfo);
		}
		set
		{
			SetProperty<EnchantCardInfo>(() => EnchantCardInfo, value);
		}
	}

	public EnchantWasteInfo? EnchantWasteInfo
	{
		get
		{
			return GetProperty(() => EnchantWasteInfo);
		}
		set
		{
			SetProperty<EnchantWasteInfo>(() => EnchantWasteInfo, value);
		}
	}

	public Usable_cera_package? Usable_cera_package
	{
		get
		{
			return GetProperty(() => Usable_cera_package);
		}
		set
		{
			SetProperty<Usable_cera_package>(() => Usable_cera_package, value);
		}
	}

	public override void InitStackable()
	{
		HNbjNjnb1h = new ServiceStackable(base.Pvf, base.File);
		BoosterInfo = Ju5jFaUF9W();
		RuAjD8rmcV();
		tHejmnQw8Z();
		Debj4ZtnI3();
		KOTjQ4es8b();
	}

	private BoosterInfo? Ju5jFaUF9W()
	{
		if (!StackableType.HasValue)
		{
			return null;
		}
		StackableType value = StackableType.Value;
		if (value == PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_可选盒子_0)
		{
			return HNbjNjnb1h.GetBoosterSelectionInfo(out BoosterInfo? selectionInfo) ? selectionInfo : null;
		}
		if ((uint)(value - 25) > 3u)
		{
			return null;
		}
		if (HNbjNjnb1h.GetBoosterInfo(out BoosterInfo boosterInfo))
		{
			return boosterInfo;
		}
		return null;
	}

	private void RuAjD8rmcV()
	{
		if (StackableType.HasValue)
		{
			StackableType value = StackableType.Value;
			if ((uint)(value - 19) <= 3u && HNbjNjnb1h.GetRecipe(out RecipeViewModel recipe))
			{
				RecipeViewModel = recipe;
			}
		}
	}

	private void tHejmnQw8Z()
	{
		if (StackableType == PvfCode.Models.Pvf.Enums.Stackable.StackableType.材料_附魔卡片_1 && HNbjNjnb1h.GetEnchantCardInfo(out EnchantCardInfo card))
		{
			EnchantCardInfo = card;
		}
	}

	private void Debj4ZtnI3()
	{
		if (StackableType == PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_附魔宝珠_0)
		{
			EnchantWasteInfo = new EnchantWasteInfo(base.File, base.Pvf);
		}
	}

	private void KOTjQ4es8b()
	{
		if (StackableType.HasValue)
		{
			StackableType value = StackableType.Value;
			if ((value == PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_点卷礼包_0 || value == PvfCode.Models.Pvf.Enums.Stackable.StackableType.消耗品_可选属性的时装礼包_0) && HNbjNjnb1h.GetUsable_cera_package(out Usable_cera_package usable_cera_package))
			{
				Usable_cera_package = usable_cera_package;
			}
		}
	}

	public StackablePreviewBase(PvfGroup pvf, PvfFile file, ImageSource? imageSource = null)
		: base(pvf, file, imageSource)
	{
	}

	[Command]
	public void OnAddRecipeAllFilesToSearchPanel()
	{
		if (RecipeViewModel == null)
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		List<string> list = new List<string>();
		foreach (RecipeItem item in RecipeViewModel.Items)
		{
			PvfFile file = item.GetFile();
			if (file == null)
			{
				StringBuilder stringBuilder2 = stringBuilder;
				StringBuilder stringBuilder3 = stringBuilder2;
				StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(12, 1, stringBuilder2);
				handler.AppendLiteral("找不到对应的文件 代码：");
				handler.AppendFormatted(file.ItemCode);
				stringBuilder3.AppendLine(ref handler);
			}
			else
			{
				list.Add(file.FileName);
			}
		}
		if (RecipeViewModel.ResultItem != null)
		{
			PvfFile file2 = RecipeViewModel.ResultItem.GetFile();
			if (file2 == null)
			{
				StringBuilder stringBuilder2 = stringBuilder;
				StringBuilder stringBuilder4 = stringBuilder2;
				StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(12, 1, stringBuilder2);
				handler.AppendLiteral("找不到对应的文件 代码：");
				handler.AppendFormatted(RecipeViewModel.ResultItem.ItemCode);
				stringBuilder4.AppendLine(ref handler);
			}
			else
			{
				list.Add(file2.FileName);
			}
		}
		if (base.File != null)
		{
			list.Add(base.File.FileName);
		}
		Ilogger ilogger = AppSetting.Instance.GetIlogger();
		if (stringBuilder.Length > 0)
		{
			ilogger.Error(stringBuilder.ToString());
		}
		if (list.Count > 0)
		{
			ilogger.AddFileListToNewSearchPanel(list, base.ItemName);
		}
	}
}
