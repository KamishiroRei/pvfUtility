using System;
using System.Collections.Generic;
using System.IO;
using DevExpress.Mvvm.DataAnnotations;
using Swordfish.NET.Collections;
using Swordfish.NET.Collections.Auxiliary;

namespace PvfCode.NPK.Utils.Models;

public class NpkFile : NpkTreeBase, IDisposable
{
	private ConcurrentObservableCollection<ImagePack> _Items;

	public string FilePath { get; set; }

	public long Size { get; set; }

	public string SizeStr => "";

	public string FileName => Path.GetFileName(FilePath);

	public int Count
	{
		get
		{
			if (Items != null)
			{
				return Items.Count;
			}
			return 0;
		}
	}

	public ConcurrentObservableCollection<ImagePack> Items
	{
		get
		{
			return _Items;
		}
		set
		{
			_Items = value;
			UpdateCount();
		}
	}

	private void UpdateCount()
	{
		RaisePropertyChanged("Count");
	}

	public NpkFile(string filePath, IEnumerable<ImagePack> packs)
	{
		Items = new ConcurrentObservableCollection<ImagePack>();
		Items.AddRange(packs);
		FilePath = filePath;
	}

	[Command]
	public void OnOpen()
	{
	}

	[Command]
	public void OnClose()
	{
	}

	public void Dispose()
	{
		Items = null;
	}
}
