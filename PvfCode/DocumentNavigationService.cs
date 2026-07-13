using System;
using System.Collections.ObjectModel;
using System.Linq;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.ViewModels.DocumentFolder;
using TextEditLib;

namespace PvfCode;

public class DocumentNavigationService : ViewModelBase
{
	private static DocumentNavigationService instance;

	public static DocumentNavigationService Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new DocumentNavigationService();
			}
			return instance;
		}
	}

	public bool IsGoTo { get; set; }

	public bool BackIsEnable
	{
		get
		{
			return GetProperty(() => BackIsEnable);
		}
		set
		{
			SetProperty(() => BackIsEnable, value);
		}
	}

	public bool ForwardsIsEnable
	{
		get
		{
			return GetProperty(() => ForwardsIsEnable);
		}
		set
		{
			SetProperty(() => ForwardsIsEnable, value);
		}
	}

	public ObservableCollection<NavigationData> Items { get; set; }

	public void RefreshViewState()
	{
		try
		{
			BackIsEnable = Items != null && Items.Count > 0;
			int num = Items.ToList().FindIndex((NavigationData it) => it.IsChecked);
			if (BackIsEnable && num > 0)
			{
				BackIsEnable = num != Items.Count - 1;
			}
			ForwardsIsEnable = num > 0;
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "DocumentNavigationService.RefreshViewState");
		}
	}

	public DocumentNavigationService()
	{
		Items = new ObservableCollection<NavigationData>();
		RefreshViewState();
	}

	public void Add(NavigationData navigationData)
	{
		try
		{
			if (IsGoTo)
			{
				IsGoTo = false;
				return;
			}
			NavigationData navigationData2 = Items.ToList().Find(item =>
				item.FilePath == navigationData.FilePath && item.DocumentOffset == navigationData.DocumentOffset);
			if (navigationData2 != null)
			{
				Items.Remove(navigationData2);
			}
			if (Items.Count >= 20)
			{
				Items.RemoveAt(Items.Count - 1);
			}
			navigationData.IsChecked = true;
			navigationData.Navigate = OnNavigationItemClick;
			RefreshViewState();
			Items.Insert(0, navigationData);
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "DocumentNavigationService.Add");
		}
	}

	public void Remove(string filePath)
	{
		try
		{
			NavigationData[] array = Items.ToArray();
			foreach (NavigationData navigationData in array)
			{
				if (navigationData.FilePath == filePath)
				{
					Items.Remove(navigationData);
				}
			}
			RefreshViewState();
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "DocumentNavigationService.Remove");
		}
	}

	public void Clear()
	{
		Items?.Clear();
		RefreshViewState();
	}

	private void OnNavigationItemClick(NavigationData navigationData)
	{
		NavigateTo(navigationData);
		RefreshViewState();
	}

	[Command]
	public void Back()
	{
		try
		{
			int num = Items.ToList().FindIndex((NavigationData it) => it.IsChecked);
			if (num == -1)
			{
				num = 0;
			}
			num++;
			if (num > Items.Count - 1)
			{
				RefreshViewState();
				return;
			}
			NavigationData navigationData = Items[num];
			navigationData.IsChecked = true;
			NavigateTo(navigationData);
			RefreshViewState();
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "DocumentNavigationService.Back");
		}
	}

	[Command]
	public void Forwards()
	{
		try
		{
			int num = Items.ToList().FindIndex((NavigationData it) => it.IsChecked);
			if (num <= 0)
			{
				RefreshViewState();
				return;
			}
			num--;
			NavigationData navigationData = Items[num];
			navigationData.IsChecked = true;
			NavigateTo(navigationData);
			RefreshViewState();
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "DocumentNavigationService.Back");
		}
	}

	private void NavigateTo(NavigationData navigationData)
	{
		try
		{
			DocumentBase document = AppCore.ViewModelBase.RootDocument.GetDocument(navigationData.FilePath);
			if (document != null && document is PvfFileDocument pvfFileDocument)
			{
				IsGoTo = true;
				pvfFileDocument.IsActive = true;
				IsGoTo = true;
				TextEdit editor = pvfFileDocument.GetEditor();
				if (navigationData.DocumentOffset <= editor.Document.TextLength)
				{
					editor.TextArea.Caret.Offset = navigationData.DocumentOffset;
					editor.ScrollTo(navigationData.Line, navigationData.Column);
					IsGoTo = true;
				}
				else
				{
					Items.Remove(navigationData);
				}
			}
		}
		catch (Exception e)
		{
			AppCore.Logger.ErrorUploadDialog(e, "DocumentNavigationService.Navigation");
		}
	}
}
