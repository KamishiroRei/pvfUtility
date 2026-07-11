using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Editors;
using ICSharpCode.AvalonEdit.Highlighting;
using PvfCode.Dot;
using PvfCode.Dot.Desktop;
using SqlSugar;
using WinCopies.Collections;

namespace PvfCode.ViewModels.Description.ViewTabComment;

public class ViewTabCommentViewModel : ViewModelBase
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass37_0
	{
		public ViewTabCommentViewModel sBoIkXWRVq;

		public PvfCommentDto ApjI03hQYB;

		public _003C_003Ec__DisplayClass37_0()
		{
		}

		internal void RNFIJ9yg3u()
		{
			sBoIkXWRVq.SelectedItem = ApjI03hQYB;
		}
	}

	private ObservableCollection<PvfCommentDto> t7cGXqwJyx;

	private ObservableCollection<PvfCommentDto> wuhGp7ETgX;

	[CompilerGenerated]
	private List<PvfCommentDto> ElcGUCMlTy;

	[CompilerGenerated]
	private PvfCommentDto VpkGc2C02J;

	private readonly Action Close;

	internal AutoSuggestEdit JNMG8pVX67;

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
			LcZGKbl07F();
		}
	}

	private ObservableCollection<PvfCommentDto> SearchResult
	{
		get
		{
			if (wuhGp7ETgX == null)
			{
				wuhGp7ETgX = new ObservableCollection<PvfCommentDto>();
			}
			return wuhGp7ETgX;
		}
		set
		{
			wuhGp7ETgX = value;
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
			return t7cGXqwJyx;
		}
		set
		{
			t7cGXqwJyx = value;
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

	public List<PvfCommentDto> SelectedItems
	{
		[CompilerGenerated]
		get
		{
			return ElcGUCMlTy;
		}
		[CompilerGenerated]
		set
		{
			ElcGUCMlTy = value;
		}
	}

	public PvfCommentDto AddData
	{
		[CompilerGenerated]
		get
		{
			return VpkGc2C02J;
		}
		[CompilerGenerated]
		set
		{
			VpkGc2C02J = value;
		}
	}

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

	private void LcZGKbl07F()
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

	private void getG9NA5Qo()
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
		AddData.OnFileTypeChanged += fDmGPeOJfr;
		getG9NA5Qo();
		SelectedItems = new List<PvfCommentDto>();
		Close = close;
	}

	private void fDmGPeOJfr(PvfFileType? P_0)
	{
		getG9NA5Qo();
	}

	[Command]
	public void OnAdd()
	{
		_003C_003Ec__DisplayClass37_0 CS_0024_003C_003E8__locals5 = new _003C_003Ec__DisplayClass37_0();
		CS_0024_003C_003E8__locals5.sBoIkXWRVq = this;
		if (LKYGZG3qCv(AddData))
		{
			CS_0024_003C_003E8__locals5.ApjI03hQYB = AddData.CloneData();
			t7cGXqwJyx.Add(CS_0024_003C_003E8__locals5.ApjI03hQYB);
			LcZGKbl07F();
			((DispatcherObject)Application.Current).Dispatcher.BeginInvoke((Delegate)(Action)delegate
			{
				CS_0024_003C_003E8__locals5.sBoIkXWRVq.SelectedItem = CS_0024_003C_003E8__locals5.ApjI03hQYB;
			}, Array.Empty<object>());
		}
	}

	private bool LKYGZG3qCv(PvfCommentDto P_0)
	{
		if (string.IsNullOrEmpty(P_0.Section))
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseInputTagName"), isError: true);
			return false;
		}
		if (string.IsNullOrEmpty(P_0.Comment))
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseInputComment"), isError: true);
			return false;
		}
		return true;
	}

	[Command]
	public void Unloaded()
	{
		AddData.OnFileTypeChanged -= fDmGPeOJfr;
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
		LcZGKbl07F();
	}

	[Command]
	public async void OnSave()
	{
		ResultData resultData = await ServicePvfTabComment.Instance.ClearAddRanged(t7cGXqwJyx.ToList());
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
			t7cGXqwJyx.Remove(item);
			if (flag)
			{
				SearchResult.Remove(item);
			}
		}
		LcZGKbl07F();
	}

	[Command]
	public void OnUpdateSelectedData()
	{
		if (SelectedItem != null)
		{
			LKYGZG3qCv(SelectedItem);
		}
	}

	[Command]
	public void OnSearch()
	{
		if (!string.IsNullOrEmpty(Keyword))
		{
			SearchResult.Clear();
			IEnumerable<PvfCommentDto> array = t7cGXqwJyx.WhereIF(WholeWordMatch, (PvfCommentDto P_0) => P_0.Section.Equals(Keyword, StringComparison.OrdinalIgnoreCase)).WhereIF(!WholeWordMatch, (PvfCommentDto P_0) => P_0.Section.Contains(Keyword, StringComparison.OrdinalIgnoreCase));
			if (array != null && array.Any())
			{
				SearchResult.AddRange(in array);
			}
			RaisePropertyChanged("Items");
			LcZGKbl07F();
		}
	}

	[Command]
	public void OnClearKeyword()
	{
		Keyword = string.Empty;
	}

	[CompilerGenerated]
	private bool EsAGJW18Er(PvfCommentDto P_0)
	{
		return P_0.Section.Equals(Keyword, StringComparison.OrdinalIgnoreCase);
	}

	[CompilerGenerated]
	private bool vjaGkHKnbj(PvfCommentDto P_0)
	{
		return P_0.Section.Contains(Keyword, StringComparison.OrdinalIgnoreCase);
	}
}
