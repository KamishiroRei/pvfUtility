using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using AutoMapper;
using PvfCode.Dot;
using PvfCode.Models.Pvf;
using PvfCode.Models.Pvf.Enums;
using PvfCode.Services.PvfParsingNew;
using PvfCode.Services.SearchModel;
using PvfCode.ViewModels.DocumentFolder;
using PvfCode.ViewModels.TreeFolder;
using PvfCode.Web.PvfEditHttpServiceModels;
using SqlSugar.Extensions;
using Utools;
using pvfUtility.WebApi.Dto;
using pvfUtility.WebApi.Dto.Res;

namespace PvfCode;

public class WebApiServer
{
	private static IMapper? mapper;

	private static WebApiServer instance;

	private HttpListener listener;

	private DateTime? lastIconRequestTime;

	public static WebApiServer Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new WebApiServer();
			}
			return instance;
		}
	}

	private PvfGroup Pvf => AppCore.ViewModelBase.PVF;

	private static IMapper Mapper
	{
		get
		{
			if (mapper == null)
			{
				mapper = new MapperConfiguration(delegate(IMapperConfigurationExpression cfg)
				{
					cfg.CreateMap<WebApiFileData, WebApiFileRootSectionData>();
				}).CreateMapper();
			}
			return mapper;
		}
	}

	public WebApiServer()
	{
		listener = new HttpListener();
	}

	public Task<ResultData> Start()
	{
		ResultData resultData = new ResultData();
		AppSetting.Instance.ClientApiOptions.StartLoading = true;
		lock (this)
		{
			Stop();
			EnsureAvailablePort();
			try
			{
				listener = new HttpListener();
				listener.Prefixes.Add($"http://127.0.0.1:{AppSetting.Instance.ClientApiOptions.Port}/");
				listener.Prefixes.Add($"http://localhost:{AppSetting.Instance.ClientApiOptions.Port}/");
				listener.Start();
				AppCore.Logger.Success(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_HTTPServiceStarted"), AppSetting.Instance.ClientApiOptions.Port));
				listener.BeginGetContext(HandleRequest, listener);
			}
			catch (Exception ex)
			{
				resultData.Msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_HTTPServiceStartError"), ex.Message);
				AppCore.Logger.Error(resultData.Msg);
			}
			AppSetting.Instance.ClientApiOptions.StartLoading = false;
		}
		return Task.FromResult(resultData);
	}

	public void Stop()
	{
		try
		{
			listener.Close();
		}
		catch (Exception)
		{
		}
	}

	private ResultData TreeNodeExists(string filePath)
	{
		ResultData resultData = new ResultData();
		try
		{
			if (!Pvf.PvfIsOpen)
			{
				resultData.Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_PleaseLoadPvfPackFirst");
			}
			if (string.IsNullOrEmpty(filePath))
			{
				throw new Exception("路径不能为空！");
			}
			if (!AppCore.ViewModelBase.PvfFileTreeViewModel.TreeGroupData.FilePathGetTreeNode(filePath).HasValue)
			{
				resultData.Msg = "不存在";
			}
		}
		catch (Exception ex)
		{
			resultData.Msg = ex.Message;
		}
		return resultData;
	}

	private ResultData NavigateToFile(string filePath, int openDocument)
	{
		ResultData resultData = new ResultData();
		try
		{
			if (!Pvf.PvfIsOpen)
			{
				resultData.Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_PleaseLoadPvfPackFirst");
			}
			if (string.IsNullOrEmpty(filePath))
			{
				throw new Exception("路径不能为空！");
			}
			if (openDocument == 1)
			{
				((DispatcherObject)Application.Current).Dispatcher.Invoke(() => AppCore.ViewModelBase.RootDocument.AddDocument(filePath, gotoNode: true));
			}
			else
			{
				AppCore.ViewModelBase.PvfFileTreeViewModel.GoToNode(filePath);
			}
		}
		catch (Exception ex)
		{
			resultData.Msg = ex.Message;
		}
		return resultData;
	}

	private ResultData<Dictionary<string, string>> GetIconsBase64(List<string> filePaths)
	{
		return ImagePack2Service.Instance.FilesToIconBase64(filePaths, Pvf);
	}

	private ResultData<Dictionary<int, ImagePack2Service.FilesToIconBase64Reponse>> GetIconsBase64New(List<string> filePaths)
	{
		return ImagePack2Service.Instance.FilesToIconBase64New(filePaths, Pvf);
	}

	private ResultData<IEnumerable<WebApiFileData>> GetFileData(string filePath)
	{
		ResultData<IEnumerable<WebApiFileData>> resultData = new ResultData<IEnumerable<WebApiFileData>>();
		if (!Pvf.PvfIsOpen)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_PleaseLoadPvfPackFirst");
			return resultData;
		}
		if (string.IsNullOrEmpty(filePath))
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_FilePathCannotBeEmpty");
			return resultData;
		}
		PvfFile file = Pvf.GetFile(filePath);
		if (file == null)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger()?.GetStr("mess_NotExist");
			return resultData;
		}
		ScriptFileParserNew scriptFileParserNew = new ScriptFileParserNew(file, Pvf);
		scriptFileParserNew.PraseStructureMain();
		resultData.Data = scriptFileParserNew.WebApiGetFileData();
		return resultData;
	}

	private ResultData<List<WebApiFileRootSectionData>> GetFileRootSections(string filePath)
	{
		ResultData<List<WebApiFileRootSectionData>> resultData = new ResultData<List<WebApiFileRootSectionData>>();
		if (!Pvf.PvfIsOpen)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_PleaseLoadPvfPackFirst");
			return resultData;
		}
		if (string.IsNullOrEmpty(filePath))
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_FilePathCannotBeEmpty");
			return resultData;
		}
		PvfFile file = Pvf.GetFile(filePath);
		if (file == null)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger()?.GetStr("mess_NotExist");
			return resultData;
		}
		ScriptFileParserNew scriptFileParserNew = new ScriptFileParserNew(file, Pvf);
		scriptFileParserNew.PraseStructureMain();
		List<WebApiFileData> list = scriptFileParserNew.WebApiGetFileData();
		if (list != null)
		{
			List<WebApiFileRootSectionData> list2 = Mapper.Map<List<WebApiFileRootSectionData>>(list);
			list2.RemoveAll((WebApiFileRootSectionData x) => !x.IsSection);
			resultData.Data = list2;
		}
		return resultData;
	}

	public ResultData<IEnumerable<string>> GetStringTable()
	{
		ResultData<IEnumerable<string>> resultData = new ResultData<IEnumerable<string>>();
		if (!Pvf.PvfIsOpen)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_PleaseLoadPvfPackFirst");
			return resultData;
		}
		string filePath = "stringtable.bin";
		if (!Pvf.FileAny(filePath))
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_FileNotExistTitle");
			return resultData;
		}
		resultData.Data = Pvf.Strtable.WebApiGetStringTab();
		return resultData;
	}

	private async Task<ResultData<Dictionary<int, LstFileInfo>>> GetLstFileRows(string filePath)
	{
		ResultData<Dictionary<int, LstFileInfo>> re = new ResultData<Dictionary<int, LstFileInfo>>();
		if (!Pvf.PvfIsOpen)
		{
			re.Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_PleaseLoadPvfPackFirst");
			return re;
		}
		if (!Pvf.FileAny(filePath))
		{
			re.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_FileNotExistTitle");
			return re;
		}
		PvfFile file = Pvf.GetFile(filePath);
		if (file.FileType != PvfFileType.lst)
		{
			re.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_FileNotLst");
			return re;
		}
		List<KeyValuePair<int, LstItem>> list = await Pvf.LstFileToLstTab(file);
		if (list == null)
		{
			re.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_FileNoLst");
			return re;
		}
		Dictionary<int, LstFileInfo> dictionary = new Dictionary<int, LstFileInfo>();
		foreach (KeyValuePair<int, LstItem> item in list)
		{
			if (!dictionary.ContainsKey(item.Key))
			{
				dictionary.Add(item.Key, new LstFileInfo
				{
					ItemCode = item.Key,
					PathHeader = item.Value.Header,
					ItemPath = item.Value.ItemPath,
					FullPath = item.Value.GetFullPath(Pvf, file),
					ItemName = Pvf.GetItemName(item.Value.FullPath)
				});
			}
		}
		re.Data = dictionary;
		return re;
	}

	private ResultData<List<string>> GetRootDirectories()
	{
		ResultData<List<string>> resultData = new ResultData<List<string>>();
		if (!Pvf.PvfIsOpen)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_PleaseLoadPvfPackFirst");
			return resultData;
		}
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, PvfTreeFileBase> item in (IEnumerable<KeyValuePair<string, PvfTreeFileBase>>)AppCore.ViewModelBase.PvfFileTreeViewModel.TreeGroupData.Trees)
		{
			if (!item.Value.IsFile)
			{
				list.Add(item.Value.FullPath);
			}
		}
		resultData.Data = list;
		return resultData;
	}

	private ResultData<string> GetVersion()
	{
		return new ResultData<string>
		{
			Data = Application.ResourceAssembly.GetName().Version.ToString()
		};
	}

	private async Task<ResultData<string>> GetIconBase64(string filePath)
	{
		ResultData<string> resultData = new ResultData<string>();
		if (!Pvf.PvfIsOpen)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_PleaseLoadPvfPackFirst");
			return resultData;
		}
		if (string.IsNullOrEmpty(filePath))
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_FilePathCannotBeEmpty");
			return resultData;
		}
		if (!Pvf.FileList.TryGetValue(filePath, out PvfFile value))
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_FileNotExistTitle");
			return resultData;
		}
		if (lastIconRequestTime.HasValue && lastIconRequestTime.Value.AddSeconds(0.5) > DateTime.Now)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_Interval");
			return resultData;
		}
		if (AppSetting.Instance.ImagePacks2Options.ImgCount == 0)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_UserNotLoadImagePack2ModelDir");
			return resultData;
		}
		lastIconRequestTime = DateTime.Now;
		if (!value.GetIcon(Pvf, out KeyValuePair<string, int>? icon))
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_FileNoIconTag");
			return resultData;
		}
		ResultData<Bitmap> image = ImagePack2Service.Instance.GetImage2(icon.Value.Key, icon.Value.Value);
		if (image == null || image.Data == null)
		{
			resultData.Msg = string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_ImagePacks2ModelDirNoIcon"), icon.Value.Key, icon.Value.Value);
			return resultData;
		}
		resultData.Data = ImageHelper.BytesToBase64Image(ImageHelper.BitmapToBytes(image.Data));
		return resultData;
	}

	private ResultData<ItemCodeToFileInfoDto> ItemCodeToFileInfo(string lstName, int? itemCode)
	{
		ResultData<ItemCodeToFileInfoDto> resultData = new ResultData<ItemCodeToFileInfoDto>();
		if (string.IsNullOrEmpty(lstName))
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_LstNameCannotBeEmpty");
			return resultData;
		}
		if (!itemCode.HasValue)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_ItemCodeCannotBeEmpty");
			return resultData;
		}
		string[] lstNames = lstName.Split(",", StringSplitOptions.RemoveEmptyEntries);
		PvfGroup pVF = AppCore.ViewModelBase.PVF;
		if (!pVF.PvfIsOpen)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_PleaseLoadPvfPackFirst");
			return resultData;
		}
		LstItem lstItem = pVF.ListFileTable.GetLstItem(lstNames, itemCode.Value);
		if (lstItem == null)
		{
			resultData.Msg = string.Format(AppSetting.Instance.GetIlogger().GetStr("mess_ItemCodeNoFound"), lstName, itemCode);
			return resultData;
		}
		resultData.Data = new ItemCodeToFileInfoDto
		{
			FilePath = lstItem.FullPath,
			ItemName = pVF.GetItemName(lstItem.FullPath),
			Path = lstItem.ItemPath
		};
		return resultData;
	}

	private ResultData<ItemCodesToFileInfosDto> ItemCodesToFileInfos(ItemCodesToFileInfosRes request)
	{
		ResultData<ItemCodesToFileInfosDto> resultData = new ResultData<ItemCodesToFileInfosDto>();
		if (request.lstNames == null || request.lstNames.Count == 0)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_LstNameCannotBeEmpty2");
			return resultData;
		}
		if (request.ItemCodes == null || request.ItemCodes.Count == 0)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_ItemCodesCannotBeEmpty");
			return resultData;
		}
		PvfGroup pVF = AppCore.ViewModelBase.PVF;
		if (!pVF.PvfIsOpen)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_PleaseLoadPvfPackFirst");
			return resultData;
		}
		Dictionary<int, ItemCodeToFileInfoDto> dictionary = new Dictionary<int, ItemCodeToFileInfoDto>();
		foreach (int itemCode in request.ItemCodes)
		{
			if (!dictionary.ContainsKey(itemCode))
			{
				string text = pVF.ListFileTable.ItemCodeConvertFilePath(request.lstNames, itemCode);
				if (text != null)
				{
					dictionary.Add(itemCode, new ItemCodeToFileInfoDto
					{
						FilePath = text,
						ItemName = pVF.GetItemName(text)
					});
				}
			}
		}
		resultData.Data = new ItemCodesToFileInfosDto
		{
			Infos = dictionary
		};
		return resultData;
	}

	private ResultData<IEnumerable<string>> GetSelectedTreeFiles()
	{
		return new ResultData<IEnumerable<string>>
		{
			Data = AppCore.ViewModelBase.PvfFileTreeViewModel.GetSelectedFilePaths(GetTreeType.File)
		};
	}

	private ResultData<IEnumerable<string>> GetSelectedSearchFiles()
	{
		return new ResultData<IEnumerable<string>>
		{
			Data = AppCore.ViewModelBase.SearchResultViewModel.TreeViewModel.GetSelectedFilePaths(GetTreeType.File)
		};
	}

	private ResultData<string?> GetSelectedTreeNode()
	{
		return new ResultData<string>
		{
			Data = AppCore.ViewModelBase.PvfFileTreeViewModel.SelectedNodeBindgBase?.Value?.FullPath
		};
	}

	private ResultData<string?> GetSelectedSearchNode()
	{
		return new ResultData<string>
		{
			Data = AppCore.ViewModelBase?.SearchResultViewModel?.TreeViewModel?.SelectedNodeBindgBase?.Value?.FullPath
		};
	}

	public ResultData FileIsExists(string filePath)
	{
		ResultData resultData = new ResultData();
		if (!Pvf.PvfIsOpen)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_PleaseLoadPvfPackFirst");
		}
		else if (!Pvf.FileAny(filePath))
		{
			resultData.Msg = AppSetting.Instance.GetIlogger()?.GetStr("mess_NotExist");
		}
		return resultData;
	}

	private object GetFiles(string? dirName, int asText, string fileType)
	{
		PvfFileType? pvfFileType = null;
		if (!string.IsNullOrEmpty(fileType) && AppSetting.Instance.PvfConfig.PvfFileTypeDic.TryGetValue(fileType, out var value))
		{
			pvfFileType = value;
		}
		if (asText != 0)
		{
			return GetFilesAsText(dirName, pvfFileType);
		}
		return GetFilesList(dirName, pvfFileType);
	}

	private ResultData<IEnumerable<string>> GetFilesList(string directory, PvfFileType? fileType)
	{
		ResultData<IEnumerable<string>> resultData = new ResultData<IEnumerable<string>>();
		if (!Pvf.PvfIsOpen)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_PleaseLoadPvfPackFirst");
		}
		else
		{
			resultData.Data = Pvf.GetFiles(directory, fileType);
		}
		return resultData;
	}

	private ResultData<string> GetFilesAsText(string directory, PvfFileType? fileType)
	{
		ResultData<string> resultData = new ResultData<string>();
		if (!Pvf.PvfIsOpen)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_PleaseLoadPvfPackFirst");
		}
		else
		{
			IEnumerable<string> files = Pvf.GetFiles(directory, fileType);
			if (files != null && files.Any())
			{
				resultData.Data = string.Join("\r\n", files);
			}
		}
		return resultData;
	}

	private ResultData<IEnumerable<string>> GetLstFilePaths()
	{
		ResultData<IEnumerable<string>> resultData = new ResultData<IEnumerable<string>>();
		if (!Pvf.PvfIsOpen)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_PleaseLoadPvfPackFirst");
			return resultData;
		}
		resultData.Data = Pvf.ListFileTable.LstFilePaths.Values;
		return resultData;
	}

	private ResultData<string> GetFileContent(string filePath, bool useCompatibleDecompiler, string? encodingType)
	{
		ResultData<string> resultData = new ResultData<string>();
		if (!Pvf.PvfIsOpen)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_PleaseLoadPvfPackFirst");
			return resultData;
		}
		if (!Pvf.FileAny(filePath))
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_FileNotExistTitle");
			return resultData;
		}
		EncodingType? encoding = null;
		if (!string.IsNullOrEmpty(encodingType) && encodingType.ToLower() != "null")
		{
			if (!Enum.TryParse(typeof(EncodingType), encodingType, out object result))
			{
				resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_EncodingError");
				return resultData;
			}
			encoding = (EncodingType?)result;
		}
		resultData.Data = Pvf.GetFileText(filePath, encoding, useCompatibleDecompiler, showAniError: false);
		return resultData;
	}

	private ResultData<FileContentListDto> GetFileContents(GetFileContentRes request)
	{
		ResultData<FileContentListDto> resultData = new ResultData<FileContentListDto>
		{
			Data = new FileContentListDto
			{
				FileContentData = new Dictionary<string, string>()
			}
		};
		if (request.FileList == null || !request.FileList.Any())
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_RequestContentCannotBeEmpty");
			return resultData;
		}
		if (!Pvf.PvfIsOpen)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_PleaseLoadPvfPackFirst");
			return resultData;
		}
		FileContentListDto fileContentListDto = new FileContentListDto
		{
			FileContentData = new Dictionary<string, string>()
		};
		EncodingType? encoding = null;
		if (!string.IsNullOrEmpty(request.EncodingType))
		{
			if (!Enum.TryParse(typeof(EncodingType), request.EncodingType, out object result))
			{
				resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_EncodingError");
				return resultData;
			}
			encoding = (EncodingType?)result;
		}
		foreach (string file in request.FileList)
		{
			if (Pvf.FileList.TryGetValue(file, out PvfFile value))
			{
				string fileText = Pvf.GetFileText(value, encoding, request.UseCompatibleDecompiler, showAniError: false);
				if (!fileContentListDto.FileContentData.ContainsKey(file))
				{
					fileContentListDto.FileContentData.Add(file, fileText);
				}
			}
		}
		resultData.Data = fileContentListDto;
		return resultData;
	}

	private ResultData<ItemInfoDto> GetItemInfo(string filePath)
	{
		ResultData<ItemInfoDto> resultData = new ResultData<ItemInfoDto>();
		if (!Pvf.PvfIsOpen)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_PleaseLoadPvfPackFirst");
			return resultData;
		}
		if (Pvf.FileList.TryGetValue(filePath, out PvfFile value))
		{
			resultData.Data = new ItemInfoDto
			{
				ItemCode = value.ItemCode,
				ItemName = Pvf.GetItemName(value)
			};
		}
		else
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_FileNotExistTitle");
		}
		return resultData;
	}

	private ResultData<Dictionary<string, ItemInfoDto>> GetItemInfos(IEnumerable<string> filePaths)
	{
		ResultData<Dictionary<string, ItemInfoDto>> resultData = new ResultData<Dictionary<string, ItemInfoDto>>();
		if (!Pvf.PvfIsOpen)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_PleaseLoadPvfPackFirst");
			return resultData;
		}
		Dictionary<string, ItemInfoDto> dictionary = new Dictionary<string, ItemInfoDto>();
		foreach (string item in filePaths)
		{
			if (Pvf.FileList.TryGetValue(item, out PvfFile value))
			{
				dictionary.Add(item, new ItemInfoDto
				{
					ItemCode = value.ItemCode,
					ItemName = Pvf.GetItemName(value)
				});
			}
		}
		resultData.Data = dictionary;
		return resultData;
	}

	private ResultData DeleteFile(string filePath)
	{
		ResultData resultData = new ResultData();
		if (!Pvf.PvfIsOpen)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_PleaseLoadPvfPackFirst");
			return resultData;
		}
		if (!Pvf.FileAny(filePath))
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_FileNotExistTitle");
		}
		else
		{
			lock (this)
			{
				Pvf.DeleteFile(filePath);
				AppCore.ViewModelBase.PvfFileTreeViewModel.TreeGroupData.DeleteTreeNode(new List<string> { filePath });
			}
		}
		return resultData;
	}

	private ResultData<IEnumerable<string>> DeleteFiles(IEnumerable<string> filePaths)
	{
		ResultData<IEnumerable<string>> resultData = new ResultData<IEnumerable<string>>();
		if (!Pvf.PvfIsOpen)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_PleaseLoadPvfPackFirst");
			return resultData;
		}
		lock (this)
		{
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			foreach (string item in filePaths)
			{
				if (Pvf.FileAny(item))
				{
					list.Add(item);
				}
				else
				{
					list2.Add(item);
				}
			}
			Pvf.DeleteFiles(list);
			AppCore.ViewModelBase.PvfFileTreeViewModel.TreeGroupData.DeleteTreeNode(list);
			resultData.Data = list2;
			return resultData;
		}
	}

	private async Task<ResultData> SavePvfPackAsync(string filePath)
	{
		ResultData resultData = new ResultData();
		if (string.IsNullOrEmpty(filePath))
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_FilePathCannotBeEmpty");
			return resultData;
		}
		if (string.IsNullOrEmpty(Path.GetFileName(filePath)))
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_FilePathNoFileName");
			return resultData;
		}
		if (!Pvf.PvfIsOpen)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_PleaseLoadPvfPackFirst");
			return resultData;
		}
		return await Task.Run(() => Pvf.SavePvfPack(filePath, isFastMode: false, AppCore.ViewModelBase.MainProgress));
	}

	private ResultData<string> GetPvfPath()
	{
		ResultData<string> resultData = new ResultData<string>();
		if (!Pvf.PvfIsOpen)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_PleaseLoadPvfPackFirst");
			return resultData;
		}
		resultData.Data = Pvf.PvfPackFilePath;
		return resultData;
	}

	private async Task<ResultData> ImportFile(string filePath, Stream stream)
	{
		ResultData result = new ResultData();
		if (!Pvf.PvfIsOpen)
		{
			result.Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_PleaseLoadPvfPackFirst");
			return result;
		}
		if (!(await AppCore.ViewModelBase.PvfFileTreeViewModel.WebApiImportFile(stream, filePath)))
		{
			result.Msg = AppSetting.Instance.GetIlogger()?.GetStr("mess_ImportFailed");
		}
		return result;
	}

	private async Task<ResultData<IEnumerable<string>>> ImportFiles(IEnumerable<ImportFileRes> fileDataList)
	{
		return await AppCore.ViewModelBase.PvfFileTreeViewModel.WebApiImportFiles(fileDataList);
	}

	private ResultData<ConcurrentDictionary<string, ConcurrentDictionary<int, string>>> FileListToLstRows(IEnumerable<string> filePaths)
	{
		ResultData<ConcurrentDictionary<string, ConcurrentDictionary<int, string>>> resultData = new ResultData<ConcurrentDictionary<string, ConcurrentDictionary<int, string>>>();
		if (!Pvf.PvfIsOpen)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_PleaseLoadPvfPackFirst");
			return resultData;
		}
		if (filePaths == null || !filePaths.Any())
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_FileListCannotBeEmpty");
			return resultData;
		}
		resultData.Data = Pvf.FilesToLstDic(filePaths);
		return resultData;
	}

	private ResultData<string> GetActiveDocumentPath()
	{
		ResultData<string> resultData = new ResultData<string>();
		ObservableCollection<DocumentBase> documents = AppCore.ViewModelBase.RootDocument.Documents;
		if (documents == null || !documents.Any())
		{
			return resultData;
		}
		foreach (DocumentBase item in documents)
		{
			if (item is PvfFileDocument { IsActive: not false } pvfFileDocument)
			{
				resultData.Data = pvfFileDocument.FullPath;
			}
		}
		return resultData;
	}

	private async Task<ResultData<List<string>>> SearchPvf(SearchConfig config)
	{
		ResultData<HashSet<string>> resultData = await Task.Run(() => new SearchService(config, Pvf).Search());
		return new ResultData<List<string>>
		{
			Data = ((resultData.Data == null) ? null : resultData.Data.ToList())
		};
	}

	private async void HandleRequest(IAsyncResult asyncResult)
	{
		HttpListenerResponse response = null;
		HttpListenerRequest request = null;
		string errorMsg = null;
		try
		{
			if (!listener.IsListening)
			{
				return;
			}
			listener.BeginGetContext(HandleRequest, null);
			HttpListenerContext httpListenerContext = listener.EndGetContext(asyncResult);
			response = httpListenerContext.Response;
			request = httpListenerContext.Request;
			response.AppendHeader("Access-Control-Allow-Origin", "*");
			using MemoryStream stream = new MemoryStream();
			string httpMethod = request.HttpMethod;
			if (!(httpMethod == "POST"))
			{
				if (!(httpMethod == "GET"))
				{
					if (httpMethod == "DELETE" && request.Url.AbsolutePath == "/file")
					{
						DeleteFileRequest(request.QueryString.Get("name"), httpListenerContext.Response);
					}
				}
				else
				{
					string absolutePath = request.Url.AbsolutePath;
					if (absolutePath != null)
					{
						switch (absolutePath.Length)
						{
						case 27:
							switch (absolutePath[24])
							{
							case 'i':
								if (absolutePath == "/Api/PvfUtiltiy/GetFileList")
								{
									object obj = GetFiles(request.QueryString.Get("dirName"), request.QueryString.Get("returnType").ToInt(), request.QueryString.Get("fileType"));
									WriteJson(obj, stream);
									response.StatusCode = 200;
								}
								break;
							case 'n':
								if (absolutePath == "/Api/PvfUtiltiy/GetItemInfo")
								{
									WriteJson(GetItemInfo(request.QueryString.Get("filePath")), stream);
									response.StatusCode = 200;
								}
								break;
							case 'c':
								if (absolutePath == "/Api/PvfUtiltiy/getFileIcon")
								{
									WriteJson(await GetIconBase64(request.QueryString.Get("filePath")), stream);
									response.StatusCode = 200;
								}
								break;
							case 'a':
								if (absolutePath == "/Api/PvfUtiltiy/getFileData")
								{
									WriteJson(GetFileData(request.QueryString.Get("filePath")), stream);
									response.StatusCode = 200;
								}
								break;
							}
							break;
						case 30:
							switch (absolutePath[19])
							{
							case 'F':
								if (absolutePath == "/Api/PvfUtiltiy/GetFileContent")
								{
									WriteJson(GetFileContent(request.QueryString.Get("filePath"), request.QueryString.Get("useCompatibleDecompiler").ToBoolen(), request.QueryString.Get("encodingType")), stream);
									response.StatusCode = 200;
								}
								break;
							case 'L':
								if (absolutePath == "/Api/PvfUtiltiy/getLstFileInfo")
								{
									WriteJson(await GetLstFileRows(request.QueryString.Get("filePath")), stream);
									response.StatusCode = 200;
								}
								break;
							case 'S':
								if (absolutePath == "/Api/PvfUtiltiy/getStringTable")
								{
									WriteJson(GetStringTable(), stream);
									response.StatusCode = 200;
								}
								break;
							}
							break;
						case 26:
							switch (absolutePath[16])
							{
							case 'D':
								if (absolutePath == "/Api/PvfUtiltiy/DeleteFile")
								{
									WriteJson(DeleteFile(request.QueryString.Get("filePath")), stream);
									response.StatusCode = 200;
								}
								break;
							case 'g':
								if (absolutePath == "/Api/PvfUtiltiy/getVersion")
								{
									WriteJson(GetVersion(), stream);
									response.StatusCode = 200;
								}
								break;
							}
							break;
						case 34:
							switch (absolutePath[16])
							{
							case 'G':
								if (absolutePath == "/Api/PvfUtiltiy/GetPvfPackFilePath")
								{
									WriteJson(GetPvfPath(), stream);
									response.StatusCode = 200;
								}
								break;
							case 'I':
								if (absolutePath == "/Api/PvfUtiltiy/ItemCodeToFileInfo")
								{
									string text = request.QueryString.Get("itemCode");
									WriteJson(ItemCodeToFileInfo(request.QueryString.Get("lstNames"), (text == null) ? ((int?)null) : ((!int.TryParse(text, out var result)) ? ((int?)null) : new int?(result))), stream);
									response.StatusCode = 200;
								}
								break;
							}
							break;
						case 28:
							switch (absolutePath[12])
							{
							case 't':
								if (absolutePath == "/Api/PvfUtiltiy/FileIsExists")
								{
									WriteJson(FileIsExists(request.QueryString.Get("filePath")), stream);
									response.StatusCode = 200;
								}
								break;
							case 'i':
								if (absolutePath == "/Api/PvfUtility/folderExists")
								{
									WriteJson(TreeNodeExists(request.QueryString.Get("filePath")), stream);
									response.StatusCode = 200;
								}
								break;
							}
							break;
						case 5:
							switch (absolutePath[1])
							{
							case 'f':
								if (absolutePath == "/file")
								{
									ExtractFile(httpListenerContext.Response, stream, request.QueryString.Get("name"));
								}
								break;
							case 'l':
								if (absolutePath == "/list")
								{
									GetDirectoryFiles(request, stream, httpListenerContext.Response);
								}
								break;
							}
							break;
						case 33:
							if (absolutePath == "/Api/PvfUtiltiy/GetAllLstFileList")
							{
								WriteJson(GetLstFilePaths(), stream);
								response.StatusCode = 200;
							}
							break;
						case 42:
							if (absolutePath == "/Api/PvfUtiltiy/GetTreeListFocusedFilePath")
							{
								WriteJson(GetSelectedTreeNode(), stream);
								response.StatusCode = 200;
							}
							break;
						case 53:
							if (absolutePath == "/Api/PvfUtiltiy/GetSearchPanelTreeListFocusedFilePath")
							{
								WriteJson(GetSelectedSearchNode(), stream);
								response.StatusCode = 200;
							}
							break;
						case 41:
							if (absolutePath == "/Api/PvfUtiltiy/GetActiveDocumentFilePath")
							{
								WriteJson(GetActiveDocumentPath(), stream);
								response.StatusCode = 200;
							}
							break;
						case 29:
							if (absolutePath == "/Api/PvfUtiltiy/SaveAsPvfFile")
							{
								WriteJson(await SavePvfPackAsync(request.QueryString.Get("filePath")), stream);
								response.StatusCode = 200;
							}
							break;
						case 36:
							if (absolutePath == "/Api/PvfUtiltiy/GetTreeSelectedFiles")
							{
								WriteJson(GetSelectedTreeFiles(), stream);
								response.StatusCode = 200;
							}
							break;
						case 43:
							if (absolutePath == "/Api/PvfUtiltiy/GetSearchPanelSelectedFiles")
							{
								WriteJson(GetSelectedSearchFiles(), stream);
								response.StatusCode = 200;
							}
							break;
						case 35:
							if (absolutePath == "/Api/PvfUtiltiy/getPvfRootDirectory")
							{
								WriteJson(GetRootDirectories(), stream);
								response.StatusCode = 200;
							}
							break;
						case 38:
							if (absolutePath == "/Api/PvfUtility/getFileRootSestionInfo")
							{
								WriteJson(GetFileRootSections(request.QueryString.Get("filePath")), stream);
								response.StatusCode = 200;
							}
							break;
						case 32:
							if (absolutePath == "/Api/PvfUtility/goToTreeListNode")
							{
								WriteJson(NavigateToFile(request.QueryString.Get("filePath"), request.QueryString.Get("openTextDocument").ObjToInt()), stream);
								response.StatusCode = 200;
							}
							break;
						case 11:
							if (absolutePath == "/listSearch")
							{
								GetAllFilePaths(request, stream, httpListenerContext.Response);
							}
							break;
						}
					}
				}
			}
			else
			{
				using StreamReader reader = new StreamReader(httpListenerContext.Request.InputStream, Encoding.UTF8);
				string requestBody = await reader.ReadToEndAsync();
				string absolutePath = request.Url.AbsolutePath;
				if (absolutePath != null)
				{
					switch (absolutePath.Length)
					{
					case 27:
						switch (absolutePath[16])
						{
						case 'D':
							if (!(absolutePath == "/Api/PvfUtiltiy/DeleteFiles"))
							{
								break;
							}
							WriteJson(DeleteFiles(requestBody.JsonToObject<IEnumerable<string>>()), stream);
							response.StatusCode = 200;
							goto end_IL_025d;
						case 'I':
							if (!(absolutePath == "/Api/PvfUtiltiy/ImportFiles"))
							{
								break;
							}
							WriteJson(await ImportFiles(requestBody.JsonToObject<IEnumerable<ImportFileRes>>()), stream);
							response.StatusCode = 200;
							goto end_IL_025d;
						}
						break;
					case 33:
						switch (absolutePath[16])
						{
						case 'F':
							if (!(absolutePath == "/Api/PvfUtiltiy/FileListToLstRows"))
							{
								break;
							}
							WriteJson(FileListToLstRows(requestBody.JsonToObject<IEnumerable<string>>()), stream);
							response.StatusCode = 200;
							goto end_IL_025d;
						case 'f':
							if (!(absolutePath == "/Api/PvfUtiltiy/filesToIconBase64"))
							{
								break;
							}
							WriteJson(await Task.Run(() => GetIconsBase64(requestBody.JsonToObject<List<string>>())), stream);
							response.StatusCode = 200;
							goto end_IL_025d;
						}
						break;
					case 36:
						switch (absolutePath[12])
						{
						case 't':
							if (!(absolutePath == "/Api/PvfUtiltiy/ItemCodesToFileInfos"))
							{
								break;
							}
							WriteJson(ItemCodesToFileInfos(requestBody.JsonToObject<ItemCodesToFileInfosRes>()), stream);
							response.StatusCode = 200;
							goto end_IL_025d;
						case 'i':
							if (!(absolutePath == "/Api/PvfUtility/filesToIconBase64New"))
							{
								break;
							}
							WriteJson(await Task.Run(() => GetIconsBase64New(requestBody.JsonToObject<List<string>>())), stream);
							response.StatusCode = 200;
							goto end_IL_025d;
						}
						break;
					case 5:
						if (!(absolutePath == "/file"))
						{
							break;
						}
						if (Pvf.PvfIsOpen)
						{
							await ImportFileRequest(response, request.QueryString.Get("name"), BytesHelper.StringToStream(requestBody));
						}
						goto end_IL_025d;
					case 26:
						if (!(absolutePath == "/Api/PvfUtiltiy/ImportFile"))
						{
							break;
						}
						WriteJson(await ImportFile(request.QueryString.Get("filePath"), BytesHelper.StringToStream(requestBody)), stream);
						response.StatusCode = 200;
						goto end_IL_025d;
					case 28:
						if (!(absolutePath == "/Api/PvfUtiltiy/GetItemInfos"))
						{
							break;
						}
						WriteJson(GetItemInfos(requestBody.JsonToObject<IEnumerable<string>>()), stream);
						response.StatusCode = 200;
						goto end_IL_025d;
					case 31:
						if (!(absolutePath == "/Api/PvfUtiltiy/GetFileContents"))
						{
							break;
						}
						WriteJson(GetFileContents(requestBody.JsonToObject<GetFileContentRes>()), stream);
						response.StatusCode = 200;
						goto end_IL_025d;
					case 25:
						if (!(absolutePath == "/Api/PvfUtiltiy/SearchPvf"))
						{
							break;
						}
						WriteJson(await SearchPvf(requestBody.JsonToObject<SearchConfig>()), stream);
						response.StatusCode = 200;
						goto end_IL_025d;
					}
				}
				if (request.Url.AbsolutePath == null || request.Url.AbsolutePath.Length == 0 || request.Url.AbsolutePath == "/")
				{
					PvfEditHttpCommand pvfEditHttpCommand = requestBody.JsonToObject<PvfEditHttpCommand>();
					string value = string.Empty;
					if (!Pvf.PvfIsOpen)
					{
						pvfEditHttpCommand.ErrorStr = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_PleaseLoadPvfPackFirst");
						value = GetPvfEditFileText(pvfEditHttpCommand);
					}
					else
					{
						switch (pvfEditHttpCommand.Cmd)
						{
						case Command.GetFileText:
							value = GetPvfEditFileText(pvfEditHttpCommand);
							break;
						case Command.GetFilePaths:
							value = GetPvfEditFilePaths(pvfEditHttpCommand, false);
							break;
						case Command.WriteFile:
							value = await PvfEditWriteFile(pvfEditHttpCommand);
							break;
						case Command.GetItemNameAndItemCode:
							value = GetPvfEditItemInfo(pvfEditHttpCommand);
							break;
						case Command.DeleteFiles:
							value = DeletePvfEditFiles(pvfEditHttpCommand);
							break;
						case Command.GetNowPvfPath:
							value = GetCurrentPvfPath(pvfEditHttpCommand);
							break;
						case Command.RefTreeDatas:
							((DispatcherObject)Application.Current).Dispatcher.Invoke((Action)delegate
							{
								AppCore.ViewModelBase.BarsVm.ApiRefTree();
							});
							break;
						case Command.GetTreeCheckedFiles:
							pvfEditHttpCommand.FilePaths = AppCore.ViewModelBase.PvfFileTreeViewModel.GetSelectedFilePaths(GetTreeType.File);
							pvfEditHttpCommand.Cmd = Command.GetFileText;
							value = pvfEditHttpCommand.ToJson();
							break;
						case Command.GetTreeCheckedFilesFromSearchResult:
							pvfEditHttpCommand.FilePaths = AppCore.ViewModelBase.SearchResultViewModel.TreeViewModel.GetSelectedFilePaths(GetTreeType.File);
							value = pvfEditHttpCommand.ToJson();
							break;
						case Command.GetFilePaths2:
							value = GetPvfEditFilePaths(pvfEditHttpCommand, true);
							break;
						default:
							pvfEditHttpCommand.ErrorStr = AppSetting.Instance.GetIlogger().GetStr("mess_UnrecognizedCommand");
							value = pvfEditHttpCommand.ToJson();
							break;
						}
					}
					StreamWriter streamWriter = new StreamWriter(stream);
					streamWriter.Write(value);
					streamWriter.Flush();
					response.StatusCode = 200;
				}
				end_IL_025d:;
			}
			response.ContentType = "application/json;charset=UTF-8";
			response.ContentEncoding = Encoding.UTF8;
			response.AppendHeader("Content-Type", "application/json;charset=UTF-8");
			if (response.StatusCode != 200)
			{
				response.StatusCode = 404;
			}
			response.ContentLength64 = stream.Length;
			await response.OutputStream.WriteAsync(stream.GetBuffer(), 0, (int)stream.Length);
		}
		catch (Exception ex)
		{
			if (request != null)
			{
				errorMsg = "请求类型：" + request.HttpMethod + " Url：" + request.Url.AbsolutePath;
			}
			errorMsg += ex.Message;
			if (!errorMsg.Contains("Cannot access a disposed object.\r\nObject name: 'System.Threading.ThreadPoolBoundHandle'.") && !errorMsg.Contains("The IAsyncResult object was not returned from the corresponding asynchronous method on this class. (Parameter 'asyncResult')"))
			{
				AppCore.Logger.Error(ex.Message);
			}
		}
		finally
		{
			response?.Close();
		}
	}

	private void WriteJson(object value, Stream stream)
	{
		StreamWriter streamWriter = new StreamWriter(stream);
		streamWriter.Write(value.ToJson());
		streamWriter.Flush();
	}

	private void GetAllFilePaths(HttpListenerRequest request, MemoryStream stream, HttpListenerResponse response)
	{
		if (!Pvf.PvfIsOpen)
		{
			response.StatusCode = 404;
			return;
		}
		StreamWriter streamWriter = new StreamWriter(stream);
		foreach (string key in Pvf.FileList.Keys)
		{
			streamWriter.WriteLine(key);
		}
		streamWriter.Flush();
		response.StatusCode = 200;
	}

	private void GetDirectoryFiles(HttpListenerRequest request, MemoryStream stream, HttpListenerResponse response)
	{
		if (Pvf.PvfIsOpen)
		{
			string path = request.QueryString.Get("path");
			StreamWriter streamWriter = new StreamWriter(stream);
			PvfFile[] fileObjs = Pvf.GetFileObjs(path);
			foreach (PvfFile pvfFile in fileObjs)
			{
				streamWriter.WriteLine(pvfFile.FileName);
			}
			streamWriter.Flush();
			response.StatusCode = 200;
		}
		else
		{
			response.StatusCode = 404;
		}
	}

	private void DeleteFileRequest(string filePath, HttpListenerResponse response)
	{
		if (Pvf.PvfIsOpen)
		{
			response.StatusCode = 404;
			return;
		}
		lock (Pvf)
		{
			Pvf.DeleteFile(filePath);
		}
		response.StatusCode = 200;
	}

	private async Task ImportFileRequest(HttpListenerResponse response, string fileName, Stream stream)
	{
		if (Pvf.PvfIsOpen)
		{
			await AppCore.ViewModelBase.PvfFileTreeViewModel.WebApiImportFile(stream, fileName);
			response.StatusCode = 200;
		}
		else
		{
			response.StatusCode = 404;
		}
	}

	private void ExtractFile(HttpListenerResponse response, Stream stream, string filePath)
	{
		if (Pvf.PvfIsOpen)
		{
			PvfFile file = Pvf.GetFile(filePath);
			if (file == null)
			{
				response.StatusCode = 404;
				return;
			}
			if (!Pvf.ExtractFile(stream, file, AppSetting.Instance.PvfConfig.ExtractConfig.DecompileBinaryAni, AppSetting.Instance.PvfConfig.ExtractConfig.DecompileScript, AppSetting.Instance.PvfConfig.ExtractConfig.ConvertConvertSimplifiedChinese, isOlWebApi: true))
			{
				response.StatusCode = 500;
				return;
			}
			response.ContentType = "application/octet-stream";
			response.ContentEncoding = null;
		}
		else
		{
			response.StatusCode = 404;
		}
	}

	private string GetPvfEditFilePaths(PvfEditHttpCommand command, bool asText)
	{
		if (string.IsNullOrEmpty(command.Value))
		{
			command.ErrorStr = "文件路径不能为空！";
			return command.ToJson();
		}
		if (command.Value == "Full")
		{
			command.Value = string.Empty;
		}
		List<string> list = Pvf.GetFiles(command.Value).ToList();
		if (asText)
		{
			if (list == null || !list.Any())
			{
				return string.Empty;
			}
			return string.Join("\r\n", list);
		}
		command.FilePaths = list;
		return command.ToJson();
	}

	private string GetPvfEditFileText(PvfEditHttpCommand command)
	{
		if (string.IsNullOrEmpty(command.Value))
		{
			command.ErrorStr = "文件路径不能为空！";
			return command.ToJson();
		}
		if (!Pvf.FileAny(command.Value))
		{
			command.ErrorStr = "文件不存在！";
			return command.ToJson();
		}
		bool? useCompatibleDecompiler = command.GetUseDecompile();
		if (AppSetting.Instance.ClientApiOptions.UseCompatibleDecompiler)
		{
			useCompatibleDecompiler = true;
		}
		command.FileText = Pvf.GetFileText(command.Value, null, useCompatibleDecompiler);
		return command.ToJson();
	}

	public async Task<string> PvfEditWriteFile(PvfEditHttpCommand cmd)
	{
		if (string.IsNullOrEmpty(cmd.Value))
		{
			cmd.ErrorStr = "文件路径不能为空！";
			return cmd.ToJson();
		}
		if (!Pvf.FileAny(cmd.Value))
		{
			await AppCore.ViewModelBase.PvfFileTreeViewModel.WebApiImportFile(BytesHelper.StringToStream(cmd.Value), cmd.Value);
		}
		else
		{
			ExtractSetting extractSetting = ((cmd.ExtractSet == null) ? (cmd.ExtractSet = new ExtractSetting()) : cmd.ExtractSet);
			Pvf.ImportUpdateFile(Pvf.GetFile(cmd.Value), BytesHelper.StringToStream(cmd.FileText), cmd.Value, extractSetting.IsEncryptScript, extractSetting.IsEncryptAni, extractSetting.IsConvertChinese);
		}
		return cmd.ToJson();
	}

	private string GetPvfEditItemInfo(PvfEditHttpCommand command)
	{
		if (string.IsNullOrEmpty(command.Value))
		{
			command.ErrorStr = "文件路径不能为空！";
			return command.ToJson();
		}
		if (!Pvf.FileAny(command.Value))
		{
			command.ErrorStr = "文件不存在！";
			return command.ToJson();
		}
		command.ItemName = Pvf.GetItemName(command.Value);
		int? itemCode = Pvf.GetItemCode(command.Value);
		if (!itemCode.HasValue)
		{
			command.ItemCode = -1;
		}
		else
		{
			command.ItemCode = itemCode.Value;
		}
		return command.ToJson();
	}

	private string DeletePvfEditFiles(PvfEditHttpCommand command)
	{
		if (command.FilePaths == null || !command.FilePaths.Any())
		{
			command.ErrorStr = "文件路径不能为空！";
			return command.ToJson();
		}
		Pvf.DeleteFiles(command.FilePaths);
		AppCore.ViewModelBase.PvfFileTreeViewModel.TreeGroupData.DeleteTreeNode(command.FilePaths);
		return command.ToJson();
	}

	private string GetCurrentPvfPath(PvfEditHttpCommand command)
	{
		command.Value = Pvf.PvfPackFilePath;
		return command.ToJson();
	}

	private static void EnsureAvailablePort()
	{
		while (PortHelper.PortInUse(AppSetting.Instance.ClientApiOptions.Port))
		{
			AppSetting.Instance.ClientApiOptions.Port++;
		}
	}
}
