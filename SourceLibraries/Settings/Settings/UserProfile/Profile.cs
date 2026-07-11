using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Settings.Interfaces;
using SettingsModel.Models;

namespace Settings.UserProfile;

public class Profile : IProfile
{
	[XmlIgnore]
	public string MainWindowName => "MainWindow";

	[XmlElement(ElementName = "WindowPosSz")]
	public SerializableDictionary<string, ViewPosSizeModel> WindowPosSz { get; set; }

	[XmlAttribute(AttributeName = "LastActiveSolution")]
	public string LastActiveSolution { get; set; }

	[XmlArrayItem("LastActiveSourceFiles", IsNullable = true)]
	public List<FileReference> LastActiveSourceFiles { get; set; }

	[XmlAttribute(AttributeName = "LastActiveTargetFile")]
	public string LastActiveTargetFile { get; set; }

	public Profile()
	{
		WindowPosSz = new SerializableDictionary<string, ViewPosSizeModel>();
		WindowPosSz.Add(MainWindowName, new ViewPosSizeModel(ViewPosSizeModel.DefaultSize));
		LastActiveSolution = (LastActiveTargetFile = string.Empty);
		LastActiveSourceFiles = new List<FileReference>();
	}

	public void CheckSettingsOnLoad(double SystemParameters_VirtualScreenLeft, double SystemParameters_VirtualScreenTop)
	{
		ViewPosSizeModel viewPosSizeModel = new ViewPosSizeModel(ViewPosSizeModel.DefaultSize);
		ViewPosSizeModel value;
		if (WindowPosSz == null)
		{
			WindowPosSz = new SerializableDictionary<string, ViewPosSizeModel>();
			WindowPosSz.Add(MainWindowName, viewPosSizeModel);
		}
		else if (WindowPosSz.TryGetValue(MainWindowName, out value) && value.DefaultConstruct)
		{
			WindowPosSz.Remove(MainWindowName);
			WindowPosSz.Add(MainWindowName, viewPosSizeModel);
		}
		viewPosSizeModel.SetValidPos(SystemParameters_VirtualScreenLeft, SystemParameters_VirtualScreenTop);
	}

	public void UpdateInsertWindowPosSize(string windowName, ViewPosSizeModel model)
	{
		if (WindowPosSz.TryGetValue(windowName, out var _))
		{
			WindowPosSz.Remove(windowName);
		}
		WindowPosSz.Add(windowName, model);
	}

	public string GetLastActivePath()
	{
		try
		{
			if (File.Exists(LastActiveSolution))
			{
				return Path.GetDirectoryName(LastActiveSolution);
			}
		}
		catch
		{
		}
		return string.Empty;
	}
}
