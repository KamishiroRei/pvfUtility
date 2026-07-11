using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace SevenZip;

public class SevenZipSfx
{
	private string _moduleFileName;

	private Dictionary<SfxModule, List<string>> _sfxCommands;

	private static Dictionary<SfxModule, List<string>> SfxSupportedModuleNames
	{
		get
		{
			Dictionary<SfxModule, List<string>> dictionary = new Dictionary<SfxModule, List<string>>
			{
				{
					SfxModule.Simple,
					new List<string>(2) { "7z.sfx", "7zCon.sfx" }
				},
				{
					SfxModule.Installer,
					new List<string>(2) { "7zS.sfx", "7zSD.sfx" }
				}
			};
			if (Environment.Is64BitProcess)
			{
				dictionary.Add(SfxModule.Default, new List<string>(1) { "7zxSD_All_x64.sfx" });
				dictionary.Add(SfxModule.Extended, new List<string>(4) { "7zxSD_All_x64.sfx", "7zxSD_Deflate_x64", "7zxSD_LZMA_x64", "7zxSD_PPMd_x64" });
			}
			else
			{
				dictionary.Add(SfxModule.Default, new List<string>(1) { "7zxSD_All.sfx" });
				dictionary.Add(SfxModule.Extended, new List<string>(4) { "7zxSD_All.sfx", "7zxSD_Deflate", "7zxSD_LZMA", "7zxSD_PPMd" });
			}
			return dictionary;
		}
	}

	public SfxModule SfxModule { get; private set; }

	public string ModuleFileName
	{
		get
		{
			return _moduleFileName;
		}
		set
		{
			if (!File.Exists(value))
			{
				throw new ArgumentException("The specified file does not exist.");
			}
			_moduleFileName = value;
			SfxModule = SfxModule.Custom;
			string fileName = Path.GetFileName(value);
			foreach (SfxModule key in SfxSupportedModuleNames.Keys)
			{
				if (SfxSupportedModuleNames[key].Contains(fileName))
				{
					SfxModule = key;
				}
			}
		}
	}

	public SevenZipSfx()
	{
		SfxModule = SfxModule.Default;
		CommonInit();
	}

	public SevenZipSfx(SfxModule module)
	{
		if (module == SfxModule.Custom)
		{
			throw new ArgumentException("You must specify the custom module executable.", "module");
		}
		SfxModule = module;
		CommonInit();
	}

	public SevenZipSfx(string moduleFileName)
	{
		SfxModule = SfxModule.Custom;
		ModuleFileName = moduleFileName;
		CommonInit();
	}

	private void CommonInit()
	{
		LoadCommandsFromResource("Configs");
	}

	private static string GetResourceString(string str)
	{
		return "SevenZip.sfx." + str;
	}

	private static SfxModule GetModuleByName(string name)
	{
		if (name.IndexOf("7z.sfx", StringComparison.Ordinal) > -1)
		{
			return SfxModule.Simple;
		}
		if (name.IndexOf("7zS.sfx", StringComparison.Ordinal) > -1)
		{
			return SfxModule.Installer;
		}
		if (name.IndexOf("7zxSD_All.sfx", StringComparison.Ordinal) > -1)
		{
			return SfxModule.Extended;
		}
		throw new SevenZipSfxValidationException("The specified configuration is unsupported.");
	}

	private void LoadCommandsFromResource(string xmlDefinitions)
	{
		using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(GetResourceString(xmlDefinitions + ".xml"));
		if (stream == null)
		{
			throw new SevenZipSfxValidationException("The configuration \"" + xmlDefinitions + "\" does not exist.");
		}
		using Stream stream2 = Assembly.GetExecutingAssembly().GetManifestResourceStream(GetResourceString(xmlDefinitions + ".xsd"));
		if (stream2 == null)
		{
			throw new SevenZipSfxValidationException("The configuration schema \"" + xmlDefinitions + "\" does not exist.");
		}
		XmlSchemaSet xmlSchemaSet = new XmlSchemaSet();
		using XmlReader schemaDocument = XmlReader.Create(stream2);
		xmlSchemaSet.Add(null, schemaDocument);
		XmlReaderSettings xmlReaderSettings = new XmlReaderSettings
		{
			ValidationType = ValidationType.Schema,
			Schemas = xmlSchemaSet
		};
		string validationErrors = "";
		xmlReaderSettings.ValidationEventHandler += delegate(object? s, ValidationEventArgs t)
		{
			validationErrors += string.Format(CultureInfo.InvariantCulture, "[{0}]: {1}\n", t.Severity.ToString(), t.Message);
		};
		using (XmlReader xmlReader = XmlReader.Create(stream, xmlReaderSettings))
		{
			_sfxCommands = new Dictionary<SfxModule, List<string>>();
			xmlReader.Read();
			xmlReader.Read();
			xmlReader.Read();
			xmlReader.Read();
			xmlReader.Read();
			xmlReader.ReadStartElement("sfxConfigs");
			xmlReader.Read();
			do
			{
				SfxModule moduleByName = GetModuleByName(xmlReader["modules"]);
				xmlReader.ReadStartElement("config");
				xmlReader.Read();
				if (xmlReader.Name == "id")
				{
					List<string> list = new List<string>();
					_sfxCommands.Add(moduleByName, list);
					do
					{
						list.Add(xmlReader["command"]);
						xmlReader.Read();
						xmlReader.Read();
					}
					while (xmlReader.Name == "id");
					xmlReader.ReadEndElement();
					xmlReader.Read();
				}
				else
				{
					_sfxCommands.Add(moduleByName, null);
				}
			}
			while (xmlReader.Name == "config");
		}
		if (!string.IsNullOrEmpty(validationErrors))
		{
			throw new SevenZipSfxValidationException("\n" + validationErrors.Substring(0, validationErrors.Length - 1));
		}
		_sfxCommands.Add(SfxModule.Default, _sfxCommands[SfxModule.Extended]);
	}

	private void ValidateSettings(Dictionary<string, string> settings)
	{
		if (SfxModule == SfxModule.Custom)
		{
			return;
		}
		List<string> list = _sfxCommands[SfxModule];
		if (list == null)
		{
			return;
		}
		List<string> list2 = new List<string>();
		foreach (string key in settings.Keys)
		{
			if (!list.Contains(key))
			{
				list2.Add(key);
			}
		}
		if (list2.Count <= 0)
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder("\nInvalid commands:\n");
		foreach (string item in list2)
		{
			stringBuilder.Append(item);
		}
		throw new SevenZipSfxValidationException(stringBuilder.ToString());
	}

	private static Stream GetSettingsStream(Dictionary<string, string> settings)
	{
		MemoryStream memoryStream = new MemoryStream();
		byte[] bytes = Encoding.UTF8.GetBytes(";!@Install@!UTF-8!\n");
		memoryStream.Write(bytes, 0, bytes.Length);
		foreach (string key in settings.Keys)
		{
			bytes = Encoding.UTF8.GetBytes(string.Format(CultureInfo.InvariantCulture, "{0}=\"{1}\"\n", key, settings[key]));
			memoryStream.Write(bytes, 0, bytes.Length);
		}
		bytes = Encoding.UTF8.GetBytes(";!@InstallEnd@!");
		memoryStream.Write(bytes, 0, bytes.Length);
		return memoryStream;
	}

	private Dictionary<string, string> GetDefaultSettings()
	{
		switch (SfxModule)
		{
		default:
			return null;
		case SfxModule.Installer:
			return new Dictionary<string, string> { { "Title", "7-Zip self-extracting archive" } };
		case SfxModule.Default:
		case SfxModule.Extended:
			return new Dictionary<string, string>
			{
				{ "GUIMode", "0" },
				{ "InstallPath", "." },
				{ "GUIFlags", "128+8" },
				{ "ExtractPathTitle", "7-Zip self-extracting archive" },
				{ "ExtractPathText", "Specify the path where to extract the files:" }
			};
		}
	}

	private static void WriteStream(Stream src, Stream dest)
	{
		if (src == null)
		{
			throw new ArgumentNullException("src");
		}
		if (dest == null)
		{
			throw new ArgumentNullException("dest");
		}
		src.Seek(0L, SeekOrigin.Begin);
		byte[] array = new byte[32768];
		int count;
		while ((count = src.Read(array, 0, array.Length)) > 0)
		{
			dest.Write(array, 0, count);
		}
	}

	public void MakeSfx(Stream archive, string sfxFileName)
	{
		using Stream sfxStream = File.Create(sfxFileName);
		MakeSfx(archive, GetDefaultSettings(), sfxStream);
	}

	public void MakeSfx(Stream archive, Stream sfxStream)
	{
		MakeSfx(archive, GetDefaultSettings(), sfxStream);
	}

	public void MakeSfx(Stream archive, Dictionary<string, string> settings, string sfxFileName)
	{
		using Stream sfxStream = File.Create(sfxFileName);
		MakeSfx(archive, settings, sfxStream);
	}

	public void MakeSfx(Stream archive, Dictionary<string, string> settings, Stream sfxStream)
	{
		if (!sfxStream.CanWrite)
		{
			throw new ArgumentException("The specified output stream can not write.", "sfxStream");
		}
		ValidateSettings(settings);
		using (Stream src = Assembly.GetExecutingAssembly().GetManifestResourceStream(GetResourceString(SfxSupportedModuleNames[SfxModule][0])))
		{
			WriteStream(src, sfxStream);
		}
		if (SfxModule == SfxModule.Custom || _sfxCommands[SfxModule] != null)
		{
			using Stream src2 = GetSettingsStream(settings);
			WriteStream(src2, sfxStream);
		}
		WriteStream(archive, sfxStream);
	}

	public void MakeSfx(string archiveFileName, string sfxFileName)
	{
		using Stream sfxStream = File.Create(sfxFileName);
		using Stream archive = new FileStream(archiveFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		MakeSfx(archive, GetDefaultSettings(), sfxStream);
	}

	public void MakeSfx(string archiveFileName, Stream sfxStream)
	{
		using Stream archive = new FileStream(archiveFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		MakeSfx(archive, GetDefaultSettings(), sfxStream);
	}

	public void MakeSfx(string archiveFileName, Dictionary<string, string> settings, string sfxFileName)
	{
		using Stream sfxStream = File.Create(sfxFileName);
		using Stream archive = new FileStream(archiveFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		MakeSfx(archive, settings, sfxStream);
	}

	public void MakeSfx(string archiveFileName, Dictionary<string, string> settings, Stream sfxStream)
	{
		using Stream archive = new FileStream(archiveFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		MakeSfx(archive, settings, sfxStream);
	}
}
