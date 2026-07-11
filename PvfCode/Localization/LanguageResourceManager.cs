using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;
using System.Xml;
using PvfCode.Dot;
using Utools;

namespace PvfCode.Localization;

internal static class LanguageResourceManager
{
	private static readonly Uri SimplifiedChineseResource =
		new Uri("/Styles/Luanguage-zh-CN.xaml", UriKind.RelativeOrAbsolute);

	public static async Task ApplyLanguageAsync(LangType language)
	{
		ResourceDictionary dictionary = await LoadLanguageDictionaryAsync(language);
		Application.Current.Resources.MergedDictionaries[1] = dictionary;
	}

	public static async Task<ResourceDictionary> LoadLanguageDictionaryAsync(LangType language)
	{
		if (language == LangType.中文简体)
		{
			return Application.LoadComponent(SimplifiedChineseResource) as ResourceDictionary;
		}

		ResultData<string> result = await EnsureLanguageFileAsync(language);
		if (result.IsError)
		{
			MessageBox.Show(result.Msg);
			return Application.LoadComponent(SimplifiedChineseResource) as ResourceDictionary;
		}

		try
		{
			using FileStream stream = File.Open(result.Data, FileMode.Open);
			return (ResourceDictionary)XamlReader.Load(stream);
		}
		catch (Exception e)
		{
			MessageBox.Show(
				"语言包加载出错 已设定为默认的简体中文 Language pack loading error has been set to the default Simplified Chinese \r\nError：" +
				e.Message);
			return Application.LoadComponent(SimplifiedChineseResource) as ResourceDictionary;
		}
	}

	private static Task<ResultData<string>> EnsureLanguageFileAsync(LangType language)
	{
		ResultData<string> result = new ResultData<string>();
		FileHelper.CheckDir(Path.Combine(AppSetting.AppBasePath, "Lang"));
		switch (language)
		{
		case LangType.中文繁体:
			ResourceDictionary dictionary =
				Application.LoadComponent(SimplifiedChineseResource) as ResourceDictionary;
			result.Data = Path.Combine(AppSetting.AppBasePath, "Lang\\lang-zh-TW.xml");
			using (XmlWriter writer = XmlWriter.Create(result.Data))
			{
				XamlDesignerSerializationManager manager = new XamlDesignerSerializationManager(writer)
				{
					XamlWriterMode = XamlWriterMode.Value
				};
				XamlWriter.Save(dictionary, manager);
			}
			File.WriteAllText(result.Data, ChineseHelper.ToTraditional(File.ReadAllText(result.Data)));
			break;
		case LangType.Korean:
			result.Data = Path.Combine(AppSetting.AppBasePath, "Lang\\lang-ko-KR.xml");
			break;
		}

		if (!File.Exists(result.Data))
		{
			result.Msg = "区域语言设置失败 找不到文件：" + result.Data +
				"\r\n\r\nRegional language setting failed File not found:" + result.Data;
		}
		return Task.FromResult(result);
	}

}
