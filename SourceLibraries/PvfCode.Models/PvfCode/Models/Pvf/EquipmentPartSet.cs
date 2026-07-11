using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using DevExpress.Mvvm;

namespace PvfCode.Models.Pvf;

public class EquipmentPartSet : ViewModelBase
{
	public class ReferencesRowViewModel
	{
		private PvfPack W5y8KpvIQ3;

		public readonly PvfFile File;

		public string ItemName
		{
			get
			{
				string text = W5y8KpvIQ3.GetItemName(File);
				if (string.IsNullOrEmpty(text))
				{
					text = File.FileName;
				}
				return text;
			}
		}

		public int? Rarity
		{
			get
			{
				if (File.GetRarity(W5y8KpvIQ3, out int rarity))
				{
					return rarity;
				}
				return null;
			}
		}

		public ReferencesRowViewModel(PvfFile file)
		{
			File = file;
		}

		public void SetPack(PvfPack pvf)
		{
			if (W5y8KpvIQ3 != pvf)
			{
				W5y8KpvIQ3 = pvf;
			}
		}
	}

	private string btnkV2fSx5;

	[CompilerGenerated]
	private string n6CkPRUODO;

	[CompilerGenerated]
	private ObservableCollection<ReferencesRowViewModel> QKakFVCtjn;

	[CompilerGenerated]
	private PvfFile uA8kXcgw5y;

	[CompilerGenerated]
	private string mLUkNL2Iwj;

	[CompilerGenerated]
	private int yWUkiPXTLE;

	public string Name
	{
		get
		{
			if (ReferencesNumber == 0 || ReferencesNumber > 1)
			{
				return btnkV2fSx5;
			}
			if (!string.IsNullOrEmpty(RSbkCXNFpo()))
			{
				return RSbkCXNFpo();
			}
			return btnkV2fSx5;
		}
		set
		{
			btnkV2fSx5 = value;
		}
	}

	public ObservableCollection<ReferencesRowViewModel> ReferencesFiles
	{
		[CompilerGenerated]
		get
		{
			return QKakFVCtjn;
		}
		[CompilerGenerated]
		set
		{
			QKakFVCtjn = value;
		}
	}

	public PvfFile ParFile
	{
		[CompilerGenerated]
		get
		{
			return uA8kXcgw5y;
		}
		[CompilerGenerated]
		set
		{
			uA8kXcgw5y = value;
		}
	}

	public string EquType
	{
		[CompilerGenerated]
		get
		{
			return mLUkNL2Iwj;
		}
		[CompilerGenerated]
		set
		{
			mLUkNL2Iwj = value;
		}
	}

	public int ReferencesNumber
	{
		[CompilerGenerated]
		get
		{
			return yWUkiPXTLE;
		}
		[CompilerGenerated]
		set
		{
			yWUkiPXTLE = value;
		}
	}

	public string ReferencesNumberText
	{
		get
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 1);
			defaultInterpolatedStringHandler.AppendLiteral("被：");
			defaultInterpolatedStringHandler.AppendFormatted(ReferencesNumber);
			defaultInterpolatedStringHandler.AppendLiteral("个装备引用");
			return defaultInterpolatedStringHandler.ToStringAndClear();
		}
	}

	[SpecialName]
	[CompilerGenerated]
	private string RSbkCXNFpo()
	{
		return n6CkPRUODO;
	}

	[SpecialName]
	[CompilerGenerated]
	private void KClkvXjyvV(string P_0)
	{
		n6CkPRUODO = P_0;
	}

	public void SetItemName(string name)
	{
		if (ReferencesNumber == 0)
		{
			KClkvXjyvV(name);
		}
	}

	public EquipmentPartSet()
	{
		ReferencesFiles = new ObservableCollection<ReferencesRowViewModel>();
	}

	public PvfFile? GetFile()
	{
		if (ReferencesFiles == null || ReferencesFiles.Count == 0)
		{
			return null;
		}
		return ReferencesFiles.FirstOrDefault()?.File;
	}

	public IEnumerable<string> GetFilePaths()
	{
		return from it in ReferencesFiles
			where it.File != null
			select it.File.FileName;
	}

	public void SetReferencesFilePack(PvfPack pvf)
	{
		if (ReferencesFiles == null)
		{
			return;
		}
		foreach (ReferencesRowViewModel referencesFile in ReferencesFiles)
		{
			referencesFile.SetPack(pvf);
		}
	}
}
