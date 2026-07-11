using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
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
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass27_0
	{
		public KeyValuePair<string, ItemCodeHoverInfoBase> g0OqfXLVVQ;

		public _003C_003Ec__DisplayClass27_0()
		{
		}

		internal bool B2Lq2HWWHv(KeyValuePair<string, ItemCodeHoverInfoBase> it)
		{
			return it.Key == g0OqfXLVVQ.Key;
		}
	}

	private bool IsLoaded;

	[CompilerGenerated]
	private ConcurrentObservableCollection<StoreListDto> c1KWWwkgbq;

	[CompilerGenerated]
	private StoreListDto FuLWmZ03Lu;

	[CompilerGenerated]
	private GetStoreListQueryData LscW294dAV;

	public ConcurrentObservableCollection<StoreListDto> Items
	{
		[CompilerGenerated]
		get
		{
			return c1KWWwkgbq;
		}
		[CompilerGenerated]
		set
		{
			c1KWWwkgbq = value;
		}
	}

	public StoreListDto SelectedItem
	{
		[CompilerGenerated]
		get
		{
			return FuLWmZ03Lu;
		}
		[CompilerGenerated]
		set
		{
			FuLWmZ03Lu = value;
		}
	}

	public GetStoreListQueryData QueryData
	{
		[CompilerGenerated]
		get
		{
			return LscW294dAV;
		}
		[CompilerGenerated]
		set
		{
			LscW294dAV = value;
		}
	}

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
		QueryData.StoreTypeChanged += kKHWlDZR5V;
		if (!IsLoaded)
		{
			IsLoaded = true;
			lock (this)
			{
				kKHWlDZR5V(QueryData.Type);
			}
		}
	}

	private void kKHWlDZR5V(StoreType P_0)
	{
		if (P_0 == StoreType.书签 || (uint)(P_0 - 2) <= 2u)
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
				await KGTWFiFwk0(resultData.Data);
				break;
			case StoreType.宏:
				await Rt0Wr1r4p3(resultData.Data);
				break;
			case StoreType.文件资源管理器注释:
				await JH0WBZV7pc(resultData.Data);
				break;
			case StoreType.脚本文件标签翻译:
				await G6HWvWAWkv(resultData.Data);
				break;
			case StoreType.代码智能提示:
				await dGuWhH83bh(resultData.Data);
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
					hx0Wj2L0Zi(resultData.Data);
					break;
				case StoreType.文件资源管理器注释:
					NlFWTuGIbi(resultData.Data);
					break;
				case StoreType.脚本文件标签翻译:
					QYhWCGRMKu(resultData.Data);
					break;
				case StoreType.代码智能提示:
					EMCWH9xiI6(resultData.Data);
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

	private void hx0Wj2L0Zi(StoreListDto P_0)
	{
		BookMarkGroupDto previewSource = new BookMarkGroupDto
		{
			Trees = P_0.ToData<ObservableConcurrentDictionaryEx<string, BookMarkDto>>()
		};
		BookMarkEditView bookMarkEditView = new BookMarkEditView(isTreeList: false, isPreview: true, previewSource);
		bookMarkEditView.Owner = Application.Current.MainWindow;
		bookMarkEditView.WindowStartupLocation = WindowStartupLocation.CenterOwner;
		bookMarkEditView.Show();
	}

	private void NlFWTuGIbi(StoreListDto P_0)
	{
		PreviewFileListComment previewFileListComment = new PreviewFileListComment(P_0.ToData<Dictionary<string, TreelistCommentRes>>());
		previewFileListComment.Owner = Application.Current.MainWindow;
		previewFileListComment.WindowStartupLocation = WindowStartupLocation.CenterOwner;
		previewFileListComment.Show();
	}

	private void QYhWCGRMKu(StoreListDto P_0)
	{
		PreviewTabCommentView previewTabCommentView = new PreviewTabCommentView(P_0.ToData<List<PvfCommentDto>>());
		previewTabCommentView.Owner = Application.Current.MainWindow;
		previewTabCommentView.WindowStartupLocation = WindowStartupLocation.CenterOwner;
		previewTabCommentView.Show();
	}

	private void EMCWH9xiI6(StoreListDto P_0)
	{
		JsonSerializerSettings settings = new JsonSerializerSettings
		{
			TypeNameHandling = TypeNameHandling.All
		};
		P_0.Data.ToString();
		Dictionary<string, List<KeyValuePair<string, ItemCodeHoverInfoBase>>> source = JsonConvert.DeserializeObject<Dictionary<string, List<KeyValuePair<string, ItemCodeHoverInfoBase>>>>(P_0.Data.ToString(), settings);
		PreviewItemCodeHoverView previewItemCodeHoverView = new PreviewItemCodeHoverView(AppSetting.Instance.EditConfig.ItemCodeConvertItemNameConfiger.ModelToXml(source));
		previewItemCodeHoverView.Owner = Application.Current.MainWindow;
		previewItemCodeHoverView.WindowStartupLocation = WindowStartupLocation.CenterOwner;
		previewItemCodeHoverView.Show();
	}

	private async Task dGuWhH83bh(StoreListDto P_0)
	{
		bool value = new StoreDownLoadOptions(AppSetting.Instance.GetIlogger()?.GetStr("mess_MergeConfig"), AppSetting.Instance.GetIlogger()?.GetStr("mess_OverrideConfig"))
		{
			WindowStartupLocation = WindowStartupLocation.CenterOwner,
			Owner = Application.Current.MainWindow
		}.ShowDialog().Value;
		Dictionary<string, List<KeyValuePair<string, ItemCodeHoverInfoBase>>> dictionary = JsonConvert.DeserializeObject<Dictionary<string, List<KeyValuePair<string, ItemCodeHoverInfoBase>>>>(settings: new JsonSerializerSettings
		{
			TypeNameHandling = TypeNameHandling.All
		}, value: P_0.Data.ToString());
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
					using List<KeyValuePair<string, ItemCodeHoverInfoBase>>.Enumerator enumerator2 = item.Value.GetEnumerator();
					while (enumerator2.MoveNext())
					{
						_003C_003Ec__DisplayClass27_0 CS_0024_003C_003E8__locals5 = new _003C_003Ec__DisplayClass27_0();
						CS_0024_003C_003E8__locals5.g0OqfXLVVQ = enumerator2.Current;
						List<KeyValuePair<string, ItemCodeHoverInfoBase>> list2 = list.FindAll((KeyValuePair<string, ItemCodeHoverInfoBase> it) => it.Key == CS_0024_003C_003E8__locals5.g0OqfXLVVQ.Key);
						if (list2 == null)
						{
							list.Add(CS_0024_003C_003E8__locals5.g0OqfXLVVQ);
							continue;
						}
						bool flag = true;
						foreach (KeyValuePair<string, ItemCodeHoverInfoBase> item2 in list2)
						{
							if (item2.Value.EqualsData(item2, xmlDocument, xmlDocument.CreateElement("File"), CS_0024_003C_003E8__locals5.g0OqfXLVVQ))
							{
								flag = false;
								break;
							}
						}
						if (flag)
						{
							list.Add(CS_0024_003C_003E8__locals5.g0OqfXLVVQ);
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

	private async Task G6HWvWAWkv(StoreListDto P_0)
	{
		List<PvfCommentDto> list = P_0.ToData<List<PvfCommentDto>>();
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

	private async Task JH0WBZV7pc(StoreListDto P_0)
	{
		bool value = new StoreDownLoadOptions(AppSetting.Instance.GetIlogger()?.GetStr("mess_MergeConfigComment"), AppSetting.Instance.GetIlogger()?.GetStr("mess_OverrideConfigComment"))
		{
			WindowStartupLocation = WindowStartupLocation.CenterOwner,
			Owner = Application.Current.MainWindow
		}.ShowDialog().Value;
		Dictionary<string, TreelistCommentRes> dictionary = P_0.ToData<Dictionary<string, TreelistCommentRes>>();
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

	private async Task KGTWFiFwk0(StoreListDto P_0)
	{
		bool value = new WinDownLoadBookMarkOptions
		{
			WindowStartupLocation = WindowStartupLocation.CenterScreen,
			Owner = Application.Current.MainWindow
		}.ShowDialog().Value;
		AppSetting.Instance.BookMarkGroup.DownSaveBookMark(P_0.ToData<ObservableConcurrentDictionaryEx<string, BookMarkDto>>(), value);
		await AppSetting.Instance.SaveSetting();
		AppCore.ShowMsg(AppSetting.Instance.GetIlogger()?.GetStr("mess_DownloadSuccess"));
	}

	private async Task Rt0Wr1r4p3(StoreListDto P_0)
	{
		await AppCore.SaveMacroData(P_0.ToData<MacroData>(), P_0.Title, AppCore.MainWin, showShareCheckBox: false);
		await AppSetting.Instance.SaveSetting();
	}

	[Command]
	public void Unloaded()
	{
		QueryData.StoreTypeChanged -= kKHWlDZR5V;
	}

	public override void Dispose()
	{
	}
}
