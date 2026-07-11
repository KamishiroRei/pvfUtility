using System.Collections.Generic;
using Settings.UserProfile;
using SettingsModel.Models;

namespace Settings.Interfaces;

public interface IProfile
{
	string LastActiveSolution { get; set; }

	string LastActiveTargetFile { get; set; }

	List<FileReference> LastActiveSourceFiles { get; set; }

	string MainWindowName { get; }

	SerializableDictionary<string, ViewPosSizeModel> WindowPosSz { get; }

	string GetLastActivePath();

	void CheckSettingsOnLoad(double SystemParameters_VirtualScreenLeft, double SystemParameters_VirtualScreenTop);

	void UpdateInsertWindowPosSize(string windowName, ViewPosSizeModel model);
}
