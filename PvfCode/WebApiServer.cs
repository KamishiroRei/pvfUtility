using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
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
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass49_0
	{
		public string aMswkXblro;

		public _003C_003Ec__DisplayClass49_0()
		{
		}

		internal void TK1wJJMv5E()
		{
			AppCore.ViewModelBase.RootDocument.AddDocument(aMswkXblro, gotoNode: true);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass80_0
	{
		public WebApiServer PqDw7bwSY3;

		public string YguwXMA49a;

		public _003C_003Ec__DisplayClass80_0()
		{
		}

		internal Task<ResultData>? L2Uw07KPrM()
		{
			return PqDw7bwSY3.Pvf.SavePvfPack(YguwXMA49a, isFastMode: false, AppCore.ViewModelBase.MainProgress);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass86_0
	{
		public SearchConfig fnJwUbsGdM;

		public WebApiServer ARRwcqZtmW;

		public _003C_003Ec__DisplayClass86_0()
		{
		}

		internal Task<ResultData<HashSet<string>>>? p1nwp7Kf61()
		{
			return new SearchService(fnJwUbsGdM, ARRwcqZtmW.Pvf).Search();
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass87_0
	{
		public WebApiServer mxpwVgUxCw;

		public string X5Tw3ce7ki;

		public _003C_003Ec__DisplayClass87_0()
		{
		}

		internal ResultData<Dictionary<string, string>> z2Xw8KSR6u()
		{
			return mxpwVgUxCw.qnpjUutHI2(X5Tw3ce7ki.JsonToObject<List<string>>());
		}

		internal ResultData<Dictionary<int, ImagePack2Service.FilesToIconBase64Reponse>> NvXwMeLN0a()
		{
			return mxpwVgUxCw.kZQjcUuJBg(X5Tw3ce7ki.JsonToObject<List<string>>());
		}
	}

	private static IMapper? fCTTKsLmB7;

	private static WebApiServer ONpT9Ar0JO;

	[CompilerGenerated]
	private HttpListener cdOTPrulPO;

	[CompilerGenerated]
	private DateTime? bjPTZ8hksQ;

	public static WebApiServer Instance
	{
		get
		{
			if (ONpT9Ar0JO == null)
			{
				ONpT9Ar0JO = new WebApiServer();
			}
			return ONpT9Ar0JO;
		}
	}

	private PvfGroup Pvf => AppCore.ViewModelBase.PVF;

	[SpecialName]
	private static IMapper jMFTnNrUfl()
	{
		if (fCTTKsLmB7 == null)
		{
			fCTTKsLmB7 = new MapperConfiguration(delegate(IMapperConfigurationExpression cfg)
			{
				cfg.CreateMap<WebApiFileData, WebApiFileRootSectionData>();
			}).CreateMapper();
		}
		return fCTTKsLmB7;
	}

	[SpecialName]
	[CompilerGenerated]
	private HttpListener ByOTdF2J2b()
	{
		return cdOTPrulPO;
	}

	[SpecialName]
	[CompilerGenerated]
	private void m8TTeBJV97(HttpListener P_0)
	{
		cdOTPrulPO = P_0;
	}

	public WebApiServer()
	{
		m8TTeBJV97(new HttpListener());
	}

	public Task<ResultData> Start()
	{
		ResultData resultData = new ResultData();
		AppSetting.Instance.ClientApiOptions.StartLoading = true;
		lock (this)
		{
			Stop();
			JJOTLXgk4T();
			try
			{
				m8TTeBJV97(new HttpListener());
				HttpListenerPrefixCollection prefixes = ByOTdF2J2b().Prefixes;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(18, 1);
				defaultInterpolatedStringHandler.AppendLiteral("http://127.0.0.1:");
				defaultInterpolatedStringHandler.AppendFormatted(AppSetting.Instance.ClientApiOptions.Port);
				defaultInterpolatedStringHandler.AppendLiteral("/");
				prefixes.Add(defaultInterpolatedStringHandler.ToStringAndClear());
				HttpListenerPrefixCollection prefixes2 = ByOTdF2J2b().Prefixes;
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler2 = new DefaultInterpolatedStringHandler(18, 1);
				defaultInterpolatedStringHandler2.AppendLiteral("http://localhost:");
				defaultInterpolatedStringHandler2.AppendFormatted(AppSetting.Instance.ClientApiOptions.Port);
				defaultInterpolatedStringHandler2.AppendLiteral("/");
				prefixes2.Add(defaultInterpolatedStringHandler2.ToStringAndClear());
				ByOTdF2J2b().Start();
				AppCore.Logger.Success(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_HTTPServiceStarted"), AppSetting.Instance.ClientApiOptions.Port));
				ByOTdF2J2b().BeginGetContext(iZ2Tisya3m, ByOTdF2J2b());
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
			ByOTdF2J2b().Close();
		}
		catch (Exception)
		{
		}
	}

	private ResultData BCejXjMefb(string P_0)
	{
		ResultData resultData = new ResultData();
		try
		{
			if (!Pvf.PvfIsOpen)
			{
				resultData.Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_PleaseLoadPvfPackFirst");
			}
			if (string.IsNullOrEmpty(P_0))
			{
				throw new Exception("路径不能为空！");
			}
			if (!AppCore.ViewModelBase.PvfFileTreeViewModel.TreeGroupData.FilePathGetTreeNode(P_0).HasValue)
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

	private ResultData rA9jp9ryuu(string P_0, int P_1)
	{
		_003C_003Ec__DisplayClass49_0 CS_0024_003C_003E8__locals4 = new _003C_003Ec__DisplayClass49_0();
		CS_0024_003C_003E8__locals4.aMswkXblro = P_0;
		ResultData resultData = new ResultData();
		try
		{
			if (!Pvf.PvfIsOpen)
			{
				resultData.Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_PleaseLoadPvfPackFirst");
			}
			if (string.IsNullOrEmpty(CS_0024_003C_003E8__locals4.aMswkXblro))
			{
				throw new Exception("路径不能为空！");
			}
			if (P_1 == 1)
			{
				((DispatcherObject)Application.Current).Dispatcher.Invoke((Action)delegate
				{
					AppCore.ViewModelBase.RootDocument.AddDocument(CS_0024_003C_003E8__locals4.aMswkXblro, gotoNode: true);
				});
			}
			else
			{
				AppCore.ViewModelBase.PvfFileTreeViewModel.GoToNode(CS_0024_003C_003E8__locals4.aMswkXblro);
			}
		}
		catch (Exception ex)
		{
			resultData.Msg = ex.Message;
		}
		return resultData;
	}

	private ResultData<Dictionary<string, string>> qnpjUutHI2(List<string> P_0)
	{
		return ImagePack2Service.Instance.FilesToIconBase64(P_0, Pvf);
	}

	private ResultData<Dictionary<int, ImagePack2Service.FilesToIconBase64Reponse>> kZQjcUuJBg(List<string> P_0)
	{
		return ImagePack2Service.Instance.FilesToIconBase64New(P_0, Pvf);
	}

	private ResultData<IEnumerable<WebApiFileData>> KNAj82HNkf(string P_0)
	{
		ResultData<IEnumerable<WebApiFileData>> resultData = new ResultData<IEnumerable<WebApiFileData>>();
		if (!Pvf.PvfIsOpen)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_PleaseLoadPvfPackFirst");
			return resultData;
		}
		if (string.IsNullOrEmpty(P_0))
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_FilePathCannotBeEmpty");
			return resultData;
		}
		PvfFile file = Pvf.GetFile(P_0);
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

	private ResultData<List<WebApiFileRootSectionData>> SGTjMF36X3(string P_0)
	{
		ResultData<List<WebApiFileRootSectionData>> resultData = new ResultData<List<WebApiFileRootSectionData>>();
		if (!Pvf.PvfIsOpen)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_PleaseLoadPvfPackFirst");
			return resultData;
		}
		if (string.IsNullOrEmpty(P_0))
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_FilePathCannotBeEmpty");
			return resultData;
		}
		PvfFile file = Pvf.GetFile(P_0);
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
			List<WebApiFileRootSectionData> list2 = jMFTnNrUfl().Map<List<WebApiFileRootSectionData>>(list);
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

	private async Task<ResultData<Dictionary<int, LstFileInfo>>> tOWjVhKJNw(string P_0)
	{
		ResultData<Dictionary<int, LstFileInfo>> re = new ResultData<Dictionary<int, LstFileInfo>>();
		if (!Pvf.PvfIsOpen)
		{
			re.Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_PleaseLoadPvfPackFirst");
			return re;
		}
		if (!Pvf.FileAny(P_0))
		{
			re.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_FileNotExistTitle");
			return re;
		}
		PvfFile file = Pvf.GetFile(P_0);
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

	private ResultData<List<string>> fQhj3VWLTy()
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

	private ResultData<string> iuGjRW9dv2()
	{
		return new ResultData<string>
		{
			Data = Application.ResourceAssembly.GetName().Version.ToString()
		};
	}

	[SpecialName]
	[CompilerGenerated]
	private DateTime? NwbTIFuish()
	{
		return bjPTZ8hksQ;
	}

	[SpecialName]
	[CompilerGenerated]
	private void DNsTEpXfKL(DateTime? P_0)
	{
		bjPTZ8hksQ = P_0;
	}

	private async Task<ResultData<string>> LiljNpyagD(string P_0)
	{
		ResultData<string> resultData = new ResultData<string>();
		if (!Pvf.PvfIsOpen)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_PleaseLoadPvfPackFirst");
			return resultData;
		}
		if (string.IsNullOrEmpty(P_0))
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_FilePathCannotBeEmpty");
			return resultData;
		}
		if (!Pvf.FileList.TryGetValue(P_0, out PvfFile value))
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_FileNotExistTitle");
			return resultData;
		}
		if (NwbTIFuish().HasValue && NwbTIFuish().Value.AddSeconds(0.5) > DateTime.Now)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_Interval");
			return resultData;
		}
		if (AppSetting.Instance.ImagePacks2Options.ImgCount == 0)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_UserNotLoadImagePack2ModelDir");
			return resultData;
		}
		DNsTEpXfKL(DateTime.Now);
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

	private ResultData<ItemCodeToFileInfoDto> UiGjzVBPNH(string P_0, int? P_1)
	{
		ResultData<ItemCodeToFileInfoDto> resultData = new ResultData<ItemCodeToFileInfoDto>();
		if (string.IsNullOrEmpty(P_0))
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_LstNameCannotBeEmpty");
			return resultData;
		}
		if (!P_1.HasValue)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_ItemCodeCannotBeEmpty");
			return resultData;
		}
		string[] lstNames = P_0.Split(",", StringSplitOptions.RemoveEmptyEntries);
		PvfGroup pVF = AppCore.ViewModelBase.PVF;
		if (!pVF.PvfIsOpen)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_PleaseLoadPvfPackFirst");
			return resultData;
		}
		LstItem lstItem = pVF.ListFileTable.GetLstItem(lstNames, P_1.Value);
		if (lstItem == null)
		{
			resultData.Msg = string.Format(AppSetting.Instance.GetIlogger().GetStr("mess_ItemCodeNoFound"), P_0, P_1);
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

	private ResultData<ItemCodesToFileInfosDto> xcbTDuaQi2(ItemCodesToFileInfosRes P_0)
	{
		ResultData<ItemCodesToFileInfosDto> resultData = new ResultData<ItemCodesToFileInfosDto>();
		if (P_0.lstNames == null || P_0.lstNames.Count == 0)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_LstNameCannotBeEmpty2");
			return resultData;
		}
		if (P_0.ItemCodes == null || P_0.ItemCodes.Count == 0)
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
		foreach (int itemCode in P_0.ItemCodes)
		{
			if (!dictionary.ContainsKey(itemCode))
			{
				string text = pVF.ListFileTable.ItemCodeConvertFilePath(P_0.lstNames, itemCode);
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

	private ResultData<IEnumerable<string>> fO8TlXJZeJ()
	{
		return new ResultData<IEnumerable<string>>
		{
			Data = AppCore.ViewModelBase.PvfFileTreeViewModel.GetSelectedFilePaths(GetTreeType.File)
		};
	}

	private ResultData<IEnumerable<string>> xPMTjsp7Or()
	{
		return new ResultData<IEnumerable<string>>
		{
			Data = AppCore.ViewModelBase.SearchResultViewModel.TreeViewModel.GetSelectedFilePaths(GetTreeType.File)
		};
	}

	private ResultData<string?> lRLTTtVes6()
	{
		return new ResultData<string>
		{
			Data = AppCore.ViewModelBase.PvfFileTreeViewModel.SelectedNodeBindgBase?.Value?.FullPath
		};
	}

	private ResultData<string?> KTvTC6xIZy()
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

	private object xjWTHCAGrP(string? dirName, int P_1, string P_2)
	{
		PvfFileType? pvfFileType = null;
		if (!string.IsNullOrEmpty(P_2) && AppSetting.Instance.PvfConfig.PvfFileTypeDic.TryGetValue(P_2, out var value))
		{
			pvfFileType = value;
		}
		if (P_1 != 0)
		{
			return JkLTvTrZnD(dirName, pvfFileType);
		}
		return EjMThe73y8(dirName, pvfFileType);
	}

	private ResultData<IEnumerable<string>> EjMThe73y8(string P_0, PvfFileType? P_1)
	{
		ResultData<IEnumerable<string>> resultData = new ResultData<IEnumerable<string>>();
		if (!Pvf.PvfIsOpen)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_PleaseLoadPvfPackFirst");
		}
		else
		{
			resultData.Data = Pvf.GetFiles(P_0, P_1);
		}
		return resultData;
	}

	private ResultData<string> JkLTvTrZnD(string P_0, PvfFileType? P_1)
	{
		ResultData<string> resultData = new ResultData<string>();
		if (!Pvf.PvfIsOpen)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_PleaseLoadPvfPackFirst");
		}
		else
		{
			IEnumerable<string> files = Pvf.GetFiles(P_0, P_1);
			if (files != null && files.Any())
			{
				resultData.Data = string.Join("\r\n", files);
			}
		}
		return resultData;
	}

	private ResultData<IEnumerable<string>> E6sTBIF7TY()
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

	private ResultData<string> bSXTFt0US9(string P_0, bool P_1, string? encodingType)
	{
		ResultData<string> resultData = new ResultData<string>();
		if (!Pvf.PvfIsOpen)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_PleaseLoadPvfPackFirst");
			return resultData;
		}
		if (!Pvf.FileAny(P_0))
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
		resultData.Data = Pvf.GetFileText(P_0, encoding, P_1, showAniError: false);
		return resultData;
	}

	private ResultData<FileContentListDto> VtfTrKbEl7(GetFileContentRes P_0)
	{
		ResultData<FileContentListDto> resultData = new ResultData<FileContentListDto>
		{
			Data = new FileContentListDto
			{
				FileContentData = new Dictionary<string, string>()
			}
		};
		if (P_0.FileList == null || !P_0.FileList.Any())
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
		if (!string.IsNullOrEmpty(P_0.EncodingType))
		{
			if (!Enum.TryParse(typeof(EncodingType), P_0.EncodingType, out object result))
			{
				resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_EncodingError");
				return resultData;
			}
			encoding = (EncodingType?)result;
		}
		foreach (string file in P_0.FileList)
		{
			if (Pvf.FileList.TryGetValue(file, out PvfFile value))
			{
				string fileText = Pvf.GetFileText(value, encoding, P_0.UseCompatibleDecompiler, showAniError: false);
				if (!fileContentListDto.FileContentData.ContainsKey(file))
				{
					fileContentListDto.FileContentData.Add(file, fileText);
				}
			}
		}
		resultData.Data = fileContentListDto;
		return resultData;
	}

	private ResultData<ItemInfoDto> xreTWRrmIX(string P_0)
	{
		ResultData<ItemInfoDto> resultData = new ResultData<ItemInfoDto>();
		if (!Pvf.PvfIsOpen)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_PleaseLoadPvfPackFirst");
			return resultData;
		}
		if (Pvf.FileList.TryGetValue(P_0, out PvfFile value))
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

	private ResultData<Dictionary<string, ItemInfoDto>> eOPTmIDUvR(IEnumerable<string> P_0)
	{
		ResultData<Dictionary<string, ItemInfoDto>> resultData = new ResultData<Dictionary<string, ItemInfoDto>>();
		if (!Pvf.PvfIsOpen)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_PleaseLoadPvfPackFirst");
			return resultData;
		}
		Dictionary<string, ItemInfoDto> dictionary = new Dictionary<string, ItemInfoDto>();
		foreach (string item in P_0)
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

	private ResultData ztVT2V9n5P(string P_0)
	{
		ResultData resultData = new ResultData();
		if (!Pvf.PvfIsOpen)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_PleaseLoadPvfPackFirst");
			return resultData;
		}
		if (!Pvf.FileAny(P_0))
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_FileNotExistTitle");
		}
		else
		{
			lock (this)
			{
				Pvf.DeleteFile(P_0);
				AppCore.ViewModelBase.PvfFileTreeViewModel.TreeGroupData.DeleteTreeNode(new List<string> { P_0 });
			}
		}
		return resultData;
	}

	private ResultData<IEnumerable<string>> r96TfCR3uk(IEnumerable<string> P_0)
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
			foreach (string item in P_0)
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

	private async Task<ResultData> IMsT5l8XOL(string P_0)
	{
		_003C_003Ec__DisplayClass80_0 CS_0024_003C_003E8__locals6 = new _003C_003Ec__DisplayClass80_0();
		CS_0024_003C_003E8__locals6.PqDw7bwSY3 = this;
		CS_0024_003C_003E8__locals6.YguwXMA49a = P_0;
		ResultData resultData = new ResultData();
		if (string.IsNullOrEmpty(CS_0024_003C_003E8__locals6.YguwXMA49a))
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_FilePathCannotBeEmpty");
			return resultData;
		}
		if (string.IsNullOrEmpty(Path.GetFileName(CS_0024_003C_003E8__locals6.YguwXMA49a)))
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_FilePathNoFileName");
			return resultData;
		}
		if (!Pvf.PvfIsOpen)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_PleaseLoadPvfPackFirst");
			return resultData;
		}
		return await Task.Run(() => CS_0024_003C_003E8__locals6.PqDw7bwSY3.Pvf.SavePvfPack(CS_0024_003C_003E8__locals6.YguwXMA49a, isFastMode: false, AppCore.ViewModelBase.MainProgress));
	}

	private ResultData<string> YplTS2iCiU()
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

	private async Task<ResultData> pFDTAbBWu9(string P_0, Stream P_1)
	{
		ResultData result = new ResultData();
		if (!Pvf.PvfIsOpen)
		{
			result.Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_PleaseLoadPvfPackFirst");
			return result;
		}
		if (!(await AppCore.ViewModelBase.PvfFileTreeViewModel.WebApiImportFile(P_1, P_0)))
		{
			result.Msg = AppSetting.Instance.GetIlogger()?.GetStr("mess_ImportFailed");
		}
		return result;
	}

	private async Task<ResultData<IEnumerable<string>>> ImportFiles(IEnumerable<ImportFileRes> fileDataList)
	{
		return await AppCore.ViewModelBase.PvfFileTreeViewModel.WebApiImportFiles(fileDataList);
	}

	private ResultData<ConcurrentDictionary<string, ConcurrentDictionary<int, string>>> J5WT4D95Sx(IEnumerable<string> P_0)
	{
		ResultData<ConcurrentDictionary<string, ConcurrentDictionary<int, string>>> resultData = new ResultData<ConcurrentDictionary<string, ConcurrentDictionary<int, string>>>();
		if (!Pvf.PvfIsOpen)
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_PleaseLoadPvfPackFirst");
			return resultData;
		}
		if (P_0 == null || !P_0.Any())
		{
			resultData.Msg = AppSetting.Instance.GetIlogger().GetStr("mess_FileListCannotBeEmpty");
			return resultData;
		}
		resultData.Data = Pvf.FilesToLstDic(P_0);
		return resultData;
	}

	private ResultData<string> wEcTYb8A41()
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

	private async Task<ResultData<List<string>>> LZVTyKuTUe(SearchConfig P_0)
	{
		_003C_003Ec__DisplayClass86_0 obj = new _003C_003Ec__DisplayClass86_0();
		obj.fnJwUbsGdM = P_0;
		obj.ARRwcqZtmW = this;
		ResultData<HashSet<string>> resultData = await Task.Run(() => new SearchService(obj.fnJwUbsGdM, obj.ARRwcqZtmW.Pvf).Search());
		return new ResultData<List<string>>
		{
			Data = ((resultData.Data == null) ? null : resultData.Data.ToList())
		};
	}

	private async void iZ2Tisya3m(IAsyncResult P_0)
	{
		_003C_003Ec__DisplayClass87_0 CS_0024_003C_003E8__locals16 = new _003C_003Ec__DisplayClass87_0();
		CS_0024_003C_003E8__locals16.mxpwVgUxCw = this;
		HttpListenerResponse response = null;
		HttpListenerRequest request = null;
		string errorMsg = null;
		try
		{
			if (!ByOTdF2J2b().IsListening)
			{
				return;
			}
			ByOTdF2J2b().BeginGetContext(iZ2Tisya3m, null);
			HttpListenerContext httpListenerContext = ByOTdF2J2b().EndGetContext(P_0);
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
						aoMTQwCZ5c(request.QueryString.Get("name"), httpListenerContext.Response);
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
									object obj = xjWTHCAGrP(request.QueryString.Get("dirName"), request.QueryString.Get("returnType").ToInt(), request.QueryString.Get("fileType"));
									lECTuD8L9C(obj, stream);
									response.StatusCode = 200;
								}
								break;
							case 'n':
								if (absolutePath == "/Api/PvfUtiltiy/GetItemInfo")
								{
									lECTuD8L9C(xreTWRrmIX(request.QueryString.Get("filePath")), stream);
									response.StatusCode = 200;
								}
								break;
							case 'c':
								if (absolutePath == "/Api/PvfUtiltiy/getFileIcon")
								{
									lECTuD8L9C(await LiljNpyagD(request.QueryString.Get("filePath")), stream);
									response.StatusCode = 200;
								}
								break;
							case 'a':
								if (absolutePath == "/Api/PvfUtiltiy/getFileData")
								{
									lECTuD8L9C(KNAj82HNkf(request.QueryString.Get("filePath")), stream);
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
									lECTuD8L9C(bSXTFt0US9(request.QueryString.Get("filePath"), request.QueryString.Get("useCompatibleDecompiler").ToBoolen(), request.QueryString.Get("encodingType")), stream);
									response.StatusCode = 200;
								}
								break;
							case 'L':
								if (absolutePath == "/Api/PvfUtiltiy/getLstFileInfo")
								{
									lECTuD8L9C(await tOWjVhKJNw(request.QueryString.Get("filePath")), stream);
									response.StatusCode = 200;
								}
								break;
							case 'S':
								if (absolutePath == "/Api/PvfUtiltiy/getStringTable")
								{
									lECTuD8L9C(GetStringTable(), stream);
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
									lECTuD8L9C(ztVT2V9n5P(request.QueryString.Get("filePath")), stream);
									response.StatusCode = 200;
								}
								break;
							case 'g':
								if (absolutePath == "/Api/PvfUtiltiy/getVersion")
								{
									lECTuD8L9C(iuGjRW9dv2(), stream);
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
									lECTuD8L9C(YplTS2iCiU(), stream);
									response.StatusCode = 200;
								}
								break;
							case 'I':
								if (absolutePath == "/Api/PvfUtiltiy/ItemCodeToFileInfo")
								{
									string text = request.QueryString.Get("itemCode");
									lECTuD8L9C(UiGjzVBPNH(request.QueryString.Get("lstNames"), (text == null) ? ((int?)null) : ((!int.TryParse(text, out var result)) ? ((int?)null) : new int?(result))), stream);
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
									lECTuD8L9C(FileIsExists(request.QueryString.Get("filePath")), stream);
									response.StatusCode = 200;
								}
								break;
							case 'i':
								if (absolutePath == "/Api/PvfUtility/folderExists")
								{
									lECTuD8L9C(BCejXjMefb(request.QueryString.Get("filePath")), stream);
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
									UuOTgscphw(httpListenerContext.Response, stream, request.QueryString.Get("name"));
								}
								break;
							case 'l':
								if (absolutePath == "/list")
								{
									eaWTxocZko(request, stream, httpListenerContext.Response);
								}
								break;
							}
							break;
						case 33:
							if (absolutePath == "/Api/PvfUtiltiy/GetAllLstFileList")
							{
								lECTuD8L9C(E6sTBIF7TY(), stream);
								response.StatusCode = 200;
							}
							break;
						case 42:
							if (absolutePath == "/Api/PvfUtiltiy/GetTreeListFocusedFilePath")
							{
								lECTuD8L9C(lRLTTtVes6(), stream);
								response.StatusCode = 200;
							}
							break;
						case 53:
							if (absolutePath == "/Api/PvfUtiltiy/GetSearchPanelTreeListFocusedFilePath")
							{
								lECTuD8L9C(KTvTC6xIZy(), stream);
								response.StatusCode = 200;
							}
							break;
						case 41:
							if (absolutePath == "/Api/PvfUtiltiy/GetActiveDocumentFilePath")
							{
								lECTuD8L9C(wEcTYb8A41(), stream);
								response.StatusCode = 200;
							}
							break;
						case 29:
							if (absolutePath == "/Api/PvfUtiltiy/SaveAsPvfFile")
							{
								lECTuD8L9C(await IMsT5l8XOL(request.QueryString.Get("filePath")), stream);
								response.StatusCode = 200;
							}
							break;
						case 36:
							if (absolutePath == "/Api/PvfUtiltiy/GetTreeSelectedFiles")
							{
								lECTuD8L9C(fO8TlXJZeJ(), stream);
								response.StatusCode = 200;
							}
							break;
						case 43:
							if (absolutePath == "/Api/PvfUtiltiy/GetSearchPanelSelectedFiles")
							{
								lECTuD8L9C(xPMTjsp7Or(), stream);
								response.StatusCode = 200;
							}
							break;
						case 35:
							if (absolutePath == "/Api/PvfUtiltiy/getPvfRootDirectory")
							{
								lECTuD8L9C(fQhj3VWLTy(), stream);
								response.StatusCode = 200;
							}
							break;
						case 38:
							if (absolutePath == "/Api/PvfUtility/getFileRootSestionInfo")
							{
								lECTuD8L9C(SGTjMF36X3(request.QueryString.Get("filePath")), stream);
								response.StatusCode = 200;
							}
							break;
						case 32:
							if (absolutePath == "/Api/PvfUtility/goToTreeListNode")
							{
								lECTuD8L9C(rA9jp9ryuu(request.QueryString.Get("filePath"), request.QueryString.Get("openTextDocument").ObjToInt()), stream);
								response.StatusCode = 200;
							}
							break;
						case 11:
							if (absolutePath == "/listSearch")
							{
								keRTGM6C1K(request, stream, httpListenerContext.Response);
							}
							break;
						}
					}
				}
			}
			else
			{
				using StreamReader reader = new StreamReader(httpListenerContext.Request.InputStream, Encoding.UTF8);
				CS_0024_003C_003E8__locals16.X5Tw3ce7ki = await reader.ReadToEndAsync();
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
							lECTuD8L9C(r96TfCR3uk(CS_0024_003C_003E8__locals16.X5Tw3ce7ki.JsonToObject<IEnumerable<string>>()), stream);
							response.StatusCode = 200;
							goto end_IL_025d;
						case 'I':
							if (!(absolutePath == "/Api/PvfUtiltiy/ImportFiles"))
							{
								break;
							}
							lECTuD8L9C(await ImportFiles(CS_0024_003C_003E8__locals16.X5Tw3ce7ki.JsonToObject<IEnumerable<ImportFileRes>>()), stream);
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
							lECTuD8L9C(J5WT4D95Sx(CS_0024_003C_003E8__locals16.X5Tw3ce7ki.JsonToObject<IEnumerable<string>>()), stream);
							response.StatusCode = 200;
							goto end_IL_025d;
						case 'f':
							if (!(absolutePath == "/Api/PvfUtiltiy/filesToIconBase64"))
							{
								break;
							}
							lECTuD8L9C(await Task.Run(() => CS_0024_003C_003E8__locals16.mxpwVgUxCw.qnpjUutHI2(CS_0024_003C_003E8__locals16.X5Tw3ce7ki.JsonToObject<List<string>>())), stream);
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
							lECTuD8L9C(xcbTDuaQi2(CS_0024_003C_003E8__locals16.X5Tw3ce7ki.JsonToObject<ItemCodesToFileInfosRes>()), stream);
							response.StatusCode = 200;
							goto end_IL_025d;
						case 'i':
							if (!(absolutePath == "/Api/PvfUtility/filesToIconBase64New"))
							{
								break;
							}
							lECTuD8L9C(await Task.Run(() => CS_0024_003C_003E8__locals16.mxpwVgUxCw.kZQjcUuJBg(CS_0024_003C_003E8__locals16.X5Tw3ce7ki.JsonToObject<List<string>>())), stream);
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
							await VV9Tab3OjJ(response, request.QueryString.Get("name"), BytesHelper.StringToStream(CS_0024_003C_003E8__locals16.X5Tw3ce7ki));
						}
						goto end_IL_025d;
					case 26:
						if (!(absolutePath == "/Api/PvfUtiltiy/ImportFile"))
						{
							break;
						}
						lECTuD8L9C(await pFDTAbBWu9(request.QueryString.Get("filePath"), BytesHelper.StringToStream(CS_0024_003C_003E8__locals16.X5Tw3ce7ki)), stream);
						response.StatusCode = 200;
						goto end_IL_025d;
					case 28:
						if (!(absolutePath == "/Api/PvfUtiltiy/GetItemInfos"))
						{
							break;
						}
						lECTuD8L9C(eOPTmIDUvR(CS_0024_003C_003E8__locals16.X5Tw3ce7ki.JsonToObject<IEnumerable<string>>()), stream);
						response.StatusCode = 200;
						goto end_IL_025d;
					case 31:
						if (!(absolutePath == "/Api/PvfUtiltiy/GetFileContents"))
						{
							break;
						}
						lECTuD8L9C(VtfTrKbEl7(CS_0024_003C_003E8__locals16.X5Tw3ce7ki.JsonToObject<GetFileContentRes>()), stream);
						response.StatusCode = 200;
						goto end_IL_025d;
					case 25:
						if (!(absolutePath == "/Api/PvfUtiltiy/SearchPvf"))
						{
							break;
						}
						lECTuD8L9C(await LZVTyKuTUe(CS_0024_003C_003E8__locals16.X5Tw3ce7ki.JsonToObject<SearchConfig>()), stream);
						response.StatusCode = 200;
						goto end_IL_025d;
					}
				}
				if (request.Url.AbsolutePath == null || request.Url.AbsolutePath.Length == 0 || request.Url.AbsolutePath == "/")
				{
					PvfEditHttpCommand pvfEditHttpCommand = CS_0024_003C_003E8__locals16.X5Tw3ce7ki.JsonToObject<PvfEditHttpCommand>();
					string value = string.Empty;
					if (!Pvf.PvfIsOpen)
					{
						pvfEditHttpCommand.ErrorStr = AppSetting.Instance.GetIlogger().GetStrNoReplace("mess_PleaseLoadPvfPackFirst");
						value = XK1T14N2Eh(pvfEditHttpCommand);
					}
					else
					{
						switch (pvfEditHttpCommand.Cmd)
						{
						case Command.GetFileText:
							value = XK1T14N2Eh(pvfEditHttpCommand);
							break;
						case Command.GetFilePaths:
							value = uQKT6u9uI4(pvfEditHttpCommand, false);
							break;
						case Command.WriteFile:
							value = await PvfEditWriteFile(pvfEditHttpCommand);
							break;
						case Command.GetItemNameAndItemCode:
							value = qTFTwSWpZh(pvfEditHttpCommand);
							break;
						case Command.DeleteFiles:
							value = Pt9TolBvst(pvfEditHttpCommand);
							break;
						case Command.GetNowPvfPath:
							value = JsfTsChXvb(pvfEditHttpCommand);
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
							value = uQKT6u9uI4(pvfEditHttpCommand, true);
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

	private void lECTuD8L9C(object P_0, Stream P_1)
	{
		StreamWriter streamWriter = new StreamWriter(P_1);
		streamWriter.Write(P_0.ToJson());
		streamWriter.Flush();
	}

	private void keRTGM6C1K(HttpListenerRequest P_0, MemoryStream P_1, HttpListenerResponse P_2)
	{
		if (!Pvf.PvfIsOpen)
		{
			P_2.StatusCode = 404;
			return;
		}
		StreamWriter streamWriter = new StreamWriter(P_1);
		foreach (string key in Pvf.FileList.Keys)
		{
			streamWriter.WriteLine(key);
		}
		streamWriter.Flush();
		P_2.StatusCode = 200;
	}

	private void eaWTxocZko(HttpListenerRequest P_0, MemoryStream P_1, HttpListenerResponse P_2)
	{
		if (Pvf.PvfIsOpen)
		{
			string path = P_0.QueryString.Get("path");
			StreamWriter streamWriter = new StreamWriter(P_1);
			PvfFile[] fileObjs = Pvf.GetFileObjs(path);
			foreach (PvfFile pvfFile in fileObjs)
			{
				streamWriter.WriteLine(pvfFile.FileName);
			}
			streamWriter.Flush();
			P_2.StatusCode = 200;
		}
		else
		{
			P_2.StatusCode = 404;
		}
	}

	private void aoMTQwCZ5c(string P_0, HttpListenerResponse P_1)
	{
		if (Pvf.PvfIsOpen)
		{
			P_1.StatusCode = 404;
			return;
		}
		lock (Pvf)
		{
			Pvf.DeleteFile(P_0);
		}
		P_1.StatusCode = 200;
	}

	private async Task VV9Tab3OjJ(HttpListenerResponse P_0, string P_1, Stream P_2)
	{
		if (Pvf.PvfIsOpen)
		{
			await AppCore.ViewModelBase.PvfFileTreeViewModel.WebApiImportFile(P_2, P_1);
			P_0.StatusCode = 200;
		}
		else
		{
			P_0.StatusCode = 404;
		}
	}

	private void UuOTgscphw(HttpListenerResponse P_0, Stream P_1, string P_2)
	{
		if (Pvf.PvfIsOpen)
		{
			PvfFile file = Pvf.GetFile(P_2);
			if (file == null)
			{
				P_0.StatusCode = 404;
				return;
			}
			if (!Pvf.ExtractFile(P_1, file, AppSetting.Instance.PvfConfig.ExtractConfig.DecompileBinaryAni, AppSetting.Instance.PvfConfig.ExtractConfig.DecompileScript, AppSetting.Instance.PvfConfig.ExtractConfig.ConvertConvertSimplifiedChinese, isOlWebApi: true))
			{
				P_0.StatusCode = 500;
				return;
			}
			P_0.ContentType = "application/octet-stream";
			P_0.ContentEncoding = null;
		}
		else
		{
			P_0.StatusCode = 404;
		}
	}

	private string uQKT6u9uI4(PvfEditHttpCommand P_0, bool P_1)
	{
		if (string.IsNullOrEmpty(P_0.Value))
		{
			P_0.ErrorStr = "文件路径不能为空！";
			return P_0.ToJson();
		}
		if (P_0.Value == "Full")
		{
			P_0.Value = string.Empty;
		}
		List<string> list = Pvf.GetFiles(P_0.Value).ToList();
		if (P_1)
		{
			if (list == null || !list.Any())
			{
				return string.Empty;
			}
			return string.Join("\r\n", list);
		}
		P_0.FilePaths = list;
		return P_0.ToJson();
	}

	private string XK1T14N2Eh(PvfEditHttpCommand P_0)
	{
		if (string.IsNullOrEmpty(P_0.Value))
		{
			P_0.ErrorStr = "文件路径不能为空！";
			return P_0.ToJson();
		}
		if (!Pvf.FileAny(P_0.Value))
		{
			P_0.ErrorStr = "文件不存在！";
			return P_0.ToJson();
		}
		bool? useCompatibleDecompiler = P_0.GetUseDecompile();
		if (AppSetting.Instance.ClientApiOptions.UseCompatibleDecompiler)
		{
			useCompatibleDecompiler = true;
		}
		P_0.FileText = Pvf.GetFileText(P_0.Value, null, useCompatibleDecompiler);
		return P_0.ToJson();
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

	private string qTFTwSWpZh(PvfEditHttpCommand P_0)
	{
		if (string.IsNullOrEmpty(P_0.Value))
		{
			P_0.ErrorStr = "文件路径不能为空！";
			return P_0.ToJson();
		}
		if (!Pvf.FileAny(P_0.Value))
		{
			P_0.ErrorStr = "文件不存在！";
			return P_0.ToJson();
		}
		P_0.ItemName = Pvf.GetItemName(P_0.Value);
		int? itemCode = Pvf.GetItemCode(P_0.Value);
		if (!itemCode.HasValue)
		{
			P_0.ItemCode = -1;
		}
		else
		{
			P_0.ItemCode = itemCode.Value;
		}
		return P_0.ToJson();
	}

	private string Pt9TolBvst(PvfEditHttpCommand P_0)
	{
		if (P_0.FilePaths == null || !P_0.FilePaths.Any())
		{
			P_0.ErrorStr = "文件路径不能为空！";
			return P_0.ToJson();
		}
		Pvf.DeleteFiles(P_0.FilePaths);
		AppCore.ViewModelBase.PvfFileTreeViewModel.TreeGroupData.DeleteTreeNode(P_0.FilePaths);
		return P_0.ToJson();
	}

	private string JsfTsChXvb(PvfEditHttpCommand P_0)
	{
		P_0.Value = Pvf.PvfPackFilePath;
		return P_0.ToJson();
	}

	private static void JJOTLXgk4T()
	{
		while (PortHelper.PortInUse(AppSetting.Instance.ClientApiOptions.Port))
		{
			AppSetting.Instance.ClientApiOptions.Port++;
		}
	}
}
