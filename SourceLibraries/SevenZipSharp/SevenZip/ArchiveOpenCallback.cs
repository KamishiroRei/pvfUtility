using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace SevenZip;

internal sealed class ArchiveOpenCallback : CallbackBase, IArchiveOpenCallback, IArchiveOpenVolumeCallback, ICryptoGetTextPassword, IDisposable
{
	private FileInfo _fileInfo;

	private Dictionary<string, InStreamWrapper> _wrappers = new Dictionary<string, InStreamWrapper>();

	private readonly List<string> _volumeFileNames = new List<string>();

	public IList<string> VolumeFileNames => _volumeFileNames;

	private void Init(string fileName)
	{
		if (string.IsNullOrEmpty(fileName))
		{
			return;
		}
		_fileInfo = new FileInfo(fileName);
		_volumeFileNames.Add(fileName);
		if (fileName.EndsWith("001"))
		{
			int num = 2;
			string text = fileName.Substring(0, fileName.Length - 3);
			string text2 = text + ((num > 99) ? num.ToString() : ((num > 9) ? ("0" + num) : ("00" + num)));
			while (File.Exists(text2))
			{
				_volumeFileNames.Add(text2);
				num++;
				text2 = text + ((num > 99) ? num.ToString() : ((num > 9) ? ("0" + num) : ("00" + num)));
			}
		}
	}

	public ArchiveOpenCallback(string fileName)
	{
		Init(fileName);
	}

	public ArchiveOpenCallback(string fileName, string password)
		: base(password)
	{
		Init(fileName);
	}

	public void SetTotal(IntPtr files, IntPtr bytes)
	{
	}

	public void SetCompleted(IntPtr files, IntPtr bytes)
	{
	}

	public int GetProperty(ItemPropId propId, ref PropVariant value)
	{
		if (_fileInfo == null)
		{
			return 0;
		}
		switch (propId)
		{
		case ItemPropId.Name:
			value.VarType = VarEnum.VT_BSTR;
			value.Value = Marshal.StringToBSTR(_fileInfo.FullName);
			break;
		case ItemPropId.IsDirectory:
			value.VarType = VarEnum.VT_BOOL;
			value.UInt64Value = (byte)(_fileInfo.Attributes & FileAttributes.Directory);
			break;
		case ItemPropId.Size:
			value.VarType = VarEnum.VT_UI8;
			value.UInt64Value = (ulong)_fileInfo.Length;
			break;
		case ItemPropId.Attributes:
			value.VarType = VarEnum.VT_UI4;
			value.UInt32Value = (uint)_fileInfo.Attributes;
			break;
		case ItemPropId.CreationTime:
			value.VarType = VarEnum.VT_FILETIME;
			value.Int64Value = _fileInfo.CreationTime.ToFileTime();
			break;
		case ItemPropId.LastAccessTime:
			value.VarType = VarEnum.VT_FILETIME;
			value.Int64Value = _fileInfo.LastAccessTime.ToFileTime();
			break;
		case ItemPropId.LastWriteTime:
			value.VarType = VarEnum.VT_FILETIME;
			value.Int64Value = _fileInfo.LastWriteTime.ToFileTime();
			break;
		}
		return 0;
	}

	public int GetStream(string name, out IInStream inStream)
	{
		if (!File.Exists(name))
		{
			name = Path.Combine(Path.GetDirectoryName(_fileInfo.FullName), name);
			if (!File.Exists(name))
			{
				inStream = null;
				AddException(new FileNotFoundException("The volume \"" + name + "\" was not found. Extraction can be impossible."));
				return 1;
			}
		}
		_volumeFileNames.Add(name);
		if (_wrappers.ContainsKey(name))
		{
			inStream = _wrappers[name];
		}
		else
		{
			try
			{
				InStreamWrapper inStreamWrapper = new InStreamWrapper(new FileStream(name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), disposeStream: true);
				_wrappers.Add(name, inStreamWrapper);
				inStream = inStreamWrapper;
			}
			catch (Exception)
			{
				AddException(new FileNotFoundException("Failed to open the volume \"" + name + "\". Extraction is impossible."));
				inStream = null;
				return 1;
			}
		}
		return 0;
	}

	public int CryptoGetTextPassword(out string password)
	{
		password = base.Password;
		return 0;
	}

	public void Dispose()
	{
		if (_wrappers != null)
		{
			foreach (InStreamWrapper value in _wrappers.Values)
			{
				value.Dispose();
			}
			_wrappers = null;
		}
		GC.SuppressFinalize(this);
	}
}
