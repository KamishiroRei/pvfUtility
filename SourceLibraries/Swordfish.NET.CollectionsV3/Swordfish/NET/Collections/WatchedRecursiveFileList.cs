using System;
using System.IO;
using System.Linq;

namespace Swordfish.NET.Collections;

public class WatchedRecursiveFileList : IDisposable
{
	private ConcurrentObservableCollection<string> _fileList;

	private FileSystemWatcher _watcher;

	private string _watchedDirectoryLocation;

	private string _fileFilter;

	public ConcurrentObservableCollection<string> FileList => _fileList;

	public WatchedRecursiveFileList(DirectoryInfo directory, string fileFilter = "*.3ds")
	{
		if (!directory.Exists)
		{
			directory.Create();
		}
		_fileList = new ConcurrentObservableCollection<string>();
		_fileFilter = fileFilter;
		_watchedDirectoryLocation = directory.FullName;
		_watcher = new FileSystemWatcher(_watchedDirectoryLocation, _fileFilter);
		_watcher.IncludeSubdirectories = true;
		_watcher.Filter = _fileFilter;
		_watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName | NotifyFilters.LastWrite;
		_watcher.Changed += watcher_Changed;
		_watcher.Created += watcher_Changed;
		_watcher.Deleted += watcher_Changed;
		_watcher.Renamed += watcher_Renamed;
		_watcher.EnableRaisingEvents = true;
		_fileList.Clear();
		_fileList.Add("");
		BuildFileList(null, "");
	}

	public string GetFullPath(string filename, string extension = ".3ds")
	{
		if (string.IsNullOrWhiteSpace(filename))
		{
			return null;
		}
		string text = Path.Combine(_watchedDirectoryLocation, filename);
		if (File.Exists(text))
		{
			return text;
		}
		return Directory.EnumerateFiles(_watchedDirectoryLocation, Path.GetFileNameWithoutExtension(filename) + extension, SearchOption.AllDirectories).FirstOrDefault();
	}

	public void Dispose()
	{
		if (_watcher != null)
		{
			_watcher.Dispose();
			_watcher = null;
		}
	}

	private void BuildFileList(DirectoryInfo dir, string directory)
	{
		if (dir == null)
		{
			dir = new DirectoryInfo(_watchedDirectoryLocation);
		}
		FileInfo[] files = dir.GetFiles(_fileFilter);
		foreach (FileInfo fileInfo in files)
		{
			_fileList.Add(Path.Combine(directory, fileInfo.Name));
		}
		DirectoryInfo[] directories = dir.GetDirectories();
		foreach (DirectoryInfo directoryInfo in directories)
		{
			BuildFileList(directoryInfo, Path.Combine(directory, directoryInfo.Name));
		}
	}

	private void RebuildFileList(FileSystemEventArgs e)
	{
		if (e is RenamedEventArgs)
		{
			int index = _fileList.IndexOf(((RenamedEventArgs)e).OldName);
			_fileList[index] = e.Name;
			return;
		}
		switch (e.ChangeType)
		{
		case WatcherChangeTypes.Changed:
		{
			int index2 = _fileList.IndexOf(e.Name);
			_fileList[index2] = e.Name;
			break;
		}
		case WatcherChangeTypes.Created:
			_fileList.Add(e.Name);
			break;
		case WatcherChangeTypes.Deleted:
			if (_fileList.Contains(e.Name))
			{
				_fileList.Remove(e.Name);
			}
			break;
		case WatcherChangeTypes.Created | WatcherChangeTypes.Deleted:
			break;
		}
	}

	private void watcher_Changed(object sender, FileSystemEventArgs e)
	{
		RebuildFileList(e);
	}

	private void watcher_Renamed(object sender, RenamedEventArgs e)
	{
		RebuildFileList(e);
	}
}
