using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Xpf.Editors;
using Microsoft.WindowsAPICodePack.Dialogs;
using PvfCode.Models.Pvf.ImportModels;
using PvfCode.ViewModels;
using PvfCode.ViewModels.DocumentFolder;
using PvfCode.ViewModels.DocumentFolder.Enums;
using PvfCode.ViewModels.TreeFolder;
using SevenZip;

namespace PvfCode.Views.ImportViews;

public class ViewImportFilesViewModel : DocumentBase
{
	public bool Is7zip
	{
		get
		{
			return GetProperty(() => Is7zip);
		}
		set
		{
			SetProperty(() => Is7zip, value, OnImportModeChanged);
		}
	}

	public string FilePathFrom7zip { get; set; }

	public PvfTreeViewModel TreeViewModel { get; set; }

	public ImportConfig Config => AppSetting.Instance.PvfConfig.ImportConfig;

	private void OnImportModeChanged()
	{
		TreeViewModel.Clear();
	}

	public ViewImportFilesViewModel()
		: base("导入文件")
	{
		base.DocumentType = PvfFileDocumentType.导入文件;
		TreeViewModel = new PvfTreeViewModel(TreeViewType.ImportFiles);
		ResetImportFiles();
		TreeViewModel.EventDropFile += OnFilesDropped;
		TreeViewModel.EventOpenDocumenting += OpenImportFile;
	}

	[Command]
	public void Loaded()
	{
		AppCore.ViewModelBase.DockLayoutManagerService.SetFloatPanelAutoHeight(this, SizeToContent.Manual);
	}

	private void OpenImportFile(PvfTreeFileBase file)
	{
		try
		{
			if (file is not PvfTreeFileImport importFile)
			{
				return;
			}
			string text = "";
			if (Is7zip)
			{
				byte[] array = ExtractArchiveFile(importFile.ImportItem?.FullPath);
				if (array != null && array.Length != 0)
				{
					text = Encoding.GetEncoding((int)AppSetting.Instance.PvfConfig.DefaultEncoding).GetString(array).TrimEnd(new char[1]);
				}
			}
			else
			{
				byte[] bytes = File.ReadAllBytes(importFile.ImportItem?.FullPath);
				text = Encoding.GetEncoding((int)AppSetting.Instance.PvfConfig.DefaultEncoding).GetString(bytes).TrimEnd(new char[1]);
			}
			AppCore.ShowDefaultScriptEditorWindow(new ViewScriptEditorViewModel(importFile.ImportItem?.FullPath, text)
			{
				IsReadOnly = true
			}, Application.Current.MainWindow);
		}
		catch (Exception ex)
		{
			AppCore.ShowMsg(ex.Message, isError: true);
		}
	}

	private void ResetImportFiles()
	{
		TreeViewModel.Clear();
	}

	[Command]
	public void SelectDiskFiles()
	{
		CommonOpenFileDialog commonOpenFileDialog = new CommonOpenFileDialog
		{
			Title = AppSetting.Instance.GetIlogger().GetStr("mess_SelectFileToImport"),
			IsFolderPicker = false,
			Multiselect = true,
			AllowPropertyEditing = true,
			EnsurePathExists = true,
			EnsureValidNames = true
		};
		if (Is7zip)
		{
			commonOpenFileDialog.Multiselect = false;
			commonOpenFileDialog.Filters.Add(new CommonFileDialogFilter(AppSetting.Instance.GetIlogger()?.GetStr("mess_7zFile"), ".7z"));
		}
		if (commonOpenFileDialog.ShowDialog(Application.Current.MainWindow) == CommonFileDialogResult.Ok)
		{
			LoadSelectedFiles(commonOpenFileDialog.FileNames);
		}
	}

	private void OnFilesDropped(IEnumerable<string> paths)
	{
		LoadSelectedFiles(paths);
	}

	private async void LoadSelectedFiles(IEnumerable<string> selectedPaths)
	{
		List<string> paths = selectedPaths?.ToList();
		if (paths == null || paths.Count == 0)
		{
			return;
		}
		if (Is7zip)
		{
			string archivePath = paths[0];
			string extension = Path.GetExtension(archivePath);
			if (!string.Equals(extension, ".7z", StringComparison.OrdinalIgnoreCase))
			{
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseDrag7zFile"));
				return;
			}
			TreeViewModel.TreeGroupData.Loading = true;
			TreeViewModel.Clear();
			FilePathFrom7zip = archivePath;
			ConcurrentBag<ImportFileItem> archiveItems = await Task.Run(() => Load7zipFile(archivePath));
			if (archiveItems != null && archiveItems.Count > 0)
			{
				await TreeViewModel.TreeGroupData.ImportFilesCreateTrees(archiveItems, Config.TargetPath, Is7zip);
			}
			TreeViewModel.TreeGroupData.Loading = false;
			return;
		}
		TreeViewModel.TreeGroupData.Loading = true;
		IEnumerable<ImportFileItem> importItems = await Task.Run(() => LoadDiskFilesAsync(paths));
		if (importItems != null)
		{
			await TreeViewModel.TreeGroupData.ImportFilesCreateTrees(importItems, Config.TargetPath);
			if (AppSetting.Instance.TreeSetting.ImportTreeListAutoExpandAllNodes)
			{
				await ((DispatcherObject)Application.Current).Dispatcher.BeginInvoke((Delegate)(Action)delegate
				{
					TreeViewModel.Service.ExpandAllNodes();
				}, Array.Empty<object>());
			}
		}
		TreeViewModel.TreeGroupData.Loading = false;
	}

	public async void TargetPath_EditValueChanged(object sender, EditValueChangedEventArgs e)
	{
		List<ImportFileItem> allImportItems = TreeViewModel.TreeGroupData.GetAllImportItems();
		if (allImportItems != null)
		{
			TreeViewModel.TreeGroupData.Loading = true;
			TreeViewModel.TreeGroupData.Clear();
			await TreeViewModel.TreeGroupData.ImportFilesCreateTrees(allImportItems, Config.TargetPath, Is7zip);
			TreeViewModel.TreeGroupData.Loading = false;
		}
	}

	private async Task<IEnumerable<ImportFileItem>> LoadDiskFilesAsync(List<string> paths)
	{
		string rootPath = paths[0].Remove(paths[0].LastIndexOf('\\'));
		ConcurrentDictionary<string, ImportFileItem> importItems = new ConcurrentDictionary<string, ImportFileItem>();
		ParallelOptions parallelOptions = new ParallelOptions
		{
			MaxDegreeOfParallelism = 2000
		};
		await Parallel.ForEachAsync(paths, parallelOptions, async delegate(string path, CancellationToken cancellationToken)
		{
			if (Directory.Exists(path))
			{
				await Parallel.ForEachAsync(new DirectoryInfo(path).GetFiles("*.*", SearchOption.AllDirectories), parallelOptions, delegate(FileInfo file, CancellationToken nestedCancellationToken)
				{
					AddImportFile(file.FullName, rootPath, importItems);
					return ValueTask.CompletedTask;
				});
			}
			else if (File.Exists(path))
			{
				AddImportFile(path, rootPath, importItems);
			}
		});
		if (importItems.Count > 0)
		{
			return importItems.Values;
		}
		return null;
	}

	private static void AddImportFile(string filePath, string rootPath, ConcurrentDictionary<string, ImportFileItem> importItems)
	{
		importItems.TryAdd(filePath, new ImportFileItem(filePath, rootPath));
	}

	public void SelectDisk7zFile()
	{
		CommonOpenFileDialog commonOpenFileDialog = new CommonOpenFileDialog();
		commonOpenFileDialog.Title = AppSetting.Instance.GetIlogger().GetStr("mess_Select7zFileSavePath");
		commonOpenFileDialog.IsFolderPicker = false;
		commonOpenFileDialog.Multiselect = false;
		commonOpenFileDialog.Filters.Add(new CommonFileDialogFilter(AppSetting.Instance.GetIlogger().GetStr("mess_7zFile"), ".7z"));
		commonOpenFileDialog.ShowDialog();
	}

	[Command]
	public void OnClearAllFiles()
	{
		ResetImportFiles();
	}

	[Command]
	public void OnImportStart()
	{
		List<ImportFileItem> allImportItems = TreeViewModel.TreeGroupData.GetAllImportItems();
		if (allImportItems == null || allImportItems.Count == 0)
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_NoFileCanBeImported"), isError: true);
			return;
		}
		Config.SourceFiles = allImportItems.ToHashSet();
		if (!Is7zip || Config.SourceFiles.Count <= 10000 || AppCore.Logger.ShowDialog(AppSetting.Instance.GetIlogger()?.GetStr("mess_ImportFileCountMoreThan10000")) == MessageResult.Yes)
		{
			AppCore.ViewModelBase.PVF.ImportFiles(Config, Is7zip, FilePathFrom7zip);
			AppSetting.Instance.SaveSetting();
			AppCore.ViewModelBase.DockLayoutManagerService.ClosePanel(this);
		}
	}

	[Command]
	public void OnSelectTarget()
	{
		string text = AppCore.SelectPvfFolderPath(Application.Current.MainWindow);
		if (text != null)
		{
			Config.TargetPath = text;
		}
	}

	public ConcurrentBag<ImportFileItem> Load7zipFile(string filePath)
	{
		SevenZipBase.SetLibraryPath(Environment.Is64BitProcess ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7z64.dll") : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7z.dll"));
		ConcurrentBag<ImportFileItem> concurrentBag = new ConcurrentBag<ImportFileItem>();
		using SevenZipExtractor sevenZipExtractor = new SevenZipExtractor(filePath);
		foreach (ArchiveFileInfo archiveFileDatum in sevenZipExtractor.ArchiveFileData)
		{
			concurrentBag.Add(new ImportFileItem(archiveFileDatum.FileName, string.Empty, archiveFileDatum.Index));
		}
		return concurrentBag;
	}

	private byte[] ExtractArchiveFile(string filePath)
	{
		SevenZipBase.SetLibraryPath(Environment.Is64BitProcess ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7z64.dll") : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "7z.dll"));
		using SevenZipExtractor sevenZipExtractor = new SevenZipExtractor(FilePathFrom7zip);
		ArchiveFileInfo archiveFileInfo = sevenZipExtractor.ArchiveFileData.ToList().Find(it => it.FileName == filePath);
		using MemoryStream memoryStream = new MemoryStream();
		sevenZipExtractor.ExtractFile(archiveFileInfo.Index, memoryStream);
		return memoryStream.ToArray();
	}

	public override void Dispose()
	{
		ResetImportFiles();
		TreeViewModel.EventDropFile -= OnFilesDropped;
		TreeViewModel.EventOpenDocumenting -= OpenImportFile;
	}
}
