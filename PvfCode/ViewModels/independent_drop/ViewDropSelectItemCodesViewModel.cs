using System.Collections.Generic;
using System.Linq;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.ViewModels.SearchPvf.SearchName;
using PvfCode.ViewModels.independent_drop.DropList;
using PvfCodeViewModels.SearchPvf.SearchName;

namespace PvfCode.ViewModels.independent_drop;

public class ViewDropSelectItemCodesViewModel : SearchNameViewModel<ListItemSelect>
{
	public int DropWeight
	{
		get
		{
			return GetProperty(() => DropWeight);
		}
		set
		{
			SetProperty(() => DropWeight, value);
		}
	}

	public ViewDropSelectItemCodesViewModel()
	{
		DropWeight = 1000;
		base.ShowGroupPanel = true;
		base.Type = SearchNameViewModelType.ItemCodeSelectTool;
		InitLstName(new List<string>
		{
			"equipment",
			"stackable"
		}, initItems: true);
	}

	[Command]
	public override void OnAddRightGroup()
	{
		if (base.SelectedItems.Count <= 0)
		{
			return;
		}
		if (base.AddGroupCheckRepeat)
		{
			List<ListItemSelect> list = new List<ListItemSelect>();
			HashSet<string> hashSet = base.GroupItems.Select((ListItemSelect it) => it.FilePath).ToHashSet();
			ListItemSelect[] array = base.SelectedItems.ToArray();
			foreach (ListItemSelect listItemSelect in array)
			{
				if (!hashSet.Contains(listItemSelect.FilePath))
				{
					listItemSelect.DropWeight = DropWeight;
					list.Add(listItemSelect);
				}
			}
			base.GroupItems.AddRange(list);
		}
		else
		{
			ListItemSelect[] array = base.SelectedItems.ToArray();
			for (int num = 0; num < array.Length; num++)
			{
				array[num].DropWeight = DropWeight;
			}
			base.GroupItems.AddRange(base.SelectedItems);
		}
		UpdateGroupItemsCount();
	}

	[Command]
	public override void OnRemoveDuplicate()
	{
		if (base.GroupItems == null || !base.GroupItems.Any())
		{
			return;
		}
		HashSet<string> hashSet = new HashSet<string>();
		List<ListItemSelect> list = new List<ListItemSelect>();
		foreach (ListItemSelect groupItem in base.GroupItems)
		{
			if (!hashSet.Contains(groupItem.FilePath))
			{
				hashSet.Add(groupItem.FilePath);
				list.Add(groupItem);
			}
		}
		base.GroupItems.Clear();
		base.GroupItems.AddRange(list);
		UpdateGroupItemsCount();
	}
}
