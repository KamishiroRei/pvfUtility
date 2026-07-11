using System.Xml.Serialization;

namespace SettingsModel.Models;

public class FileReference
{
	[XmlAttribute(AttributeName = "path")]
	public string path { get; set; }
}
