using System;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.MVVMServices;
using PvfCode.ViewModels.DocumentFolder.Enums;

namespace PvfCode.ViewModels.DocumentFolder;

public abstract class DocumentBase : ViewModelBase, IDisposable
{
	public virtual int? Rarity => null;

	public ImageSource Icon
	{
		get
		{
			return GetProperty(() => Icon);
		}
		set
		{
			SetProperty<ImageSource>(() => Icon, value);
		}
	}

	public IDcoumentPanelService DocumentService => GetService<IDcoumentPanelService>();

	public bool IsLoading
	{
		get
		{
			return GetProperty(() => IsLoading);
		}
		set
		{
			SetProperty(() => IsLoading, value);
		}
	}

	public bool IsSelected
	{
		get
		{
			return GetProperty(() => IsSelected);
		}
		set
		{
			SetProperty(() => IsSelected, value);
		}
	}

	public virtual string FileName => DocumentPath;

	public string DocumentPath { get; set; }

	public bool IsActive
	{
		get
		{
			return GetProperty(() => IsActive);
		}
		set
		{
			SetProperty(() => IsActive, value);
			if (value)
			{
				IsSelected = true;
			}
		}
	}

	public PvfFileDocumentType DocumentType { get; set; }

	public DocumentBase(string documentPath)
	{
		DocumentPath = documentPath;
	}

	[Command]
	public void OnShowContextMenu()
	{
		DocumentService.Test();
		AppCore.ViewModelBase.DockLayoutManagerService.ShowContextMenu(this);
	}

	public void PreviewMouseDown(object sender, MouseButtonEventArgs e)
	{
		if (e.ChangedButton == MouseButton.Middle)
		{
			AppCore.ViewModelBase.RootDocument.OnClose(this);
		}
	}

	public void PreviewMouseWheel(object sender, MouseWheelEventArgs e)
	{
		if (e.Delta > 0)
		{
			AppCore.ViewModelBase.DocumentGroupService.LastDocument(this);
		}
		else
		{
			AppCore.ViewModelBase.DocumentGroupService.NextDocument(this);
		}
	}

	public void MouseDown(object sender, MouseButtonEventArgs e)
	{
		if (e.XButton2 == MouseButtonState.Pressed)
		{
			AppCore.ViewModelBase.DocumentGroupService.LastDocument(this);
		}
		if (e.XButton1 == MouseButtonState.Pressed)
		{
			AppCore.ViewModelBase.DocumentGroupService.NextDocument(this);
		}
	}

	public abstract void Dispose();
}
