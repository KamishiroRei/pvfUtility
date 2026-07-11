using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
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

	[CompilerGenerated]
	private TreeShowNpkIconChanged Rb2nh8uwqt;

	private bool? ykQnTNO5A8;

	private bool? Iwgn04eR3n;

	private bool? fkAnsHKRiC;

	private bool TDvnQ6r97U;

	private bool Ua9n6ZBgEv;

	private TreeListGetItemNameAndItemCodeFormat UrwnyfQW3i;

	private string QmQnwHSpGm;

	private bool? V9nnoe7Dgm;

	private bool bQyn2jagaO;

	private bool JvEntGJYyl;

	[CompilerGenerated]
	private SolidColorBrush qxIn9W1xG3;

	private Dictionary<ThemeType, TreeColorConfig>? ogqnRGTuv6;

	private bool wp5nW5SZpV;

	private bool zb1nq92F5g;

	private bool W4Ynz7k7yL;

	public bool TreeShowNpkIcon
	{
		get
		{
			if (!ykQnTNO5A8.HasValue)
			{
				ykQnTNO5A8 = true;
			}
			return ykQnTNO5A8.Value;
		}
		set
		{
			ykQnTNO5A8 = value;
			RaisePropertyChanged("TreeShowNpkIcon");
		}
	}

	public bool UseRarityColor
	{
		get
		{
			if (!Iwgn04eR3n.HasValue)
			{
				Iwgn04eR3n = true;
			}
			return Iwgn04eR3n.Value;
		}
		set
		{
			Iwgn04eR3n = value;
			RaisePropertyChanged("UseRarityColor");
		}
	}

	public bool TreeListControlTopShowFilePathTextBox
	{
		get
		{
			if (!fkAnsHKRiC.HasValue)
			{
				fkAnsHKRiC = true;
			}
			return fkAnsHKRiC.Value;
		}
		set
		{
			fkAnsHKRiC = value;
			RaisePropertyChanged("TreeListControlTopShowFilePathTextBox");
		}
	}

	public bool GetItemCodeSort
	{
		get
		{
			return TDvnQ6r97U;
		}
		set
		{
			TDvnQ6r97U = value;
			RaisePropertyChanged("GetItemCodeSort");
		}
	}

	public bool GetItemNameAndItemCodeIsSort
	{
		get
		{
			return Ua9n6ZBgEv;
		}
		set
		{
			Ua9n6ZBgEv = value;
			RaisePropertyChanged("GetItemNameAndItemCodeIsSort");
		}
	}

	public TreeListGetItemNameAndItemCodeFormat TreeListGetItemNameAndItemCodeFormat
	{
		get
		{
			return UrwnyfQW3i;
		}
		set
		{
			UrwnyfQW3i = value;
			RaisePropertyChanged("TreeListGetItemNameAndItemCodeFormat");
		}
	}

	public string GetItemNameAndItemCodeSplitChar
	{
		get
		{
			if (string.IsNullOrEmpty(QmQnwHSpGm))
			{
				QmQnwHSpGm = "----";
			}
			return QmQnwHSpGm;
		}
		set
		{
			QmQnwHSpGm = value;
			RaisePropertyChanged("GetItemNameAndItemCodeSplitChar");
		}
	}

	public bool TwConvertSimplified
	{
		get
		{
			if (!V9nnoe7Dgm.HasValue)
			{
				V9nnoe7Dgm = true;
			}
			return V9nnoe7Dgm.Value;
		}
		set
		{
			V9nnoe7Dgm = value;
			RaisePropertyChanged("TwConvertSimplified");
		}
	}

	public bool ShowHorizontalLines
	{
		get
		{
			return bQyn2jagaO;
		}
		set
		{
			bQyn2jagaO = value;
			RaisePropertyChanged("ShowHorizontalLines");
		}
	}

	public bool AllowHorizontalScrollingAutoWidth
	{
		get
		{
			return JvEntGJYyl;
		}
		set
		{
			JvEntGJYyl = value;
			RaisePropertyChanged("AllowHorizontalScrollingAutoWidth");
		}
	}

	public SolidColorBrush TreeFilePathForeBrush
	{
		[CompilerGenerated]
		get
		{
			return qxIn9W1xG3;
		}
		[CompilerGenerated]
		set
		{
			qxIn9W1xG3 = value;
		}
	}

	public Dictionary<ThemeType, TreeColorConfig> TreeSolidColors
	{
		get
		{
			if (ogqnRGTuv6 == null)
			{
				ogqnRGTuv6 = new Dictionary<ThemeType, TreeColorConfig>();
			}
			foreach (object value in Enum.GetValues(typeof(ThemeType)))
			{
				ThemeType themeType = (ThemeType)Enum.Parse(typeof(ThemeType), value.ToString());
				if (!ogqnRGTuv6.ContainsKey(themeType))
				{
					ogqnRGTuv6.Add(themeType, new TreeColorConfig(themeType));
				}
			}
			return ogqnRGTuv6;
		}
		set
		{
			ogqnRGTuv6 = value;
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
			return wp5nW5SZpV;
		}
		set
		{
			wp5nW5SZpV = value;
			RaisePropertyChanged("ImportTreeListAutoExpandAllNodes");
		}
	}

	public bool UseEvenRowBackground
	{
		get
		{
			return zb1nq92F5g;
		}
		set
		{
			zb1nq92F5g = value;
			RaisePropertyChanged("UseEvenRowBackground");
		}
	}

	public bool CopyFilesSetToClipboard
	{
		get
		{
			return W4Ynz7k7yL;
		}
		set
		{
			W4Ynz7k7yL = value;
			RaisePropertyChanged("CopyFilesSetToClipboard");
		}
	}

	public event TreeShowNpkIconChanged EventTreeShowNpkIconChanged
	{
		[CompilerGenerated]
		add
		{
			TreeShowNpkIconChanged treeShowNpkIconChanged = Rb2nh8uwqt;
			TreeShowNpkIconChanged treeShowNpkIconChanged2;
			do
			{
				treeShowNpkIconChanged2 = treeShowNpkIconChanged;
				TreeShowNpkIconChanged value2 = (TreeShowNpkIconChanged)Delegate.Combine(treeShowNpkIconChanged2, value);
				treeShowNpkIconChanged = Interlocked.CompareExchange(ref Rb2nh8uwqt, value2, treeShowNpkIconChanged2);
			}
			while ((object)treeShowNpkIconChanged != treeShowNpkIconChanged2);
		}
		[CompilerGenerated]
		remove
		{
			TreeShowNpkIconChanged treeShowNpkIconChanged = Rb2nh8uwqt;
			TreeShowNpkIconChanged treeShowNpkIconChanged2;
			do
			{
				treeShowNpkIconChanged2 = treeShowNpkIconChanged;
				TreeShowNpkIconChanged value2 = (TreeShowNpkIconChanged)Delegate.Remove(treeShowNpkIconChanged2, value);
				treeShowNpkIconChanged = Interlocked.CompareExchange(ref Rb2nh8uwqt, value2, treeShowNpkIconChanged2);
			}
			while ((object)treeShowNpkIconChanged != treeShowNpkIconChanged2);
		}
	}

	[Command]
	public void TreeShowNpkIconC()
	{
		Rb2nh8uwqt?.Invoke();
	}

	public void ChangedNowTreeColorConfig()
	{
		RaisePropertyChanged("NowTreeColorConfig");
	}

	public TreeConfig()
	{
	}
}
