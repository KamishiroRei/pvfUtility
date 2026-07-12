using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using PvfCode.LoggerBase;
using PvfCode.Models.Pvf.Enums;
using PvfCode.Services.PreviewPvfFileFolder;

namespace PvfCode.ViewModels.NpcShopEditor;

public class NpcShopItem : ViewModelBase
{
	private string? price;
	private string? needMaterialItemCode;
	private string? needMaterialCount;
	private bool isLoadingPurchaseData;
	private bool isPriceModified;
	private bool isNeedMaterialModified;
	private bool isPurchaseDataLoaded;
	private List<int> additionalNeedMaterialValues = new List<int>();

	public PvfGroup Pvf => AppCore.ViewModelBase.PVF;

	public virtual PvfFile? File
	{
		get
		{
			if (ItemCode == -1 || ItemCode == -2)
			{
				return null;
			}
			return Pvf.ListFileTable.ItemCodeConvertPvfFile(Pvf, ItemCode);
		}
	}

	public virtual int ItemCode
	{
		get
		{
			return GetProperty(() => ItemCode);
		}
		set
		{
			if (GetProperty(() => ItemCode) == value)
			{
				return;
			}
			SetProperty(() => ItemCode, value);
			RaisePropertyChanged(nameof(File));
			RaisePropertyChanged(nameof(ItemName));
			RaisePropertyChanged(nameof(ImageSource));
			RaisePropertyChanged(nameof(EquIsSealing));
			RaisePropertyChanged(nameof(IsNull));
			RaisePropertyChanged(nameof(CanEditPurchaseData));
			ReloadPurchaseData();
		}
	}

	public string? Price
	{
		get => price;
		set
		{
			if (string.Equals(price, value, System.StringComparison.Ordinal))
			{
				return;
			}
			price = value;
			RaisePropertyChanged(nameof(Price));
			if (!isLoadingPurchaseData)
			{
				isPriceModified = true;
				RaisePropertyChanged(nameof(IsPriceModified));
				RaisePropertyChanged(nameof(IsPurchaseDataModified));
			}
		}
	}

	public string? NeedMaterialItemCode
	{
		get => needMaterialItemCode;
		set
		{
			if (string.Equals(needMaterialItemCode, value, System.StringComparison.Ordinal))
			{
				return;
			}
			needMaterialItemCode = value;
			RaisePropertyChanged(nameof(NeedMaterialItemCode));
			MarkNeedMaterialModified();
		}
	}

	public string? NeedMaterialCount
	{
		get => needMaterialCount;
		set
		{
			if (string.Equals(needMaterialCount, value, System.StringComparison.Ordinal))
			{
				return;
			}
			needMaterialCount = value;
			RaisePropertyChanged(nameof(NeedMaterialCount));
			MarkNeedMaterialModified();
		}
	}

	public bool CanEditPurchaseData => File?.FileType is PvfFileType.equ or PvfFileType.stk;

	public bool IsPurchaseDataModified => isPriceModified || isNeedMaterialModified;

	public bool IsPriceModified => isPriceModified;

	public bool IsNeedMaterialModified => isNeedMaterialModified;

	internal IReadOnlyList<int> AdditionalNeedMaterialValues => additionalNeedMaterialValues;

	public string? ItemName
	{
		get
		{
			PvfFile file = File;
			if (file == null)
			{
				return null;
			}
			return Pvf.GetItemName(file);
		}
	}

	public virtual ImageSource? ImageSource
	{
		get
		{
			if (ItemCode == -1)
			{
				return null;
			}
			PvfFile file = File;
			if (file == null)
			{
				return Res.Instance.ReadNull;
			}
			ImagePack2Service.Instance.TreeGetIcon(Pvf, file, out ImageSource imageSource);
			if (imageSource != null)
			{
				return imageSource;
			}
			return Res.Instance.ReadNull;
		}
	}

	public bool EquIsSealing
	{
		get
		{
			PvfFile file = File;
			if (file == null || file.FileType != PvfFileType.equ)
			{
				return false;
			}
			if (file.GetAttachType(Pvf, out var attachType))
			{
				return attachType == AttachType.sealing;
			}
			return false;
		}
	}

	public bool IsNull
	{
		get
		{
			if (ItemCode != -1)
			{
				return ItemCode == -2;
			}
			return true;
		}
	}

	public FilePreviewDataBase? PreviewBase
	{
		get
		{
			if (IsNull)
			{
				return null;
			}
			PvfFile file = File;
			if (file == null)
			{
				return null;
			}
			ImagePack2Service.Instance.TreeGetIcon(Pvf, file, out ImageSource imageSource);
			return FilePreviewDataBase.Create(Pvf, file, imageSource);
		}
	}

	public NpcShopItem(int itemCode)
	{
		ItemCode = itemCode;
	}

	public NpcShopItem()
	{
		ItemCode = -1;
	}

	internal void EnsurePurchaseDataLoaded()
	{
		if (!isPurchaseDataLoaded)
		{
			ReloadPurchaseData();
		}
	}

	internal void InvalidatePurchaseData()
	{
		isPurchaseDataLoaded = false;
		isPriceModified = false;
		isNeedMaterialModified = false;
		price = null;
		needMaterialItemCode = null;
		needMaterialCount = null;
		additionalNeedMaterialValues = new List<int>();
		RaisePurchaseDataPropertiesChanged();
	}

	internal void ReloadPurchaseData()
	{
		isLoadingPurchaseData = true;
		try
		{
			PvfFile? file = File;
			Price = file != null && file.GetPrice(Pvf, out string loadedPrice) ? loadedPrice : null;

			List<int> materialValues = new List<int>();
			if (file != null)
			{
				file.GetSectionIntArray(Pvf, "[need material]", out materialValues);
			}
			NeedMaterialItemCode = materialValues.Count > 0 ? materialValues[0].ToString() : null;
			NeedMaterialCount = materialValues.Count > 1 ? materialValues[1].ToString() : null;
			additionalNeedMaterialValues = materialValues.Skip(2).ToList();
		}
		finally
		{
			isLoadingPurchaseData = false;
			isPurchaseDataLoaded = true;
			isPriceModified = false;
			isNeedMaterialModified = false;
			RaisePurchaseDataPropertiesChanged();
			RaisePropertyChanged(nameof(PreviewBase));
		}
	}

	internal bool TryValidatePurchaseData(out string? error)
	{
		if (isPriceModified && !NpcShopPurchaseSectionFormatter.TryValidatePrice(Price, out error))
		{
			return false;
		}
		if (isNeedMaterialModified && !NpcShopPurchaseSectionFormatter.TryValidateNeedMaterial(
			NeedMaterialItemCode,
			NeedMaterialCount,
			AdditionalNeedMaterialValues,
			out error))
		{
			return false;
		}
		error = null;
		return true;
	}

	private void MarkNeedMaterialModified()
	{
		if (isLoadingPurchaseData)
		{
			return;
		}
		isNeedMaterialModified = true;
		RaisePropertyChanged(nameof(IsNeedMaterialModified));
		RaisePropertyChanged(nameof(IsPurchaseDataModified));
	}

	private void RaisePurchaseDataPropertiesChanged()
	{
		RaisePropertyChanged(nameof(Price));
		RaisePropertyChanged(nameof(NeedMaterialItemCode));
		RaisePropertyChanged(nameof(NeedMaterialCount));
		RaisePropertyChanged(nameof(IsPriceModified));
		RaisePropertyChanged(nameof(IsNeedMaterialModified));
		RaisePropertyChanged(nameof(IsPurchaseDataModified));
	}

	[Command]
	public void OnOpenFile()
	{
		if (ItemCode != -1)
		{
			Ilogger ilogger = AppSetting.Instance.GetIlogger();
			PvfFile file = File;
			if (file == null)
			{
				DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(10, 1);
				defaultInterpolatedStringHandler.AppendLiteral("代码对应文件不存在：");
				defaultInterpolatedStringHandler.AppendFormatted(ItemCode);
				ilogger.Error(defaultInterpolatedStringHandler.ToStringAndClear());
			}
			else
			{
				ilogger.OpenPvfFileDocument(file.FileName);
			}
		}
	}
}
