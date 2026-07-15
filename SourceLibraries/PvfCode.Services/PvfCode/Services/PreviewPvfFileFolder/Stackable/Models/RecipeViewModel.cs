using System.Collections.Generic;
using PvfCode.Dot;

namespace PvfCode.Services.PreviewPvfFileFolder.Stackable.Models;

public class RecipeViewModel
{
	public List<RecipeItem> Items { get; set; }

	public RecipeItem ResultItem { get; set; }

	public RecipeSubJob? SubJob { get; set; }

	public bool HasSubJob { get; set; }

	public RecipeSubJobType SubJobType { get; set; }

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
