using System.IO;
using HL.Manager;

namespace HL.Resources;

internal class HLResources
{
	public static Stream OpenStream(string prefix, string name)
	{
		string text = prefix + "." + name;
		return typeof(HLResources).Assembly.GetManifestResourceStream(text) ?? throw new FileNotFoundException("The resource file '" + text + "' was not found.");
	}

	internal static void RegisterBuiltInHighlightings(DefaultHighlightingManager hlm, IHLTheme theme)
	{
		if (!theme.IsBuiltInThemesRegistered)
		{
			hlm.RegisterHighlighting(theme, "Script", new string[1] { ".script" }, "Script.xshd");
			hlm.RegisterHighlighting(theme, "Lst", new string[1] { ".lst" }, "Lst.xshd");
			hlm.RegisterHighlighting(theme, "Kor", new string[1] { ".str" }, "Kor.xshd");
			hlm.RegisterHighlighting(theme, "StringTable", new string[1] { ".bin" }, "StringTable.xshd");
			hlm.RegisterHighlighting(theme, "XmlDoc", null, "XmlDoc.xshd");
			hlm.RegisterHighlighting(theme, "C#", new string[1] { ".cs" }, "CSharp-Mode.xshd");
			hlm.RegisterHighlighting(theme, "JavaScript", new string[1] { ".js" }, "JavaScript-Mode.xshd");
			hlm.RegisterHighlighting(theme, "HTML", new string[2] { ".htm", ".html" }, "HTML-Mode.xshd");
			hlm.RegisterHighlighting(theme, "ASP/XHTML", new string[6] { ".asp", ".aspx", ".asax", ".asmx", ".ascx", ".master" }, "ASPX.xshd");
			hlm.RegisterHighlighting(theme, "Boo", new string[1] { ".boo" }, "Boo.xshd");
			hlm.RegisterHighlighting(theme, "Coco", new string[1] { ".atg" }, "Coco-Mode.xshd");
			hlm.RegisterHighlighting(theme, "CSS", new string[1] { ".css" }, "CSS-Mode.xshd");
			hlm.RegisterHighlighting(theme, "C++", new string[5] { ".c", ".h", ".cc", ".cpp", ".hpp" }, "CPP-Mode.xshd");
			hlm.RegisterHighlighting(theme, "Java", new string[1] { ".java" }, "Java-Mode.xshd");
			hlm.RegisterHighlighting(theme, "Patch", new string[2] { ".patch", ".diff" }, "Patch-Mode.xshd");
			hlm.RegisterHighlighting(theme, "PowerShell", new string[3] { ".ps1", ".psm1", ".psd1" }, "PowerShell.xshd");
			hlm.RegisterHighlighting(theme, "PHP", new string[1] { ".php" }, "PHP-Mode.xshd");
			hlm.RegisterHighlighting(theme, "Python", new string[2] { ".py", ".pyw" }, "Python-Mode.xshd");
			hlm.RegisterHighlighting(theme, "TeX", new string[1] { ".tex" }, "Tex-Mode.xshd");
			hlm.RegisterHighlighting(theme, "TSQL", new string[1] { ".sql" }, "TSQL-Mode.xshd");
			hlm.RegisterHighlighting(theme, "VB", new string[1] { ".vb" }, "VB-Mode.xshd");
			hlm.RegisterHighlighting(theme, "XML", ".xml;.xsl;.xslt;.xsd;.manifest;.config;.addin;.xshd;.wxs;.wxi;.wxl;.proj;.csproj;.vbproj;.ilproj;.booproj;.build;.xfrm;.targets;.xaml;.xpt;.xft;.map;.wsdl;.disco;.ps1xml;.nuspec".Split(';'), "XML-Mode.xshd");
			hlm.RegisterHighlighting(theme, "MarkDown", new string[1] { ".md" }, "MarkDown-Mode.xshd");
			hlm.RegisterHighlighting(theme, "ActionScript3", new string[1] { ".as" }, "AS3.xshd");
			hlm.RegisterHighlighting(theme, "BAT", new string[2] { ".bat", ".dos" }, "DOSBATCH.xshd");
			hlm.RegisterHighlighting(theme, "F#", new string[1] { ".fs" }, "FSharp-Mode.xshd");
			hlm.RegisterHighlighting(theme, "HLSL", new string[1] { ".fx" }, "HLSL.xshd");
			hlm.RegisterHighlighting(theme, "INI", new string[4] { ".cfg", ".conf", ".ini", ".iss" }, "INI.xshd");
			hlm.RegisterHighlighting(theme, "LOG", new string[1] { ".log" }, "Log.xshd");
			hlm.RegisterHighlighting(theme, "Pascal", new string[1] { ".pas" }, "Pascal.xshd");
			hlm.RegisterHighlighting(theme, "PLSQL", new string[1] { ".plsql" }, "PLSQL.xshd");
			hlm.RegisterHighlighting(theme, "Ruby", new string[1] { ".rb" }, "Ruby.xshd");
			hlm.RegisterHighlighting(theme, "Scheme", new string[4] { ".sls", ".sps", ".ss", ".scm" }, "scheme.xshd");
			hlm.RegisterHighlighting(theme, "Squirrel", new string[1] { ".nut" }, "squirrel.xshd");
			hlm.RegisterHighlighting(theme, "TXT", new string[1] { ".txt" }, "TXT.xshd");
			hlm.RegisterHighlighting(theme, "VTL", new string[2] { ".vtl", ".vm" }, "vtl.xshd");
			theme.IsBuiltInThemesRegistered = true;
		}
	}
}
