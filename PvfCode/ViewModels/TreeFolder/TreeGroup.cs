using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Collections.Pooled;
using PvfCode.Dot;
using PvfCode.Models.Pvf.Enums;
using PvfCode.Models.Pvf.ImportModels;
using PvfCode.Models.Pvf.PvfFileDiffModels.Enums;
using Swordfish.NET.Collections.Auxiliary;
using Utools;

namespace PvfCode.ViewModels.TreeFolder;

public class TreeGroup : ModelBase
{
	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass38_0
	{
		public TreeGroup S6aLXs0qDA;

		public PooledList<string> XEBLpxAx9T;

		public PvfGroup pvf;

		public ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> QqnLU81aQY;

		public _003C_003Ec__DisplayClass38_0()
		{
		}

		internal Task? qaDL7GbYju()
		{
			return S6aLXs0qDA.lA3r5FSjih(XEBLpxAx9T, pvf, QqnLU81aQY);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass39_0
	{
		public ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> QDFL8NknZv;

		public char[] ePCLMRmgtc;

		public TreeGroup fJcLVVknAP;

		public PvfGroup pvf;

		public bool twrL3JLi9K;

		public _003C_003Ec__DisplayClass39_0()
		{
		}

		internal void mwoLcebG4N(string fullPath, ParallelLoopState ct)
		{
			ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> observableConcurrentDictionaryEx = QDFL8NknZv;
			string[] array = fullPath.Split(ePCLMRmgtc);
			short num = 0;
			int num2 = array.Length - 1;
			StringBuilder stringBuilder = new StringBuilder();
			string[] array2 = array;
			foreach (string text in array2)
			{
				lock (fJcLVVknAP)
				{
					if (num == 0)
					{
						stringBuilder.Append(text);
					}
					else
					{
						stringBuilder.Append("/");
						stringBuilder.Append(text);
					}
					if (observableConcurrentDictionaryEx.TryGetValue(text, out var value))
					{
						observableConcurrentDictionaryEx = value.Children;
					}
					else
					{
						PvfTreeFile pvfTreeFile = new PvfTreeFile(pvf, stringBuilder.ToString(), text, num == num2, num);
						if (num == 0)
						{
							twrL3JLi9K = true;
						}
						observableConcurrentDictionaryEx.AddTry(text, pvfTreeFile);
						observableConcurrentDictionaryEx = pvfTreeFile.Children;
					}
				}
				num++;
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass40_0
	{
		public TreeGroup i7mLNl4QOg;

		public PooledList<string> lUrLzqYVje;

		public PvfGroup pvf;

		public ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> qrFnDZbnNd;

		public _003C_003Ec__DisplayClass40_0()
		{
		}

		internal Task? Om9LRuVudD()
		{
			return i7mLNl4QOg.CoIrSffY4x(lUrLzqYVje, pvf, qrFnDZbnNd);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass41_0
	{
		public ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> bDenjb4WRF;

		public char[] TZynTyfmlf;

		public TreeGroup a1KnCTiUlc;

		public PvfGroup pvf;

		public bool n15nHBTFWb;

		public _003C_003Ec__DisplayClass41_0()
		{
		}

		internal void kWInl57WK3(string fullPath, ParallelLoopState ct)
		{
			ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> observableConcurrentDictionaryEx = bDenjb4WRF;
			string[] array = fullPath.Split(TZynTyfmlf);
			short num = 0;
			_ = array.Length;
			StringBuilder stringBuilder = new StringBuilder();
			string[] array2 = array;
			foreach (string text in array2)
			{
				lock (a1KnCTiUlc)
				{
					if (num == 0)
					{
						stringBuilder.Append(text);
					}
					else
					{
						stringBuilder.Append("/");
						stringBuilder.Append(text);
					}
					if (observableConcurrentDictionaryEx.TryGetValue(text, out var value))
					{
						observableConcurrentDictionaryEx = value.Children;
					}
					else
					{
						PvfTreeFile pvfTreeFile = new PvfTreeFile(pvf, stringBuilder.ToString(), text, isFile: false, num);
						if (num == 0)
						{
							n15nHBTFWb = true;
						}
						observableConcurrentDictionaryEx.AddTry(text, pvfTreeFile);
						observableConcurrentDictionaryEx = pvfTreeFile.Children;
					}
				}
				num++;
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass42_0
	{
		public TreeGroup sePnvd4KSJ;

		public IEnumerable<string> SlbnBf61BW;

		public PvfGroup pvf;

		public Dictionary<string, TreelistCommentRes> Fd2nFjFmJI;

		public ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> lPjnrHc3o5;

		public _003C_003Ec__DisplayClass42_0()
		{
		}

		internal Task? hSXnhrUBoJ()
		{
			return sePnvd4KSJ.HKorAHHYjO(SlbnBf61BW, pvf, Fd2nFjFmJI, lPjnrHc3o5);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass43_0
	{
		public ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> pJ5nmGSOwZ;

		public char[] Crpn22E5Ig;

		public TreeGroup mmcnfpj5dS;

		public PvfGroup pvf;

		public Dictionary<string, TreelistCommentRes> OESn5XB05M;

		public _003C_003Ec__DisplayClass43_0()
		{
		}

		internal void sm1nW4SsXd(string fullPath, ParallelLoopState ct)
		{
			ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> observableConcurrentDictionaryEx = pJ5nmGSOwZ;
			string[] array = fullPath.Split(Crpn22E5Ig);
			short num = 0;
			int num2 = array.Length - 1;
			string text = null;
			string[] array2 = array;
			foreach (string text2 in array2)
			{
				text = ((num != 0) ? (text + "/" + text2) : text2);
				lock (mmcnfpj5dS)
				{
					if (observableConcurrentDictionaryEx.TryGetValue(text2, out var value))
					{
						observableConcurrentDictionaryEx = value.Children;
					}
					else
					{
						PvfTreeFileFileListDescription pvfTreeFileFileListDescription = new PvfTreeFileFileListDescription(pvf, text, text2, num == num2, num, OESn5XB05M);
						observableConcurrentDictionaryEx.AddTry(text2, pvfTreeFileFileListDescription);
						observableConcurrentDictionaryEx = pvfTreeFileFileListDescription.Children;
					}
				}
				num++;
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass44_0
	{
		public TreeGroup FKrnAio8l7;

		public IDictionary<string, List<PvfFileDiffType>?> hDBn4m9wvn;

		public TreeViewType KKUnY3L0di;

		public _003C_003Ec__DisplayClass44_0()
		{
		}

		internal Task? bOOnSi5k3k()
		{
			return FKrnAio8l7.DiffCreateTreesTask(hDBn4m9wvn, KKUnY3L0di);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass45_0
	{
		public TreeGroup LukniEyyua;

		public char[] MaBnufusMi;

		public TreeViewType ndJnGC331B;

		public _003C_003Ec__DisplayClass45_0()
		{
		}

		internal void nhAny30X2W(KeyValuePair<string, List<PvfFileDiffType>> row, ParallelLoopState ct)
		{
			ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> observableConcurrentDictionaryEx = LukniEyyua.Trees;
			string[] array = row.Key.Split(MaBnufusMi);
			short num = 0;
			int num2 = array.Length - 1;
			StringBuilder stringBuilder = new StringBuilder();
			string[] array2 = array;
			foreach (string text in array2)
			{
				lock (LukniEyyua)
				{
					if (num == 0)
					{
						stringBuilder.Append(text);
					}
					else
					{
						stringBuilder.Append("/");
						stringBuilder.Append(text);
					}
					if (observableConcurrentDictionaryEx.TryGetValue(text, out var value))
					{
						observableConcurrentDictionaryEx = value.Children;
					}
					else
					{
						bool flag = num == num2;
						PvfTreeFileDiff pvfTreeFileDiff = new PvfTreeFileDiff(AppCore.ViewModelBase.PVF, stringBuilder.ToString(), text, flag, num, flag ? row.Value : null, ndJnGC331B);
						observableConcurrentDictionaryEx.AddTry(text, pvfTreeFileDiff);
						observableConcurrentDictionaryEx = pvfTreeFileDiff.Children;
					}
				}
				num++;
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass46_0
	{
		public TreeGroup o3LnQkqvux;

		public IEnumerable<ImportFileItem> eWKnaofprt;

		public string FIFngE8hfR;

		public bool Iddn6MwvkV;

		public _003C_003Ec__DisplayClass46_0()
		{
		}

		internal Task? CWInxxef0s()
		{
			return o3LnQkqvux.lMtr4mBvmV(eWKnaofprt.ToArray(), FIFngE8hfR, Iddn6MwvkV);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass47_0
	{
		public TreeGroup IYKnwywOYZ;

		public bool tPjno1xXjH;

		public string tLsnsfm9rk;

		public char[] nCknLvvcQd;

		public _003C_003Ec__DisplayClass47_0()
		{
		}

		internal async ValueTask JEZn16Paqg(ImportFileItem importItem, CancellationToken ct)
		{
			ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> observableConcurrentDictionaryEx = IYKnwywOYZ.Trees;
			string text = ((!tPjno1xXjH) ? (tLsnsfm9rk + importItem.FilePath) : importItem.FilePath);
			string[] array = text.ToLower().Split(nCknLvvcQd, StringSplitOptions.RemoveEmptyEntries);
			short num = 0;
			int num2 = array.Length - 1;
			StringBuilder stringBuilder = new StringBuilder();
			string[] array2 = array;
			foreach (string text2 in array2)
			{
				lock (IYKnwywOYZ)
				{
					if (num == 0)
					{
						stringBuilder.Append(text2);
					}
					else
					{
						stringBuilder.Append("/");
						stringBuilder.Append(text2);
					}
					if (observableConcurrentDictionaryEx.TryGetValue(text2, out var value))
					{
						observableConcurrentDictionaryEx = value.Children;
					}
					else
					{
						bool flag = num == num2;
						PvfTreeFileImport pvfTreeFileImport = new PvfTreeFileImport(AppCore.ViewModelBase.PVF, stringBuilder.ToString(), text2, flag, num, flag ? importItem : null);
						observableConcurrentDictionaryEx.AddTry(text2, pvfTreeFileImport);
						observableConcurrentDictionaryEx = pvfTreeFileImport.Children;
					}
				}
				num++;
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass52_0
	{
		public IEnumerable<KeyValuePair<string, PvfTreeFileBase>> jtTnq3Sjo1;

		public TreeGroup qoqndVZiXV;

		public _003C_003Ec__DisplayClass52_0()
		{
		}

		internal void RmjnnOJZIE(ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> trees)
		{
			foreach (KeyValuePair<string, PvfTreeFileBase> item in jtTnq3Sjo1)
			{
				string fullPath = item.Value.FullPath;
				if (fullPath.IndexOf("/") < 0)
				{
					trees.RemoveTry(item.Key);
					continue;
				}
				ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> observableConcurrentDictionaryEx = trees;
				string[] array = fullPath.Split(qoqndVZiXV.BGHr16HlmT, StringSplitOptions.RemoveEmptyEntries);
				PvfTreeFileBase value = null;
				for (int i = 0; i < array.Length - 1; i++)
				{
					string key = array[i];
					if (observableConcurrentDictionaryEx.TryGetValue(key, out value))
					{
						observableConcurrentDictionaryEx = value.Children;
					}
				}
				observableConcurrentDictionaryEx.RemoveTry(array[^1]);
			}
			trees.NotifyObserversOfChange();
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass53_0
	{
		public KeyValuePair<string, PvfTreeFileBase> rR1ntvGGDs;

		public TreeGroup OEPnbhtVr5;

		public _003C_003Ec__DisplayClass53_0()
		{
		}

		internal void e0dnegg6ml(ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> trees)
		{
			if (trees == null)
			{
				return;
			}
			string fullPath = rR1ntvGGDs.Value.FullPath;
			if (fullPath.IndexOf("/") < 0)
			{
				trees.RemoveTry(rR1ntvGGDs.Key);
				return;
			}
			ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> observableConcurrentDictionaryEx = trees;
			string[] array = fullPath.Split(OEPnbhtVr5.BGHr16HlmT, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < array.Length - 1; i++)
			{
				string key = array[i];
				if (observableConcurrentDictionaryEx.TryGetValue(key, out var value))
				{
					observableConcurrentDictionaryEx = value.Children;
				}
			}
			observableConcurrentDictionaryEx.RemoveTry(array[^1]);
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass71_0
	{
		public TreeGroup FS8nEksdNx;

		public string UqFnOVMKZ7;

		public ConcurrentBag<KeyValuePair<string, PvfTreeFileBase>> zq2nK6S0Ok;

		public _003C_003Ec__DisplayClass71_0()
		{
		}

		internal void NPsnIuqYUy(KeyValuePair<string, PvfTreeFileBase> row)
		{
			lock (FS8nEksdNx)
			{
				PvfTreeFileBase value = row.Value;
				if (FS8nEksdNx.SearchPanelOptions.FilePath && value.FullPath.ToLower().Contains(UqFnOVMKZ7))
				{
					zq2nK6S0Ok.Add(row);
				}
				else if (FS8nEksdNx.SearchPanelOptions.ItemName && value.ItemName != null && value.ItemName.Contains(UqFnOVMKZ7))
				{
					zq2nK6S0Ok.Add(row);
				}
				else if (FS8nEksdNx.SearchPanelOptions.Comment && value.Comment != null && value.Comment.Contains(UqFnOVMKZ7))
				{
					zq2nK6S0Ok.Add(row);
				}
				else if (FS8nEksdNx.SearchPanelOptions.ItemCode && value.ItemCodeStr != null && value.ItemCodeStr.Contains(UqFnOVMKZ7))
				{
					zq2nK6S0Ok.Add(row);
				}
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass71_1
	{
		public Action<KeyValuePair<string, PvfTreeFileBase>> SrMnPaK3ph;

		public _003C_003Ec__DisplayClass71_0 QMcnZgHGhI;

		public _003C_003Ec__DisplayClass71_1()
		{
		}

		internal async void eQ8n9kcYSX(KeyValuePair<string, PvfTreeFileBase> item)
		{
			if (item.Value.IsFile)
			{
				SrMnPaK3ph(item);
				return;
			}
			SrMnPaK3ph(item);
			if (item.Value.HaveChildren())
			{
				await QMcnZgHGhI.FS8nEksdNx.IKuruKSxLh(item.Value.Children, SrMnPaK3ph);
			}
		}
	}

	[CompilerGenerated]
	private sealed class _003C_003Ec__DisplayClass72_0
	{
		public Action<KeyValuePair<string, PvfTreeFileBase>> YQ9nkSHal0;

		public TreeGroup aBln021vWA;

		public _003C_003Ec__DisplayClass72_0()
		{
		}

		internal async void o33nJ5jotA(KeyValuePair<string, PvfTreeFileBase> item)
		{
			if (item.Value.IsFile)
			{
				YQ9nkSHal0(item);
				return;
			}
			YQ9nkSHal0(item);
			if (item.Value.HaveChildren())
			{
				await aBln021vWA.IKuruKSxLh(item.Value.Children, YQ9nkSHal0);
			}
		}
	}

	private readonly TreeViewType TreeType;

	public ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> _Trees;

	[CompilerGenerated]
	private SearchPanelOptions hdQrxcasEW;

	public ObservableConcurrentDictionaryEx<string, PvfTreeFileBase>? _SearchResultTrees;

	[CompilerGenerated]
	private bool Ws2rQhMbAY;

	private PVfTreeChildrenSelector thVra7sWbO;

	private bool YlBrgwFXss;

	private string I85r6UanfN;

	private readonly char[] BGHr16HlmT;

	public ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> Trees
	{
		get
		{
			if (!ShowSearchResulTrees)
			{
				return _Trees;
			}
			return _SearchResultTrees;
		}
		set
		{
			_Trees = value;
			DoNotify("Trees");
			if (!ShowSearchResulTrees)
			{
				UpdateFileCount();
			}
		}
	}

	public SearchPanelOptions SearchPanelOptions
	{
		[CompilerGenerated]
		get
		{
			return hdQrxcasEW;
		}
		[CompilerGenerated]
		set
		{
			hdQrxcasEW = value;
		}
	}

	public bool ShowSearchResulTrees
	{
		[CompilerGenerated]
		get
		{
			return Ws2rQhMbAY;
		}
		[CompilerGenerated]
		set
		{
			Ws2rQhMbAY = value;
		}
	}

	public PVfTreeChildrenSelector ChildNodesSelector
	{
		get
		{
			return thVra7sWbO;
		}
		set
		{
			thVra7sWbO = value;
			DoNotify("ChildNodesSelector");
		}
	}

	public bool Loading
	{
		get
		{
			return YlBrgwFXss;
		}
		set
		{
			YlBrgwFXss = value;
			DoNotify("Loading");
		}
	}

	public string LoadingTitle
	{
		get
		{
			return I85r6UanfN;
		}
		set
		{
			I85r6UanfN = value;
			DoNotify(LoadingTitle);
		}
	}

	public int FileCount => A9xryKLBwM();

	public ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> GetTrees()
	{
		return _Trees;
	}

	public void ClearSearchResult()
	{
		_SearchResultTrees = null;
		ShowSearchResulTrees = false;
		DoNotify("Trees");
	}

	public TreeGroup(TreeViewType treeType)
	{
		BGHr16HlmT = new char[2] { '\\', '/' };
		SearchPanelOptions = new SearchPanelOptions();
		TreeType = treeType;
		Trees = new ObservableConcurrentDictionaryEx<string, PvfTreeFileBase>();
		N0irfsd7Nn();
		if (TreeType == TreeViewType.SearchResult)
		{
			LoadingTitle = AppSetting.Instance.GetIlogger().GetStr("mess_Searching");
		}
		else
		{
			LoadingTitle = "Loading...";
		}
	}

	public void UpdateFileCount()
	{
		DoNotify("FileCount");
	}

	private IEnumerable VhwrWxK4bx(object P_0)
	{
		if (P_0 == null)
		{
			return null;
		}
		KeyValuePair<string, PvfTreeFileBase>? keyValuePair = (KeyValuePair<string, PvfTreeFileBase>)P_0;
		if (!keyValuePair.HasValue)
		{
			return null;
		}
		if (keyValuePair.Value.Value.IsFileMethon())
		{
			return null;
		}
		return keyValuePair.Value.Value.Children;
	}

	private IEnumerable BJarmdogIw(object P_0)
	{
		if (P_0 == null)
		{
			return null;
		}
		KeyValuePair<string, PvfTreeFileBase> keyValuePair = (KeyValuePair<string, PvfTreeFileBase>)P_0;
		if (keyValuePair.Value.IsFileMethon())
		{
			return null;
		}
		return Onyr2hxYdM(keyValuePair.Value.Children);
	}

	private Dictionary<string, PvfTreeFileBase> Onyr2hxYdM(IDictionary<string, PvfTreeFileBase> P_0)
	{
		Dictionary<string, PvfTreeFileBase> dictionary = new Dictionary<string, PvfTreeFileBase>();
		foreach (KeyValuePair<string, PvfTreeFileBase> item in P_0)
		{
			if (!item.Value.IsFile)
			{
				dictionary.Add(item.Key, item.Value);
			}
		}
		return dictionary;
	}

	private void N0irfsd7Nn()
	{
		if (TreeType == TreeViewType.SelectFolder)
		{
			ChildNodesSelector = new PVfTreeChildrenSelector(BJarmdogIw);
		}
		else
		{
			ChildNodesSelector = new PVfTreeChildrenSelector(VhwrWxK4bx);
		}
	}

	public void Clear()
	{
		_Trees = null;
		_SearchResultTrees = null;
		Trees = null;
		ClearSearchResult();
	}

	public async Task<ObservableConcurrentDictionaryEx<string, PvfTreeFileBase>> CreateTrees(PooledList<string> fileList, PvfGroup? pvf = null, ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> source = null)
	{
		_003C_003Ec__DisplayClass38_0 CS_0024_003C_003E8__locals15 = new _003C_003Ec__DisplayClass38_0();
		CS_0024_003C_003E8__locals15.S6aLXs0qDA = this;
		CS_0024_003C_003E8__locals15.XEBLpxAx9T = fileList;
		CS_0024_003C_003E8__locals15.pvf = pvf;
		CS_0024_003C_003E8__locals15.QqnLU81aQY = source;
		if (Trees == null)
		{
			Trees = new ObservableConcurrentDictionaryEx<string, PvfTreeFileBase>();
		}
		if (CS_0024_003C_003E8__locals15.QqnLU81aQY == null)
		{
			CS_0024_003C_003E8__locals15.QqnLU81aQY = _Trees;
		}
		if (CS_0024_003C_003E8__locals15.XEBLpxAx9T == null || CS_0024_003C_003E8__locals15.XEBLpxAx9T.Count() == 0)
		{
			return Trees;
		}
		Loading = true;
		if (CS_0024_003C_003E8__locals15.pvf == null)
		{
			CS_0024_003C_003E8__locals15.pvf = AppCore.ViewModelBase.PVF;
		}
		await Task.Run(() => CS_0024_003C_003E8__locals15.S6aLXs0qDA.lA3r5FSjih(CS_0024_003C_003E8__locals15.XEBLpxAx9T, CS_0024_003C_003E8__locals15.pvf, CS_0024_003C_003E8__locals15.QqnLU81aQY));
		UpdateFileCount();
		if (Trees != null)
		{
			CS_0024_003C_003E8__locals15.QqnLU81aQY.NotifyObserversOfChange();
		}
		Loading = false;
		return Trees;
	}

	private Task lA3r5FSjih(IEnumerable<string> P_0, PvfGroup P_1, ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> P_2)
	{
		_003C_003Ec__DisplayClass39_0 CS_0024_003C_003E8__locals12 = new _003C_003Ec__DisplayClass39_0();
		CS_0024_003C_003E8__locals12.QDFL8NknZv = P_2;
		CS_0024_003C_003E8__locals12.fJcLVVknAP = this;
		CS_0024_003C_003E8__locals12.pvf = P_1;
		if (!P_0.Any())
		{
			return Task.CompletedTask;
		}
		ParallelOptions parallelOptions = new ParallelOptions
		{
			MaxDegreeOfParallelism = 1
		};
		if (P_0.Count() >= 100)
		{
			parallelOptions.MaxDegreeOfParallelism = 100;
		}
		else
		{
			parallelOptions.MaxDegreeOfParallelism = 1;
		}
		CS_0024_003C_003E8__locals12.ePCLMRmgtc = BGHr16HlmT;
		CS_0024_003C_003E8__locals12.twrL3JLi9K = false;
		Parallel.ForEach(P_0.Where((string it) => it != null), parallelOptions, delegate(string fullPath, ParallelLoopState ct)
		{
			ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> observableConcurrentDictionaryEx = CS_0024_003C_003E8__locals12.QDFL8NknZv;
			string[] array = fullPath.Split(CS_0024_003C_003E8__locals12.ePCLMRmgtc);
			short num = 0;
			int num2 = array.Length - 1;
			StringBuilder stringBuilder = new StringBuilder();
			string[] array2 = array;
			foreach (string text in array2)
			{
				lock (CS_0024_003C_003E8__locals12.fJcLVVknAP)
				{
					if (num == 0)
					{
						stringBuilder.Append(text);
					}
					else
					{
						stringBuilder.Append("/");
						stringBuilder.Append(text);
					}
					if (observableConcurrentDictionaryEx.TryGetValue(text, out var value))
					{
						observableConcurrentDictionaryEx = value.Children;
					}
					else
					{
						PvfTreeFile pvfTreeFile = new PvfTreeFile(CS_0024_003C_003E8__locals12.pvf, stringBuilder.ToString(), text, num == num2, num);
						if (num == 0)
						{
							CS_0024_003C_003E8__locals12.twrL3JLi9K = true;
						}
						observableConcurrentDictionaryEx.AddTry(text, pvfTreeFile);
						observableConcurrentDictionaryEx = pvfTreeFile.Children;
					}
				}
				num++;
			}
		});
		return Task.CompletedTask;
	}

	public async Task CreateTreesFolder(PooledList<string> fileList, PvfGroup? pvf = null, ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> source = null)
	{
		_003C_003Ec__DisplayClass40_0 CS_0024_003C_003E8__locals15 = new _003C_003Ec__DisplayClass40_0();
		CS_0024_003C_003E8__locals15.i7mLNl4QOg = this;
		CS_0024_003C_003E8__locals15.lUrLzqYVje = fileList;
		CS_0024_003C_003E8__locals15.pvf = pvf;
		CS_0024_003C_003E8__locals15.qrFnDZbnNd = source;
		if (Trees == null)
		{
			Trees = new ObservableConcurrentDictionaryEx<string, PvfTreeFileBase>();
		}
		if (CS_0024_003C_003E8__locals15.qrFnDZbnNd == null)
		{
			CS_0024_003C_003E8__locals15.qrFnDZbnNd = _Trees;
		}
		if (CS_0024_003C_003E8__locals15.lUrLzqYVje != null && CS_0024_003C_003E8__locals15.lUrLzqYVje.Count() != 0)
		{
			Loading = true;
			if (CS_0024_003C_003E8__locals15.pvf == null)
			{
				CS_0024_003C_003E8__locals15.pvf = AppCore.ViewModelBase.PVF;
			}
			await Task.Run(() => CS_0024_003C_003E8__locals15.i7mLNl4QOg.CoIrSffY4x(CS_0024_003C_003E8__locals15.lUrLzqYVje, CS_0024_003C_003E8__locals15.pvf, CS_0024_003C_003E8__locals15.qrFnDZbnNd));
			UpdateFileCount();
			if (Trees != null)
			{
				CS_0024_003C_003E8__locals15.qrFnDZbnNd.NotifyObserversOfChange();
			}
			Loading = false;
		}
	}

	private Task CoIrSffY4x(IEnumerable<string> P_0, PvfGroup P_1, ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> P_2)
	{
		_003C_003Ec__DisplayClass41_0 CS_0024_003C_003E8__locals12 = new _003C_003Ec__DisplayClass41_0();
		CS_0024_003C_003E8__locals12.bDenjb4WRF = P_2;
		CS_0024_003C_003E8__locals12.a1KnCTiUlc = this;
		CS_0024_003C_003E8__locals12.pvf = P_1;
		if (!P_0.Any())
		{
			return Task.CompletedTask;
		}
		ParallelOptions parallelOptions = new ParallelOptions
		{
			MaxDegreeOfParallelism = 1
		};
		if (P_0.Count() >= 100)
		{
			parallelOptions.MaxDegreeOfParallelism = 100;
		}
		else
		{
			parallelOptions.MaxDegreeOfParallelism = 1;
		}
		CS_0024_003C_003E8__locals12.TZynTyfmlf = BGHr16HlmT;
		CS_0024_003C_003E8__locals12.n15nHBTFWb = false;
		Parallel.ForEach(P_0.Where((string it) => it != null), parallelOptions, delegate(string fullPath, ParallelLoopState ct)
		{
			ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> observableConcurrentDictionaryEx = CS_0024_003C_003E8__locals12.bDenjb4WRF;
			string[] array = fullPath.Split(CS_0024_003C_003E8__locals12.TZynTyfmlf);
			short num = 0;
			_ = array.Length;
			StringBuilder stringBuilder = new StringBuilder();
			string[] array2 = array;
			foreach (string text in array2)
			{
				lock (CS_0024_003C_003E8__locals12.a1KnCTiUlc)
				{
					if (num == 0)
					{
						stringBuilder.Append(text);
					}
					else
					{
						stringBuilder.Append("/");
						stringBuilder.Append(text);
					}
					if (observableConcurrentDictionaryEx.TryGetValue(text, out var value))
					{
						observableConcurrentDictionaryEx = value.Children;
					}
					else
					{
						PvfTreeFile pvfTreeFile = new PvfTreeFile(CS_0024_003C_003E8__locals12.pvf, stringBuilder.ToString(), text, isFile: false, num);
						if (num == 0)
						{
							CS_0024_003C_003E8__locals12.n15nHBTFWb = true;
						}
						observableConcurrentDictionaryEx.AddTry(text, pvfTreeFile);
						observableConcurrentDictionaryEx = pvfTreeFile.Children;
					}
				}
				num++;
			}
		});
		return Task.CompletedTask;
	}

	public async Task<ObservableConcurrentDictionaryEx<string, PvfTreeFileBase>> CreateFileListDescriptionTrees(IEnumerable<string> fileList, Dictionary<string, TreelistCommentRes> treelistCommentSource, PvfGroup? pvf = null, ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> source = null)
	{
		_003C_003Ec__DisplayClass42_0 CS_0024_003C_003E8__locals17 = new _003C_003Ec__DisplayClass42_0();
		CS_0024_003C_003E8__locals17.sePnvd4KSJ = this;
		CS_0024_003C_003E8__locals17.SlbnBf61BW = fileList;
		CS_0024_003C_003E8__locals17.pvf = pvf;
		CS_0024_003C_003E8__locals17.Fd2nFjFmJI = treelistCommentSource;
		CS_0024_003C_003E8__locals17.lPjnrHc3o5 = source;
		if (Trees == null)
		{
			Trees = new ObservableConcurrentDictionaryEx<string, PvfTreeFileBase>();
		}
		if (CS_0024_003C_003E8__locals17.lPjnrHc3o5 == null)
		{
			CS_0024_003C_003E8__locals17.lPjnrHc3o5 = _Trees;
		}
		if (CS_0024_003C_003E8__locals17.SlbnBf61BW == null || CS_0024_003C_003E8__locals17.SlbnBf61BW.Count() == 0)
		{
			return Trees;
		}
		Loading = true;
		if (CS_0024_003C_003E8__locals17.pvf == null)
		{
			CS_0024_003C_003E8__locals17.pvf = AppCore.ViewModelBase.PVF;
		}
		await Task.Run(() => CS_0024_003C_003E8__locals17.sePnvd4KSJ.HKorAHHYjO(CS_0024_003C_003E8__locals17.SlbnBf61BW, CS_0024_003C_003E8__locals17.pvf, CS_0024_003C_003E8__locals17.Fd2nFjFmJI, CS_0024_003C_003E8__locals17.lPjnrHc3o5));
		if (Trees != null)
		{
			DoNotify("Trees");
			CS_0024_003C_003E8__locals17.lPjnrHc3o5.NotifyObserversOfChange();
		}
		UpdateFileCount();
		Loading = false;
		return Trees;
	}

	private Task HKorAHHYjO(IEnumerable<string> P_0, PvfGroup P_1, Dictionary<string, TreelistCommentRes> P_2, ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> P_3)
	{
		_003C_003Ec__DisplayClass43_0 CS_0024_003C_003E8__locals11 = new _003C_003Ec__DisplayClass43_0();
		CS_0024_003C_003E8__locals11.pJ5nmGSOwZ = P_3;
		CS_0024_003C_003E8__locals11.mmcnfpj5dS = this;
		CS_0024_003C_003E8__locals11.pvf = P_1;
		CS_0024_003C_003E8__locals11.OESn5XB05M = P_2;
		if (!P_0.Any())
		{
			return Task.CompletedTask;
		}
		ParallelOptions parallelOptions = new ParallelOptions
		{
			MaxDegreeOfParallelism = 1
		};
		if (P_0.Count() >= 100)
		{
			parallelOptions.MaxDegreeOfParallelism = 100;
		}
		else
		{
			parallelOptions.MaxDegreeOfParallelism = 1;
		}
		CS_0024_003C_003E8__locals11.Crpn22E5Ig = BGHr16HlmT;
		Parallel.ForEach(P_0.Where((string it) => it != null), parallelOptions, delegate(string fullPath, ParallelLoopState ct)
		{
			ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> observableConcurrentDictionaryEx = CS_0024_003C_003E8__locals11.pJ5nmGSOwZ;
			string[] array = fullPath.Split(CS_0024_003C_003E8__locals11.Crpn22E5Ig);
			short num = 0;
			int num2 = array.Length - 1;
			string text = null;
			string[] array2 = array;
			foreach (string text2 in array2)
			{
				text = ((num != 0) ? (text + "/" + text2) : text2);
				lock (CS_0024_003C_003E8__locals11.mmcnfpj5dS)
				{
					if (observableConcurrentDictionaryEx.TryGetValue(text2, out var value))
					{
						observableConcurrentDictionaryEx = value.Children;
					}
					else
					{
						PvfTreeFileFileListDescription pvfTreeFileFileListDescription = new PvfTreeFileFileListDescription(CS_0024_003C_003E8__locals11.pvf, text, text2, num == num2, num, CS_0024_003C_003E8__locals11.OESn5XB05M);
						observableConcurrentDictionaryEx.AddTry(text2, pvfTreeFileFileListDescription);
						observableConcurrentDictionaryEx = pvfTreeFileFileListDescription.Children;
					}
				}
				num++;
			}
		});
		return Task.CompletedTask;
	}

	public async Task DiffCreateTrees(IDictionary<string, List<PvfFileDiffType>?> fileList, TreeViewType treeViewType)
	{
		_003C_003Ec__DisplayClass44_0 CS_0024_003C_003E8__locals8 = new _003C_003Ec__DisplayClass44_0();
		CS_0024_003C_003E8__locals8.FKrnAio8l7 = this;
		CS_0024_003C_003E8__locals8.hDBn4m9wvn = fileList;
		CS_0024_003C_003E8__locals8.KKUnY3L0di = treeViewType;
		if (Trees == null)
		{
			Trees = new ObservableConcurrentDictionaryEx<string, PvfTreeFileBase>();
		}
		if (CS_0024_003C_003E8__locals8.hDBn4m9wvn != null && CS_0024_003C_003E8__locals8.hDBn4m9wvn.Count() != 0)
		{
			Loading = true;
			await Task.Run(() => CS_0024_003C_003E8__locals8.FKrnAio8l7.DiffCreateTreesTask(CS_0024_003C_003E8__locals8.hDBn4m9wvn, CS_0024_003C_003E8__locals8.KKUnY3L0di));
			if (Trees != null)
			{
				DoNotify("Trees");
				Trees.NotifyObserversOfChange();
			}
			UpdateFileCount();
			Loading = false;
		}
	}

	public Task DiffCreateTreesTask(IDictionary<string, List<PvfFileDiffType>?> fileList, TreeViewType treeViewType)
	{
		_003C_003Ec__DisplayClass45_0 CS_0024_003C_003E8__locals7 = new _003C_003Ec__DisplayClass45_0();
		CS_0024_003C_003E8__locals7.LukniEyyua = this;
		CS_0024_003C_003E8__locals7.ndJnGC331B = treeViewType;
		if (!fileList.Any())
		{
			return Task.CompletedTask;
		}
		ParallelOptions parallelOptions = new ParallelOptions
		{
			MaxDegreeOfParallelism = 100
		};
		if (fileList.Count() >= 100)
		{
			parallelOptions.MaxDegreeOfParallelism = 100;
		}
		else
		{
			parallelOptions.MaxDegreeOfParallelism = 1;
		}
		CS_0024_003C_003E8__locals7.MaBnufusMi = BGHr16HlmT;
		Parallel.ForEach(from it in fileList.ToArray()
			where it.Key != null
			select it, parallelOptions, delegate(KeyValuePair<string, List<PvfFileDiffType>> row, ParallelLoopState ct)
		{
			ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> observableConcurrentDictionaryEx = CS_0024_003C_003E8__locals7.LukniEyyua.Trees;
			string[] array = row.Key.Split(CS_0024_003C_003E8__locals7.MaBnufusMi);
			short num = 0;
			int num2 = array.Length - 1;
			StringBuilder stringBuilder = new StringBuilder();
			string[] array2 = array;
			foreach (string text in array2)
			{
				lock (CS_0024_003C_003E8__locals7.LukniEyyua)
				{
					if (num == 0)
					{
						stringBuilder.Append(text);
					}
					else
					{
						stringBuilder.Append("/");
						stringBuilder.Append(text);
					}
					if (observableConcurrentDictionaryEx.TryGetValue(text, out var value))
					{
						observableConcurrentDictionaryEx = value.Children;
					}
					else
					{
						bool flag = num == num2;
						PvfTreeFileDiff pvfTreeFileDiff = new PvfTreeFileDiff(AppCore.ViewModelBase.PVF, stringBuilder.ToString(), text, flag, num, flag ? row.Value : null, CS_0024_003C_003E8__locals7.ndJnGC331B);
						observableConcurrentDictionaryEx.AddTry(text, pvfTreeFileDiff);
						observableConcurrentDictionaryEx = pvfTreeFileDiff.Children;
					}
				}
				num++;
			}
		});
		return Task.CompletedTask;
	}

	public async Task ImportFilesCreateTrees(IEnumerable<ImportFileItem> fileList, string targetPath, bool is7zip = false)
	{
		_003C_003Ec__DisplayClass46_0 CS_0024_003C_003E8__locals9 = new _003C_003Ec__DisplayClass46_0();
		CS_0024_003C_003E8__locals9.o3LnQkqvux = this;
		CS_0024_003C_003E8__locals9.eWKnaofprt = fileList;
		CS_0024_003C_003E8__locals9.FIFngE8hfR = targetPath;
		CS_0024_003C_003E8__locals9.Iddn6MwvkV = is7zip;
		if (Trees == null)
		{
			Trees = new ObservableConcurrentDictionaryEx<string, PvfTreeFileBase>();
		}
		if (CS_0024_003C_003E8__locals9.eWKnaofprt != null)
		{
			await Task.Run(() => CS_0024_003C_003E8__locals9.o3LnQkqvux.lMtr4mBvmV(CS_0024_003C_003E8__locals9.eWKnaofprt.ToArray(), CS_0024_003C_003E8__locals9.FIFngE8hfR, CS_0024_003C_003E8__locals9.Iddn6MwvkV));
			if (Trees != null)
			{
				Trees.NotifyObserversOfChange();
			}
			UpdateFileCount();
		}
	}

	private async Task lMtr4mBvmV(IEnumerable<ImportFileItem> P_0, string P_1, bool P_2)
	{
		_003C_003Ec__DisplayClass47_0 CS_0024_003C_003E8__locals12 = new _003C_003Ec__DisplayClass47_0();
		CS_0024_003C_003E8__locals12.IYKnwywOYZ = this;
		CS_0024_003C_003E8__locals12.tLsnsfm9rk = P_1;
		ParallelOptions parallelOptions = new ParallelOptions
		{
			MaxDegreeOfParallelism = 1
		};
		if (P_0.Count() >= 100)
		{
			parallelOptions.MaxDegreeOfParallelism = 100;
		}
		else
		{
			parallelOptions.MaxDegreeOfParallelism = 1;
		}
		if (P_2 && !string.IsNullOrEmpty(CS_0024_003C_003E8__locals12.tLsnsfm9rk))
		{
			CS_0024_003C_003E8__locals12.tLsnsfm9rk += "/";
		}
		CS_0024_003C_003E8__locals12.tPjno1xXjH = CS_0024_003C_003E8__locals12.tLsnsfm9rk == null;
		CS_0024_003C_003E8__locals12.nCknLvvcQd = BGHr16HlmT;
		await Parallel.ForEachAsync(P_0, parallelOptions, async delegate(ImportFileItem importItem, CancellationToken ct)
		{
			ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> observableConcurrentDictionaryEx = CS_0024_003C_003E8__locals12.IYKnwywOYZ.Trees;
			string text = ((!CS_0024_003C_003E8__locals12.tPjno1xXjH) ? (CS_0024_003C_003E8__locals12.tLsnsfm9rk + importItem.FilePath) : importItem.FilePath);
			string[] array = text.ToLower().Split(CS_0024_003C_003E8__locals12.nCknLvvcQd, StringSplitOptions.RemoveEmptyEntries);
			short num = 0;
			int num2 = array.Length - 1;
			StringBuilder stringBuilder = new StringBuilder();
			string[] array2 = array;
			foreach (string text2 in array2)
			{
				lock (CS_0024_003C_003E8__locals12.IYKnwywOYZ)
				{
					if (num == 0)
					{
						stringBuilder.Append(text2);
					}
					else
					{
						stringBuilder.Append("/");
						stringBuilder.Append(text2);
					}
					if (observableConcurrentDictionaryEx.TryGetValue(text2, out var value))
					{
						observableConcurrentDictionaryEx = value.Children;
					}
					else
					{
						bool flag = num == num2;
						PvfTreeFileImport pvfTreeFileImport = new PvfTreeFileImport(AppCore.ViewModelBase.PVF, stringBuilder.ToString(), text2, flag, num, flag ? importItem : null);
						observableConcurrentDictionaryEx.AddTry(text2, pvfTreeFileImport);
						observableConcurrentDictionaryEx = pvfTreeFileImport.Children;
					}
				}
				num++;
			}
		});
	}

	public void AddNode(string key, PvfTreeFileBase tree, IDictionary<string, PvfTreeFileBase>? source = null)
	{
		if (source == null)
		{
			source = _Trees;
			source.Add(key, tree);
		}
		else
		{
			ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> observableConcurrentDictionaryEx = _Trees;
			string[] array = tree.FullPath.Split(BGHr16HlmT, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < array.Length; i++)
			{
				if (observableConcurrentDictionaryEx.ContainsKey(array[i]))
				{
					observableConcurrentDictionaryEx = observableConcurrentDictionaryEx[array[i]].Children;
				}
			}
			observableConcurrentDictionaryEx.Add(key, tree);
		}
		Trees.NotifyObserversOfChange();
	}

	public void AddFolder(string newFilePath, int level)
	{
	}

	public PooledSet<string> SelectedNodesToFilePaths(IEnumerable<KeyValuePair<string, PvfTreeFileBase>> selecteddic, GetTreeType type)
	{
		PooledSet<string> pooledSet = new PooledSet<string>();
		if (selecteddic == null || selecteddic.Count() == 0)
		{
			return pooledSet;
		}
		foreach (KeyValuePair<string, PvfTreeFileBase> item in from it in selecteddic
			orderby it.Key
			orderby it.Value.IsFile descending
			select it)
		{
			switch (type)
			{
			case GetTreeType.File:
				if (item.Value.IsFile)
				{
					pooledSet.Add(item.Value.FullPath);
				}
				break;
			case GetTreeType.Folder:
				if (!item.Value.IsFile)
				{
					pooledSet.Add(item.Value.FullPath);
				}
				break;
			case GetTreeType.All:
				pooledSet.Add(item.Value.FullPath);
				break;
			}
			if (item.Value.HaveChildren())
			{
				PooledSet<string> pooledSet2 = SelectedNodesToFilePaths(item.Value.Children, type);
				if (pooledSet2.Count > 0)
				{
					pooledSet.AddRange(pooledSet2.ToArray());
				}
			}
		}
		return pooledSet;
	}

	public PooledSet<string> SelectedNodesToFilePaths(IDictionary<string, PvfTreeFileBase> dic, GetTreeType type)
	{
		PooledSet<string> pooledSet = new PooledSet<string>();
		foreach (KeyValuePair<string, PvfTreeFileBase> item in from it in dic
			orderby it.Key
			orderby it.Value.IsFile descending
			select it)
		{
			switch (type)
			{
			case GetTreeType.File:
				if (item.Value.IsFile)
				{
					pooledSet.Add(item.Value.FullPath);
				}
				break;
			case GetTreeType.Folder:
				if (!item.Value.IsFile)
				{
					pooledSet.Add(item.Value.FullPath);
				}
				break;
			case GetTreeType.All:
				pooledSet.Add(item.Value.FullPath);
				break;
			}
			if (item.Value.HaveChildren())
			{
				PooledSet<string> pooledSet2 = SelectedNodesToFilePaths(item.Value.Children, type);
				if (pooledSet2.Count > 0)
				{
					pooledSet.AddRange(pooledSet2.ToArray());
				}
			}
		}
		return pooledSet;
	}

	public void DeleteTreeNodes(IEnumerable<KeyValuePair<string, PvfTreeFileBase>> selecteddic)
	{
		_003C_003Ec__DisplayClass52_0 obj = new _003C_003Ec__DisplayClass52_0();
		obj.jtTnq3Sjo1 = selecteddic;
		obj.qoqndVZiXV = this;
		Action<ObservableConcurrentDictionaryEx<string, PvfTreeFileBase>> action = delegate(ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> trees)
		{
			foreach (KeyValuePair<string, PvfTreeFileBase> item in obj.jtTnq3Sjo1)
			{
				string fullPath = item.Value.FullPath;
				if (fullPath.IndexOf("/") < 0)
				{
					trees.RemoveTry(item.Key);
				}
				else
				{
					ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> observableConcurrentDictionaryEx = trees;
					string[] array = fullPath.Split(obj.qoqndVZiXV.BGHr16HlmT, StringSplitOptions.RemoveEmptyEntries);
					PvfTreeFileBase value = null;
					for (int i = 0; i < array.Length - 1; i++)
					{
						string key = array[i];
						if (observableConcurrentDictionaryEx.TryGetValue(key, out value))
						{
							observableConcurrentDictionaryEx = value.Children;
						}
					}
					observableConcurrentDictionaryEx.RemoveTry(array[^1]);
				}
			}
			trees.NotifyObserversOfChange();
		};
		if (_Trees != null && _Trees.Any())
		{
			action(_Trees);
		}
		if (_SearchResultTrees != null && _SearchResultTrees.Any())
		{
			action(_SearchResultTrees);
		}
	}

	public void DeleteTreeNode(KeyValuePair<string, PvfTreeFileBase> node)
	{
		_003C_003Ec__DisplayClass53_0 obj = new _003C_003Ec__DisplayClass53_0();
		obj.rR1ntvGGDs = node;
		obj.OEPnbhtVr5 = this;
		Action<ObservableConcurrentDictionaryEx<string, PvfTreeFileBase>> action = delegate(ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> trees)
		{
			if (trees != null)
			{
				string fullPath = obj.rR1ntvGGDs.Value.FullPath;
				if (fullPath.IndexOf("/") < 0)
				{
					trees.RemoveTry(obj.rR1ntvGGDs.Key);
				}
				else
				{
					ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> observableConcurrentDictionaryEx = trees;
					string[] array = fullPath.Split(obj.OEPnbhtVr5.BGHr16HlmT, StringSplitOptions.RemoveEmptyEntries);
					for (int i = 0; i < array.Length - 1; i++)
					{
						string key = array[i];
						if (observableConcurrentDictionaryEx.TryGetValue(key, out var value))
						{
							observableConcurrentDictionaryEx = value.Children;
						}
					}
					observableConcurrentDictionaryEx.RemoveTry(array[^1]);
				}
			}
		};
		if (ShowSearchResulTrees)
		{
			action(_SearchResultTrees);
		}
		action(_Trees);
		Trees.NotifyObserversOfChange();
	}

	public void DeleteTreeNode(IEnumerable<string> fileList)
	{
		if (fileList == null)
		{
			return;
		}
		HashSet<KeyValuePair<string, PvfTreeFileBase>> hashSet = new HashSet<KeyValuePair<string, PvfTreeFileBase>>();
		foreach (string file in fileList)
		{
			KeyValuePair<string, PvfTreeFileBase>? keyValuePair = FilePathGetTreeNode(file);
			if (keyValuePair.HasValue)
			{
				hashSet.Add(keyValuePair.Value);
			}
		}
		DeleteTreeNodes(hashSet);
	}

	public List<KeyValuePair<string, PvfTreeFileBase>> FilePathGetTreeNode(IEnumerable<string> fileList)
	{
		if (fileList == null)
		{
			return null;
		}
		List<KeyValuePair<string, PvfTreeFileBase>> list = new List<KeyValuePair<string, PvfTreeFileBase>>();
		foreach (string file in fileList)
		{
			KeyValuePair<string, PvfTreeFileBase>? keyValuePair = FilePathGetTreeNode(file);
			if (keyValuePair.HasValue)
			{
				list.Add(keyValuePair.Value);
			}
		}
		return list;
	}

	public KeyValuePair<string, PvfTreeFileBase>? FilePathGetTreeNode(string filePath, IDictionary<string, PvfTreeFileBase>? source = null)
	{
		if (source == null)
		{
			source = Trees;
		}
		if (source == null)
		{
			return null;
		}
		if (filePath.IndexOf("/") < 0)
		{
			if (source.ContainsKey(filePath))
			{
				return pvBrY1tENX(Trees, filePath);
			}
			return null;
		}
		IDictionary<string, PvfTreeFileBase> dictionary = source;
		string[] array = filePath.Split(BGHr16HlmT, StringSplitOptions.RemoveEmptyEntries);
		for (int i = 0; i < array.Length - 1; i++)
		{
			string key = array[i];
			if (dictionary.TryGetValue(key, out var value))
			{
				dictionary = value.Children;
			}
		}
		return pvBrY1tENX(dictionary, array[^1]);
	}

	private KeyValuePair<string, PvfTreeFileBase>? pvBrY1tENX(IDictionary<string, PvfTreeFileBase> P_0, string P_1)
	{
		foreach (KeyValuePair<string, PvfTreeFileBase> item in P_0)
		{
			if (item.Key == P_1)
			{
				return item;
			}
		}
		return null;
	}

	public List<string> GetFolderChildren(PvfTreeFileBase pvfTreeFile)
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, PvfTreeFileBase> item in (IEnumerable<KeyValuePair<string, PvfTreeFileBase>>)pvfTreeFile.Children)
		{
			if (item.Value.IsFile)
			{
				list.Add(item.Value.FullPath);
			}
			else if (item.Value.HaveChildren())
			{
				List<string> folderChildren = GetFolderChildren(item.Value);
				if (folderChildren.Count > 0)
				{
					list.AddRange(folderChildren);
				}
			}
		}
		return list;
	}

	public int GetFolderChildrenFolderCount(PvfTreeFileBase pvfTreeFile)
	{
		int num = 0;
		foreach (KeyValuePair<string, PvfTreeFileBase> item in (IEnumerable<KeyValuePair<string, PvfTreeFileBase>>)pvfTreeFile.Children)
		{
			if (!item.Value.IsFile)
			{
				if (item.Value.HaveChildren())
				{
					int folderChildrenFolderCount = GetFolderChildrenFolderCount(item.Value);
					num += folderChildrenFolderCount;
				}
				num++;
			}
		}
		return num;
	}

	public IList<string> GetFolderPaths()
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, PvfTreeFileBase> item in (IEnumerable<KeyValuePair<string, PvfTreeFileBase>>)Trees)
		{
			if (item.Value.IsFile)
			{
				continue;
			}
			list.Add(item.Key);
			if (item.Value.HaveChildren())
			{
				List<string> folderPaths = GetFolderPaths(item.Value);
				if (folderPaths.Any())
				{
					list.AddRange(folderPaths);
				}
			}
		}
		return list;
	}

	public List<string> GetFolderPaths(PvfTreeFileBase treeFile)
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<string, PvfTreeFileBase> item in (IEnumerable<KeyValuePair<string, PvfTreeFileBase>>)treeFile.Children)
		{
			if (item.Value.IsFile)
			{
				continue;
			}
			list.Add(item.Value.FullPath);
			if (item.Value.HaveChildren())
			{
				List<string> folderPaths = GetFolderPaths(item.Value);
				if (folderPaths.Any())
				{
					list.AddRange(folderPaths);
				}
			}
		}
		return list;
	}

	private int A9xryKLBwM()
	{
		if (Trees == null)
		{
			return 0;
		}
		int location = 0;
		KeyValuePair<string, PvfTreeFileBase>[] array = Trees.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			KeyValuePair<string, PvfTreeFileBase> keyValuePair = array[i];
			if (keyValuePair.Value.IsFile)
			{
				Interlocked.Increment(ref location);
			}
			else if (keyValuePair.Value.HaveChildren())
			{
				location += lbjriqJTMB(keyValuePair.Value.Children.Values);
			}
		}
		return location;
	}

	private int lbjriqJTMB(IEnumerable<PvfTreeFileBase> P_0)
	{
		int location = 0;
		PvfTreeFileBase[] array = P_0.ToArray();
		foreach (PvfTreeFileBase pvfTreeFileBase in array)
		{
			if (pvfTreeFileBase.IsFile)
			{
				Interlocked.Increment(ref location);
			}
			else if (pvfTreeFileBase.HaveChildren())
			{
				location += lbjriqJTMB(pvfTreeFileBase.Children.Values);
			}
		}
		return location;
	}

	public List<string> GetAllFilePaths()
	{
		List<string> list = new List<string>();
		if (Trees == null)
		{
			return list;
		}
		foreach (PvfTreeFileBase value in Trees.Values)
		{
			if (value.IsFile)
			{
				list.Add(value.FullPath);
			}
			else if (value.HaveChildren())
			{
				List<string> filePaths = GetFilePaths(value.Children.Values);
				if (filePaths.Count > 0)
				{
					list.AddRange(filePaths);
				}
			}
		}
		return list;
	}

	public List<string> GetFilePaths(IEnumerable<PvfTreeFileBase> values)
	{
		List<string> list = new List<string>();
		foreach (PvfTreeFileBase value in values)
		{
			if (value.IsFile)
			{
				list.Add(value.FullPath);
			}
			else if (value.HaveChildren())
			{
				List<string> filePaths = GetFilePaths(value.Children.Values.ToList());
				if (filePaths.Count > 0)
				{
					list.AddRange(filePaths);
				}
			}
		}
		return list;
	}

	public List<ImportFileItem> GetAllImportItems()
	{
		if (Trees == null)
		{
			return null;
		}
		List<ImportFileItem> list = new List<ImportFileItem>();
		foreach (PvfTreeFileImport value in Trees.Values)
		{
			if (value.IsFile)
			{
				list.Add(value.ImportItem);
			}
			else if (value.HaveChildren())
			{
				List<ImportFileItem> importItems = GetImportItems(value.Children.Values);
				if (importItems.Count > 0)
				{
					list.AddRange(importItems);
				}
			}
		}
		return list;
	}

	public List<ImportFileItem> GetImportItems(IEnumerable<PvfTreeFileBase> values)
	{
		List<ImportFileItem> list = new List<ImportFileItem>();
		foreach (PvfTreeFileImport value in values)
		{
			if (value.IsFile)
			{
				list.Add(value.ImportItem);
			}
			else if (value.HaveChildren())
			{
				List<ImportFileItem> importItems = GetImportItems(value.Children.Values);
				if (importItems.Count > 0)
				{
					list.AddRange(importItems);
				}
			}
		}
		return list;
	}

	public Task SetFilesCutStatus(IEnumerable<KeyValuePair<string, PvfTreeFileBase>> treeFiles)
	{
		if (treeFiles == null)
		{
			return Task.CompletedTask;
		}
		Parallel.ForEach<KeyValuePair<string, PvfTreeFileBase>>(treeFiles, delegate(KeyValuePair<string, PvfTreeFileBase> row)
		{
			if (!row.Value.IsShearStatus.HasValue)
			{
				row.Value.IsShearStatus = true;
				row.Value.DoNotifyStatus();
			}
		});
		return Task.CompletedTask;
	}

	public Task ClearFileCopyStatus(IEnumerable<string> fileList)
	{
		if (Trees == null || !Trees.Any())
		{
			return Task.CompletedTask;
		}
		Parallel.ForEach(fileList, delegate(string P_0)
		{
			KeyValuePair<string, PvfTreeFileBase>? keyValuePair = FilePathGetTreeNode(P_0);
			if (keyValuePair.HasValue)
			{
				keyValuePair.Value.Value.IsShearStatus = null;
				keyValuePair.Value.Value.DoNotifyStatus();
			}
		});
		return Task.CompletedTask;
	}

	public bool Any(string filePath)
	{
		if (Trees == null || !Trees.Any())
		{
			return false;
		}
		if (filePath.IndexOf('/') < 0)
		{
			return Trees.ContainsKey(filePath);
		}
		ObservableConcurrentDictionaryEx<string, PvfTreeFileBase> observableConcurrentDictionaryEx = Trees;
		string[] array = filePath.Split(BGHr16HlmT, StringSplitOptions.RemoveEmptyEntries);
		foreach (string key in array)
		{
			if (observableConcurrentDictionaryEx.TryGetValue(key, out var value))
			{
				observableConcurrentDictionaryEx = value.Children;
				continue;
			}
			return false;
		}
		return true;
	}

	public Task<int> SearchFileList(string keyword)
	{
		_003C_003Ec__DisplayClass71_0 _003C_003Ec__DisplayClass71_2 = new _003C_003Ec__DisplayClass71_0();
		_003C_003Ec__DisplayClass71_2.FS8nEksdNx = this;
		_003C_003Ec__DisplayClass71_2.UqFnOVMKZ7 = keyword;
		if (AppSetting.Instance.PvfConfig.DefaultEncoding == EncodingType.TW && SearchPanelOptions.ConvertTw && !AppSetting.Instance.TreeSetting.TwConvertSimplified)
		{
			_003C_003Ec__DisplayClass71_2.UqFnOVMKZ7 = ChineseHelper.ToTraditional(_003C_003Ec__DisplayClass71_2.UqFnOVMKZ7);
		}
		_003C_003Ec__DisplayClass71_2.zq2nK6S0Ok = new ConcurrentBag<KeyValuePair<string, PvfTreeFileBase>>();
		if (_Trees != null && _Trees.Any())
		{
			_003C_003Ec__DisplayClass71_1 CS_0024_003C_003E8__locals19 = new _003C_003Ec__DisplayClass71_1();
			CS_0024_003C_003E8__locals19.QMcnZgHGhI = _003C_003Ec__DisplayClass71_2;
			CS_0024_003C_003E8__locals19.SrMnPaK3ph = delegate(KeyValuePair<string, PvfTreeFileBase> row)
			{
				lock (CS_0024_003C_003E8__locals19.QMcnZgHGhI.FS8nEksdNx)
				{
					PvfTreeFileBase value = row.Value;
					if (CS_0024_003C_003E8__locals19.QMcnZgHGhI.FS8nEksdNx.SearchPanelOptions.FilePath && value.FullPath.ToLower().Contains(CS_0024_003C_003E8__locals19.QMcnZgHGhI.UqFnOVMKZ7))
					{
						CS_0024_003C_003E8__locals19.QMcnZgHGhI.zq2nK6S0Ok.Add(row);
					}
					else if (CS_0024_003C_003E8__locals19.QMcnZgHGhI.FS8nEksdNx.SearchPanelOptions.ItemName && value.ItemName != null && value.ItemName.Contains(CS_0024_003C_003E8__locals19.QMcnZgHGhI.UqFnOVMKZ7))
					{
						CS_0024_003C_003E8__locals19.QMcnZgHGhI.zq2nK6S0Ok.Add(row);
					}
					else if (CS_0024_003C_003E8__locals19.QMcnZgHGhI.FS8nEksdNx.SearchPanelOptions.Comment && value.Comment != null && value.Comment.Contains(CS_0024_003C_003E8__locals19.QMcnZgHGhI.UqFnOVMKZ7))
					{
						CS_0024_003C_003E8__locals19.QMcnZgHGhI.zq2nK6S0Ok.Add(row);
					}
					else if (CS_0024_003C_003E8__locals19.QMcnZgHGhI.FS8nEksdNx.SearchPanelOptions.ItemCode && value.ItemCodeStr != null && value.ItemCodeStr.Contains(CS_0024_003C_003E8__locals19.QMcnZgHGhI.UqFnOVMKZ7))
					{
						CS_0024_003C_003E8__locals19.QMcnZgHGhI.zq2nK6S0Ok.Add(row);
					}
				}
			};
			Parallel.ForEach<KeyValuePair<string, PvfTreeFileBase>>(_Trees, async delegate(KeyValuePair<string, PvfTreeFileBase> item)
			{
				if (item.Value.IsFile)
				{
					CS_0024_003C_003E8__locals19.SrMnPaK3ph(item);
				}
				else
				{
					CS_0024_003C_003E8__locals19.SrMnPaK3ph(item);
					if (item.Value.HaveChildren())
					{
						await CS_0024_003C_003E8__locals19.QMcnZgHGhI.FS8nEksdNx.IKuruKSxLh(item.Value.Children, CS_0024_003C_003E8__locals19.SrMnPaK3ph);
					}
				}
			});
		}
		_SearchResultTrees.AddRange(_003C_003Ec__DisplayClass71_2.zq2nK6S0Ok);
		ShowSearchResulTrees = true;
		DoNotify("Trees");
		return Task.FromResult(_003C_003Ec__DisplayClass71_2.zq2nK6S0Ok.Count);
	}

	private Task IKuruKSxLh(IEnumerable<KeyValuePair<string, PvfTreeFileBase>> trees, Action<KeyValuePair<string, PvfTreeFileBase>> treeAction)
	{
		_003C_003Ec__DisplayClass72_0 CS_0024_003C_003E8__locals6 = new _003C_003Ec__DisplayClass72_0();
		CS_0024_003C_003E8__locals6.YQ9nkSHal0 = treeAction;
		CS_0024_003C_003E8__locals6.aBln021vWA = this;
		Parallel.ForEach<KeyValuePair<string, PvfTreeFileBase>>(trees, async delegate(KeyValuePair<string, PvfTreeFileBase> item)
		{
			if (item.Value.IsFile)
			{
				CS_0024_003C_003E8__locals6.YQ9nkSHal0(item);
			}
			else
			{
				CS_0024_003C_003E8__locals6.YQ9nkSHal0(item);
				if (item.Value.HaveChildren())
				{
					await CS_0024_003C_003E8__locals6.aBln021vWA.IKuruKSxLh(item.Value.Children, CS_0024_003C_003E8__locals6.YQ9nkSHal0);
				}
			}
		});
		return Task.CompletedTask;
	}

	[CompilerGenerated]
	private void Hj0rGgHZ4P(string P_0)
	{
		KeyValuePair<string, PvfTreeFileBase>? keyValuePair = FilePathGetTreeNode(P_0);
		if (keyValuePair.HasValue)
		{
			keyValuePair.Value.Value.IsShearStatus = null;
			keyValuePair.Value.Value.DoNotifyStatus();
		}
	}
}
