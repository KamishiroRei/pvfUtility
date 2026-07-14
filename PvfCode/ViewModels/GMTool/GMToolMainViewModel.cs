using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Xpf;
using GMTool.Dot;
using GMTool.Dot.QueryModel;
using GMTool.Dot.QueryModel.Enums;
using GMTool.Dot.taiwan_cain_2nd;
using GMTool.Services;
using GMTool.SqlModel.Enums;
using PvfCode.Dot;
using PvfCode.Models.Pvf.Enums;
using PvfCode.Services.GMTool.Models;
using PvfCode.Services.SearchModel;
using Swordfish.NET.Collections;

namespace PvfCode.ViewModels.GMTool;

public class GMToolMainViewModel : ViewModelBase
{
	public ConcurrentObservableCollection<AccountDto> AccountItems { get; set; }

	public List<AccountDto> SelectedAccountItems { get; set; }

	public ConcurrentObservableCollection<CharacInfoDto> CharacItems { get; set; }

	public List<CharacInfoDto> SelectedCharacInfoItems { get; set; }

	public ConcurrentObservableCollection<ItemCodePostalData> PostalItemCodeItems { get; set; }

	public List<ItemCodePostalData> SelectedPostalItemCodeItems { get; set; }

	public FindUserDto FindDto { get; set; }

	public ItemCodeFindRes ItemCodeFindRes { get; set; }

	public PostalSendRes PostaSendResData { get; set; }

	public OutPutViewModel OutPutViewModel { get; set; }

	public GMToolMainViewModel()
	{
		OutPutViewModel = new OutPutViewModel();
		SelectedAccountItems = new List<AccountDto>();
		SelectedCharacInfoItems = new List<CharacInfoDto>();
		SelectedPostalItemCodeItems = new List<ItemCodePostalData>();
		FindDto = new FindUserDto();
		AccountItems = new ConcurrentObservableCollection<AccountDto>();
		CharacItems = new ConcurrentObservableCollection<CharacInfoDto>();
		PostaSendResData = new PostalSendRes
		{
			add_info = 1
		};
		PostalItemCodeItems = new ConcurrentObservableCollection<ItemCodePostalData>();
		ItemCodeFindRes = new ItemCodeFindRes();
	}

	[Command]
	public async void OnFind(FindUserType? findUserType = null)
	{
		FindUserDto findUserDto = (FindUserDto)FindDto.Clone();
		if (findUserDto.Keyword == null)
		{
			findUserDto.Keyword = string.Empty;
		}
		if (findUserType.HasValue)
		{
			findUserDto.FindUserType = findUserType.Value;
		}
		else
		{
			findUserDto.FindUserType = FindDto.FindUserType;
		}
		switch (findUserDto.FindUserType)
		{
		case FindUserType.UserName:
		case FindUserType.UID:
		case FindUserType.IP:
		case FindUserType.RegTime:
		case FindUserType.OnLineUser:
		{
			AccountItems?.Clear();
			ConcurrentObservableCollection<AccountDto> accountItems = AccountItems;
			accountItems.AddRange(await new DnfSqlService(AppSetting.Instance.GameOptions.GameServerOptions.GetDb()).FindUserFromUserName(findUserDto));
			break;
		}
		case FindUserType.CharacName:
		case FindUserType.CharacId:
		case FindUserType.OnLineCharac:
		{
			CharacItems?.Clear();
			ConcurrentObservableCollection<CharacInfoDto> characItems = CharacItems;
			characItems.AddRange(await new DnfSqlService(AppSetting.Instance.GameOptions.GameServerOptions.GetDb()).FindCharac(findUserDto));
			break;
		}
		}
	}

	[Command]
	public async void UserGridDoubleClick(RowClickArgs nodeClickArgs)
	{
		if (nodeClickArgs.Item != null && nodeClickArgs.Item is AccountDto accountDto)
		{
			CharacItems?.Clear();
			FindUserDto dto = new FindUserDto
			{
				FindUserType = FindUserType.UidFindCharacs,
				Keyword = accountDto.UID.ToString()
			};
			ConcurrentObservableCollection<CharacInfoDto> characItems = CharacItems;
			characItems.AddRange(await new DnfSqlService(AppSetting.Instance.GameOptions.GameServerOptions.GetDb()).FindCharac(dto));
		}
	}

	[Command]
	public async void CharacGridDoubleClick(RowClickArgs nodeClickArgs)
	{
		if (nodeClickArgs.Item != null && nodeClickArgs.Item is CharacInfoDto characInfoDto)
		{
			FindUserDto dto = new FindUserDto
			{
				FindUserType = FindUserType.UID,
				Keyword = characInfoDto.UID.ToString()
			};
			AccountItems?.Clear();
			ConcurrentObservableCollection<AccountDto> accountItems = AccountItems;
			accountItems.AddRange(await new DnfSqlService(AppSetting.Instance.GameOptions.GameServerOptions.GetDb()).FindUserFromUserName(dto));
		}
	}

	[Command]
	public void OnRecharge()
	{
		_ = AppSetting.Instance.GMToolOptions.TopUpOptions;
	}

	[Command]
	public void OnCearRecharge()
	{
	}

	[Command]
	public async void OnSendPostal()
	{
		if (SelectedCharacInfoItems.Count != 0)
		{
			List<PostalSendRes> postalItems = CreatePostalItems();
			ResultData<string> resultData = await new DnfSqlService(AppSetting.Instance.GameOptions.GameServerOptions.GetDb()).SendPostal(SelectedCharacInfoItems, postalItems, AppSetting.Instance.GMToolOptions.PostalSendTitle, AppSetting.Instance.GMToolOptions.PostalSendText);
			if (resultData.IsError)
			{
				AppCore.ShowMsg(resultData.Msg);
			}
			else
			{
				OutPutViewModel.Success(resultData.Data);
			}
		}
	}

	private List<PostalSendRes> CreatePostalItems()
	{
		List<PostalSendRes> list = new List<PostalSendRes>();
		PostalSendRes postaSendResData = PostaSendResData;
		if (SelectedPostalItemCodeItems.Count == 1 && SelectedPostalItemCodeItems[0].ItemCode == PostaSendResData.item_id)
		{
			ItemCodePostalData itemCodePostalData = SelectedPostalItemCodeItems[0];
			PostalSendRes postalSendRes = postaSendResData.Clone();
			postalSendRes.IsEqu = itemCodePostalData.IsEqu;
			postalSendRes.ItemName = itemCodePostalData.ItemName;
			InitializePostalItem(postalSendRes, itemCodePostalData);
			list.Add(postalSendRes);
		}
		else if (SelectedPostalItemCodeItems.Count > 1)
		{
			foreach (ItemCodePostalData selectedPostalItemCodeItem in SelectedPostalItemCodeItems)
			{
				PostalSendRes postalSendRes2 = postaSendResData.Clone();
				postalSendRes2.item_id = selectedPostalItemCodeItem.ItemCode;
				postalSendRes2.IsEqu = selectedPostalItemCodeItem.IsEqu;
				postalSendRes2.ItemName = selectedPostalItemCodeItem.ItemName;
				InitializePostalItem(postalSendRes2, selectedPostalItemCodeItem);
				list.Add(postalSendRes2);
			}
		}
		else
		{
			PostalSendRes postalSendRes3 = postaSendResData.Clone();
			PvfGroup pVF = AppCore.ViewModelBase.PVF;
			if (pVF.PvfIsOpen)
			{
				string text = pVF.ListFileTable.ItemCodeConvertFilePath(new string[2]
				{
					"equipment",
					"stackable"
				}, postalSendRes3.item_id);
				if (text != null)
				{
					postalSendRes3.ItemName = pVF.GetItemName(text);
				}
			}
			list.Add(postalSendRes3);
		}
		return list;
	}

	private void InitializePostalItem(PostalSendRes postalItem, ItemCodePostalData itemCodeData)
	{
		if (itemCodeData.IsEqu)
		{
			switch (itemCodeData.EquType)
			{
			case EquTypeDefault.Default:
				postalItem.PostalType = PostalType.普通邮件;
				break;
			case EquTypeDefault.Avatar:
				postalItem.PostalType = PostalType.时装邮件;
				break;
			case EquTypeDefault.Pet:
			case EquTypeDefault.PetEqu:
				postalItem.PostalType = PostalType.宠物;
				break;
			case EquTypeDefault.PetEgg:
				postalItem.PostalType = PostalType.宠物蛋;
				break;
			}
		}
		postalItem.Init();
	}

	[Command]
	public void OnClearPostal()
	{
		Task.Run((Func<Task?>)FindPostalItemCodeTask);
	}

	public async Task FindPostalItemCodeTask()
	{
		PostalItemCodeItems.Clear();
		if (!string.IsNullOrEmpty(ItemCodeFindRes.Keyword))
		{
			ResultData<IEnumerable<ItemCodePostalData>> resultData = await new SearchService(new SearchConfig(), AppCore.ViewModelBase.PVF).SearchItemCode(ItemCodeFindRes.Keyword, ItemCodeFindRes.WholeWordMatch, ItemCodeFindRes.IsStartMatch);
			if (resultData.Data != null && resultData.Data.Any())
			{
				PostalItemCodeItems.AddRange(resultData.Data.ToList());
			}
		}
	}

	public string GetCharacImage(int job)
	{
		if (job < 10 && job > -1)
		{
			return $"/images/pngs/dnfcharacdefaulticon/{job}.png";
		}
		return null;
	}
}
