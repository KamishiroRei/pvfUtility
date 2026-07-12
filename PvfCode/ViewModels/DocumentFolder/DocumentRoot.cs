using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Docking;
using DevExpress.Xpf.Docking.Base;
using PvfCode.ViewModels.DocumentFolder.Enums;
using PvfCode.ViewModels.DocumentFolder.SearchViewModels;
using PvfCode.ViewModels.PvfDiffTool;
using PvfCode.Views.ImportViews;

namespace PvfCode.ViewModels.DocumentFolder;

public class DocumentRoot : ViewModelBase
{
	public ObservableCollection<DocumentBase> Documents { get; set; }

	public DocumentsSearchManager DocumentsSearchManager { get; set; }

	public DocumentRoot()
	{
		DocumentsSearchManager = new DocumentsSearchManager();
		Documents = new ObservableCollection<DocumentBase>();
		ThemeSwitcher.Instance.PvfCodeThemeChangedEvent += jpHfpZ34Sr;
	}

	public DocumentBase GetDocument(string filePath)
	{
		DocumentBase documentBase = Documents.FirstOrDefault(it => it.DocumentPath == filePath);
		if (documentBase != null)
		{
			return documentBase;
		}
		return null;
	}

	private void jpHfpZ34Sr()
	{
		foreach (PvfFileDocument item in Documents.Where((DocumentBase it) => it is PvfFileDocument))
		{
			item.SetIHighlighting();
			if (item.IsActive && item.SearchPanel.Visibility)
			{
				item.SearchPanel.ShowReplacePanel = false;
				item.SearchPanel.ShowReplacePanel = true;
			}
		}
	}

	public void AddDocument(PvfFile file, bool gotoNode = false)
	{
		if (file == null)
		{
			return;
		}
		if (CheckIsOpen(file.FileName, out DocumentBase docu))
		{
			docu.IsActive = true;
		}
		else
		{
			PvfFileDocument pvfFileDocument = new PvfFileDocument(file)
			{
				DocumentPath = file.FileName
			};
			pvfFileDocument.Activated += OnPvfDocumentActivated;
			Documents.Add(pvfFileDocument);
			pvfFileDocument.IsActive = true;
		}
		if (!gotoNode)
		{
			return;
		}
		Task.Run(delegate
		{
			((DispatcherObject)Application.Current).Dispatcher.BeginInvoke((Delegate)(Action)delegate
			{
				AppCore.ViewModelBase.PvfFileTreeViewModel.GoToNode(file.FileName);
			}, Array.Empty<object>());
		});
	}

	public void OpenPreview(PvfFileDocument sourceDocument)
	{
		if (sourceDocument == null || !PvfPreviewDocument.Supports(sourceDocument.File))
		{
			return;
		}
		PvfPreviewDocument preview = Documents.OfType<PvfPreviewDocument>().FirstOrDefault();
		if (preview != null)
		{
			preview.SetSource(sourceDocument);
			return;
		}
		preview = new PvfPreviewDocument(sourceDocument);
		Documents.Add(preview);
		preview.IsActive = true;
		SplitPreviewRight(preview, sourceDocument, 0);
	}

	private void OnPvfDocumentActivated(object sender, EventArgs e)
	{
		if (sender is PvfFileDocument sourceDocument)
		{
			OpenPreview(sourceDocument);
		}
	}

	private void SplitPreviewRight(PvfPreviewDocument preview, PvfFileDocument sourceDocument, int attempt)
	{
		if (Application.Current == null)
		{
			return;
		}
		Application.Current.Dispatcher.BeginInvoke((Action)delegate
		{
			if (!Documents.Contains(preview))
			{
				return;
			}
			if (AppCore.ViewModelBase.DockLayoutManagerService.SplitRight(preview))
			{
				sourceDocument.IsActive = true;
				return;
			}
			if (attempt < 8)
			{
				SplitPreviewRight(preview, sourceDocument, attempt + 1);
			}
		}, attempt == 0 ? DispatcherPriority.Loaded : DispatcherPriority.Background);
	}

	public void AddDocument(string filePath, bool gotoNode = false)
	{
		PvfFile file = AppCore.ViewModelBase.PVF.GetFile(filePath);
		if (file == null)
		{
			AppCore.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_FileNotExist"), filePath), isError: true);
		}
		else
		{
			AddDocument(file, gotoNode);
		}
	}

	public DocumentBase AddControl(PvfFileDocumentType pvfFileDocumentType)
	{
		if (CheckIsOpen(epZfUfxg6Z(pvfFileDocumentType), out DocumentBase docu))
		{
			docu.IsActive = true;
			return docu;
		}
		switch (pvfFileDocumentType)
		{
		case PvfFileDocumentType.起始页:
			docu = new DocumentIndexViewModel();
			break;
		case PvfFileDocumentType.PVF差异比较器:
			docu = new PvfDiffToolViewModel();
			break;
		case PvfFileDocumentType.发布:
			docu = new ViewPvfReleaseViewModel();
			break;
		case PvfFileDocumentType.导入文件:
			docu = new ViewImportFilesViewModel();
			break;
		}
		if (docu == null)
		{
			return null;
		}
		Documents.Add(docu);
		docu.IsActive = true;
		return docu;
	}

	private string epZfUfxg6Z(PvfFileDocumentType P_0)
	{
		switch (P_0)
		{
		case PvfFileDocumentType.起始页:
			return AppSetting.Instance.GetIlogger()?.GetStr("DocumentName_StartPage");
		case PvfFileDocumentType.PVF差异比较器:
			return AppSetting.Instance.GetIlogger()?.GetStr("DocumentName_PvfDiff");
		case PvfFileDocumentType.发布:
			return AppSetting.Instance.GetIlogger()?.GetStr("DocumentName_Publish");
		default:
			return "";
		}
	}

	public void BookMarkOpenDocument(string filePath)
	{
		if (!AppCore.ViewModelBase.PVF.PvfIsOpen)
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseLoadPvfPackFirst"));
		}
		else if (string.IsNullOrEmpty(Path.GetExtension(filePath)))
		{
			BookMarkGoToNode(filePath);
		}
		else
		{
			AddDocument(filePath, AppCore.ViewModelBase.PvfFileTreeViewModel.TreeGroupData.Any(filePath));
		}
	}

	public void BookMarkGoToNode(string filePath)
	{
		if (!string.IsNullOrEmpty(filePath))
		{
			filePath = filePath.ToLower();
			if (AppCore.ViewModelBase.PvfFileTreeViewModel.TreeGroupData.Any(filePath))
			{
				AppCore.ViewModelBase.PvfFileTreeViewModel.GoToNode(filePath);
			}
			else
			{
				AppCore.ShowMsg(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_PathNotExist"), filePath));
			}
		}
	}

	public bool CheckIsOpen(string filePath, out DocumentBase? docu)
	{
		docu = Documents.FirstOrDefault(it => it.DocumentPath == filePath);
		return docu != null;
	}

	public void ChangedTextEditorIHighlightingDefinition()
	{
	}

	public void DockItemClosing(object sender, ItemCancelEventArgs e)
	{
		if (e.Item.DataContext is KeyValuePair<string, PvfFileDocument>)
		{
			nUgfcLTvyr(e.Item.DataContext, e);
			return;
		}
		if (e.Item is FloatGroup)
		{
			List<BaseLayoutItem> list = new List<BaseLayoutItem>();
			GetAllItems(e.Item, list);
			{
				foreach (BaseLayoutItem item in list)
				{
					nUgfcLTvyr(item.DataContext, e);
				}
				return;
			}
		}
		e.Cancel = true;
	}

	public List<BaseLayoutItem> GetAllItems(BaseLayoutItem item, List<BaseLayoutItem> list)
	{
		if (item is LayoutGroup layoutGroup)
		{
			foreach (BaseLayoutItem item2 in layoutGroup.Items)
			{
				GetAllItems(item2, list);
			}
		}
		else
		{
			list.Add(item);
		}
		return list;
	}

	[Command]
	public void OnDocumentClose(DocumentPanel panel)
	{
		DocumentBase document = (DocumentBase)panel.Content;
		if (Documents.Any(it => it.DocumentPath == document.DocumentPath))
		{
			OnClose(document);
			panel = null;
		}
	}

	[Command]
	public void OnClose(DocumentBase doc)
	{
		if (doc is PvfFileDocument pvfFileDocument)
		{
			if (!AppCore.IsSaveAllDocument && pvfFileDocument.TextIsChanged && AppCore.Logger.ShowDialog(AppSetting.Instance.GetIlogger()?.GetStr("mess_CloseCurrentDocumentDialog")) != MessageResult.Yes)
			{
				return;
			}
			DocumentNavigationService.Instance.Remove(pvfFileDocument.FullPath);
		}
		CloseLinkedPreview(doc);
		if (doc is PvfFileDocument sourceDocument)
		{
			sourceDocument.Activated -= OnPvfDocumentActivated;
		}
		doc.Dispose();
		if (Documents.Contains(doc))
		{
			Documents.Remove(doc);
		}
		else
		{
			AppCore.ViewModelBase.DockLayoutManagerService.ClosePanel(doc);
		}
	}

	private void nUgfcLTvyr(object P_0, ItemCancelEventArgs P_1)
	{
		DocumentBase documentBase = (DocumentBase)P_0;
		if (!CheckIsOpen(documentBase.DocumentPath, out DocumentBase _))
		{
			return;
		}
		if (!AppCore.IsSaveAllDocument && documentBase is PvfFileDocument { TextIsChanged: not false } && AppCore.Logger.ShowDialog(AppSetting.Instance.GetIlogger()?.GetStr("mess_CloseCurrentDocumentDialog")) != MessageResult.Yes)
		{
			P_1.Cancel = true;
			return;
		}
		if (documentBase.DocumentType == PvfFileDocumentType.PVF文档)
		{
			DocumentNavigationService.Instance.Remove((documentBase as PvfFileDocument).FullPath);
		}
		CloseLinkedPreview(documentBase);
		if (documentBase is PvfFileDocument sourceDocument)
		{
			sourceDocument.Activated -= OnPvfDocumentActivated;
		}
		documentBase.Dispose();
		Documents.Remove(documentBase);
	}

	private void CloseLinkedPreview(DocumentBase document)
	{
		if (document is not PvfFileDocument sourceDocument)
		{
			return;
		}
		PvfPreviewDocument preview = Documents.OfType<PvfPreviewDocument>()
			.FirstOrDefault(it => ReferenceEquals(it.SourceDocument, sourceDocument));
		if (preview != null)
		{
			preview.Dispose();
			Documents.Remove(preview);
		}
	}

	public void RemoveDocument(string filePath, bool isShowDialog)
	{
		if (CheckIsOpen(filePath, out DocumentBase docu) && (!isShowDialog || !(docu is PvfFileDocument { TextIsChanged: not false }) || AppCore.Logger.ShowDialog(AppSetting.Instance.GetIlogger()?.GetStr("mess_CloseCurrentDocumentDialog")) == MessageResult.Yes))
		{
			CloseLinkedPreview(docu);
			if (docu is PvfFileDocument sourceDocument)
			{
				sourceDocument.Activated -= OnPvfDocumentActivated;
			}
			docu.Dispose();
			Documents.Remove(docu);
		}
	}

	public void FloatDocument(DocumentBase doc)
	{
		AppCore.ViewModelBase.DockLayoutManagerService.Float(doc);
	}

	public bool CheckNotSavedDocumentIsAny()
	{
		return Documents.Any((DocumentBase it) => it is PvfFileDocument pvfFileDocument && pvfFileDocument.TextIsChanged);
	}

	public List<string> GetNotSaveFiles()
	{
		return (from it in Documents
			where it is PvfFileDocument pvfFileDocument && pvfFileDocument.TextIsChanged
			select ((PvfFileDocument)it).FullPath).ToList();
	}

	public void RemoveDocuments(IEnumerable<string> filePaths)
	{
		AppCore.IsSaveAllDocument = true;
		foreach (string filePath in filePaths)
		{
			RemoveDocument(filePath, isShowDialog: false);
		}
		AppCore.IsSaveAllDocument = false;
	}

	public void SaveAllDocument()
	{
		foreach (PvfFileDocument item in Documents.Where((DocumentBase it) => it is PvfFileDocument))
		{
			if (item.TextIsChanged)
			{
				item.OnSave();
			}
		}
	}

	public void Clear()
	{
		DocumentNavigationService.Instance.Clear();
		DocumentsSearchManager.Clear();
		foreach (PvfFileDocument sourceDocument in Documents.OfType<PvfFileDocument>())
		{
			sourceDocument.Activated -= OnPvfDocumentActivated;
		}
		foreach (DocumentBase document in Documents.OrderByDescending(it => it is PvfPreviewDocument).ToArray())
		{
			document.Dispose();
		}
		Documents.Clear();
	}
}
