using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using PvfCode.Controls;
using PvfCode.Dot.Desktop;
using PvfCode.ViewModels.TreeFolder;

namespace PvfCode.Views.BookMark;

public class EditBookmarkView : ThemedWindow, IComponentConnector
{
	internal EditBox txtTitle;

	internal ButtonEdit txtFilePath;

	internal Button btnSave;

	internal Button btnCancel;

	private bool contentLoaded;

	public KeyValuePair<string, BookMarkDto> Row { get; set; }

	public string BookMarkTitle
	{
		get
		{
			if (!Row.Value.IsFile)
			{
				return "目录名称：";
			}
			return "书签名称：";
		}
	}

	public string WinTitle { get; set; }

	public EditBookmarkView(KeyValuePair<string, BookMarkDto> row, bool isAdd)
	{
		WinTitle = ((!isAdd) ? "修改书签 " : (row.Value.IsFile ? "新建书签" : "新建目录"));
		Row = row;
		base.DataContext = this;
		InitializeComponent();
		txtTitle.Value = row.Key;
		txtFilePath.EditValue = row.Value.FilePath;
		base.Loaded += OnLoaded;
		txtTitle.input.KeyDown += OnInputKeyDown;
		txtFilePath.KeyDown += OnInputKeyDown;
	}

	private void OnInputKeyDown(object sender, KeyEventArgs e)
	{
		if (e.Key == Key.Enter)
		{
			SaveAndClose();
		}
	}

	private void OnLoaded(object sender, RoutedEventArgs e)
	{
		txtTitle.input.Focus();
	}

	private void OnSelectFilePath(object sender, RoutedEventArgs e)
	{
		if (!AppCore.ViewModelBase.PVF.PvfIsOpen)
		{
			MessageBox.Show("请先载入PVF封包");
			return;
		}
		IEnumerable<string> enumerable = AppCore.SelectPvfFileList(TreeViewType.FileList, "选择书签路径");
		if (enumerable != null)
		{
			txtFilePath.EditValue = enumerable.FirstOrDefault();
		}
	}

	private void OnSave(object sender, RoutedEventArgs e)
	{
		SaveAndClose();
	}

	private void SaveAndClose()
	{
		string text = null;
		if (Row.Value.IsFile)
		{
			if (txtFilePath.EditValue == null)
			{
				return;
			}
			text = txtFilePath.EditValue.ToString();
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
		}
		string value = txtTitle.Value;
		if (!string.IsNullOrEmpty(txtTitle.Value))
		{
			Row.Value.FilePath = text;
			Row = new KeyValuePair<string, BookMarkDto>(value, Row.Value);
			base.DialogResult = true;
		}
	}

	private void OnCancel(object sender, RoutedEventArgs e)
	{
		base.DialogResult = false;
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	public void InitializeComponent()
	{
		if (!contentLoaded)
		{
			contentLoaded = true;
			Uri resourceLocator = new Uri("/pvfUtility;V2026.1.22.2;component/views/bookmark/editbookmarkview.xaml", UriKind.Relative);
			System.Windows.Application.LoadComponent(this, resourceLocator);
		}
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	internal Delegate _CreateDelegate(Type delegateType, string handler)
	{
		return Delegate.CreateDelegate(delegateType, this, handler);
	}

	[DebuggerNonUserCode]
	[GeneratedCode("PresentationBuildTasks", "10.0.1.0")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	void IComponentConnector.Connect(int connectionId, object target)
	{
		switch (connectionId)
		{
		case 1:
			txtTitle = (EditBox)target;
			break;
		case 2:
			txtFilePath = (ButtonEdit)target;
			break;
		case 3:
			((ButtonInfo)target).Click += OnSelectFilePath;
			break;
		case 4:
			btnSave = (Button)target;
			btnSave.Click += OnSave;
			break;
		case 5:
			btnCancel = (Button)target;
			btnCancel.Click += OnCancel;
			break;
		default:
			contentLoaded = true;
			break;
		}
	}
}
