using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DevExpress.Mvvm;

namespace PvfCode.Models.Pvf;

public class EquipmentPartSet : ViewModelBase
{
	public class ReferencesRowViewModel
	{
		private PvfPack pack;

		public readonly PvfFile File;

		public string ItemName
		{
			get
			{
				string text = pack.GetItemName(File);
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
				if (File.GetRarity(pack, out int rarity))
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
			if (pack != pvf)
			{
				pack = pvf;
			}
		}
	}

	private string name;

	private string referencedItemName;

	public string Name
	{
		get
		{
			if (ReferencesNumber == 0 || ReferencesNumber > 1)
			{
				return name;
			}
			if (!string.IsNullOrEmpty(referencedItemName))
			{
				return referencedItemName;
			}
			return name;
		}
		set
		{
			name = value;
		}
	}

	public ObservableCollection<ReferencesRowViewModel> ReferencesFiles { get; set; }

	public PvfFile ParFile { get; set; }

	public string EquType { get; set; }

	public int ReferencesNumber { get; set; }

	public string ReferencesNumberText => $"被：{ReferencesNumber}个装备引用";

	public void SetItemName(string name)
	{
		if (ReferencesNumber == 0)
		{
			referencedItemName = name;
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
