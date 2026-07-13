using System.Linq;
using System.Windows;
using Utools;

namespace PvfCode.ViewModels;

public class SettingMenuItem
{
	private ObservableConcurrentDictionaryEx<string, SettingMenuItem> children;

	public ObservableConcurrentDictionaryEx<string, SettingMenuItem> Children
	{
		get
		{
			if (children == null)
			{
				children = new ObservableConcurrentDictionaryEx<string, SettingMenuItem>();
			}
			return children;
		}
		set
		{
			children = value;
		}
	}

	public DataTemplate? Data { get; set; }

	public string Parname { get; set; }

	public bool HaveChildren()
	{
		if (children == null)
		{
			return false;
		}
		return children.Any();
	}

	public SettingMenuItem()
	{
	}
}
