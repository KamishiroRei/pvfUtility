using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PvfCode.Dot;
using PvfCode.Dot.Desktop;
using PvfCode.Dot.Desktop.AdvertisingPosition;
using PvfCode.Dot.Desktop.Enums;
using PvfCode.Dot.Desktop.interfaces;
using PvfCode.Models.CodeCompletionModels;
using PvfCode.Models.Enums;
using Utools;

namespace PvfCode;

public class ServiceCloud : ModelBase
{
	private static ServiceCloud instance;

	private readonly string apiUrl;

	public PvfCodePublicCloudDataRes PublicCloudData;

	private bool isAdmin;

	private LoginResultDto user;

	public static ServiceCloud Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new ServiceCloud();
			}
			return instance;
		}
	}

	public bool IsLogin => User != null;

	public bool IsAdmin
	{
		get
		{
			return isAdmin;
		}
		set
		{
			isAdmin = value;
			DoNotify("IsAdmin");
		}
	}

	public LoginResultDto User
	{
		get
		{
			return user;
		}
		set
		{
			user = value;
			DoNotify("User");
			DoNotify("IsLogin");
			IsAdmin = value != null && User.IsAdmin;
		}
	}

	public ServiceCloud()
	{
		apiUrl = CloudOptionsService.Options.ApiUrl;
		PublicCloudData = new PvfCodePublicCloudDataRes();
	}

	public async Task<ResultData<PvfCodePublicCloudDataRes>> GetPvfCodeCloudPublicData()
	{
		ResultData<PvfCodePublicCloudDataRes> data = await GetResultAsync<PvfCodePublicCloudDataRes>("GetPvfCodeCloudPublicData");
		try
		{
			List<CodeCompletionData> list = new List<CodeCompletionData>();
			if (!data.IsError)
			{
				PublicCloudData = data.Data;
				try
				{
					string text = JsonConvert.SerializeObject(PublicCloudData.CodeCompletionDataDtos);
					if (text != null)
					{
						list = JsonConvert.DeserializeObject<List<CodeCompletionData>>(text);
					}
					if (list == null)
					{
						list = new List<CodeCompletionData>();
					}
				}
				catch (Exception ex)
				{
					AppSetting.Instance.GetIlogger().Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_SmartTipsLoadError"), ex.Message));
				}
			}
			AppSetting.Instance.EditConfig.InitCompletionDatas(list);
			if (PublicCloudData.TreeListComment != null && PublicCloudData.TreeListComment.Any())
			{
				foreach (KeyValuePair<string, TreelistCommentRes> item in PublicCloudData.TreeListComment)
				{
					if (!AppSetting.Instance.PvfConfig.TreelistCommentDic.ContainsKey(item.Key))
					{
						AppSetting.Instance.PvfConfig.TreelistCommentDic.Add(item.Key, item.Value);
					}
				}
			}
		}
		catch (Exception ex2)
		{
			data.Msg = ex2.Message;
		}
		if (data.IsError)
		{
			AppSetting.Instance.GetIlogger()?.Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_CloudDataError"), data.Msg));
		}
		return data;
	}

	public async Task<ResultData> UploadAccountCloudBackup(AccountCloudBackupDto dto)
	{
		return await PostResultAsync("UploadAccountCloudBackup", dto.ToJson());
	}

	public async Task<ResultData<AccountCloudBackupDto>> DownloadAccountCloudBackup()
	{
		return await GetResultAsync<AccountCloudBackupDto>("GetAccountCloudBackup");
	}

	public async Task<ResultData> AddAdvert(AdvertisingPositionInfoDto dto)
	{
		return await PostResultAsync("AddAdvert", dto.ToJson());
	}

	public async Task<ResultData> DeleteAdvert(AdvertisingPositionInfoDto dto)
	{
		return await PostResultAsync("DeleteAdvert", dto.ToJson());
	}

	public async Task<ResultData<List<AdvertisingPositionInfoDto>>> GetAdvertList()
	{
		return await GetResultAsync<List<AdvertisingPositionInfoDto>>("GetAdvertList");
	}

	public async Task<ResultData> UpdateAdvert(AdvertisingPositionInfoDto dto)
	{
		return await PostResultAsync("UpdateAdvert", dto.ToJson());
	}

	public async Task DesktopErrorMessage(string errorMess, string version)
	{
		try
		{
			DesktopErrorMessageRes obj = new DesktopErrorMessageRes
			{
				Message = errorMess,
				Version = version
			};
			await PostResultAsync("DesktopErrorMessage", ToJson(obj));
		}
		catch (Exception)
		{
		}
	}

	public async Task<ResultData> AddBookMarkGroup(IBookMarkGroup dto)
	{
		return await PostResultAsync("AddBookMarkGroup", dto.ToJson());
	}

	public async Task<ResultData<IEnumerable<BookMarkGroupDto>>> BookMarkStoreGetList(string keyword)
	{
		return await Task.Run(() => GetResultAsync<IEnumerable<BookMarkGroupDto>>("BookMarkStoreGetList?keyWord=" + keyword));
	}

	public async Task<ResultData<BookMarkGroupDto>> GetPreviewBookMarkData(int id)
	{
		return await Task.Run(() => GetResultAsync<BookMarkGroupDto>($"GetPreviewBookMarkData?id={id}"));
	}

	public async Task<ResultData<BookMarkGroupDto>> DownLoadBookMark(int id)
	{
		return await Task.Run(() => GetResultAsync<BookMarkGroupDto>($"DownLoadBookMark?id={id}"));
	}

	public async Task<ResultData<IList<MacroDataDto>>> MacroStoreGetList(SearchMacroRes searchRes)
	{
		return await Task.Run(() => PostResultAsync<IList<MacroDataDto>>("MacroStoreGetList", ToJson(searchRes)));
	}

	public async Task<ResultData<MacroDataDto>> DownLoadMacroDataDto(int id)
	{
		return await Task.Run(() => GetResultAsync<MacroDataDto>($"DownLoadMacroDataDto?id={id}"));
	}

	public async Task<ResultData> SaveTreelistComment(TreelistCommentRes dto)
	{
		ResultData re = new ResultData();
		if (AppSetting.Instance.PvfConfig.TreelistCommentDic.ContainsKey(dto.FilePath))
		{
			AppSetting.Instance.PvfConfig.TreelistCommentDic[dto.FilePath] = dto;
		}
		else
		{
			AppSetting.Instance.PvfConfig.TreelistCommentDic.Add(dto.FilePath, dto);
		}
		if (AppSetting.Instance.StoreOptions.UploadFileListComment)
		{
			await Task.Run((Func<Task<ResultData>?>)Instance.UploadStoreListDatas);
		}
		await AppSetting.Instance.SaveSetting();
		return re;
	}

	public async Task<ResultData<PvfCommentDto>> GetPvfComment(PvfCommentDtoRes res)
	{
		ResultData<PvfCommentDto> cloudData = new ResultData<PvfCommentDto>();
		try
		{
			ResultData<PvfCommentDto> diskData = await ServicePvfTabComment.Instance.GetPvfComment(res);
			if (diskData.Data != null)
			{
				diskData.Data = (PvfCommentDto)diskData.Data.Clone();
			}
			cloudData = await PostResultAsync<PvfCommentDto>("GetPvfComment", ToJson(res));
			if (AppSetting.Instance.PvfConfig.PvfCommentPriority == PvfCommentPriority.云端)
			{
				if (cloudData.IsError)
				{
					return diskData;
				}
				if (diskData.IsError)
				{
					return cloudData;
				}
				if (diskData.Data.UpdateTime > cloudData.Data.UpdateTime)
				{
					return diskData;
				}
				return cloudData;
			}
			if (diskData.IsError)
			{
				return cloudData;
			}
			if (cloudData.IsError)
			{
				return cloudData;
			}
			if (diskData.Data.UpdateTime > cloudData.Data.UpdateTime)
			{
				return diskData;
			}
			return cloudData;
		}
		catch (Exception ex)
		{
			cloudData.Msg = ex.Message;
		}
		return cloudData;
	}

	public async Task<ResultData> AddPvfComment(PvfCommentDto dto)
	{
		return await PostResultAsync("AddPvfComment", ToJson(dto));
	}

	public async Task<ResultData> AddCodeCompletionData(CodeCompletionData codeCompletionData, bool isShare)
	{
		ResultData re = new ResultData();
		try
		{
			string json = codeCompletionData.ToJson();
			AppSetting.Instance.EditConfig.SaveCompletionDatas(JsonConvert.DeserializeObject<CodeCompletionData>(json));
			await AppSetting.Instance.SaveSetting();
			if (isShare)
			{
				ResultData resultData = await PostResultAsync("AddCodeCompletionData", JsonConvert.DeserializeObject<CodeCompletionDataDto>(json).ToJson());
				if (resultData.IsError)
				{
					AppSetting.Instance.GetIlogger()?.Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_ShareFailed"), resultData.Msg));
				}
			}
		}
		catch (Exception ex)
		{
			re.Msg = ex.Message;
		}
		return re;
	}

	public async Task<ResultData> ShareMacroData(MacroDataRes dto)
	{
		return await Task.Run(() => PostResultAsync("ShareMacroData", ToJson(dto)));
	}

	public async Task<ResultData> UploadScriptFileContentFormatting(ScriptFileContentFormattingRes res)
	{
		return await PostResultAsync("UploadDiskScriptFileContentFormatting", ToJson(res));
	}

	public async Task<ResultData> UploadStoreListDatas()
	{
		List<StoreListRes> list = new List<StoreListRes>();
		if (AppSetting.Instance.BookMarkGroup.IsShare)
		{
			list.Add(new StoreListRes
			{
				Data = AppSetting.Instance.BookMarkGroup.Trees,
				Description = AppSetting.Instance.BookMarkGroup.Instructions,
				DetailedInstructions = AppSetting.Instance.BookMarkGroup.DetailedInstructions,
				Title = AppSetting.Instance.BookMarkGroup.Title,
				Type = StoreType.书签
			});
		}
		if (AppSetting.Instance.StoreOptions.UploadFileListComment)
		{
			StoreListRes item = AppSetting.Instance.StoreOptions.FileListComment.CloneData(AppSetting.Instance.PvfConfig.TreelistCommentDic);
			list.Add(item);
		}
		if (AppSetting.Instance.StoreOptions.UploadTabComment)
		{
			ResultData<List<PvfCommentDto>> resultData = await ServicePvfTabComment.Instance.GetList();
			if (resultData.Data?.Any() ?? false)
			{
				StoreListRes item2 = AppSetting.Instance.StoreOptions.TabComment.CloneData(resultData.Data);
				list.Add(item2);
			}
		}
		if (AppSetting.Instance.StoreOptions.UploadItemCodeHoverConfig)
		{
			StoreListRes item3 = AppSetting.Instance.StoreOptions.ItemCodeHoverConfig.CloneData(AppSetting.Instance.EditConfig.ItemCodeConvertItemNameConfiger.DIC);
			list.Add(item3);
			AppSetting.Instance.StoreOptions.ItemCodeHoverConfig.Data = null;
		}
		JsonSerializerSettings settings = new JsonSerializerSettings
		{
			TypeNameHandling = TypeNameHandling.All,
			ContractResolver = new JsonStringNullToEmpty()
		};
		string text = JsonConvert.SerializeObject(list, settings);
		return await PostResultAsync("UploadStoreListDatas", text);
	}

	public async Task<ResultData> DeleteStoreData(int id)
	{
		return await GetResultAsync<ResultData>($"DeleteStoreData?id={id}");
	}

	public async Task<ResultData<IEnumerable<PvfCommentDto>>> GetAllComment()
	{
		return await GetResultAsync<IEnumerable<PvfCommentDto>>("GetAllComment");
	}

	public async Task<ResultData<List<StoreListDto>>> GetStoreListData(GetStoreListQueryData queryData)
	{
		return await PostResultAsync<List<StoreListDto>>("GetStoreListData", ToJson(queryData));
	}

	public async Task<ResultData<StoreListDto>> GetStoreData(int id)
	{
		return await GetResultAsync<StoreListDto>($"GetStoreData?id={id}");
	}

	public async Task<ResultData<StoreListDto>> GetPreviewStoreData(int id)
	{
		return await GetResultAsync<StoreListDto>($"GetPreviewStoreData?id={id}");
	}

	public async Task<ResultData> AddStoreData(StoreListRes res)
	{
		return await PostResultAsync("AddStoreData", res.ToJson());
	}

	public async Task<ResultData<LoginResultDto>> Login(LoginAccountRes res)
	{
		ResultData<LoginResultDto> re = new ResultData<LoginResultDto>();
		int errid = 0;
		try
		{
			string text = ToJson(res);
			errid = 1;
			re = await PostResultAsync<LoginResultDto>("Login", text);
			errid = 2;
			if (re == null)
			{
				re = new ResultData<LoginResultDto>
				{
					Msg = "登录失败 resultIsNullt3"
				};
				return re;
			}
			if (!re.IsError)
			{
				errid = 3;
				User = re.Data;
				errid = 4;
				DoNotify("IsLogin");
				errid = 5;
				if (re.Data.Phone == 0L)
				{
					AppSetting.Instance.GetIlogger().Warning(AppSetting.Instance.GetIlogger()?.GetStr("mess_NoPhone"));
				}
			}
		}
		catch (Exception e)
		{
			AppSetting.Instance.GetIlogger().ErrorUploadDialog(e, "ServiceCloud.Login Errid:" + errid);
		}
		return re;
	}

	public async Task<ResultData> Logout()
	{
		User = null;
		return await GetResultAsync("Logout");
	}

	public async Task<ResultData> Registered(RegIsteredAccountRes res)
	{
		return await PostResultAsync("Registered", ToJson(res));
	}

	public async Task<ResultData> ForgetPassword(RegIsteredAccountRes res)
	{
		return await PostResultAsync("ForgetPassword", ToJson(res));
	}

	public async Task<ResultData> SaveUserInfo(EditUserInfoRes res)
	{
		return await PostResultAsync("SaveUserInfo", ToJson(res));
	}

	public async Task RefreshToken()
	{
		if (IsLogin)
		{
			string text = ToJson(new RefreshTokenRes
			{
				Token = User.TokenData.GetToken("45-069-dspfg;lfghfgh2fg2h+6fg2h6f2sdf")
			});
			ResultData<TokenResultDot> resultData = await PostResultAsync<TokenResultDot>("RefreshToken", text);
			if (resultData == null)
			{
				AppSetting.Instance.GetIlogger()?.Error("Token刷新失败 ResultIsNull");
			}
			else if (resultData.IsError)
			{
				AppSetting.Instance.GetIlogger()?.Error(string.Format(AppSetting.Instance.GetIlogger()?.GetStr("mess_RefreshLoginError"), resultData.Msg));
			}
			else
			{
				User.TokenData.Token = resultData.Data.Token;
			}
		}
	}

	public async Task<ResultData<IEnumerable<ContributeInfoDto>, IEnumerable<ContributeInfoDto>>> GetContributeInfoList()
	{
		return await GetResultAsync<IEnumerable<ContributeInfoDto>, IEnumerable<ContributeInfoDto>>("GetContributeInfoList");
	}

	private async Task<ResultData> GetResultAsync(string endpoint)
	{
		ResultData<string> response = await SendGetAsync(endpoint);
		if (response.IsError)
		{
			return response;
		}
		return response.Data.JsonToObject<ResultData>();
	}

	private async Task<ResultData<T>> GetResultAsync<T>(string endpoint)
	{
		ResultData<string> response = await SendGetAsync(endpoint);
		if (response.IsError)
		{
			return new ResultData<T>
			{
				Msg = response.Msg
			};
		}
		return response.Data.JsonToObject<ResultData<T>>();
	}

	private async Task<ResultData<TFirst, TSecond>> GetResultAsync<TFirst, TSecond>(string endpoint)
	{
		ResultData<string> response = await SendGetAsync(endpoint);
		if (response.IsError)
		{
			return new ResultData<TFirst, TSecond>
			{
				Msg = response.Msg
			};
		}
		return response.Data.JsonToObject<ResultData<TFirst, TSecond>>();
	}

	private Task<ResultData<string>> SendGetAsync(string endpoint)
	{
		return Task.FromResult(new ResultData<string> { Msg = "联网功能已移除" });
	}

	private async Task<ResultData> PostResultAsync(string endpoint, string json, bool useAbsoluteUrl = false)
	{
		ResultData<string> response = await SendPostAsync(endpoint, json, useAbsoluteUrl);
		if (response.IsError || string.IsNullOrEmpty(response.Data))
		{
			return response;
		}
		return response.Data.JsonToObject<ResultData>();
	}

	private async Task<ResultData<T>> PostResultAsync<T>(string endpoint, string json, bool useAbsoluteUrl = false)
	{
		ResultData<string> response = await SendPostAsync(endpoint, json, useAbsoluteUrl);
		if (response.IsError)
		{
			return new ResultData<T>
			{
				Msg = response.Msg
			};
		}
		return response.Data.JsonToObject<ResultData<T>>();
	}

	private Task<ResultData<string>> SendPostAsync(string endpoint, string json, bool useAbsoluteUrl = false)
	{
		return Task.FromResult(new ResultData<string> { Msg = "联网功能已移除" });
	}

	public string ToJson(object obj)
	{
		return JsonConvert.SerializeObject(obj, Formatting.None, new JsonSerializerSettings
		{
			ContractResolver = new JsonStringNullToEmpty()
		});
	}
}
