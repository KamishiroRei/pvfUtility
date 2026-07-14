using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using DevExpress.Mvvm.DataAnnotations;
using Newtonsoft.Json;
using PvfCode.Dot;
using PvfCode.Dot.Desktop;
using PvfCode.Dot.Desktop.Enums;
using PvfCode.Models.Macro;
using PvfCode.Models.Options.Editor.ItemCodeHoverConfigModels;
using PvfCode.ViewModels.DocumentFolder;
using PvfCode.ViewModels.DocumentFolder.Enums;
using PvfCode.Views.BookMark;
using PvfCode.Views.Store;
using PvfCode.Views.Store.Preview;
using Swordfish.NET.Collections;
using Utools;

namespace PvfCode.ViewModels.Store;

public class StoreListViewModel : DocumentBase
{
	private bool IsLoaded;

	public ConcurrentObservableCollection<StoreListDto> Items { get; set; }

	public StoreListDto SelectedItem { get; set; }

	public GetStoreListQueryData QueryData { get; set; }

	public bool PreviewButtonVisibility
	{
		get
		{
			return GetProperty(() => PreviewButtonVisibility);
		}
		set
		{
			SetProperty(() => PreviewButtonVisibility, value);
		}
	}

	public StoreListViewModel(StoreType storeType)
		: base(AppSetting.Instance.GetIlogger()?.GetStr("DocumentName_ExtensionStore"))
	{
		base.DocumentType = PvfFileDocumentType.商店;
		QueryData = new GetStoreListQueryData
		{
			Type = storeType
		};
		Items = new ConcurrentObservableCollection<StoreListDto>();
	}

	[Command]
	public void Loaded()
	{
		QueryData.StoreTypeChanged += OnStoreTypeChanged;
		if (!IsLoaded)
		{
			IsLoaded = true;
			lock (this)
			{
				OnStoreTypeChanged(QueryData.Type);
			}
		}
	}

	private void OnStoreTypeChanged(StoreType storeType)
	{
		if (storeType == StoreType.书签 || (uint)(storeType - 2) <= 2u)
		{
			PreviewButtonVisibility = true;
		}
		else
		{
			PreviewButtonVisibility = false;
		}
		lock (this)
		{
			OnRefSource();
		}
	}

	[Command]
	public async void OnDown(StoreListDto storeListDto)
	{
		StoreListDto storeListDto2 = ((storeListDto == null) ? SelectedItem : storeListDto);
		if (storeListDto2 == null)
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseSelectDownloadData"));
			return;
		}
		base.IsLoading = true;
		WindowLoading win = AppCore.CreateLoading(AppSetting.Instance.GetIlogger()?.GetStr("mess_Downloading"), Application.Current.MainWindow);
		win.Show();
		ResultData<StoreListDto> resultData = await ServiceCloud.Instance.GetStoreData(storeListDto2.Id);
		if (resultData.IsError)
		{
			win.Close();
			AppCore.ShowMsg(resultData.Msg, isError: true);
		}
		else
		{
			win.Close();
			switch (resultData.Data.Type)
			{
			case StoreType.书签:
				await DownloadBookMarksAsync(resultData.Data);
				break;
			case StoreType.宏:
				await DownloadMacroAsync(resultData.Data);
				break;
			case StoreType.文件资源管理器注释:
				await DownloadFileListCommentsAsync(resultData.Data);
				break;
			case StoreType.脚本文件标签翻译:
				await DownloadTabCommentsAsync(resultData.Data);
				break;
			case StoreType.代码智能提示:
				await DownloadItemCodeHoverAsync(resultData.Data);
				break;
			}
		}
		base.IsLoading = false;
	}

	[Command]
	public async void OnPreview(StoreListDto storeListDto)
	{
		StoreListDto storeListDto2 = ((storeListDto == null) ? SelectedItem : storeListDto);
		if (storeListDto2 == null)
		{
			AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PleaseSelectPreviewData"));
			return;
		}
		base.IsLoading = true;
		WindowLoading win = AppCore.CreateLoading(AppSetting.Instance.GetIlogger()?.GetStr("mess_Loading"), Application.Current.MainWindow);
		win.Show();
		ResultData<StoreListDto> resultData = await ServiceCloud.Instance.GetPreviewStoreData(storeListDto2.Id);
		if (resultData.IsError)
		{
			win.Close();
			AppCore.ShowMsg(resultData.Msg, isError: true);
		}
		else
		{
			win.Close();
			if (resultData.Data == null)
			{
				AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_PreviewFailed"));
			}
			else
			{
				switch (resultData.Data.Type)
				{
				case StoreType.书签:
					PreviewBookMarks(resultData.Data);
					break;
				case StoreType.文件资源管理器注释:
					PreviewFileListComments(resultData.Data);
					break;
				case StoreType.脚本文件标签翻译:
					PreviewTabComments(resultData.Data);
					break;
				case StoreType.代码智能提示:
					PreviewItemCodeHover(resultData.Data);
					break;
				}
			}
		}
		base.IsLoading = false;
	}

	[Command]
	public async void OnRefSource()
	{
		Items.Clear();
		base.IsLoading = true;
		ResultData<List<StoreListDto>> resultData = await ServiceCloud.Instance.GetStoreListData(QueryData);
		if (resultData.IsError)
		{
			AppCore.ShowMsg(resultData.Msg, isError: true);
		}
		else
		{
			Items.AddRange(resultData.Data);
		}
		base.IsLoading = false;
	}

	[Command]
	public async void OnDelete()
	{
		if (SelectedItem != null)
		{
			ResultData resultData = await ServiceCloud.Instance.DeleteStoreData(SelectedItem.Id);
			if (resultData.IsError)
			{
				AppCore.ShowMsg(resultData.Msg, isError: true);
			}
			else
			{
				Items?.Remove(SelectedItem);
			}
		}
	}

	private void PreviewBookMarks(StoreListDto storeData)
	{
		BookMarkGroupDto previewSource = new BookMarkGroupDto
		{
			Trees = storeData.ToData<ObservableConcurrentDictionaryEx<string, BookMarkDto>>()
		};
		BookMarkEditView bookMarkEditView = new BookMarkEditView(isTreeList: false, isPreview: true, previewSource);
		bookMarkEditView.Owner = Application.Current.MainWindow;
		bookMarkEditView.WindowStartupLocation = WindowStartupLocation.CenterOwner;
		bookMarkEditView.Show();
	}

	private void PreviewFileListComments(StoreListDto storeData)
	{
		PreviewFileListComment previewFileListComment = new PreviewFileListComment(storeData.ToData<Dictionary<string, TreelistCommentRes>>());
		previewFileListComment.Owner = Application.Current.MainWindow;
		previewFileListComment.WindowStartupLocation = WindowStartupLocation.CenterOwner;
		previewFileListComment.Show();
	}

	private void PreviewTabComments(StoreListDto storeData)
	{
		PreviewTabCommentView previewTabCommentView = new PreviewTabCommentView(storeData.ToData<List<PvfCommentDto>>());
		previewTabCommentView.Owner = Application.Current.MainWindow;
		previewTabCommentView.WindowStartupLocation = WindowStartupLocation.CenterOwner;
		previewTabCommentView.Show();
	}

	private void PreviewItemCodeHover(StoreListDto storeData)
	{
		JsonSerializerSettings settings = new JsonSerializerSettings
		{
			TypeNameHandling = TypeNameHandling.All
		};
		storeData.Data.ToString();
		Dictionary<string, List<KeyValuePair<string, ItemCodeHoverInfoBase>>> source = JsonConvert.DeserializeObject<Dictionary<string, List<KeyValuePair<string, ItemCodeHoverInfoBase>>>>(storeData.Data.ToString(), settings);
		PreviewItemCodeHoverView previewItemCodeHoverView = new PreviewItemCodeHoverView(AppSetting.Instance.EditConfig.ItemCodeConvertItemNameConfiger.ModelToXml(source));
		previewItemCodeHoverView.Owner = Application.Current.MainWindow;
		previewItemCodeHoverView.WindowStartupLocation = WindowStartupLocation.CenterOwner;
		previewItemCodeHoverView.Show();
	}

	private async Task DownloadItemCodeHoverAsync(StoreListDto storeData)
	{
		bool value = new StoreDownLoadOptions(AppSetting.Instance.GetIlogger()?.GetStr("mess_MergeConfig"), AppSetting.Instance.GetIlogger()?.GetStr("mess_OverrideConfig"))
		{
			WindowStartupLocation = WindowStartupLocation.CenterOwner,
			Owner = Application.Current.MainWindow
		}.ShowDialog().Value;
		Dictionary<string, List<KeyValuePair<string, ItemCodeHoverInfoBase>>> dictionary = JsonConvert.DeserializeObject<Dictionary<string, List<KeyValuePair<string, ItemCodeHoverInfoBase>>>>(settings: new JsonSerializerSettings
		{
			TypeNameHandling = TypeNameHandling.All
		}, value: storeData.Data.ToString());
		if (value)
		{
			XmlDocument xmlDocument = new XmlDocument();
			XmlDeclaration newChild = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
			xmlDocument.AppendChild(newChild);
			XmlElement newChild2 = xmlDocument.CreateElement("", "root", "");
			xmlDocument.AppendChild(newChild2);
			foreach (KeyValuePair<string, List<KeyValuePair<string, ItemCodeHoverInfoBase>>> item in dictionary)
			{
				if (AppSetting.Instance.EditConfig.ItemCodeConvertItemNameConfiger.DIC.ContainsKey(item.Key))
				{
					List<KeyValuePair<string, ItemCodeHoverInfoBase>> list = AppSetting.Instance.EditConfig.ItemCodeConvertItemNameConfiger.DIC[item.Key];
					foreach (KeyValuePair<string, ItemCodeHoverInfoBase> incomingItem in item.Value)
					{
						List<KeyValuePair<string, ItemCodeHoverInfoBase>> list2 = list.FindAll(it => it.Key == incomingItem.Key);
						if (list2 == null)
						{
							list.Add(incomingItem);
							continue;
						}
						bool flag = true;
						foreach (KeyValuePair<string, ItemCodeHoverInfoBase> item2 in list2)
						{
							if (item2.Value.EqualsData(item2, xmlDocument, xmlDocument.CreateElement("File"), incomingItem))
							{
								flag = false;
								break;
							}
						}
						if (flag)
						{
							list.Add(incomingItem);
						}
					}
				}
				else
				{
					AppSetting.Instance.EditConfig.ItemCodeConvertItemNameConfiger.DIC.Add(item.Key, item.Value);
				}
			}
		}
		else
		{
			AppSetting.Instance.EditConfig.ItemCodeConvertItemNameConfiger.DIC = dictionary;
		}
		string contents = AppSetting.Instance.EditConfig.ItemCodeConvertItemNameConfiger.ModelToXml(dictionary);
		await File.WriteAllTextAsync(AppSetting.Instance.EditConfig.ItemCodeConvertItemNameConfiger.SavePath, contents);
		AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_DownloadSuccess"));
	}

	private async Task DownloadTabCommentsAsync(StoreListDto storeData)
	{
		List<PvfCommentDto> list = storeData.ToData<List<PvfCommentDto>>();
		if (!new StoreDownLoadOptions(AppSetting.Instance.GetIlogger()?.GetStr("mess_MergeConfigComment"), AppSetting.Instance.GetIlogger()?.GetStr("mess_OverrideConfigComment"))
		{
			WindowStartupLocation = WindowStartupLocation.CenterOwner,
			Owner = Application.Current.MainWindow
		}.ShowDialog().Value)
		{
			await ServicePvfTabComment.Instance.ClearAddRanged(list);
		}
		else
		{
			foreach (PvfCommentDto item in list)
			{
				await ServicePvfTabComment.Instance.AddComment(item);
			}
		}
		await AppSetting.Instance.SaveSetting();
		AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_DownloadSuccess"));
	}

	private async Task DownloadFileListCommentsAsync(StoreListDto storeData)
	{
		bool value = new StoreDownLoadOptions(AppSetting.Instance.GetIlogger()?.GetStr("mess_MergeConfigComment"), AppSetting.Instance.GetIlogger()?.GetStr("mess_OverrideConfigComment"))
		{
			WindowStartupLocation = WindowStartupLocation.CenterOwner,
			Owner = Application.Current.MainWindow
		}.ShowDialog().Value;
		Dictionary<string, TreelistCommentRes> dictionary = storeData.ToData<Dictionary<string, TreelistCommentRes>>();
		if (value)
		{
			foreach (KeyValuePair<string, TreelistCommentRes> item in dictionary)
			{
				if (!AppSetting.Instance.PvfConfig.TreelistCommentDic.ContainsKey(item.Key))
				{
					AppSetting.Instance.PvfConfig.TreelistCommentDic.Add(item.Key, item.Value);
				}
			}
		}
		else
		{
			AppSetting.Instance.PvfConfig.TreelistCommentDic = dictionary;
		}
		await AppSetting.Instance.SaveSetting();
		AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_DownloadSuccess"));
	}

	private async Task DownloadBookMarksAsync(StoreListDto storeData)
	{
		bool value = new WinDownLoadBookMarkOptions
		{
			WindowStartupLocation = WindowStartupLocation.CenterScreen,
			Owner = Application.Current.MainWindow
		}.ShowDialog().Value;
		AppSetting.Instance.BookMarkGroup.DownSaveBookMark(storeData.ToData<ObservableConcurrentDictionaryEx<string, BookMarkDto>>(), value);
		await AppSetting.Instance.SaveSetting();
		AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_DownloadSuccess"));
	}

	private async Task DownloadMacroAsync(StoreListDto storeData)
	{
		await AppCore.SaveMacroData(storeData.ToData<MacroData>(), storeData.Title, AppCore.MainWin, showShareCheckBox: false);
		await AppSetting.Instance.SaveSetting();
	}

	[Command]
	public void Unloaded()
	{
		QueryData.StoreTypeChanged -= OnStoreTypeChanged;
	}

	public override void Dispose()
	{
	}
}
