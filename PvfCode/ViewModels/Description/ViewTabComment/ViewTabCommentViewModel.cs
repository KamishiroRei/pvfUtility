using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using ICSharpCode.AvalonEdit.Highlighting;
using PvfCode.Dot;
using PvfCode.Dot.Desktop;
using SqlSugar;
using WinCopies.Collections;

namespace PvfCode.ViewModels.Description.ViewTabComment;

public class ViewTabCommentViewModel : ViewModelBase
{
	private ObservableCollection<PvfCommentDto> items;

	private ObservableCollection<PvfCommentDto> searchResult;

	private readonly Action Close;

	public bool Focusable
	{
		get
		{
			return GetProperty(() => Focusable);
		}
		set
		{
			SetProperty(() => Focusable, value);
		}
	}

	public string Count
	{
		get
		{
			return GetProperty(() => Count);
		}
		set
		{
			SetProperty<string>(() => Count, value);
		}
	}

	public string Keyword
	{
		get
		{
			return GetProperty(() => Keyword);
		}
		set
		{
			SetProperty<string>(() => Keyword, value);
			if (string.IsNullOrEmpty(value))
			{
				RaisePropertyChanged("Items");
			}
			UpdateCount();
		}
	}

	private ObservableCollection<PvfCommentDto> SearchResult
	{
		get
		{
			if (searchResult == null)
			{
				searchResult = new ObservableCollection<PvfCommentDto>();
			}
			return searchResult;
		}
		set
		{
			searchResult = value;
		}
	}

	public ObservableCollection<PvfCommentDto> Items
	{
		get
		{
			if (!string.IsNullOrEmpty(Keyword))
			{
				return SearchResult;
			}
			return items;
		}
		set
		{
			items = value;
		}
	}

	public PvfCommentDto SelectedItem
	{
		get
		{
			return GetProperty(() => SelectedItem);
		}
		set
		{
			SetProperty<PvfCommentDto>(() => SelectedItem, value);
		}
	}

	public List<PvfCommentDto> SelectedItems { get; set; }

	public PvfCommentDto AddData { get; set; }

	public IHighlightingDefinition AddDataHighlighting
	{
		get
		{
			return GetProperty(() => AddDataHighlighting);
		}
		set
		{
			SetProperty<IHighlightingDefinition>(() => AddDataHighlighting, value);
		}
	}

	public bool WholeWordMatch
	{
		get
		{
			return GetProperty(() => WholeWordMatch);
		}
		set
		{
			SetProperty(() => WholeWordMatch, value);
		}
	}

	private void UpdateCount()
	{
		if (Items == null)
		{
			Count = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_Total"), 0);
		}
		else
		{
			Count = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_Total"), Items.Count);
		}
	}

	private void UpdateAddDataHighlighting()
	{
		if (!AddData.FileType.HasValue)
		{
			AddDataHighlighting = ThemeSwitcher.Instance.GetHighlightingDefinition(PvfFileType.equ);
		}
		else
		{
			AddDataHighlighting = ThemeSwitcher.Instance.GetHighlightingDefinition(AddData.FileType.Value);
		}
	}

	public ViewTabCommentViewModel(Action close)
	{
		AddData = new PvfCommentDto
		{
			FileType = PvfFileType.equ,
			Comment = AppSetting.Instance.GetIlogger()?.GetStr("ViewTabCommentViewModel_DefaultSectionComment")
		};
		AddData.OnFileTypeChanged += OnAddDataFileTypeChanged;
		UpdateAddDataHighlighting();
		SelectedItems = new List<PvfCommentDto>();
		Close = close;
	}

	private void OnAddDataFileTypeChanged(PvfFileType? fileType)
	{
		UpdateAddDataHighlighting();
	}

	[Command]
	public void OnAdd()
	{
		if (ValidateComment(AddData))
		{
			PvfCommentDto comment = AddData.CloneData();
			items.Add(comment);
			UpdateCount();
			((DispatcherObject)Application.Current).Dispatcher.BeginInvoke((Delegate)(Action)delegate
			{
				SelectedItem = comment;
			}, Array.Empty<object>());
		}
	}

	private bool ValidateComment(PvfCommentDto comment)
	{
		if (string.IsNullOrEmpty(comment.Section))
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseInputTagName"), isError: true);
			return false;
		}
		if (string.IsNullOrEmpty(comment.Comment))
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseInputComment"), isError: true);
			return false;
		}
		return true;
	}

	[Command]
	public void Unloaded()
	{
		AddData.OnFileTypeChanged -= OnAddDataFileTypeChanged;
		Application.Current.MainWindow.Activate();
	}

	[Command]
	public async void Loaded()
	{
		ResultData<List<PvfCommentDto>> resultData = await ServicePvfTabComment.Instance.GetList();
		if (resultData.IsError)
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_GetTranslateListError") + resultData.Msg);
			return;
		}
		if (resultData.Data == null)
		{
			resultData.Data = new List<PvfCommentDto>();
		}
		Items = new ObservableCollection<PvfCommentDto>(resultData.Data);
		RaisePropertyChanged("Items");
		UpdateCount();
	}

	[Command]
	public async void OnSave()
	{
		ResultData resultData = await ServicePvfTabComment.Instance.ClearAddRanged(items.ToList());
		if (resultData.IsError)
		{
			AppCore.ShowMsg(resultData.Msg, isError: true);
			return;
		}
		Close?.Invoke();
	}

	[Command]
	public void OnClose()
	{
		Close?.Invoke();
	}

	[Command]
	public void OnSeleteSelectedItems()
	{
		bool flag = !string.IsNullOrEmpty(Keyword);
		PvfCommentDto[] array = SelectedItems.ToArray();
		foreach (PvfCommentDto item in array)
		{
			items.Remove(item);
			if (flag)
			{
				SearchResult.Remove(item);
			}
		}
		UpdateCount();
	}

	[Command]
	public void OnUpdateSelectedData()
	{
		if (SelectedItem != null)
		{
			ValidateComment(SelectedItem);
		}
	}

	[Command]
	public void OnSearch()
	{
		if (!string.IsNullOrEmpty(Keyword))
		{
			SearchResult.Clear();
			IEnumerable<PvfCommentDto> matchingComments = items.WhereIF(WholeWordMatch, (PvfCommentDto comment) => comment.Section.Equals(Keyword, StringComparison.OrdinalIgnoreCase)).WhereIF(!WholeWordMatch, (PvfCommentDto comment) => comment.Section.Contains(Keyword, StringComparison.OrdinalIgnoreCase));
			if (matchingComments != null && matchingComments.Any())
			{
				SearchResult.AddRange(in matchingComments);
			}
			RaisePropertyChanged("Items");
			UpdateCount();
		}
	}

	[Command]
	public void OnClearKeyword()
	{
		Keyword = string.Empty;
	}
}
