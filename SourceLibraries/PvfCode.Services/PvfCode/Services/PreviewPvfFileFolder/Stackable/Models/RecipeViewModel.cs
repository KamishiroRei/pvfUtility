using System.Collections.Generic;
using System.Runtime.CompilerServices;
using PvfCode.Dot;

namespace PvfCode.Services.PreviewPvfFileFolder.Stackable.Models;

public class RecipeViewModel
{
	[CompilerGenerated]
	private List<RecipeItem> FUrjVBr5Vb;

	[CompilerGenerated]
	private RecipeItem fiVjkRkDQV;

	[CompilerGenerated]
	private RecipeSubJob? RrGjf4OZDu;

	[CompilerGenerated]
	private bool Wmqj71NhW0;

	[CompilerGenerated]
	private RecipeSubJobType zGtjTHe7gC;

	public List<RecipeItem> Items
	{
		[CompilerGenerated]
		get
		{
			return FUrjVBr5Vb;
		}
		[CompilerGenerated]
		set
		{
			FUrjVBr5Vb = value;
		}
	}

	public RecipeItem ResultItem
	{
		[CompilerGenerated]
		get
		{
			return fiVjkRkDQV;
		}
		[CompilerGenerated]
		set
		{
			fiVjkRkDQV = value;
		}
	}

	public RecipeSubJob? SubJob
	{
		[CompilerGenerated]
		get
		{
			return RrGjf4OZDu;
		}
		[CompilerGenerated]
		set
		{
			RrGjf4OZDu = value;
		}
	}

	public bool HasSubJob
	{
		[CompilerGenerated]
		get
		{
			return Wmqj71NhW0;
		}
		[CompilerGenerated]
		set
		{
			Wmqj71NhW0 = value;
		}
	}

	public RecipeSubJobType SubJobType
	{
		[CompilerGenerated]
		get
		{
			return zGtjTHe7gC;
		}
		[CompilerGenerated]
		set
		{
			zGtjTHe7gC = value;
		}
	}

	public static ResultData<RecipeViewModel> Create(List<int> datas, PvfGroup pvf)
	{
		ResultData<RecipeViewModel> resultData = new ResultData<RecipeViewModel>();
		if (datas.Count == 0)
		{
			resultData.Msg = "设计图所需物品 数据错误 长度不正确 0";
			return resultData;
		}
		int num = 0;
		RecipeViewModel recipeViewModel = new RecipeViewModel();
		int num2 = datas[num];
		if (num2 * 2 + 1 > datas.Count)
		{
			resultData.Msg = "设计图所需物品 数据错误 长度不正确";
			return resultData;
		}
		recipeViewModel.Items = new List<RecipeItem>();
		num += num2 * 2;
		for (int i = 1; i < num2 * 2; i += 2)
		{
			recipeViewModel.Items.Add(new RecipeItem(datas[i], datas[i + 1], pvf));
		}
		num++;
		if (num2 * 2 + 2 > datas.Count)
		{
			resultData.Msg = "数据错误 设计结果物品 长度不正确";
			return resultData;
		}
		int num3 = datas[num];
		if (num2 * 2 + 2 + num3 + 2 > datas.Count)
		{
			resultData.Msg = "设计图设计结果物品 数据错误 长度不正确";
			return resultData;
		}
		num++;
		recipeViewModel.ResultItem = new RecipeItem(datas[num], datas[num + 1], pvf);
		num += 2;
		recipeViewModel.HasSubJob = datas[num] == 1;
		if (recipeViewModel.HasSubJob)
		{
			if (num > datas.Count)
			{
				resultData.Msg = "设计图副职业信息 数据错误 长度不正确";
				return resultData;
			}
			recipeViewModel.SubJob = new RecipeSubJob
			{
				SubJobSkillCode = datas[num + 1],
				SubJobLevel = datas[num + 2]
			};
			num += 3;
			if (num > datas.Count)
			{
				resultData.Msg = "设计图副职业信息 数据错误 长度不正确2";
				return resultData;
			}
			recipeViewModel.SubJobType = (RecipeSubJobType)datas[num];
		}
		resultData.Data = recipeViewModel;
		return resultData;
	}

	public RecipeViewModel()
	{
	}
}
