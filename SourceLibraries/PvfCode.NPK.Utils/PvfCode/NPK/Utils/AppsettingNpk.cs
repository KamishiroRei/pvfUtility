using System;
using System.IO;
using System.Windows;
using DevExpress.Mvvm;
using PvfCode.NPK.Utils.Options;
using Utools;

namespace PvfCode.NPK.Utils;

public class AppsettingNpk : ViewModelBase
{
	private static AppsettingNpk _Instance;

	private CanvasOptions _CanvasOptions;

	private static string SettingSavePath
	{
		get
		{
			string text = Path.Combine(AppBasePath, "NpkStudioConfig.json");
			FileHelper.CheckDir(Path.GetDirectoryName(text));
			return text;
		}
	}

	public static string AppBasePath
	{
		get
		{
			string text = Path.Combine(AppContext.BaseDirectory, "Options");
			FileHelper.CheckDir(text);
			return text;
		}
	}

	public static AppsettingNpk Instance
	{
		get
		{
			if (_Instance == null)
			{
				_Instance = ReadSetting();
			}
			return _Instance;
		}
		set
		{
			_Instance = value;
		}
	}

	public string AppName { get; set; } = "NpkStudio";

	public CanvasOptions CanvasOptions
	{
		get
		{
			if (_CanvasOptions == null)
			{
				_CanvasOptions = new CanvasOptions();
			}
			return _CanvasOptions;
		}
		set
		{
			_CanvasOptions = value;
			RaisePropertyChanged("CanvasOptions");
		}
	}

	private static AppsettingNpk ReadSetting()
	{
		try
		{
			if (File.Exists(SettingSavePath))
			{
				string text = File.ReadAllText(SettingSavePath);
				if (string.IsNullOrEmpty(text))
				{
					return new AppsettingNpk();
				}
				return text.JsonToObject<AppsettingNpk>();
			}
			return new AppsettingNpk();
		}
		catch (Exception ex)
		{
			if (File.Exists(SettingSavePath))
			{
				string directoryName = Path.GetDirectoryName(SettingSavePath);
				int num = 0;
				string text2 = Path.Combine(directoryName, $"NpkStudioConfig({num}).json");
				while (File.Exists(text2))
				{
					num++;
					text2 = Path.Combine(directoryName, $"NpkStudioConfig({num}).json");
				}
				try
				{
					File.Copy(SettingSavePath, text2, overwrite: true);
				}
				catch (Exception)
				{
				}
				MessageBox.Show($"配置文件加载时发生错误：{ex.Message}\r\n\r\n已为您将配置文件备份到：{text2}\r\n\r\n请发送给作者分析修复", "错误", MessageBoxButton.OK, MessageBoxImage.Hand);
			}
			return new AppsettingNpk();
		}
	}
}
