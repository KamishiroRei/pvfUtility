using System;
using System.Collections.Generic;
using System.Windows.Media;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using Newtonsoft.Json;
using PvfCode.Models.Enums;
using PvfCode.Models.Options.Enums;

namespace PvfCode;

[JsonObject(MemberSerialization.OptOut)]
public class TreeConfig : ViewModelBase
{
	public delegate void TreeShowNpkIconChanged();

	private bool? treeShowNpkIcon;

	private bool? useRarityColor;

	private bool? treeListControlTopShowFilePathTextBox;

	private bool getItemCodeSort;

	private bool getItemNameAndItemCodeIsSort;

	private TreeListGetItemNameAndItemCodeFormat itemNameAndItemCodeFormat;

	private string itemNameAndItemCodeSplitChar;

	private bool? twConvertSimplified;

	private bool showHorizontalLines;

	private bool allowHorizontalScrollingAutoWidth;

	private Dictionary<ThemeType, TreeColorConfig>? treeSolidColors;

	private bool importTreeListAutoExpandAllNodes;

	private bool useEvenRowBackground;

	private bool copyFilesSetToClipboard;

	public bool TreeShowNpkIcon
	{
		get
		{
			if (!treeShowNpkIcon.HasValue)
			{
				treeShowNpkIcon = true;
			}
			return treeShowNpkIcon.Value;
		}
		set
		{
			treeShowNpkIcon = value;
			RaisePropertyChanged("TreeShowNpkIcon");
		}
	}

	public bool UseRarityColor
	{
		get
		{
			if (!useRarityColor.HasValue)
			{
				useRarityColor = true;
			}
			return useRarityColor.Value;
		}
		set
		{
			useRarityColor = value;
			RaisePropertyChanged("UseRarityColor");
		}
	}

	public bool TreeListControlTopShowFilePathTextBox
	{
		get
		{
			if (!treeListControlTopShowFilePathTextBox.HasValue)
			{
				treeListControlTopShowFilePathTextBox = true;
			}
			return treeListControlTopShowFilePathTextBox.Value;
		}
		set
		{
			treeListControlTopShowFilePathTextBox = value;
			RaisePropertyChanged("TreeListControlTopShowFilePathTextBox");
		}
	}

	public bool GetItemCodeSort
	{
		get
		{
			return getItemCodeSort;
		}
		set
		{
			getItemCodeSort = value;
			RaisePropertyChanged("GetItemCodeSort");
		}
	}

	public bool GetItemNameAndItemCodeIsSort
	{
		get
		{
			return getItemNameAndItemCodeIsSort;
		}
		set
		{
			getItemNameAndItemCodeIsSort = value;
			RaisePropertyChanged("GetItemNameAndItemCodeIsSort");
		}
	}

	public TreeListGetItemNameAndItemCodeFormat TreeListGetItemNameAndItemCodeFormat
	{
		get
		{
			return itemNameAndItemCodeFormat;
		}
		set
		{
			itemNameAndItemCodeFormat = value;
			RaisePropertyChanged("TreeListGetItemNameAndItemCodeFormat");
		}
	}

	public string GetItemNameAndItemCodeSplitChar
	{
		get
		{
			if (string.IsNullOrEmpty(itemNameAndItemCodeSplitChar))
			{
				itemNameAndItemCodeSplitChar = "----";
			}
			return itemNameAndItemCodeSplitChar;
		}
		set
		{
			itemNameAndItemCodeSplitChar = value;
			RaisePropertyChanged("GetItemNameAndItemCodeSplitChar");
		}
	}

	public bool TwConvertSimplified
	{
		get
		{
			if (!twConvertSimplified.HasValue)
			{
				twConvertSimplified = true;
			}
			return twConvertSimplified.Value;
		}
		set
		{
			twConvertSimplified = value;
			RaisePropertyChanged("TwConvertSimplified");
		}
	}

	public bool ShowHorizontalLines
	{
		get
		{
			return showHorizontalLines;
		}
		set
		{
			showHorizontalLines = value;
			RaisePropertyChanged("ShowHorizontalLines");
		}
	}

	public bool AllowHorizontalScrollingAutoWidth
	{
		get
		{
			return allowHorizontalScrollingAutoWidth;
		}
		set
		{
			allowHorizontalScrollingAutoWidth = value;
			RaisePropertyChanged("AllowHorizontalScrollingAutoWidth");
		}
	}

	public SolidColorBrush TreeFilePathForeBrush { get; set; }

	public Dictionary<ThemeType, TreeColorConfig> TreeSolidColors
	{
		get
		{
			if (treeSolidColors == null)
			{
				treeSolidColors = new Dictionary<ThemeType, TreeColorConfig>();
			}
			foreach (object value in Enum.GetValues(typeof(ThemeType)))
			{
				ThemeType themeType = (ThemeType)Enum.Parse(typeof(ThemeType), value.ToString());
				if (!treeSolidColors.ContainsKey(themeType))
				{
					treeSolidColors.Add(themeType, new TreeColorConfig(themeType));
				}
			}
			return treeSolidColors;
		}
		set
		{
			treeSolidColors = value;
			RaisePropertyChanged("TreeSolidColors");
		}
	}

	[JsonIgnore]
	public TreeColorConfig NowTreeColorConfig
	{
		get
		{
			if (TreeSolidColors.TryGetValue(AppSetting.Instance.NowThemeType, out TreeColorConfig value))
			{
				return value;
			}
			return null;
		}
	}

	public bool ImportTreeListAutoExpandAllNodes
	{
		get
		{
			return importTreeListAutoExpandAllNodes;
		}
		set
		{
			importTreeListAutoExpandAllNodes = value;
			RaisePropertyChanged("ImportTreeListAutoExpandAllNodes");
		}
	}

	public bool UseEvenRowBackground
	{
		get
		{
			return useEvenRowBackground;
		}
		set
		{
			useEvenRowBackground = value;
			RaisePropertyChanged("UseEvenRowBackground");
		}
	}

	public bool CopyFilesSetToClipboard
	{
		get
		{
			return copyFilesSetToClipboard;
		}
		set
		{
			copyFilesSetToClipboard = value;
			RaisePropertyChanged("CopyFilesSetToClipboard");
		}
	}

	public event TreeShowNpkIconChanged EventTreeShowNpkIconChanged;

	[Command]
	public void TreeShowNpkIconC()
	{
		EventTreeShowNpkIconChanged?.Invoke();
	}

	public void ChangedNowTreeColorConfig()
	{
		RaisePropertyChanged("NowTreeColorConfig");
	}

	public TreeConfig()
	{
	}
}
